package cz.com.spcss.ica;

import cz.com.spcss.model.IcaSignatureParameters;
import cz.ica.remoteseal.CadesOptions;
import cz.ica.remoteseal.CommonOptions;
import org.junit.Test;
import java.io.IOException;

public class IcaSignerTest {

    private final CadesOptions.CadesType cadesType = CadesOptions.CadesType.ctInternal;
    private final CommonOptions.HashAlgorithm hashAlgorithm = CommonOptions.HashAlgorithm.haSHA256;
    private final String inputPath = "src/test/resources/test.txt";
    private final String ouputPath = "target/testdoc.p7m";
    private final String documentId = "Doc1";

    @Test
    public void testIsSigned() throws IOException {
        signTest();
    }

    private void signTest() throws IOException {
        IcaSignatureParameters icaSignatureParameters = new IcaSignatureParameters(cadesType,
                hashAlgorithm,
                false);
        IcaSigner icaSigner = new IcaSigner();
        icaSigner.signFile(inputPath, ouputPath, documentId, icaSignatureParameters);
    }
}
