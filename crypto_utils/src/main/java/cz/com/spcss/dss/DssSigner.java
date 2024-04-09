package cz.com.spcss.dss;

import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import cz.com.spcss.model.DssSignatureParameters;
import cz.com.spcss.utilities.CryptoUtilities;
import eu.europa.esig.dss.cades.signature.CAdESService;
import eu.europa.esig.dss.signature.DocumentSignatureService;
import org.jetbrains.annotations.NotNull;
import eu.europa.esig.dss.*;
import eu.europa.esig.dss.token.*;
import eu.europa.esig.dss.utils.Utils;
import eu.europa.esig.dss.validation.CommonCertificateVerifier;
import eu.europa.esig.dss.xades.signature.XAdESService;

import org.apache.commons.lang3.Validate;
import org.apache.log4j.Logger;

import eu.europa.esig.dss.AbstractSignatureParameters;
import eu.europa.esig.dss.DSSDocument;
import eu.europa.esig.dss.SignatureValue;
import eu.europa.esig.dss.ToBeSigned;


import java.io.FileInputStream;
import java.io.IOException;
import java.net.URI;
import java.security.KeyStore;
import java.util.List;

public class DssSigner {
    private static final Logger LOG = Logger.getLogger(DssSigner.class);
	
    private DSSDocument toSignDocument;
    private DSSDocument signedDocument;
    private SignatureTokenConnection signingToken;
    private AbstractSignatureParameters parameters;


    public SignResult signFile(@NotNull String inputFile, @NotNull String outputFile, @NotNull DssSignatureParameters dssSignatureParameters) throws IOException {
        Validate.notBlank(inputFile, "inputFile can't be blank");
        Validate.notBlank(outputFile, "outputFile can't be blank");
        Validate.notNull(dssSignatureParameters, "dssSignatureParameters can't be null");

        toSignDocument = new InMemoryDocument(new FileInputStream(inputFile));
        signingToken = getToken(dssSignatureParameters.getSignerPrivateKeySpec());
        DSSPrivateKeyEntry signer = getSigner(signingToken.getKeys());
        parameters = dssSignatureParameters.getParameters(signer, toSignDocument);

        try {
            DocumentSignatureService service = getSignatureService(dssSignatureParameters.getSignatureForm());
            ToBeSigned dataToSign = service.getDataToSign(toSignDocument, parameters);
            DigestAlgorithm digestAlgorithm = parameters.getDigestAlgorithm();
            SignatureValue signatureValue = signingToken.sign(dataToSign, digestAlgorithm, signer);
            signedDocument = (DSSDocument) service.signDocument(toSignDocument, parameters, signatureValue);
            signedDocument.save(outputFile);
        } catch (DSSException e) {
        	LOG.error("Exception wjhile signing",e);
            return new SignResult(ResultCodes.NOOK, "Error when signing file " + e);
        }

        return new SignResult(ResultCodes.OK, "Sign file result code - OK");
    }

    private DSSPrivateKeyEntry getSigner(List<DSSPrivateKeyEntry> keys) {
        if (Utils.isCollectionEmpty(keys)) {
            throw new IllegalStateException("No certificate found");
        }
        return keys.get(0);
    }

    private SignatureTokenConnection getToken(URI uri) throws IOException {
        return new Pkcs12SignatureToken(uri.getPath(), new KeyStore.PasswordProtection(CryptoUtilities.splitQuery(uri.getQuery()).get("password").toCharArray()));
    }

    private DocumentSignatureService getSignatureService(SignatureForm signatureForm) {
        switch (signatureForm) {
            case CAdES:
                return new CAdESService(new CommonCertificateVerifier());
            case XAdES:
                return new XAdESService(new CommonCertificateVerifier());            	
            default:
                throw new IllegalArgumentException("Unsupported signatureForm " + signatureForm);
        }

    }
}
