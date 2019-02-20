package cz.com.spcss.model;

import eu.europa.esig.dss.cades.CAdESSignatureParameters;
import eu.europa.esig.dss.token.DSSPrivateKeyEntry;
import eu.europa.esig.dss.x509.CertificateToken;
import org.apache.commons.lang3.StringUtils;
import org.jetbrains.annotations.NotNull;
import eu.europa.esig.dss.*;
import org.apache.commons.lang3.Validate;
import org.jetbrains.annotations.Nullable;

import java.net.URI;
import java.net.URISyntaxException;

public class DssSignatureParameters {

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

        if (signatureForm == SignatureForm.CAdES) {
            setCadesSignatureLevel();
        }
    }

    public AbstractSignatureParameters getParameters(DSSPrivateKeyEntry signer) {
        CertificateToken certificate = signer.getCertificate();
        CertificateToken[] certificateChain = signer.getCertificateChain();

        AbstractSignatureParameters parameters = new CAdESSignatureParameters();
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
