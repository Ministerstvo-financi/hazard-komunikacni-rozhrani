using ValidationPilotServices.Infrastructure.Attributes;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public class CodeBookProfiler
    {
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:CodeBooks:FieldNameColumn")]
        public string FieldName { get; set; }
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:CodeBooks:ValueColumn")]
        public string Value { get; set; }
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:CodeBooks:DescriptionColumn")]
        public string Description { get; set; }
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:CodeBooks:DescriptionLocalizedColumn")]
        public string DescriptionLocalized { get; set; }
    }
}
