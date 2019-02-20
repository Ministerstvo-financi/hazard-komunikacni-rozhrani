using ValidationPilotServices.Infrastructure.Attributes;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public class FileStructureProfile
    {
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:ModelColumn")]
        public string Model { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:FileNameColumn")]
        public string File { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:OrderColumn")]
        public string Order { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:FieldNameColumn")]
        public string FieldName { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:FieldNameKeyColumn")]
        public string FieldNameKey { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:FieldDataTypeColumn")]
        public string FieldType { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:DomainTypeColumn")]
        public string DomainType { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:NullableColumn")]
        public string IsNullable { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:PresenceColumn")]
        public string Presence { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:ConditionColumn")]
        public string Condition { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:MinLengthColumn")]
        public string MinLength { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:MaxLengthColumn")]
        public string MaxLength { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:RegExpColumn")]
        public string RegExp { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:ContextColumn")]
        public string Context { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:NoteColumn")]
        public string Note { get; set; }
    }
}
