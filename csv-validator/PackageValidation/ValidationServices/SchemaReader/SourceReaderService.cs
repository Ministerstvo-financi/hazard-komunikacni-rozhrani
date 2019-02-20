using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ValidationPilotServices.SchemaReader
{
    /// <summary>
    /// This class specifies functions and methods to read source CSV files and convert in into
    /// Dynamic collection for the further processing.
    /// </summary>
    public class SourceReaderService
    {
        public string FolderName { get; private set; }

        public SourceReaderService(string folderName)
        {
            this.FolderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,folderName);
        }

        /// <summary>
        /// This function returns FileInfo object combined with Folder and file.
        /// </summary>
        /// <returns>FileInfo object with full file path defined.</returns>
        protected virtual FileInfo GetFileInfo(string fileName)
        {
            return new FileInfo(Path.Combine(this.FolderName, fileName));
        }

        /// <summary>
        /// This function returns collection of the records red and converted
        /// from source CSV file.
        /// CSV converter is CSVHelper.
        /// </summary>
        /// <typeparam name="T">The type of the class to convert source.</typeparam>
        /// <typeparam name="TM">The class contains mapper to convert strings.</typeparam>
        /// <param name="fileName">The name of the source CSV file.</param>
        /// <returns>The objects collection red from the CSV file.</returns>
        public virtual List<T> GetCsvData<T, TM>(string fileName) where TM : ClassMap<T>
        {
            FileInfo fi = this.GetFileInfo(fileName);

            if (!fi.Exists)
            {
                throw new FileNotFoundException($"The source file {fi.FullName} is not found.");
            }

            using (TextReader fileReader = File.OpenText(fi.FullName))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = true;
                //csv.Configuration.CultureInfo = new System.Globalization.CultureInfo("en-us");
                csv.Configuration.RegisterClassMap<TM>();

                


                return csv.GetRecords<T>().ToList();
            }
        }
    }
}
