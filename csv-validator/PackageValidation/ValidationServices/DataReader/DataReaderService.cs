using CsvHelper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.DataReader
{


    public class DataReaderService : ValidationCoreService
    {
        private readonly bool _checkMissingFiles = false;

        /// <summary>
        /// The package object contained all initial parameters for validating files received.
        /// </summary>
        private readonly PackageReaderService _package;

        /// <summary>
        /// The object specifies result of validation
        /// to serialized to the output result file.
        /// </summary>
        private readonly ValidationResultCollection _result;

        /// <summary>
        /// GET; PRIVATELY SET;
        /// The location of the package to get files for validating.
        /// </summary>
        public string DataSourceFolder { get; private set; }

        private string CurrentFileToValidate { get; set; }

        /// <summary>
        /// GET; SET;
        /// This property consists of string contained log files collection in the package folder.
        /// This files must not be validated and must be passed during validation process.
        /// The files collection set in json configuration file. 
        /// </summary>
        [ConfigurationReader(ConfigurationParameter = "SourceLocation:LogFilesInOperationFolder")]
        public string FilesToPass { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:CheckFileComposition")]
        public string CheckFileComposition { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:FieldsSeparator")]
        public string FieldsSeparator { get; set; }

        [ConfigurationReader(ConfigurationParameter = "MaximalLineLength")]
        public int MaxLineLength { get; set; }

        [ConfigurationReader(ConfigurationParameter = "MaximalErrorsPerFile")]
        public int MaximalErrorsPerFile { get; set; }

        [ConfigurationReader(ConfigurationParameter = "SpecificationVersion")]
        public String SpecificationVersion { get; set; }





        public DataReaderService(PackageReaderService packageType) : base()
        {
            if (packageType == null)
            {
                throw new ArgumentNullException();
            }
            this._package = packageType;
            this.DataSourceFolder = packageType.package_source_full_path;
            this._result = new ValidationResultCollection(packageType.package_number, packageType.package_source_full_path);
            this.ConfigFieldsIni();
        }

        #region PRIVATE FUNCTIONS AND METHODS

        protected override void Clean()
        {
            base.Clean();
            this.CurrentFileToValidate = string.Empty;
        }

        private void ValidationErrorMessage(EnumValidationResult errorType, string currentField, int rowId, string errorMsg)
        {
            this.LogError(string.Format(this.error_message_template,
                this._package.package_number,
                this.CurrentFileToValidate,
                rowId >= 0 ? rowId.ToString() : "",
                currentField,
                errorType,
                errorMsg));
        }

        private void PackageErrorMessage(EnumValidationResult errorType, string msg, bool isFatal = false)
        {
            this.LogError(string.Format(this.error_message_template,
                this._package.package_number,
                this.CurrentFileToValidate,
                "",
                "",
                errorType,
                msg));
        }

        private void FileErrorMessage(EnumValidationResult errorType, string msg)
        {
            this.LogError(string.Format(this.error_message_template,
                this._package.package_number,
                this.CurrentFileToValidate,
                "",
                "",
                errorType,
                msg));
        }

        private void LineErrorMessage(EnumValidationResult errorType, int lineNumber, string msg)
        {
            this.LogError(string.Format(this.error_message_template,
               this._package.package_number,
               this.CurrentFileToValidate,
               lineNumber,
               "",
               errorType,
               msg));
        }

        private void FieldErrorMessage(EnumValidationResult errorType, int lineNumber, string fieldName, int position,
            string msg)
        {
            this.LogError(string.Format(this.error_message_template,
               this._package.package_number,
               this.CurrentFileToValidate,
               lineNumber,
               fieldName,
               errorType,
               msg));
        }


        /// <summary>
        /// This function returns TRUE if code book item selected by code book name and
        /// code book code exists. Otherwise this function returns false.
        /// </summary>
        /// <param name="fieldItem"></param>
        /// <param name="valueToValidate"></param>
        /// <param name="currentCodeBook"></param>
        /// <returns></returns>
        private bool CodeBookFieldValidate(FieldItem fieldItem, string valueToValidate, out string currentCodeBook)
        {
            currentCodeBook = string.Empty;
            CodeBook codeBook = fieldItem.FieldType as CodeBook;
            if (codeBook == null || string.IsNullOrEmpty(valueToValidate))
            {
                return true;
            }

            currentCodeBook = codeBook.CodeBookName;

            if (!this._package.CodeBook.IsValidValue(codeBook.CodeBookName, valueToValidate, out string message))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This procedure sets IsNullable property for the item depends on
        /// conditions.
        /// </summary>
        /// <param name="item">Field structure item.</param>
        /// <param name="source">Dynamic object read from csv file to get a conditions to set IsNullable property.</param>
        /// <param name="rowId"></param>
        private void NullableFieldItemDefinition(FieldItem item, ExpandoObject source, int rowId)
        {
            switch (item.Presence)
            {
                case EnumPresence.MANDATORY:
                    item.FieldType.IsNullable = false;
                    break;
                case EnumPresence.NON_MANDATORY:
                    item.FieldType.IsNullable = true;
                    break;
                default:
                    //FIXME - skip mandatoryif test
                    item.FieldType.IsNullable = true;
                    // if (!item.GetNullableFieldDefinition(source, out string message))
                    // {
                    //     ValidationErrorMessage(EnumValidationResult.ERR_FIELD_MANDATORY,
                    //         item.FieldName,
                    //         rowId,
                    //         message
                    //     );
                    // }
                    break;
            }
        }

        /// <summary>
        /// This function returns TRUE 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        private bool ValidateItemByValue(FieldItem item, string value, int rowId)
        {
            item.FieldType.Value = value;

            if (!item.FieldType.IsValid)
            {
                this.ValidationErrorMessage(EnumValidationResult.ERR_FIELD_DOMAIN_TYPE, item.FieldName,
                    rowId,
                    item.FieldType.ErrorMessage);
                return item.FieldType.IsValid;
            }

            //check is the field type is code book
            if (!this.CodeBookFieldValidate(item, value, out string currentCodeBook))
            {
                this.ValidationErrorMessage(
                    EnumValidationResult.ERR_FIELD_CODEBOOK,
                    item.FieldName,
                    rowId,
                    $"Value >>{value}<< not found in the codebook {currentCodeBook}"
                );
                return item.FieldType.IsValid;
            }

            return item.FieldType.IsValid;
        }

        private bool ValidateByContextCondition(FieldItem item, string sourceValue, int rowId)
        {
            if (string.IsNullOrEmpty(item.ContextConditionParameter))
            {
                return true;
            }

            if (sourceValue == null)
            {
                return true;
            }

            switch (item.ContextConditionParameter.ToUpper())
            {
                case "EQUALSGAMETYPE":
                    if (_package.GameType.ToUpper().Equals(sourceValue.ToUpper()))
                    {
                        return true;
                    }
                    return false;
                case "EQUALSOPERATORID":
                    if (_package.OperatorId.ToUpper().Equals(sourceValue.ToUpper()))
                    {
                        return true;
                    }
                    return false;
                case "STARTSWITHOPERATORID":
                    if (sourceValue.ToUpper().StartsWith(_package.OperatorId.ToUpper()))
                    {
                        return true;
                    }
                    return false;
                case "WITHINPACKAGETIMESPAN":
                    // FIXME: not yet implemented
                    return true;
                default: return true;
            }

            // if (!string.IsNullOrEmpty(item.ContextConditionParameter) && item.ContextCondition != null)
            // {
            //     object value =
            //         this._package.GetContextConditionParameter(item.ContextConditionParameter);
            //     if (value == null)
            //     {
            //         ValidationErrorMessage(
            //             EnumValidationResult.ERR_FIELD_CONTEXT,
            //             item.FieldName,
            //             rowId,
            //             $"Can't validate {item.ContextConditionParameter} condition. The variable to validate with is not defined."
            //         );
            //         return item.FieldType.IsValid;
            //     }

            //     if (!item.ContextCondition(sourceValue, value))
            //     {
            //         string msgTemplate =
            //             this._package.GetContextValidationErrorMessageTemplate(item.ContextConditionParameter);

            //         if (string.IsNullOrEmpty(msgTemplate))
            //         {
            //             this.ValidationErrorMessage(EnumValidationResult.ERR_FIELD_CONTEXT, item.FieldName, rowId, $"Invalid value: {sourceValue}");
            //         }
            //         else
            //         {
            //             this.ValidationErrorMessage(EnumValidationResult.ERR_FIELD_CONTEXT, item.FieldName, rowId, string.Format(msgTemplate.Trim(), sourceValue, value.ToString()));
            //         }

            //         return item.FieldType.IsValid;
            //     }
            // }

            // return item.FieldType.IsValid;
        }

        private void ReadFileData(FileInfo fileInfo, out int linesNumber)
        {
            linesNumber = 0;

            using (StreamReader reader = new StreamReader(fileInfo.FullName))
            using (CsvReader csv = new CsvReader(reader))
            {
                csv.Configuration.Delimiter = this.FieldsSeparator;
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.LineBreakInQuotedFieldIsBadData = true;
                csv.Configuration.AllowComments = false;
                // csv.Configuration.BadDataFound = (c)=>{
                //     this.ValidationErrorMessage(EnumValidationResult.ERR_LINE_INVALID_CSV, "", -1, "invalid CSV form");
                // };
                int headerCount = 0;

                bool moreRecords = true;
                while (true)
                {
                    if (ErrorsCounter > MaximalErrorsPerFile)
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_FILE_TOO_MANY_ERRORS, linesNumber, "Too many errors detected. Aborting further processing of the file");
                        this.FileErrorMessage(EnumValidationResult.ERR_FILE_TOO_MANY_ERRORS, "Too many errors");
                        break;
                    }

                    try
                    {
                        moreRecords = csv.Read();
                        if (!moreRecords)
                        {
                            break;
                        }
                        // Check the line size
                        // ERR_LINE_TOO_LONG
                        if (TypeExtensions.GetBytesCount(csv.Context.RecordBuilder.ToString()) > this.MaxLineLength)
                        {
                            this.LineErrorMessage(EnumValidationResult.ERR_LINE_TOO_LONG, linesNumber + 1, "The line is too long.");
                            break;
                        }

                        if (linesNumber == 0)
                        {
                            linesNumber++;
                            continue;
                        }
                        else if (linesNumber == 1)
                        {
                            csv.ReadHeader();

                            //ERR_LINE_BAD_HEADER - validation type
                            if (!this._package.FieldsCollection.ValidateFieldsComposition(this.ValidationErrorMessage, this.CurrentFileToValidate,
                                csv.Context.Record))
                            {
                                this.ValidationErrorMessage(EnumValidationResult.ERR_LINE, "", 1, "");
                                break;
                            }

                            headerCount = csv.Context.Record.Length;
                            linesNumber++;
                            continue;
                        }

                        if (csv.Context == null || csv.Context.Record == null)
                        {
                            this.ValidationErrorMessage(EnumValidationResult.ERR_LINE_INVALID_FIELDS, "", linesNumber, $"Failed to extract fields from line");
                            linesNumber++;
                            continue;
                        }

                        //ERR_LINE_BAD_FIELD_COUNT - validation type
                        if (headerCount != csv.Context.Record.Length)
                        {
                            this.ValidationErrorMessage(EnumValidationResult.ERR_LINE_BAD_FIELD_COUNT, "", linesNumber, $"{csv.Context.Record.Length} fields expected in metadata line found {headerCount}");
                            linesNumber++;
                            continue;
                        }

                        ExpandoObject row = csv.GetRecord<dynamic>();

                        foreach (KeyValuePair<string, object> key in row.ToList())
                        {
                            try
                            {
                                FieldItem fieldItem = this._package.FieldsCollection.GetValidator(fileInfo.Name, key.Key);

                                if (fieldItem != null)
                                {
                                    //set nullable property to validate
                                    this.NullableFieldItemDefinition(fieldItem, row, linesNumber);

                                    //validate value
                                    if (!this.ValidateItemByValue(fieldItem, key.Value.ToString(), linesNumber))
                                    {
                                        continue;
                                    }

                                    //validate context condition
                                    if (!this.ValidateByContextCondition(fieldItem, key.Value.ToString(), linesNumber))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    this.ValidationErrorMessage(EnumValidationResult.ERR_INTERNAL, key.Key, linesNumber, "no validators found for field");
                                }

                            }
                            catch (Exception ex)
                            {
                                this.ValidationErrorMessage(EnumValidationResult.ERR_INTERNAL, key.Key, linesNumber, $"exception while validating field:{ex.Message}");
                            }
                        }
                        linesNumber++;

                    }
                    catch (BadDataException ex)
                    {
                        string detailMsg = "Other error.";
                        if (ex.ReadingContext.Record != null)
                        {
                            if (ex.ReadingContext.Record.Contains("\""))
                            {
                                detailMsg = "Field contains invalid character: >>\"<<";
                            }
                            else if (ex.ReadingContext.Record.Contains("'"))
                            {
                                detailMsg = "Field contains invalid character: >>'<<";
                            }
                            else if (ex.ReadingContext.Record.Contains("\r") || ex.ReadingContext.Record.Contains("\n"))
                            {
                                detailMsg = "Field contains invalid character: >>CR or LF<<";
                            }
                            else
                            {
                                detailMsg = "Other error.";
                            }
                        }
                        this.ValidationErrorMessage(EnumValidationResult.ERR_LINE_INVALID_CSV, "", linesNumber, $"line is not well formed CSV record index:{ex.ReadingContext.CurrentIndex}, char position: {ex.ReadingContext.CharPosition}. Detail - {detailMsg}");
                        linesNumber++;
                    }
                    catch (Exception ex)
                    {
                        LoggerService.LoggerService.GetGlobalLog().Error("Exception while processing CSV row", ex);
                        this.ValidationErrorMessage(EnumValidationResult.ERR_INTERNAL, "", linesNumber, $"exception while processing line:{ex.Message}");
                        linesNumber++;
                    }
                }
            }
        }

        private string FileSha256HashCalculation(FileInfo fi)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(fi.FullName))
                {
                    var hash = sha256.ComputeHash(stream);
                    return $"sha256:{BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()}";
                }
            }
        }

        /// <summary>
        /// This function returns collection of the files which are compulsory for the package
        /// validation. 
        /// </summary>
        /// <returns>The compulsory files array.</returns>
        private string[] GetMandatoryFilesCollection()
        {
            // FilesCompositionReaderService service = new FilesCompositionReaderService(this._package.Model);
            // List<SpecialCodeBookProfile> target = service.Get();

            // List<string> filesByModel = target.Select(p => p.Description).ToList();

            ModelGameFileReaderService modelGameFileService = new ModelGameFileReaderService(this._package.Model, this._package.GameType);
            List<ModelGameFileProfile> modelGameFiles = modelGameFileService.Get();

            List<string> filesByModelAndGame = modelGameFiles.Select(p => p.File).ToList();

            // List<String> finalList = filesByModel.Intersect(filesByModelAndGame).ToList();
            List<string> finalList = filesByModelAndGame;

            if (!finalList.Any())
            {
                throw new ArgumentOutOfRangeException($"There are no any file collection found for the model {this._package.Model}");
            }

            return finalList.ToArray();
        }

        /// <summary>
        /// This function returns files collection to validate from the Package folder.
        /// </summary>
        /// <returns>The files collection to validate.</returns>
        private List<FileInfo> GetFilesCollectionValidated()
        {
            DirectoryInfo di = new DirectoryInfo(this.DataSourceFolder);
            List<FileInfo> files = new List<FileInfo>();

            foreach (FileInfo fi in di.GetFiles())
            {
                //validation result and log are allowed
                if (fi.Name.StartsWith("validation-"))
                {
                    continue;
                }

                if (this.FilesToPass.Contains(fi.Name))
                {
                    continue;
                }

                string fn = this._package.FilesCollection.FirstOrDefault(p => p.ToUpper().Equals(fi.Name.ToUpper()));

                if (string.IsNullOrEmpty(fn))
                {
                    FileErrorMessage(EnumValidationResult.ERR_PKG_EXTRA_FILES, $"The file {fi.Name} contained in package {this._package.package_number} is not expected.");
                    this._result.AddItem(fi.Name, 0, EnumValidationResult.ERR_PKG_EXTRA_FILES, string.Empty);
                    continue;
                }

                files.Add(fi);
            }
            return files;
        }

        /// <summary>
        /// This function returns 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private List<string> FilesCompositionValidate(List<FileInfo> collection)
        {
            List<string> missed = new List<string>();

            foreach (string file in this.GetMandatoryFilesCollection())
            {
                FileInfo fi = collection.FirstOrDefault(p => p.Name.ToUpper().Equals(file.ToUpper()));
                if (fi == null)
                {
                    ValidationErrorMessage(
                        EnumValidationResult.ERR_PKG_MISSING_FILE,
                        file,
                        -1,
                        "Mandatory file is missing"
                    );
                    missed.Add(file);
                    continue;
                }
            }

            return missed;
        }

        private void ReadFileDataValidationCycle(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                this.Clean();
                this.CurrentFileToValidate = file.Name;

                if (!this.IsFileMetaDataValid(file))
                {
                    this._result.AddItem(this.CurrentFileToValidate, 0, EnumValidationResult.ERR_LINE_BAD_META, string.Empty);
                    continue;
                }

                string fileHash = this.FileSha256HashCalculation(file);
                LoggerService.LoggerService.GetValidationProcessingLog().Info($"Start validate {file.Name}");
                this.ReadFileData(file, out var counter);

                if (this.IsValid)
                {
                    LoggerService.LoggerService.GetValidationProcessingLog().Info($"The file {file.Name} is valid.");
                }
                else
                {

                    FileErrorMessage(EnumValidationResult.ERR_FILE, $"The file {file.Name} is invalid.");
                }

                this._result.AddItem(file.Name, counter - 2,
                    (this.IsValid ? EnumValidationResult.VALID : EnumValidationResult.ERROR_INVALID), fileHash);
            }
        }

        /// <summary>
        /// This function returns TRUE if the first file row (Metadata) is valid.
        /// Otherwise, the function add to log file error messages and returns False;
        /// </summary>
        /// <param name="fileInfo">The File info contains of file information.</param>
        /// <returns>True if the file's metadata is valid.</returns>
        private bool IsFileMetaDataValid(FileInfo fileInfo)
        {
            try
            {
                using (var binStream = new FileStream(fileInfo.FullName, FileMode.Open))
                {
                    var buf = new byte[4];
                    int count = binStream.Read(buf, 0, 4);
                    if (count < 4)
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_LINE_META, 1, "Metadata line too short");
                        return false;
                    }

                    if (buf[0] == 0x00 && buf[1] == 0x00 && buf[2] == 0xfe && buf[3] == 0xff)
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_LINE_BAD_UTF8, 1, "File starts with BOM varianta UTF-32BE, (big-endian)");
                        return false;
                    }

                    if (buf[0] == 0xff && buf[1] == 0xfe && buf[2] == 0x00 && buf[3] == 0x00)
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_LINE_BAD_UTF8, 1, "File starts with BOM varianta UTF-32LE, (little-endian)");
                        return false;
                    }

                    if (buf[0] == 0xfe && buf[1] == 0xff)
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_LINE_BAD_UTF8, 1, "File starts with BOM varianta UTF-16BE, (big-endian)");
                        return false;
                    }

                    if (buf[0] == 0xff && buf[1] == 0xfe)
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_LINE_BAD_UTF8, 1, "File starts with BOM varianta UTF-16LE, (little-endian)");
                        return false;
                    }


                    if ( ! ( buf[0] == '#' || (buf[0] == 0xef && buf[1] == 0xbb && buf[2] == 0xbf && buf[3] == '#') ) )
                    {
                        this.LineErrorMessage(EnumValidationResult.ERR_LINE_INVALID_HASH, 1, "File should start with hash # or UTF-8 BOM and hash #");
                        return false;                        
                    }
                }

                using (StreamReader reader = new StreamReader(fileInfo.FullName))
                using (var csv = new CsvReader(reader))
                {
                    int lineCounter = 0;
                    csv.Configuration.Delimiter = this.FieldsSeparator;

                    while (csv.Read())
                    {
                        if (lineCounter == 0)
                        {
                            string row = csv.Context.RawRecordBuilder.ToString();

                            if (string.IsNullOrEmpty(row) || !row.StartsWith("#"))
                            {
                                this.LineErrorMessage(EnumValidationResult.ERR_LINE_INVALID_HASH, 1, "The file metadata has not a hash tag.");
                                break;
                            }

                            if (csv.Context.Record.Length != 4)
                            {
                                this.LineErrorMessage(EnumValidationResult.ERR_LINE_SPLIT_META, 1, "The file metadata has wrong number of fields.");
                                break;
                            }

                            //date time file creation validate
                            if (!DateTime.TryParse(csv.GetField<string>(2), out DateTime date) || date >= DateTime.Now)
                            {
                                this.FieldErrorMessage(EnumValidationResult.ERR_META_FIELD_DATETIME, 1, "CreatedAt", 0, "CreatedAt field has an invalid format.");
                            }
                            else
                            {
                                if (date >= DateTime.Now)
                                {
                                    this.FieldErrorMessage(EnumValidationResult.ERR_META_FIELD_DATETIME, 1, "CreatedAt", 0, "CreatedAt field must be less then current date.");
                                }
                            }

                            if (!csv.GetField<string>(0).Substring(1).ToUpper().Equals(this._package.package_number.ToUpper()))
                            {
                                this.FieldErrorMessage(EnumValidationResult.ERR_META_FIELD_BAD_PACKAGE, 1, "PackageNumber", 0, "The Package Number field is not equals the Package Number of the folder.");
                            }

                            if (!csv.GetField<string>(1).ToUpper().Equals(this.CurrentFileToValidate.ToUpper()))
                            {
                                this.FieldErrorMessage(EnumValidationResult.ERR_META_FIELD_BAD_PACKAGE, 1, "FileName", 0, "The Model field is not equals the Model of the folder.");
                            }

                            if (!csv.GetField<string>(3).Equals(this.SpecificationVersion))
                            {
                                this.FieldErrorMessage(EnumValidationResult.ERR_META_FIELD_VERSION, 1, "PackageVersion", 0, "The PackageVersion field is not equals the PackageVersion of the folder.");
                            }

                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this.ValidationErrorMessage(EnumValidationResult.ERR_INTERNAL, "", 0, $"{ex.Message}");
                LoggerService.LoggerService.GetValidationProcessingLog().Error($"Exception while validating metadata line {this._package.package_number} in {this.CurrentFileToValidate}", ex);
            }

            return this.IsValid;
        }

        #endregion

        /// <summary>
        /// This procedure gets file collection from folder selected and
        /// starts validation process for each file.
        /// </summary>
        public void StartValidationProcess(string fileToValidate = null)
        {
            LoggerService.LoggerService.GetValidationProcessingLog().Info($"Start processing package {this._package.package_number}");

            //get files collection
            List<FileInfo> files = this.GetFilesCollectionValidated();

            if (!files.Any())
            {
                throw new FileNotFoundException($"There are no any files found in {this.DataSourceFolder}");
            }

            //validate files composition
            if (this._checkMissingFiles)
            {
                List<string> missed = this.FilesCompositionValidate(files);

                if (missed.Any())
                {
                    string mess = string.Join(",", missed);
                    LoggerService.LoggerService.GetGlobalLog().Warn($"Missing files: {mess}");
                    throw new ArgumentException("Missing files");
                }
            }

            //limit to single file if requested
            if (fileToValidate != null)
            {
                files = files.Where(f => f.Name.Equals(fileToValidate)).ToList();
            }
            //files validation cycle
            this.ReadFileDataValidationCycle(files);

            this._result.Write();
            //FIXME - convert message format
            LoggerService.LoggerService.GetValidationProcessingLog().Info($"The Package {this._package.package_number} validation is completed.");
        }

    }
}
