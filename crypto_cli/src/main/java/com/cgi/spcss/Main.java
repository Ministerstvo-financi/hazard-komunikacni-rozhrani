package com.cgi.spcss;

import com.cgi.spcss.cli.DssCommands;
import com.cgi.spcss.cli.OpenSSLCommands;
import cz.com.spcss.model.SignResult;
import org.apache.commons.cli.*;
import org.apache.log4j.Logger;

public class Main {

    private static final Logger LOG = Logger.getLogger(Main.class);

    // -f validateCertificate -i C:/testFiles/keystore/ec.europa.eu.1.cer
    // -f validateFile -i C:/testFiles/signedFiles/dss-test-signed-cades-baseline-b.pkcs7 -o C:/testFiles/target/result.txt -cert C:/testFiles/keystore/ec.europa.eu.1.cer -cert C:/testFiles/keystore/ec.europa.eu.2.cer
    // -f signFile -i C:/testFiles/test.txt -o C:/testFiles/target/dss-test-signed-cades-baseline-b.pkcs7 -spks C:/testFiles/user_a_rsa.p12 -pass password
    // -f encryptFile -i C:/testFiles/testFiles/myDoc.txt -o C:/testFiles/encrypted-rsa.p7e -cert C:/testFiles/testFiles/rsa.pem -cert C:/testFiles/testFiles/rsa2.pem
    // -f decryptFile -i C:/testFiles/testFiles/encrypted-rsa.p7e -o C:/testFiles/myDoc-rsa-dec.txt -k C:/testFiles/testFiles/rsa.key -cert C:/testFiles/testFiles/rsa.pem

    public static void main(String[] args){
        Options options = new Options();

        Option functionOption = new Option("f", "functionName", true, "function name");
        functionOption.setRequired(true);
        options.addOption(functionOption);

        Option inputOption = new Option("i", "inputFile", true, "Path to input file");
        inputOption.setRequired(true);
        options.addOption(inputOption);

        Option outputOption = new Option("o", "outputFile", true, "Path to output file");
        outputOption.setRequired(false);
        options.addOption(outputOption);

        Option keyOption = new Option("k", "keyFile", true, "Path to key file");
        keyOption.setRequired(false);
        options.addOption(keyOption);

        Option certificateOption = new Option("cert", "certificate", true, "tested certificate");
        certificateOption.setRequired(false);
        certificateOption.setArgs(Option.UNLIMITED_VALUES);
        options.addOption(certificateOption);

        Option signerPrivateKeySpecOption = new Option("spks", "signerPrivateKeySpec", true, "signerPrivateKeySpec URI");
        signerPrivateKeySpecOption.setRequired(false);
        options.addOption(signerPrivateKeySpecOption);

        Option signerPrivateKeySpecPass = new Option("pass", "password", true, "signerPrivateKeySpec password");
        signerPrivateKeySpecPass.setRequired(false);
        options.addOption(signerPrivateKeySpecPass);

        CommandLineParser parser = new DefaultParser();
        HelpFormatter formatter = new HelpFormatter();
        CommandLine cmd;

        try {
            cmd = parser.parse(options, args);
        } catch (ParseException e) {
            LOG.error(e.getMessage());
            formatter.printHelp("CRYPTO UTILS", options);
            System.exit(1);
            return;
        }

        DssCommands dssCommands = new DssCommands();
        OpenSSLCommands openSSLCommands = new OpenSSLCommands();
        SignResult signResult;
        switch (cmd.getOptionValue("functionName")) {
            case "validateCertificate":
                signResult = dssCommands.validateCertificate(cmd);
                LOG.info("ValidateCertificate " +  signResult.getResultMessage());
                break;
            case "validateFile":
                signResult = dssCommands.validateFile(cmd);
                LOG.info("ValidateFile " +  signResult.getResultMessage());
                break;
            case "signFile":
                signResult = dssCommands.signFile(cmd);
                LOG.info(signResult.getResultMessage());
                break;
            case "encryptFile":
                signResult = openSSLCommands.encryptFile(cmd);
                LOG.info(signResult.getResultMessage());
                break;
            case "decryptFile":
                signResult = openSSLCommands.decryptFile(cmd);
                LOG.info(signResult.getResultMessage());
                break;
            default:
                LOG.info("Unsupported function "  + cmd.getOptionValue("functionOption"));
                System.exit(1);
                break;
        }
        System.exit(0);
    }
}
