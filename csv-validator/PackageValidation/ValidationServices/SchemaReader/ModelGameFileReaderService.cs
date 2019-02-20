using System;
using System.Collections.Generic;
using System.Linq;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.SchemaReader
{
    public class ModelGameFileReaderService : SourceReaderService
    {

        private readonly string _modelName;
        private readonly string _gameType;

 
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SpecialCodeBook:ModelGameFile")]
        public string ModelGameFile { get; set; }

        public ModelGameFileReaderService(string modelName, string gameType) : base(ConfigReader.GetSourceFolderName())
        {
            this._modelName = modelName.ToUpper();
            this._gameType = gameType.ToUpper();
            this.ConfigFieldsIni();
        }

        public List<ModelGameFileProfile> Get()
        {
            if (string.IsNullOrEmpty(this.ModelGameFile))
            {
                throw new ArgumentNullException($"The file name of the source is not defined.");
            }

            return this.GetCsvData<ModelGameFileProfile, ModelGameTypeProfileMap>(this.ModelGameFile)
                .Where(p => (p.Model.ToUpper().Equals(this._modelName) && p.GameType.ToUpper().Equals(this._gameType))).ToList();
        }
    }
}
