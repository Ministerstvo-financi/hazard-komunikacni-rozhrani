using ValidationPilotServices.Infrastructure.Conditions;
using ValidationPilotServices.Infrastructure.Enums;

namespace ValidationPilotServices.DataTypes
{
    public class FieldItem
    {
        public string FileSource { get; set; }
        public int Order { get; set; }
        public string FieldName { get; set; }
        public string FieldNameUpper { 
            get {
                return FieldName.ToUpper();
            }
        }
        public string DomainType { get; set; }
        public BaseValidator FieldType { get; set; }
        public string CodeBook { get; set; }
        public EnumPresence Presence { get; set; }
        public string IsNullableCondition { get; set; }
        public ContextConditionValidationsHandler ContextCondition { get; set; }
        public string ContextConditionParameter { get; set; }

    }
}
