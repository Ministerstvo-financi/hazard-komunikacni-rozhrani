using System;
using System.Collections.Generic;
using System.Linq;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.SchemaReader
{
    /// <summary>
    /// This class specifies functions and methods to collect and build fields structure
    /// selected from the CSV source file.
    /// The file location and file name are defined in configuration file: structureSourceSettings.json
    /// The folder name is saved in 'SourceLocation:Folder' parameter.
    /// The field's structure file name is saved in 'SourceLocation:SchemaStructure:FileName' parameter.
    ///
    /// Profile filed structure is defined in <see cref="FileStructureProfile"/> and map of the fields in
    /// <see cref="FileStructureProfileMap"/>.
    /// </summary>
    public class FieldsStructureReaderService : SourceReaderService
    {
        private readonly string _model;

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:SchemaStructure:FileName")]
        public string FileName { get; set; }
        
        public FieldsStructureReaderService(string model) : base(ConfigReader.GetSourceFolderName())
        {
            this._model = model;
            this.ConfigFieldsIni();
        }

        /// <summary>
        /// This function returns fields structure profiles collection read from CSV file.
        /// </summary>
        /// <returns>The fields profile collection.</returns>
        public List<FileStructureProfile> GetCsvData()
        {
            return this.GetCsvData<FileStructureProfile, FileStructureProfileMap>(this.FileName);
        }

        /// <summary>
        /// This function returns fields structure collection retrieved from the fields source file.
        /// The folder name and file name are defined in configuration file: structureSourceSettings.json
        /// </summary>
        /// <returns>The fields structure collection.</returns>
        public Fields Get()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn( $"The file name of the source is not defined.");
                throw new ArgumentException($"The file name of the source is not defined.");
            }

            string modelType = this._model;
            List<FileStructureProfile> source = this.GetCsvData();

            Fields fields = new Fields();
            foreach (FileStructureProfile item in source.Where(p => p.Model.Equals(modelType)))
            {
                try
                {

                    FieldItem fieldItem = new FieldItem
                    {
                        Order = int.Parse(item.Order),
                        FileSource = item.File,
                        FieldName = item.FieldNameKey,
                        Presence = item.Presence.IsNullableFieldFromFieldProfile(),
                        IsNullableCondition = item.Condition,
                        ContextConditionParameter = item.Context,
                        DomainType = item.DomainType,
                        FieldType = item.ValidatorFieldBuilder()
                    };

                    //if context validation condition exists - define the function
                    if (!string.IsNullOrEmpty(item.Context))
                    {
                        fieldItem.GetContextConditionMethodDefine();
                    }

                    //check presence property
                    if (fieldItem.Presence == EnumPresence.MANDATORY_IF &&
                        string.IsNullOrEmpty(fieldItem.IsNullableCondition))
                    {
                        throw new ArgumentNullException($"Field: {fieldItem.FieldName}, " +
                                                        $"Model: {this._model}, " +
                                                        $"File Structure: {fieldItem.FileSource} - has mandatory if presence. However no any conditions defined.");
                    }

                    fields.Items.Add(fieldItem);

                }
                catch (Exception ex)
                {
                    LoggerService.LoggerService.GetGlobalLog().Warn($"File structure Reader: {ex.Message}",ex);
                    throw new ArgumentException($"File structure Reader: {ex.Message}");
                }
            }

            return fields;
        }
    }
}
