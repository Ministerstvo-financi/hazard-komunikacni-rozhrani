package cz.com.spcss.model;

import cz.ica.remoteseal.CadesOptions;
import cz.ica.remoteseal.CommonOptions;

public class IcaSignatureParameters {

    private CadesOptions.CadesType cadesType;
    private CommonOptions.HashAlgorithm hashAlgorithm;
    private boolean addTimestamp;

    public IcaSignatureParameters(CadesOptions.CadesType cadesType, CommonOptions.HashAlgorithm hashAlgorithm, boolean addTimestamp) {
        this.cadesType = cadesType;
        this.hashAlgorithm = hashAlgorithm;
        this.addTimestamp = addTimestamp;
    }

    public CadesOptions.CadesType getCadesType() {
        return cadesType;
    }

    public CommonOptions.HashAlgorithm getHashAlgorithm() {
        return hashAlgorithm;
    }

    public boolean getAddTimestamp() {
        return addTimestamp;
    }

}
