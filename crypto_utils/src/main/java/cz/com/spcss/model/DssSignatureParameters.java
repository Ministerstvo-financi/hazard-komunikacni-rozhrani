package cz.com.spcss.model;

import eu.europa.esig.dss.cades.CAdESSignatureParameters;
import eu.europa.esig.dss.token.DSSPrivateKeyEntry;
import eu.europa.esig.dss.x509.CertificateToken;
import eu.europa.esig.dss.xades.DSSReference;
import eu.europa.esig.dss.xades.DSSTransform;
import eu.europa.esig.dss.xades.XAdESSignatureParameters;

import org.apache.commons.lang3.StringUtils;
import org.jetbrains.annotations.NotNull;
import eu.europa.esig.dss.*;
import org.apache.commons.lang3.Validate;
import org.jetbrains.annotations.Nullable;

import java.net.URI;
import java.net.URISyntaxException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

public class DssSignatureParameters {

    public static final String XADES = "XADES";
    public static final String CADES = "CADES";
	private URI signerPrivateKeySpec;
    private URI timestampSourceSpec;
    private SignatureForm signatureForm;
    private SignaturePackaging signaturePackaging;
    private DigestAlgorithm digestAlgorithm;
    private SignatureLevel signatureLevel;

    public DssSignatureParameters(@NotNull String signerPrivateKeySpec,
                                  @NotNull String signerPrivateKeySpecPass,
                                  @Nullable String timestampSourceSpec,
                                  @NotNull SignatureForm signatureForm,
                                  @NotNull SignaturePackaging signaturePackaging,
                                  @NotNull DigestAlgorithm digestAlgorithm,
                                  @Nullable SignatureLevel signatureLevel) throws URISyntaxException {
        Validate.notBlank(signerPrivateKeySpec, "signerPrivateKeySpec can't be blank");
        Validate.notBlank(signerPrivateKeySpecPass, "signerPrivateKeySpecPass can't be blank");
        Validate.notNull(signatureForm, "signatureForm can't be null");
        Validate.notNull(signaturePackaging, "signaturePackaging can't be null");
        Validate.notNull(digestAlgorithm, "digestAlgorithm can't be null");

        this.signerPrivateKeySpec = new URI(signerPrivateKeySpec+ "?password=" + signerPrivateKeySpecPass);
        this.signatureForm = signatureForm;
        this.signaturePackaging = signaturePackaging;
        this.digestAlgorithm = digestAlgorithm;

        if (signatureLevel != null) {
            this.signatureLevel = signatureLevel;
        }

        if (timestampSourceSpec != null && StringUtils.isNotBlank(timestampSourceSpec)) {
            this.timestampSourceSpec = new URI(timestampSourceSpec);
        } else {
            this.timestampSourceSpec = null;
        }

        if (SignatureForm.CAdES.equals(signatureForm)) {
            setCadesSignatureLevel();
        }
        
        if (SignatureForm.XAdES.equals(signatureForm) ) {
        	setXadesSignatureLevel();
        	this.signaturePackaging=SignaturePackaging.ENVELOPED;
        }
    }

    public AbstractSignatureParameters getParameters(DSSPrivateKeyEntry signer, DSSDocument docToSign) {
        CertificateToken certificate = signer.getCertificate();
        CertificateToken[] certificateChain = signer.getCertificateChain();

        AbstractSignatureParameters parameters=null;
        if (SignatureForm.XAdES.equals(signatureForm)) {
	        XAdESSignatureParameters xadesParameters = new XAdESSignatureParameters();
	        
	        xadesParameters.setEmbedXML(true);
	        
	        List<DSSTransform> transforms = new ArrayList<DSSTransform>();
	        DSSTransform canonicalize = new DSSTransform();
	        canonicalize.setAlgorithm("http://www.w3.org/2001/10/xml-exc-c14n#");;
	        transforms.add(canonicalize);
	        
	        List<DSSReference> references=new ArrayList<DSSReference>();
	        DSSReference ref = new DSSReference();
	        ref.setContents(docToSign);
	        ref.setUri("#sigdoc");
	        ref.setDigestMethodAlgorithm(digestAlgorithm);
	    
	        
	        ref.setTransforms(transforms);
	        references.add(ref);	        
	        xadesParameters.setReferences(references);
	        
//	        byte[] signedConetnt=null;
//	        try {
//	        	signedConetnt=((InMemoryDocument)docToSign).getBytes();
//	        }catch (Exception e) {
//	        	throw new RuntimeException(e.getMessage(),e);
//	        }
//	        xadesParameters.setSignedAdESObject(signedConetnt);
	        
	        parameters=xadesParameters;
        } else {
	        parameters = new CAdESSignatureParameters();

        }
        parameters.setSignatureLevel(signatureLevel);
        parameters.setSignaturePackaging(signaturePackaging);
        parameters.setDigestAlgorithm(digestAlgorithm);
        parameters.setSigningCertificate(certificate);
        parameters.setCertificateChain(certificateChain);        

        return parameters;
    }

    private void setCadesSignatureLevel() {
        if (timestampSourceSpec != null) {
            signatureLevel = SignatureLevel.CAdES_BASELINE_T;
        } else {
            signatureLevel = SignatureLevel.CAdES_BASELINE_B;
        }
    }

    private void setXadesSignatureLevel() {
        if (timestampSourceSpec != null) {
            signatureLevel = SignatureLevel.XAdES_BASELINE_T;
        } else {
            signatureLevel = SignatureLevel.XAdES_BASELINE_B;
        }
    }

    
    public URI getSignerPrivateKeySpec() {
        return signerPrivateKeySpec;
    }

    public URI getTimestampSourceSpec() {
        return timestampSourceSpec;
    }

    public SignatureForm getSignatureForm() {
        return signatureForm;
    }

    public SignaturePackaging getSignaturePackaging() {
        return signaturePackaging;
    }

    public DigestAlgorithm getDigestAlgorithm() {
        return digestAlgorithm;
    }

    public SignatureLevel getSignatureLevel() {
        return signatureLevel;
    }
}
