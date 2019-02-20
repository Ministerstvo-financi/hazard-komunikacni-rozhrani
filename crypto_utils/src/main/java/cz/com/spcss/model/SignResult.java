package cz.com.spcss.model;

public class SignResult {

    private ResultCodes resultCode;
    private String resultMessage;

    public SignResult(ResultCodes resultCode, String resultMessage) {
        this.resultCode = resultCode;
        this.resultMessage = resultMessage;
    }

    public ResultCodes getResultCode() {
        return resultCode;
    }

    public void setResultCode(ResultCodes resultCode) {
        this.resultCode = resultCode;
    }

    public String getResultMessage() {
        return resultMessage;
    }

    public void setResultMessage(String resultMessage) {
        this.resultMessage = resultMessage;
    }
}
