using ValidationPilotServices.Infrastructure.Attributes;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public class ModelGameFileProfile
    {
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:ModelColumn")]
        public string Model { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:GameTypeColumn")]
        public string GameType { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:FileNameColumn")]
        public string File { get; set; }

    }
}
