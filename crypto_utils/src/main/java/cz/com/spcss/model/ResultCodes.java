package cz.com.spcss.model;

public enum ResultCodes {
    OK,
    NOOK,

    //ERR_PKG_SIG_INVALID_DER – pokud není možné přečíst ASN.1 DER struktury balíčku
    ERR_PKG_SIG_INVALID_DER,
    //ERR_PKG_SIG_INVALID_SIGNATURE – pokud nesouhlasí kryptografické ověření podpisu
    ERR_PKG_SIG_INVALID_SIGNATURE,
    //ERR_PKG_SIG_CERT_UNTRUSTED – pokud není možné ověřit platnost certifikátu
    ERR_PKG_SIG_CERT_UNTRUSTED,
    //ERR_PKG_SIG_CERT_UNKNOWN – pokud certifikát není mezi registrovanými certifikáty operátora
    ERR_PKG_SIG_CERT_UNKNOWN,

    INFO_PKG_SIG_CERT_OK, 
    ERR_CERT_PARSE,
    ERR_CERT_CHAIN_OTHER,
    ERR_CERT_NO_CHAIN,
    ERR_CERT_ROOT_MISSING,
    ERR_CERT_EUTL_INVALID,
    ERR_CERT_EUTL_INVALID_OTH,
    ERR_CERT_NOT_CA,
    ERR_VALID_CERT_NOT_FOR_QSEAL
    
}
