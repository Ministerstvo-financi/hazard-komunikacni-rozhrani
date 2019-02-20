using System;
using System.IO;
using ValidationPilotServices.Infrastructure;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotServices.ConfigService
{
    /// <summary>
    /// This class specifies properties, functions and methods for prestarting validation process.
    /// </summary>
    public class MetaDataCheckService : ValidationCore
    {
        private readonly string _folderName;

        public string FolderName => this._folderName == null ? string.Empty : Path.Combine(AppDomain.CurrentDomain.BaseDirectory,this._folderName);

        [ConfigurationReader(ConfigurationParameter = "SourceLocation:FilesToProvideValidation")]
        public string SpecialCodeBooks { get; set; }

        public MetaDataCheckService()
        {
            this.ConfigFieldsIni();
            if (string.IsNullOrEmpty(this.SpecialCodeBooks))
            {
                throw new ArgumentNullException($"There is no any special code books definitions found.");
            }

            this._folderName = ConfigReader.GetSourceFolderName();
            if (string.IsNullOrEmpty(this._folderName))
            {
                throw new ArgumentNullException($"The schema source location is not defined.");
            }
        }


        /// <summary>
        /// Checks existence of special codebooks
        /// </summary>
        public bool Ini()
        {
            string[] files = this.SpecialCodeBooks.Split(";");

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(Path.Combine(this.FolderName, file));
                if (!fi.Exists)
                {
                    this.AddErrorMessage($"The core file {fi.FullName} is not found.");
                }
            }

            return this.IsValid;
        }


    }
}
