package cz.com.spcss.dss;

import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import eu.europa.esig.dss.*;
import eu.europa.esig.dss.cades.validation.CAdESSignature;
import eu.europa.esig.dss.client.crl.OnlineCRLSource;
import eu.europa.esig.dss.client.http.commons.CommonsDataLoader;
import eu.europa.esig.dss.client.http.proxy.ProxyConfig;
import eu.europa.esig.dss.client.http.proxy.ProxyProperties;
import eu.europa.esig.dss.client.ocsp.OnlineOCSPSource;
import eu.europa.esig.dss.tsl.TrustedListsCertificateSource;
import eu.europa.esig.dss.tsl.service.TSLRepository;
import eu.europa.esig.dss.tsl.service.TSLValidationJob;
import eu.europa.esig.dss.utils.Utils;
import eu.europa.esig.dss.validation.*;
import eu.europa.esig.dss.validation.policy.rules.Indication;
import eu.europa.esig.dss.validation.reports.*;
import eu.europa.esig.dss.validation.reports.wrapper.DiagnosticData;
import eu.europa.esig.dss.x509.CertificatePool;
import eu.europa.esig.dss.x509.CertificateSource;
import eu.europa.esig.dss.x509.CertificateToken;
import eu.europa.esig.dss.x509.KeyStoreCertificateSource;
import org.apache.commons.io.FilenameUtils;
import org.apache.commons.lang3.Validate;

import org.apache.log4j.Logger;
import org.bouncycastle.cms.CMSException;
import org.bouncycastle.cms.CMSSignedData;
import org.bouncycastle.cms.SignerInformation;
import org.jetbrains.annotations.NotNull;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.util.JAXBSource;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.stream.StreamResult;
import javax.xml.transform.stream.StreamSource;
import java.io.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.Timer;
import java.util.TimerTask;

public class DssValidator {
    static {
        setProperties();
    }
    private static final Logger LOG = Logger.getLogger(DssValidator.class);

    private static Properties properties;

    private static final String DETAILED_REPORT_XSTL = "xstl/detailed-report.xslt";
    private static final String SIMPLE_REPORT_XSTL = "xstl/simple-report.xslt";

    private static final String DEFAULT_KEYSTORE_PATH = "/keystore.p12";
    private static final String DEFAULT_KEYSTORE_PASSWORD = "dss-password";
    private static final String DEFAULT_KEYSTORE_TYPE = "PKCS12";

    private CMSSignedData cmsSignedData;
    private DSSDocument signedDocument;
    private static CertificatePool validationCertPool = null;

    public SignResult validateFile(@NotNull String inputFile,
                                   @NotNull String outputFile,
                                   @NotNull List<String> certificateFiles) throws IOException, JAXBException, TransformerException {
        Validate.notBlank(inputFile, "inputFile can't be blank");
        Validate.notBlank(outputFile, "outputFile can't be blank");
        Validate.notNull(certificateFiles, "certificateFiles can't be null");

        setProperties();
        String certificateReportPath = FilenameUtils.removeExtension(outputFile);
        // test new InMemoryFiDocument(new FileInputStream((inputFile.getPath())));
        signedDocument = new FileDocument(inputFile);
        validateDSSDocument(signedDocument);
        if (!isSupported(signedDocument)) {
            return new SignResult(ResultCodes.ERR_PKG_SIG_INVALID_DER, "Nebylo možné přečíst ASN.1 DER struktury balíčku");
        }

        TrustedListsCertificateSource trustedCertSource = loadTSL();
        SignResult validateSignatureResult = signatureValidate(trustedCertSource, new FileDocument(inputFile), certificateReportPath);
        if (!validateSignatureResult.getResultCode().equals(ResultCodes.OK)) {
            return validateSignatureResult;
        }

        List<AdvancedSignature> signatures = getSignatures(signedDocument);
        if (!signatures.isEmpty()) {
            AdvancedSignature signature = signatures.get(0);

            try {
                if (!validSignatureDSSID(signature, certificateFiles)) {
                    return new SignResult(ResultCodes.ERR_PKG_SIG_CERT_UNKNOWN, "Certifikát není mezi registrovanými certifikáty operátora");
                }
            } catch (DSSException e) {
                return new SignResult(ResultCodes.ERR_PKG_SIG_CERT_UNTRUSTED, "Nebylo možné ověřit platnost certifikátu");
            }
            List<DSSDocument> originalDocuments = getOriginalDocuments(signedDocument, signature.getId());
            if (!originalDocuments.isEmpty()) {
                originalDocuments.get(0).save(outputFile);
            }
        } else {
            return new SignResult(ResultCodes.ERR_PKG_SIG_CERT_UNTRUSTED, "Nebylo možné ověřit platnost certifikátu");
        }
        return new SignResult(ResultCodes.OK, ResultCodes.INFO_PKG_SIG_CERT_OK.toString());
    }

    public SignResult validateCertificate(@NotNull String testedCertificate) throws IOException, JAXBException, TransformerException {
        CommonsDataLoader dataLoader = new CommonsDataLoader();
        ProxyConfig proxyConfig = getProxyConfig();
        OnlineOCSPSource ocspSource = new OnlineOCSPSource();
        OnlineCRLSource crlSource =	new OnlineCRLSource();
        if (proxyConfig!=null) {
        	dataLoader.setProxyConfig(proxyConfig);
        }
    	ocspSource.setDataLoader(dataLoader);
    	crlSource.setDataLoader(dataLoader);
    	
        Validate.notBlank(testedCertificate, "testedCertificate can't be blank");

        TrustedListsCertificateSource trustedCertSource = loadTSL();
        String certificateReportPath = FilenameUtils.removeExtension(testedCertificate);

        CertificateToken token = DSSUtils.loadCertificate(new File(testedCertificate));
        CertificateVerifier cv = new CommonCertificateVerifier();
        cv.setDataLoader(dataLoader);
        cv.setOcspSource(ocspSource);
        cv.setCrlSource(crlSource);
        cv.setTrustedCertSource(trustedCertSource);

        CertificateValidator validator = CertificateValidator.fromCertificate(token);
        validator.setCertificateVerifier(cv);

        CertificateReports certificateReports = validator.validate();

        DiagnosticData diagnosticData = certificateReports.getDiagnosticData();
        SimpleCertificateReport simpleReport = certificateReports.getSimpleReport();
        DetailedReport detailedReport = certificateReports.getDetailedReport();

        String simpleCertificateReportPath = certificateReportPath+ ".SimpleCertificateReport.xml";
        String detailedCertificateReportPath = certificateReportPath+ ".DetailedCertificateReport.xml";
        String diagnosticCertificateDataPath = certificateReportPath+ ".diagnosticCertificateData.xml";
        generateReport(simpleReport, simpleCertificateReportPath);
        generateReport(detailedReport, detailedCertificateReportPath, false);
        generateReport(diagnosticData, diagnosticCertificateDataPath);

        for (String certificateId : simpleReport.getCertificateIds()) {
            if (simpleReport.getCertificateIndication(certificateId) != null && simpleReport.getCertificateIndication(certificateId).toString().equals("PASSED")) {
            	if (simpleReport.getQualificationAtCertificateIssuance()==CertificateQualification.QCERT_FOR_ESEAL || simpleReport.getQualificationAtCertificateIssuance()==CertificateQualification.QCERT_FOR_ESEAL_QSCD) {
            		return new SignResult(ResultCodes.OK, ResultCodes.INFO_PKG_SIG_CERT_OK.toString());
            	} else {
            		return new SignResult(ResultCodes.ERR_VALID_CERT_NOT_FOR_QSEAL,"Certifikát je platný, ale není určen pro pečetění. Certificate valid but not for qseal purposes");
            	}
            }
        }
        return new SignResult(ResultCodes.ERR_CERT_EUTL_INVALID, "Nepodařilo se ověřit certifikát vůči EU Trust Listu. Certificate not validated in the context of EU Trust list.");
    }

    private boolean validSignatureDSSID(AdvancedSignature advancedSignature, List<String> certificateFiles) throws DSSException {
        String SignatureDSSID = advancedSignature.getSigningCertificateToken().getDSSIdAsString();
        for (String certificateFile : certificateFiles) {
            CertificateToken token = DSSUtils.loadCertificate(new File(certificateFile));
            if (token.getDSSIdAsString().equals(SignatureDSSID)) {
                return true;
            }
        }
        return false;
    }

    private SignResult signatureValidate(CertificateSource trustedCertSource, DSSDocument document, String certificateReportPath) throws JAXBException, TransformerException {
        CommonsDataLoader dataLoader = new CommonsDataLoader();
        ProxyConfig proxyConfig = getProxyConfig();
        OnlineOCSPSource ocspSource = new OnlineOCSPSource();
        OnlineCRLSource crlSource =	new OnlineCRLSource();
        if (proxyConfig!=null) {
        	dataLoader.setProxyConfig(proxyConfig);
        }
    	ocspSource.setDataLoader(dataLoader);
    	crlSource.setDataLoader(dataLoader);
        
        CertificateVerifier cv = new CommonCertificateVerifier();
        cv.setDataLoader(dataLoader);
        cv.setOcspSource(ocspSource);
        cv.setCrlSource(crlSource);
        cv.setTrustedCertSource(trustedCertSource);

        SignedDocumentValidator documentValidator = SignedDocumentValidator.fromDocument(document);
        documentValidator.setCertificateVerifier(cv);

        Reports reports = documentValidator.validateDocument();
        SimpleReport simpleReport = reports.getSimpleReport();
        DetailedReport detailedReport = reports.getDetailedReport();
        DiagnosticData diagnosticData = reports.getDiagnosticData();

        String simpleReportPath = certificateReportPath + ".SimpleReport.xml";
        String detailedReportPath = certificateReportPath + ".DetailedReport.xml";
        String diagnosticDataPath = certificateReportPath + ".diagnosticData.xml";
        generateReport(simpleReport, simpleReportPath);
        generateReport(detailedReport, detailedReportPath, true);
        generateReport(diagnosticData, diagnosticDataPath);

        for (String signatureId : simpleReport.getSignatureIdList()) {
            if (simpleReport.getIndication(signatureId) != null && simpleReport.getIndication(signatureId).toString().equals(Indication.TOTAL_PASSED.toString())) {
                return new SignResult(ResultCodes.OK, ResultCodes.INFO_PKG_SIG_CERT_OK.toString());
            }
        }
        return new SignResult(ResultCodes.ERR_PKG_SIG_INVALID_SIGNATURE, "Nesouhlasí kryptografické ověření podpisu");
    }

    static private final Object cachedTrustedCertificatesSourceLock=new Object();
    static private TrustedListsCertificateSource cachedTrustedCertificatesSource;
    static {
    	new Timer().schedule(new TimerTask() {
			@Override
			public void run() {
				try {
					synchronized (cachedTrustedCertificatesSourceLock) {
						cachedTrustedCertificatesSource =  null;			
					}
				}
				catch (Exception e) {
					LOG.error("failed to refresh TSL", e);
				}
			}
		}, 20*60*1000 );
    }

    private static ProxyConfig getProxyConfig() {
        if (System.getenv("PROXY_HOST")!=null && System.getenv("PROXY_PORT")!=null) {
        	LOG.info(String.format("Will use PROXY for communication %s:%s",System.getenv("PROXY_HOST"),System.getenv("PROXY_PORT")));
            ProxyConfig proxyConfig=new ProxyConfig();
            ProxyProperties props = new ProxyProperties();
            props.setHost(System.getenv("PROXY_HOST"));
            props.setPort(Integer.parseInt(System.getenv("PROXY_PORT")));
            proxyConfig.setHttpProperties(props);
            proxyConfig.setHttpsProperties(props);
            return proxyConfig;
        }
        return null;
    }
    
    
    private static TrustedListsCertificateSource loadTSL() throws IOException {
    	synchronized (cachedTrustedCertificatesSourceLock) {
        	if ( cachedTrustedCertificatesSource!=null) {
        		return cachedTrustedCertificatesSource;
        	}
        	
        	TSLRepository tslRepository = new TSLRepository();
            tslRepository.setCacheDirectoryPath(properties.getProperty("CacheTSL"));

            TrustedListsCertificateSource certificateSource = new TrustedListsCertificateSource();
            tslRepository.setTrustedListsCertificateSource(certificateSource);

            TSLValidationJob job = new TSLValidationJob();
            CommonsDataLoader dataLoader = new CommonsDataLoader();
            ProxyConfig proxyConfig = getProxyConfig();
            if (proxyConfig!=null) {
            	dataLoader.setProxyConfig(proxyConfig);
            }
            
            
            job.setDataLoader(dataLoader);
            job.setCheckLOTLSignature(true);
            job.setCheckTSLSignatures(true);
            job.setLotlUrl(properties.getProperty("LotlUrl"));
            job.setLotlCode(properties.getProperty("LotlCode"));

            // This information is needed to be able to filter the LOTL pivots
            job.setLotlRootSchemeInfoUri(properties.getProperty("LotlRootSchemeInfoUri"));

            // The keystore contains certificates referenced in the Official Journal Link (OJ URL)
            KeyStoreCertificateSource keyStoreCertificateSource = getKeyStoreCertificateSource();
            job.setOjUrl(properties.getProperty("OjUrl"));
            job.setOjContentKeyStore(keyStoreCertificateSource);
            job.setRepository(tslRepository);
            job.refresh();

            CertificatePool newCertPool=new CertificatePool();
            newCertPool.importCerts(keyStoreCertificateSource);
            validationCertPool = newCertPool;

            cachedTrustedCertificatesSource = certificateSource;
            return certificateSource;    		
		}
    }

    private static KeyStoreCertificateSource getKeyStoreCertificateSource() throws IOException {
        File keystoreFile = new File(properties.getProperty("KeyStoreCertificatePath"));
        if (!keystoreFile.exists()) {
            LOG.error("FAILED to read keystore file - using default keystore");
            final InputStream keystoreStream = DssValidator.class.getResourceAsStream(DEFAULT_KEYSTORE_PATH);
            return new KeyStoreCertificateSource(keystoreStream, DEFAULT_KEYSTORE_TYPE, DEFAULT_KEYSTORE_PASSWORD);
        }
        return new KeyStoreCertificateSource(new File(properties.getProperty("KeyStoreCertificatePath")), properties.getProperty("KeyStoreCertificateType"),
                properties.getProperty("KeyStoreCertificatePassword"));
    }

    private void validateDSSDocument(DSSDocument document) {
        try (InputStream inputStream = document.openStream()) {
            cmsSignedData = new CMSSignedData(inputStream);
        } catch (IOException | CMSException e) {
            throw new DSSException("Not a valid CAdES file", e);
        }
    }

    private boolean isSupported(DSSDocument dssDocument) {
        byte firstByte = DSSUtils.readFirstByte(dssDocument);
        return DSSASN1Utils.isASN1SequenceTag(firstByte);
    }

    private List<AdvancedSignature> getSignatures(DSSDocument document) {
        List<AdvancedSignature> signatures = new ArrayList<>();
        if (cmsSignedData != null) {
            for (final Object signerInformationObject : cmsSignedData.getSignerInfos().getSigners()) {

                final SignerInformation signerInformation = (SignerInformation) signerInformationObject;
                final CAdESSignature cadesSignature = new CAdESSignature(cmsSignedData, signerInformation, validationCertPool);
                if (document != null) {
                    cadesSignature.setSignatureFilename(document.getName());
                }
                signatures.add(cadesSignature);
            }
        }
        return signatures;
    }

    private List<DSSDocument> getOriginalDocuments(DSSDocument document, final String signatureId) throws DSSException {
        if (Utils.isStringBlank(signatureId)) {
            throw new NullPointerException("signatureId null");
        }
        List<DSSDocument> results = new ArrayList<>();

        for (final Object signerInformationObject : cmsSignedData.getSignerInfos().getSigners()) {

            final SignerInformation signerInformation = (SignerInformation) signerInformationObject;
            final CAdESSignature cadesSignature = new CAdESSignature(cmsSignedData, signerInformation, validationCertPool);
            cadesSignature.setSignatureFilename(document.getName());
            if (Utils.areStringsEqual(cadesSignature.getId(), signatureId)) {
                results.add(cadesSignature.getOriginalDocument());
            }
        }
        return results;
    }

    private void generateReport(SimpleReport report, String certificateReportPath) throws JAXBException, TransformerException {
        File file = new File(certificateReportPath);
        JAXBContext jaxbContext = JAXBContext.newInstance(eu.europa.esig.dss.jaxb.simplereport.SimpleReport.class);
        Marshaller jaxbMarshaller = jaxbContext.createMarshaller();
        jaxbMarshaller.marshal(report.getJaxbModel(), file);

        TransformerFactory tf = TransformerFactory.newInstance();
        StreamSource xslt = new StreamSource(getClass().getClassLoader().getResourceAsStream(SIMPLE_REPORT_XSTL));
        Transformer transformer = tf.newTransformer(xslt);

        JAXBSource source = new JAXBSource(jaxbContext, report.getJaxbModel());
        String certificateReportPathHtml = FilenameUtils.removeExtension(certificateReportPath) + ".html";
        StreamResult result = new StreamResult(new File(certificateReportPathHtml));
        transformer.transform(source, result);
    }

    private void generateReport(DetailedReport report, String certificateReportPath, boolean isSignature) throws JAXBException, TransformerException {
        File file = new File(certificateReportPath);
        JAXBContext jaxbContext = JAXBContext.newInstance(eu.europa.esig.dss.jaxb.detailedreport.DetailedReport.class);
        Marshaller jaxbMarshaller = jaxbContext.createMarshaller();
        jaxbMarshaller.marshal(report.getJAXBModel(), file);

        if(isSignature) {
            TransformerFactory tf = TransformerFactory.newInstance();
            StreamSource xslt = new StreamSource(getClass().getClassLoader().getResourceAsStream(DETAILED_REPORT_XSTL));
            Transformer transformer = tf.newTransformer(xslt);

            JAXBSource source = new JAXBSource(jaxbContext, report.getJAXBModel());
            String certificateReportPathHtml = FilenameUtils.removeExtension(certificateReportPath) + ".html";
            StreamResult result = new StreamResult(new File(certificateReportPathHtml));
            transformer.transform(source, result);
        }
    }

    private void generateReport(DiagnosticData report, String certificateReportPath) throws JAXBException {
        File file = new File(certificateReportPath);
        JAXBContext jaxbContext = JAXBContext.newInstance(eu.europa.esig.dss.jaxb.diagnostic.DiagnosticData.class);
        Marshaller jaxbMarshaller = jaxbContext.createMarshaller();
        jaxbMarshaller.marshal(report.getJaxbModel(), file);
    }

    private void generateReport(SimpleCertificateReport report, String certificateReportPath) throws JAXBException {
        File file = new File(certificateReportPath);
        JAXBContext jaxbContext = JAXBContext.newInstance(eu.europa.esig.dss.jaxb.simplecertificatereport.SimpleCertificateReport.class);
        Marshaller jaxbMarshaller = jaxbContext.createMarshaller();
        jaxbMarshaller.marshal(report.getJaxbModel(), file);
    }

    private static void setProperties() {
        final InputStream propertiesStream = DssValidator.class.getResourceAsStream("/keystore.properties");
        try {
            properties = new Properties();
            properties.load(propertiesStream);
            propertiesStream.close();
            for (Map.Entry<Object, Object> property : properties.entrySet()) {
                String envName="AISG_" + property.getKey();
                if (System.getenv().containsKey(envName)) {
                    property.setValue(System.getenv().get(envName));
                }
            }
        } catch (FileNotFoundException e) {
            LOG.error("Properties file not found " + e);
        } catch (IOException e) {
            LOG.error("Properties IO exception " + e);
        }
    }
}
