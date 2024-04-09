package com.cgi.spcss.cli;

import cz.com.spcss.dss.DssSigner;
import cz.com.spcss.dss.DssValidator;
import cz.com.spcss.model.DssSignatureParameters;
import cz.com.spcss.model.ResultCodes;
import cz.com.spcss.model.SignResult;
import cz.com.spcss.utilities.CryptoUtilities;
import eu.europa.esig.dss.DigestAlgorithm;
import eu.europa.esig.dss.SignatureForm;
import eu.europa.esig.dss.SignaturePackaging;
import org.apache.commons.cli.CommandLine;
import org.apache.commons.lang3.StringUtils;
import org.apache.log4j.Logger;

import javax.xml.bind.JAXBException;
import javax.xml.transform.TransformerException;
import java.io.*;
import java.net.URISyntaxException;
import java.util.Arrays;
import java.util.List;

public class DssCommands {
    private static final Logger LOG = Logger.getLogger(DssCommands.class);

    private static final SignatureForm SIGNATURE_FORM = SignatureForm.CAdES;
    private static final SignatureForm SIGNATURE_FORM_XML = SignatureForm.XAdES;
    private static final SignaturePackaging SIGNATURE_PACKAGING_CADES = SignaturePackaging.ENVELOPING;
    private static final SignaturePackaging SIGNATURE_PACKAGING_XADES = SignaturePackaging.ENVELOPING;
    private static final DigestAlgorithm DIGEST_ALGORITHM = DigestAlgorithm.SHA256;

    public SignResult validateCertificate(CommandLine cmd){
        LOG.info("Starting validateCertificate");
        String certPath;

        try {
            certPath = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("inputFile"));
        }
        catch (IOException e){
            return new SignResult(ResultCodes.NOOK, "ValidateCertificate IOException - Invalid argument values " + e);
        }

        if(StringUtils.isNotBlank(certPath)) {
            DssValidator dssValidator = new DssValidator();
            try {
                return dssValidator.validateCertificate(certPath);
            } catch (IOException e) {
                return new SignResult(ResultCodes.NOOK, "ValidateCertificate IOException " + e);
            } catch (JAXBException e) {
                return new SignResult(ResultCodes.NOOK, "ValidateCertificate JAXBException " + e);
            } catch (TransformerException e) {
                return new SignResult(ResultCodes.NOOK, "ValidateCertificate TransformerException " + e);
            }
        }
        return new SignResult(ResultCodes.NOOK, "Invalid argument values for validateCertificate");
    }

    public SignResult validateFile(CommandLine cmd){
        LOG.info("Starting validateFile");
        String inputFile;
        String outputFile;
        String[] certFiles;

        try {
            inputFile = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("inputFile"));
            outputFile = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("outputFile"));
            certFiles = CryptoUtilities.getInputOrInputFromFile(cmd.getOptionValues("certificate"));
        }
        catch (IOException e){
            return new SignResult(ResultCodes.NOOK, "ValidateFile IOException - Invalid argument values " + e);
        }

        if(StringUtils.isNotBlank(inputFile) && StringUtils.isNotBlank(outputFile) && certFiles.length != 0) {
            DssValidator dssValidator = new DssValidator();
            List<String> certificateFiles = Arrays.asList(certFiles);
            try {
                return dssValidator.validateFile(inputFile, outputFile, certificateFiles);
            } catch (IOException e) {
                return new SignResult(ResultCodes.NOOK, "ValidateFile IOException " + e);
            } catch (JAXBException e) {
                return new SignResult(ResultCodes.NOOK, "ValidateFile JAXBException " + e);
            } catch (TransformerException e) {
                return new SignResult(ResultCodes.NOOK, "ValidateFile TransformerException " + e);
            }
        }
        return new SignResult(ResultCodes.NOOK, "Invalid argument values for validateFile");
    }

    public SignResult signFile(CommandLine cmd){
        LOG.info("Starting signFile");
        String inputFile;
        String outputFile;
        String signerPrivateKeySpecPass;
        String signerPrivateKeySpec;
        String timestampSourceSpec;
        String format;
        try {
            inputFile = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("inputFile"));
            outputFile = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("outputFile"));
            signerPrivateKeySpec = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("signerPrivateKeySpec"));
            signerPrivateKeySpecPass = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("password"));
            timestampSourceSpec = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("timestampSourceSpec"));
            format = CryptoUtilities.getInputOrInputFromFileAndStrip(cmd.getOptionValue("format"));
            if (format==null) {
            	format=DssSignatureParameters.CADES;
            }
            LOG.info(String.format("Format: %s", format));
        }
        catch (IOException e){
        	LOG.info("Exception while signing",e);
            return new SignResult(ResultCodes.NOOK, "SignFile IOException - Invalid argument values " + e);
        }

        if(StringUtils.isNotBlank(inputFile) && StringUtils.isNotBlank(outputFile) && StringUtils.isNotBlank(signerPrivateKeySpecPass) && StringUtils.isNotBlank(signerPrivateKeySpec)) {
            DssSigner dssSigner = new DssSigner();
            try {
            	SignatureForm signatureFrom = SignatureForm.CAdES;
            	SignaturePackaging signaturePackaging = SIGNATURE_PACKAGING_CADES;
            	if (DssSignatureParameters.XADES.equals(format)) {
            		signatureFrom=SignatureForm.XAdES;
            		signaturePackaging=SIGNATURE_PACKAGING_XADES;
            	}
                DssSignatureParameters dssSignatureParameters = new DssSignatureParameters(signerPrivateKeySpec, signerPrivateKeySpecPass, timestampSourceSpec, signatureFrom, signaturePackaging, DIGEST_ALGORITHM, null);
                return dssSigner.signFile(inputFile, outputFile, dssSignatureParameters);
            } catch (IOException e) {
                return new SignResult(ResultCodes.NOOK, "SignFile IOException " + e);
            } catch (URISyntaxException e) {
                return new SignResult(ResultCodes.NOOK, "SignFile URISyntaxException " + e);
            }
        }
        return new SignResult(ResultCodes.NOOK, "Invalid argument values for signFile");
    }
}
