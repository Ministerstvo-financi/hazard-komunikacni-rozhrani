using System;
using System.Collections.Generic;
using System.Text;

namespace ValidationPilotServices.Infrastructure.Attributes
{
    public class ConfigurationReaderAttribute : Attribute
    {
        public string ConfigurationParameter { get; set; }
    }
}
