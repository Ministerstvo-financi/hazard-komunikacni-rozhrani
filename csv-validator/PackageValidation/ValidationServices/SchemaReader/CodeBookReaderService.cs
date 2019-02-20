using System;
using System.Collections.Generic;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.SchemaReader
{
    /// <summary>
    /// This class contains functions and methods to read and convert
    /// Code Book data from CSV source into <see cref="CodeBookCollection"/>
    /// items collection.
    ///
    /// File location and parameters to read and convert are retrieving from
    /// structureSourceSettings.json file.
    /// </summary>
    public class CodeBookReaderService : SourceReaderService
    {
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:CodeBooks:FileName")]
        public string FileName { get; set; }

        public CodeBookReaderService() : base(ConfigReader.GetSourceFolderName())
        {
            this.ConfigFieldsIni();
        }

        /// <summary>
        /// This function returns <see cref="CodeBookCollection"/> collection retried from
        /// CSV source file. 
        /// </summary>
        /// <returns>The  <see cref="CodeBookCollection"/> items collection.</returns>
        public CodeBookCollection Get()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                throw new ArgumentNullException($"The file name of the source is not defined.");
            }

            List<CodeBookProfiler> source = this.GetCsvData<CodeBookProfiler, CodeBookProfilerMap>(this.FileName);
            CodeBookCollection target = new CodeBookCollection();

            foreach (CodeBookProfiler item in source)
            {
                target.Add(item.FieldName, item.Value, item.DescriptionLocalized);
            }
            return target;
        }
    }
}
