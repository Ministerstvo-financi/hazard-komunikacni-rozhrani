using CsvHelper.Configuration;
using System;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public sealed class FileStructureProfileMap : ClassMap<FileStructureProfile>
    {
        public FileStructureProfileMap()
        {
            Type type = typeof(FileStructureProfile);
            Map(p => p.Model).Name(type.GetMapPropertyFromConfig("Model"));
            Map(p => p.File).Name(type.GetMapPropertyFromConfig("File"));
            Map(p => p.Order).Name(type.GetMapPropertyFromConfig("Order"));
            Map(p => p.FieldName).Name(type.GetMapPropertyFromConfig("FieldName"));
            Map(p => p.FieldNameKey).Name(type.GetMapPropertyFromConfig("FieldNameKey"));
            Map(p => p.FieldType).Name(type.GetMapPropertyFromConfig("FieldType"));
            Map(p => p.DomainType).Name(type.GetMapPropertyFromConfig("DomainType"));
            Map(p => p.IsNullable).Name(type.GetMapPropertyFromConfig("IsNullable"));
            Map(p => p.Presence).Name(type.GetMapPropertyFromConfig("Presence"));
            Map(p => p.Condition).Name(type.GetMapPropertyFromConfig("Condition"));
            Map(p => p.MaxLength).Name(type.GetMapPropertyFromConfig("MaxLength"));
            Map(p => p.MinLength).Name(type.GetMapPropertyFromConfig("MinLength"));
            Map(p => p.RegExp).Name(type.GetMapPropertyFromConfig("RegExp"));
            Map(p => p.Context).Name(type.GetMapPropertyFromConfig("Context"));
            Map(p => p.Note).Name(type.GetMapPropertyFromConfig("Note"));
        }
    }
}
