using System;
using System.Collections.Generic;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.SchemaReader
{
    public class GameTypeCodeBookReaderService : SourceReaderService
    {

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SpecialCodeBook:GameTypeFileName")]
        public string FileName { get; set; }

        public GameTypeCodeBookReaderService() : base(ConfigReader.GetSourceFolderName())
        {
            this.ConfigFieldsIni();
        }

        public List<SpecialCodeBookProfile> Get()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                throw new ArgumentNullException($"The file name of the source is not defined.");
            }

            return this.GetCsvData<SpecialCodeBookProfile, SpecialCodeBookProfilerMap>(this.FileName);
        }
    }
}
