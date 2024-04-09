package cz.com.spcss.openssl;


import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import org.apache.log4j.Logger;
import org.bouncycastle.crypto.tls.BulkCipherAlgorithm;

import java.io.*;
import java.net.URI;
import java.net.URL;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Duration;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.regex.Pattern;
import java.util.regex.PatternSyntaxException;

public class FileCryptor {
    static
    {
        setProperties();
    }

    private static final Logger LOG = Logger.getLogger(FileCryptor.class);

    private static Properties properties;
    
    private final HttpClient httpClient = HttpClient.newBuilder().build();

    public SignResult encryptFile(String inputPath, String outputPath, List<String> recipientCertificates) throws IOException, InterruptedException {
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

    public SignResult decryptFile(String inputPath, String outputPath, String keyPath, String recipientCertificate) throws IOException, InterruptedException {

    	String inputFile = null;
    	String useHsmProp = null;
    	Pattern useHsmRegex = null;
    	boolean useHsm = false;
    	try {
    		Path inputPathPath = Path.of(inputPath);
    		inputFile = inputPathPath.getFileName().toString();
	    	useHsmProp = properties.getProperty("USE_HSM");
	    	String useHsmRegexStr  = properties.getProperty("USE_HSM_FILTER_REGEX");
	    	useHsmRegex = useHsmRegexStr!=null?Pattern.compile(useHsmRegexStr):Pattern.compile(".*");
			String hsmServiceBaseUrlStr=properties.getProperty("HSM_SERVICE_BASE_URL");
			URI hsmServiceBaseUrl = hsmServiceBaseUrlStr!=null?URI.create(hsmServiceBaseUrlStr):null;
	    	if ("true".equals(useHsmProp.toLowerCase()) && hsmServiceBaseUrl!=null ) {
	    		useHsm = useHsmRegex.matcher(inputFile).find();
	    	}
	    	
    	} catch (Exception e) {
    		LOG.warn("Exception while deciding whether to use HSM for decryption - falling back to local decryption",e);
    	}

    	if (useHsm) {
        	return decryptFileHsm(inputPath,outputPath,keyPath, recipientCertificate);    			
    	}
    	return decryptFileLocal(inputPath, outputPath, keyPath, recipientCertificate);    	
    }
    
    
    public SignResult decryptFileHsm(String inputPath, String outputPath, String keyPath, String recipientCertificate) throws IOException, InterruptedException {
    	LOG.info(String.format("Using HSM service to decrypt file %s", inputPath));
		Path inputPathPath = Path.of(inputPath);
		Path outPathPath = Path.of(outputPath);
		String inputFile = inputPathPath.getFileName().toString();
		String hsmServiceBaseUrlStr=properties.getProperty("HSM_SERVICE_BASE_URL");
		URI hsmServiceBaseUrl =  URI.create(hsmServiceBaseUrlStr).resolve(inputFile);
		LOG.info(String.format("Sending command to HSM decrypt service: %s", hsmServiceBaseUrl.toString()));
		HttpRequest req = HttpRequest.newBuilder(hsmServiceBaseUrl).GET().timeout(Duration.ofMinutes(2)).build();
		HttpResponse<String> response = httpClient.send(req, HttpResponse.BodyHandlers.ofString());
		boolean outputExists = Files.exists(outPathPath); 
		if ( response.statusCode() == 200 && outputExists ) {
	    	LOG.info(String.format("File decrypted ok using HSM: %s", inputPath));
	        return new SignResult(ResultCodes.OK, "DecryptFile HSM - OK");
		}
		LOG.error(String.format("failed to decrypt file using HSM. status code: %d, outputExist: %b, response: %s",response.statusCode(), outputExists, response.body()));
        return new SignResult(ResultCodes.NOOK_HSM, "DecryptFile HSM - NOOK " + response.body());
    }

    
    
    public SignResult decryptFileLocal(String inputPath, String outputPath, String keyPath, String recipientCertificate) throws IOException, InterruptedException {
        if (!validateDerCertificate(recipientCertificate))
            return new SignResult(ResultCodes.ERR_PKG_ENC_INVALID_DER, "Error while reading ANS1 DER structures");

        ProcessBuilder builder = new ProcessBuilder(properties.getProperty("openSSL"), "cms", "-binary", "-decrypt", "-in", inputPath, "-out", outputPath, "-inform", "DER", "-inkey", keyPath, "-recip", recipientCertificate);

        String errorMessage = runCommand(builder);
        if (errorMessage != null) {
            return new SignResult(ResultCodes.NOOK, "DecryptFile - NOOK " + errorMessage);
        }
        return new SignResult(ResultCodes.OK, "DecryptFile - OK");
    }

    protected boolean validateDerCertificate(String recipientCertificate) throws IOException, InterruptedException {
        ProcessBuilder builder = new ProcessBuilder(properties.getProperty("openSSL"), "asn1parse", "-inform", "DER", "-in", recipientCertificate, "-strictpem");

        String errorMessage = runCommand(builder);
        LOG.info(errorMessage);
        return errorMessage == null;
    }

    public static String runCommand(ProcessBuilder builder) throws IOException, InterruptedException {
        builder.redirectErrorStream(true);

        Process process = builder.start();
        BufferedReader r = new BufferedReader(new InputStreamReader(process.getInputStream(), StandardCharsets.UTF_8));
        String line;
        StringBuilder error = new StringBuilder();
        while (true) {
            line = r.readLine();
            if (line == null) {
                break;
            }
            error.append(line).append(System.getProperty("line.separator"));
        }
        if (process.waitFor() == 0) {
            return null;
        }
        return error.toString();
    }

    private static void setProperties() {
        final InputStream propertiesStream = FileCryptor.class.getResourceAsStream("/openssl.properties");
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
