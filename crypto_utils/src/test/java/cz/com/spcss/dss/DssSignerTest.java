package cz.com.spcss.dss;

import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import cz.com.spcss.model.DssSignatureParameters;
import eu.europa.esig.dss.DigestAlgorithm;
import eu.europa.esig.dss.SignatureForm;
import eu.europa.esig.dss.SignaturePackaging;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;
import java.net.URISyntaxException;

import static org.hamcrest.Matchers.samePropertyValuesAs;
import static org.junit.Assert.assertThat;


public class DssSignerTest {

    private final String inputPath = "src/test/resources/test.txt";
    private final String inputPathXml = "src/test/resources/test.xml";
    private final String outputPath = "target/dss-test-signed-cades-baseline-b.pkcs7";
    private final String outputPathXml = "target/dss-test-signed-xades-baseline-b.xml";
    private final String timestampSourceSpec = null;
    private SignatureForm signatureForm = SignatureForm.CAdES;
    private SignatureForm signatureFormXades = SignatureForm.XAdES;
    private SignaturePackaging signaturePackaging = SignaturePackaging.ENVELOPING;
    private SignaturePackaging signaturePackagingXades = SignaturePackaging.ENVELOPING;
    private DigestAlgorithm digestAlgorithm = DigestAlgorithm.SHA256;

    private String signerPrivateKeySpec;
    private String signerPrivateKeySpecPass;

    @Before
    public void setPaths() {
        ClassLoader classLoader = getClass().getClassLoader();
        String certPath = classLoader.getResource("user_a_rsa.p12").getPath();
        signerPrivateKeySpec = certPath;
        signerPrivateKeySpecPass = "password";
    }


    @Test
    public void testIsSigned() throws IOException, URISyntaxException {
        DssSignatureParameters dssSignatureParameters = new DssSignatureParameters(signerPrivateKeySpec, signerPrivateKeySpecPass, timestampSourceSpec, signatureForm, signaturePackaging, digestAlgorithm, null);
        SignResult signResult = singTest(inputPath, outputPath, dssSignatureParameters);
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.OK, "Sign file result code - OK")));
    }

    @Test
    public void testIsSignedXades() throws IOException, URISyntaxException {
        DssSignatureParameters dssSignatureParameters = new DssSignatureParameters(signerPrivateKeySpec, signerPrivateKeySpecPass, timestampSourceSpec, signatureFormXades, signaturePackaging, digestAlgorithm, null);
        SignResult signResult = singTest(inputPathXml, outputPathXml, dssSignatureParameters);
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.OK, "Sign file result code - OK")));
    }


    @Test(expected = IOException.class)
    public void testWrongFileInput() throws IOException, URISyntaxException {
        DssSignatureParameters dssSignatureParameters = new DssSignatureParameters("X", signerPrivateKeySpecPass, timestampSourceSpec, signatureForm, signaturePackaging, digestAlgorithm, null);
        singTest("X", "X", dssSignatureParameters);
    }

    @Test(expected = IllegalArgumentException.class)
    public void testBlankInput() throws IOException, URISyntaxException {
        DssSignatureParameters dssSignatureParameters = new DssSignatureParameters("", signerPrivateKeySpecPass, "", signatureForm, signaturePackaging, digestAlgorithm, null);
        singTest("", "", dssSignatureParameters);
    }

    private SignResult singTest(String inputPath, String outputPath, DssSignatureParameters dssSignatureParameters) throws IOException {
        DssSigner dssSigner = new DssSigner();
        return dssSigner.signFile(inputPath, outputPath, dssSignatureParameters);
    }
}
