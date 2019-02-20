package cz.com.spcss.ica;

import cz.com.spcss.model.IcaSignatureParameters;
import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import cz.ica.remoteseal.*;
import org.apache.commons.io.FilenameUtils;
import org.apache.commons.lang3.Validate;
import org.jetbrains.annotations.NotNull;

import java.io.FileNotFoundException;
import java.io.InputStream;
import java.util.Map;
import java.util.Properties;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

import org.apache.log4j.Logger;

public class IcaSigner {
    static {
        setProperties();
    }
    private static final Logger LOG = Logger.getLogger(IcaSigner.class);

    private static Properties properties;

    public SignResult signFile(@NotNull String inputPath, @NotNull String outputPath, @NotNull String documentId, @NotNull IcaSignatureParameters icaSignatureParameters) throws IOException {
        Validate.notBlank(inputPath, "inputPath can't be blank");
        Validate.notBlank(outputPath, "outputPath can't be blank");
        Validate.notBlank(documentId, "documentId can't be blank");
        Validate.notNull(icaSignatureParameters, "icaSignatureParameters can't be null");

        String logFilePath =  FilenameUtils.removeExtension(outputPath) + ".txt";

        try {
            RSeC client = new RSeC(properties.getProperty("lib"));
            client.InitializeLogging(logFilePath);
            InputData activationFile = new InputData(properties.getProperty("activationFile"));

            CadesOptions cadesOptions = new CadesOptions(activationFile,
                    icaSignatureParameters.getHashAlgorithm(), icaSignatureParameters.getAddTimestamp(), icaSignatureParameters.getCadesType());
            byte[] fileToSign = Files.readAllBytes(Paths.get(inputPath));
            byte[] sealedCades = client.SealCades(cadesOptions, fileToSign, documentId);
            Files.write(Paths.get(outputPath), sealedCades);
        } catch (RSeCException e) {
            return new SignResult(ResultCodes.NOOK, "Error when signing file " + e);
        }

        return new SignResult(ResultCodes.OK, "Sign file result code - OK");
    }

    private static void setProperties() {
        final InputStream propertiesStream = ClassLoader.getSystemClassLoader().getResourceAsStream("ica.properties");
        try {
            properties = new Properties();
            properties.load(propertiesStream);
            propertiesStream.close();
            for (Map.Entry<Object, Object> property : properties.entrySet()) {
                if (System.getenv().containsKey("AISG_" + property.getKey())) {
                    property.setValue(System.getenv().get(property.getKey()));
                }
            }
        } catch (FileNotFoundException e) {
            LOG.error("Properties file not found " + e);
        } catch (IOException e) {
            LOG.error("Properties IO exception " + e);
        }
    }
}
