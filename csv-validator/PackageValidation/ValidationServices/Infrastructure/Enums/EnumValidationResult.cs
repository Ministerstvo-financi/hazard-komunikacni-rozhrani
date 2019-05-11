using System.ComponentModel;

namespace ValidationPilotServices.Infrastructure.Enums
{
    /// <summary>
    /// If the package contains files other than those listed in the table it
    /// will be flagged as erroneous and the processing is terminated by the
    /// error code ERR_PKG _EXTRA_FILES and enumeration of the extra files.
    /// </summary>
    public enum EnumValidationResult
    {
        [Description("VALID")]
        VALID,
        [Description("ERR_LINE_BAD_HEADER")]
        ERR_LINE_BAD_HEADER,
        [Description("ERR_LINE_INVALID_HASH")]
        ERR_LINE_INVALID_HASH,
        [Description("ERR_LINE_SPLIT_META")]
        ERR_LINE_SPLIT_META,
        [Description("ERR_META_FIELD_DATETIME")]
        ERR_META_FIELD_DATETIME,
        [Description("ERR_META_FIELD_BAD_PACKAGE")]
        ERR_META_FIELD_BAD_PACKAGE,
        [Description("ERR_META_FIELD_VERSION")]
        ERR_META_FIELD_VERSION,
        [Description("ERR_LINE_BAD_META")]
        ERR_LINE_BAD_META,
        [Description("ERR_LINE_TOO_LONG")]
        ERR_LINE_TOO_LONG,
        [Description("ERR_LINE_BAD_UTF8")]
        ERR_LINE_BAD_UTF8,
        [Description("ERR_PKG _EXTRA_FILES ")]
        ERR_PKG_EXTRA_FILES,
        [Description("ERR_PKG_MISSING_FILE")]
        ERR_PKG_MISSING_FILE,
        [Description("ERROR_INVALID")]
        ERROR_INVALID,
        [Description("FILE_DOES_NOT_EXIST")]
        FILE_DOES_NOT_EXIST,
        [Description("ERR_LINE_BAD_FIELD_COUNT")]
        ERR_LINE_BAD_FIELD_COUNT,

        //unused
        ERR_FIELD_BAD_FORMAT_RE,

        ERR_FIELD_CONTEXT, 
        ERR_FIELD_CTX_GAMETYPE,
        ERR_FIELD_CTX_OPERATORID_PREFIX,
        ERR_FIELD_CTX_OPERTORID,
        ERR_FIELD_CTX_PGKDATE,
        ERR_FIELD_CTX_PKG_TIMESPAN,
        ERR_FIELD_DOMAIN_TYPE,
        ERR_FIELD_MANDATORY,
        ERR_FIELD_NOT_NULL,
        ERR_FIELD_TOO_LONG,
        ERR_FIELD_TOO_SHORT,
        ERR_FIELD_TYPE,
        ERR_FIELD_CODEBOOK,
        ERR_FILE,
        ERR_FILE_TOO_MANY_ERRORS,
        ERR_FILE_BAD_RECORD_COUNT,
        ERR_LINE_BAD_UTF,
        ERR_LINE_INVALID_CSV,
        ERR_LINE_INVALID_FIELDS,
        ERR_LINE_META,
        ERR_LINE,
        ERR_META_FIELD,
        ERR_META_FIELD_BAD_NAME,
        ERR_PKG,

        ERR_INTERNAL



    }
}
