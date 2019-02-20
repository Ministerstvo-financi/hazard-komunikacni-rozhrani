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

    INFO_PKG_SIG_CERT_OK;
}
