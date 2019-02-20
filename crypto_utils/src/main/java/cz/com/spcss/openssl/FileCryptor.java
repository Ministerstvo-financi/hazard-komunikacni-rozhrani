package cz.com.spcss.openssl;


import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import org.apache.log4j.Logger;

import java.io.*;
import java.util.List;
import java.util.Map;
import java.util.Properties;

public class FileCryptor {
    static
    {
        setProperties();
    }

    private static final Logger LOG = Logger.getLogger(FileCryptor.class);

    private static Properties properties;


    public SignResult encryptFile(String inputPath, String outputPath, List<String> recipientCertificates) throws IOException {
        ProcessBuilder builder = new ProcessBuilder(properties.getProperty("openSSL"), "cms", "-encrypt", "-in", inputPath, "-binary", "-out", outputPath, "-outform", "DER", "-aes-256-cbc");
        for (String recipientCertificate : recipientCertificates) {
            builder.command().add("-recip");
            builder.command().add(recipientCertificate);
        }

        String errorMessage = runCommand(builder);
        if (errorMessage != null) {
            return new SignResult(ResultCodes.NOOK, "EncryptFile - NOOK " + errorMessage);
        }
        return new SignResult(ResultCodes.OK, "EncryptFile - OK");
    }

    public SignResult decryptFile(String inputPath, String outputPath, String keyPath, String recipientCertificate) throws IOException {
        ProcessBuilder builder = new ProcessBuilder(properties.getProperty("openSSL"), "cms", "-decrypt", "-in", inputPath, "-out", outputPath, "-inform", "DER", "-inkey", keyPath, "-recip", recipientCertificate);

        String errorMessage = runCommand(builder);
        if (errorMessage != null) {
            return new SignResult(ResultCodes.NOOK, "DecryptFile - NOOK " + errorMessage);
        }
        return new SignResult(ResultCodes.OK, "DecryptFile - OK");
    }

    private String runCommand(ProcessBuilder builder) throws IOException {
        builder.redirectErrorStream(true);
        Process process = builder.start();

        BufferedReader r = new BufferedReader(new InputStreamReader(process.getInputStream()));
        String line;
        StringBuilder error = new StringBuilder();
        while (true) {
            line = r.readLine();
            if (line == null) {
                break;
            }
            error.append(line);
        }
        if (error.toString().equals("")) {
            return null;
        }
        return error.toString();
    }

    private static void setProperties() {
        final InputStream propertiesStream = ClassLoader.getSystemClassLoader().getResourceAsStream("openssl.properties");
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
