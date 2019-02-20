using System;
using ValidationPilotServices.Infrastructure.Enums;

namespace ValidationPilotServices.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class IsNullableFieldVerificationAttribute : Attribute
    { 
        public EnumConditionType ConditionType { get; private set; }
        public string[] FieldsToCheck { get; set; }

        public IsNullableFieldVerificationAttribute(EnumConditionType type)
        {
            this.ConditionType = type;
        }
    }
}
