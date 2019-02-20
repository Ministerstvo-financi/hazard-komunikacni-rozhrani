package cz.com.spcss.dss;

import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import eu.europa.esig.dss.DSSException;
import org.junit.Before;
import org.junit.Test;

import javax.xml.bind.JAXBException;
import javax.xml.transform.TransformerException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import static org.hamcrest.Matchers.samePropertyValuesAs;
import static org.junit.Assert.assertThat;


public class DssValidatorTest {
    private final String inputFile1 = "src/test/resources/signedFiles/dss-test-signed-cades-baseline-b.pkcs7";
    private final String inputFile2 = "src/test/resources/signedFiles/testdoc.p7m";
    private final String inputFile3 = "src/test/resources/signedFiles/2A35D77823E3EA4EB41F6FC620A95FE6.pdf";
    private final String inputFile4 = "src/test/resources/signedFiles/vypis-0.pdf";

    private final String certPath1 = "src/test/resources/keystore/ec.europa.eu.1.cer";
    private final String certPath2 = "src/test/resources/keystore/ec.europa.eu.2.cer";
    private final String certPath3 = "src/test/resources/keystore/2A35D77823E3EA4EB41F6FC620A95FE6.cer";

    private final String outputFile = "target/result.txt";
    private List<String> certificateFiles = new ArrayList<>();;
    private List<String> certificateFilesForDoc = new ArrayList<>();;

    @Before
    public void setPaths() {
        certificateFiles.add(certPath1);
        certificateFiles.add(certPath2);

        certificateFilesForDoc.add(certPath3);
    }

    @Test
    public void testIsSignedNOOK1() throws IOException, JAXBException, TransformerException {
        SignResult signResult = validateTask(inputFile1, outputFile, certificateFiles);
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.NOOK, ResultCodes.ERR_PKG_SIG_INVALID_SIGNATURE.toString())));
    }

    @Test
    public void testIsSignedNOOK2() throws IOException, JAXBException, TransformerException {
        SignResult signResult = validateTask(inputFile2, outputFile, certificateFiles);
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.NOOK, ResultCodes.ERR_PKG_SIG_INVALID_SIGNATURE.toString())));
    }

    @Test(expected = DSSException.class)
    public void testIsSigned3() throws IOException, JAXBException, TransformerException {
        SignResult signResult = validateTask(inputFile3, outputFile, certificateFilesForDoc);
        //assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.NOOK, ResultCodes.INFO_PKG_SIG_CERT_OK.toString())));
    }

    @Test
    public void testCertificateOK1() throws JAXBException, TransformerException, IOException {
        SignResult signResult = validateCertificateTask(certificateFilesForDoc.get(0));
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.OK, ResultCodes.INFO_PKG_SIG_CERT_OK.toString())));
    }

    @Test(expected = DSSException.class)
    public void testCertificateUnableLoadCert() throws JAXBException, TransformerException, IOException {
        SignResult signResult = validateCertificateTask(inputFile4);
    }

    @Test
    public void testCertificateOK2() throws JAXBException, TransformerException, IOException {
        SignResult signResult = validateCertificateTask(certificateFilesForDoc.get(0));
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.OK, ResultCodes.INFO_PKG_SIG_CERT_OK.toString())));
    }

    @Test
    public void testCertificateUNTRUSTED() throws JAXBException, TransformerException, IOException {
        SignResult signResult = validateCertificateTask(inputFile1);
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.NOOK, ResultCodes.ERR_PKG_SIG_CERT_UNTRUSTED.toString())));
    }

    private SignResult validateTask(String input, String output, List<String> certificateFiles) throws IOException, JAXBException, TransformerException {
        DssValidator dssValidator = new DssValidator();
        return dssValidator.validateFile(input, output, certificateFiles);
    }

    private SignResult validateCertificateTask(String input) throws TransformerException, JAXBException, IOException {
        DssValidator dssValidator = new DssValidator();
        return dssValidator.validateCertificate(input);
    }
}
