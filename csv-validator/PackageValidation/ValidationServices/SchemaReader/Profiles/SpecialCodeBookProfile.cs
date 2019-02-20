using System;
using System.Collections.Generic;
using System.Text;
using ValidationPilotServices.Infrastructure.Attributes;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public sealed class SpecialCodeBookProfile
    {
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SpecialCodeBook:KeyColumn")]
        public string Key { get; set; }
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SpecialCodeBook:DescriptionColumn")]
        public string Description { get; set; }
    }
}
