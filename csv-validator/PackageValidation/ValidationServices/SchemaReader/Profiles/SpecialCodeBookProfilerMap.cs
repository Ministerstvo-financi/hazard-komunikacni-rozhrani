using CsvHelper.Configuration;
using System;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public sealed class SpecialCodeBookProfilerMap : ClassMap<SpecialCodeBookProfile>
    {
        public SpecialCodeBookProfilerMap()
        {
            Type type = typeof(SpecialCodeBookProfile);
            Map(p => p.Key).Name(type.GetMapPropertyFromConfig("Key"));
            Map(p => p.Description).Name(type.GetMapPropertyFromConfig("Description"));
        }
    }
}
