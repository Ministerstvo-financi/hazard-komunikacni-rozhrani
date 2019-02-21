package cz.com.spcss.openssl;

import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import static org.hamcrest.Matchers.samePropertyValuesAs;
import static org.junit.Assert.assertThat;

public class FileCryptorTest {

    private final String inputEncryptPath = "src/test/resources/testFiles/myDoc.txt";
    private final String outputEncryptPath = "encrypted-rsa.p7e";

    private final String inputDecryptPath = "src/test/resources/testFiles/encrypted-rsa.p7e";
    private final String outputDecryptPath = "myDoc-rsa-dec.txt";
    private final String keyPath = "src/test/resources/testFiles/rsa.key";

    private List<String> certificateFiles;

    @Before
    public void setPaths() {
        String certPath1 = "src/test/resources/testFiles/rsa.pem";
        String certPath2 = "src/test/resources/testFiles/rsa2.pem";

        certificateFiles = new ArrayList<>();
        certificateFiles.add(certPath1);
        certificateFiles.add(certPath2);
    }

    @Test
    public void testEncryptOK() throws IOException {
        SignResult signResult = encryptTest(inputEncryptPath, outputEncryptPath, certificateFiles);
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.OK, "EncryptFile - OK")));
    }

    @Test
    public void testEncryptNOOK() throws IOException {
        SignResult signResult = encryptTest(inputEncryptPath + "FAIL", outputEncryptPath, certificateFiles);
        assertThat(signResult.getResultCode(), samePropertyValuesAs(new SignResult(ResultCodes.NOOK, "EncryptFile - NOOK").getResultCode()));
    }

    @Test
    public void testDecryptOK() throws IOException {
        SignResult signResult =  decryptTest(inputDecryptPath, outputDecryptPath, keyPath, certificateFiles.get(0));
        assertThat(signResult, samePropertyValuesAs(new SignResult(ResultCodes.OK, "DecryptFile - OK")));
    }

    @Test
    public void testDecryptNOOK() throws IOException {
        SignResult signResult =  decryptTest(inputDecryptPath, outputDecryptPath, keyPath + "FAIL", certificateFiles.get(0));
        assertThat(signResult.getResultCode(), samePropertyValuesAs(new SignResult(ResultCodes.NOOK, "DecryptFile - NOOK").getResultCode()));
    }


    private SignResult encryptTest(String inputPath, String outputPath, List<String> certificateFiles) throws IOException {
        FileCryptor fileCryptor = new FileCryptor();
        return fileCryptor.encryptFile(inputPath, outputPath, certificateFiles);
    }

    private SignResult decryptTest(String inputPath, String outputPath, String keyPath, String certificateFile) throws IOException {
        FileCryptor fileCryptor = new FileCryptor();
        return fileCryptor.decryptFile(inputPath, outputPath, keyPath, certificateFile);
    }
}