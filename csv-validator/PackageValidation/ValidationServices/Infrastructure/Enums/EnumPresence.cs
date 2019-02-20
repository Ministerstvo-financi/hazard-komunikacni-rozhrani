using System.ComponentModel;

namespace ValidationPilotServices.Infrastructure.Enums
{
    public enum EnumPresence
    {
        [Description("")]
        NON_MANDATORY,
        [Description("mandatory")]
        MANDATORY,
        [Description("mandatorif")]
        MANDATORY_IF
    }
}
