using CsvHelper.Configuration;
using System;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public sealed class ModelGameTypeProfileMap : ClassMap<ModelGameFileProfile>
    {
        public ModelGameTypeProfileMap()
        {
            Type type = typeof(ModelGameFileProfile);
            Map(p => p.Model).Name(type.GetMapPropertyFromConfig("Model"));
            Map(p => p.File).Name(type.GetMapPropertyFromConfig("File"));
            Map(p => p.GameType).Name(type.GetMapPropertyFromConfig("GameType"));
        }
    }
}
