package com.cgi.spcss.cli;

import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import cz.com.spcss.openssl.FileCryptor;
import cz.com.spcss.utilities.CryptoUtilities;
import org.apache.commons.cli.CommandLine;
import org.apache.commons.lang3.StringUtils;
import org.apache.log4j.Logger;

import java.io.IOException;
import java.util.Arrays;
import java.util.List;

public class OpenSSLCommands {
    private static final Logger LOG = Logger.getLogger(OpenSSLCommands.class);

    public SignResult encryptFile(CommandLine cmd) throws InterruptedException{
        LOG.info("Starting encryptFile");
        String inputFile;
        String outputFile;
        String[] certFiles;
        try {
            inputFile = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValue("inputFile"));
            outputFile = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValue("outputFile"));
            certFiles = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValues("certificate"));
        }
        catch (IOException e){
            return new SignResult(ResultCodes.NOOK, "EncryptFile IOException - Invalid argument values " + e);
        }

        if(StringUtils.isNotBlank(inputFile) && StringUtils.isNotBlank(outputFile) && certFiles.length != 0) {
            List<String> certificateFiles = Arrays.asList(certFiles);
            FileCryptor fileCryptor = new FileCryptor();
            try {
                return fileCryptor.encryptFile(inputFile, outputFile, certificateFiles);
            } catch (IOException e) {
                return new SignResult(ResultCodes.NOOK, "EncryptFile IOException " + e);
            }
        }
        return new SignResult(ResultCodes.NOOK, "Invalid argument values for encryptFile");
    }

    public SignResult decryptFile(CommandLine cmd) throws InterruptedException{
        LOG.info("Starting decryptFile");
        String inputFile;
        String outputFile;
        String keyFile;
        String certFiles;
        try {
            inputFile = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValue("inputFile"));
            outputFile = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValue("outputFile"));
            keyFile = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValue("keyFile"));
            certFiles = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValue("certificate"));
        }
        catch (IOException e){
            return new SignResult(ResultCodes.NOOK, "DecryptFile IOException - Invalid argument values " + e);
        }

        if(StringUtils.isNotBlank(inputFile) && StringUtils.isNotBlank(outputFile) && StringUtils.isNotBlank(keyFile) && StringUtils.isNotBlank(certFiles)) {
            FileCryptor fileCryptor = new FileCryptor();
            try {
                return fileCryptor.decryptFile(inputFile, outputFile, keyFile, certFiles);
            } catch (IOException e) {
                return new SignResult(ResultCodes.NOOK, "DecryptFile IOException " + e);
            }
        }
        return new SignResult(ResultCodes.NOOK, "Invalid argument values for decryptFile");
    }
}
