using CsvHelper.Configuration;
using System;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public sealed class CodeBookProfilerMap : ClassMap<CodeBookProfiler>
    {
        public CodeBookProfilerMap()
        {
            Type type = typeof(CodeBookProfiler);
            Map(p => p.FieldName).Name(type.GetMapPropertyFromConfig("FieldName"));
            Map(p => p.Value).Name(type.GetMapPropertyFromConfig("Value"));
            Map(p => p.Description).Name(type.GetMapPropertyFromConfig("Description"));
            Map(p => p.DescriptionLocalized).Name(type.GetMapPropertyFromConfig("DescriptionLocalized"));

        }
    }
}
