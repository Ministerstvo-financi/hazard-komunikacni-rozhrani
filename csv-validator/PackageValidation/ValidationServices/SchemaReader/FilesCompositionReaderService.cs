using System;
using System.Collections.Generic;
using System.Linq;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.SchemaReader
{
    public class FilesCompositionReaderService : SourceReaderService
    {

        private readonly string _modelName;

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SpecialCodeBook:FileCompositionCollection")]
        public string FileName { get; set; }

        public FilesCompositionReaderService(string modelName) : base(ConfigReader.GetSourceFolderName())
        {
            this._modelName = modelName.ToUpper();
            this.ConfigFieldsIni();
        }

        public List<SpecialCodeBookProfile> Get()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                throw new ArgumentNullException($"The file name of the source is not defined.");
            }

            return this.GetCsvData<SpecialCodeBookProfile, SpecialCodeBookProfilerMap>(this.FileName)
                .Where(p => p.Key.ToUpper().Equals(this._modelName)).ToList();
        }
    }
}
