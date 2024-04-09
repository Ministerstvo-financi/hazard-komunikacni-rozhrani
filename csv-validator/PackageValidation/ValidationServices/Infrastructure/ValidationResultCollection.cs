using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotServices.Infrastructure
{
    public class ValidationResultCollection
    {
        public class ResultItem
        {
            public string PackageName { get; set; }
            public string FileName { get; set; }
            public string ValidationResult { get; set; }
            public int LineCount { get; set; }
            public string Hash { get; set; }
        }

        private readonly string _outputFileName = "validation-report.csv";
        private readonly string _packageNumber;
        private readonly string _fullReportFileName;
        private readonly string _sourcePath;

        public List<ResultItem> Items { get; set; } = new List<ResultItem>();

        public ValidationResultCollection(string packageNumber, string fileFullPath)
        {
            this._packageNumber = packageNumber;
            this._fullReportFileName = new FileInfo(Path.Combine(fileFullPath, this._outputFileName)).FullName;
            this._sourcePath = fileFullPath;
        }

        public void AddItem(string fileName, int lines, EnumValidationResult result, string hashcode)
        {
            this.Items.Add(new ResultItem
            {
                PackageName = this._packageNumber,
                FileName = fileName,
                LineCount = lines,
                ValidationResult = result.GetItemDescription(),
                Hash = hashcode
            });
        }

        public bool Write()
        {
            using (var writer = new StreamWriter(this._fullReportFileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter=";";
                csv.WriteRecords<ResultItem>(this.Items);
            }

            return true;
        }
    }
}
