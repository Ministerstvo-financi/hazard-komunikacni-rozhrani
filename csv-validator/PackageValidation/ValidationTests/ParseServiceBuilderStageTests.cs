using CsvHelper;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.SchemaReader;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class ParseServiceBuilderStageTests : CoreTest
    {
        public ParseServiceBuilderStageTests(ITestOutputHelper output) : base(output)
        {
        }

        #region FUNCTIONS AND PROCEDURES SUPPORT 

        private Fields GetFields()
        {
            FieldsStructureReaderService service = new FieldsStructureReaderService("V");
            return service.Get();
        }

        private FileInfo GetFileInfo(string fileName)
        {
            return new FileInfo(Path.Combine(@"DataSource", fileName));
        }

        #endregion

        [Fact]
        public void ValidateFileStage_ManualTest()
        {
            string filename = "provozovatel.csv";
            Fields fields = this.GetFields();
            Assert.NotEmpty(fields.Items);

            FileInfo fi = this.GetFileInfo(filename);
            Assert.True(fi.Exists);

            using (TextReader fileReader = File.OpenText(fi.FullName))
            {
                var csv = new CsvReader(fileReader,System.Globalization.CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                while (csv.Read())
                {
                    ExpandoObject row = csv.GetRecord<dynamic>();

                    foreach (KeyValuePair<string, object> key in row.ToList())
                    {
                        FieldItem fieldItem = fields.GetValidator(filename, key.Key);

                        if (fieldItem != null)
                        {
                            fieldItem.FieldType.Value = key.Value.ToString();
                            bool flag = fieldItem.FieldType.IsValid;

                            this.Output.WriteLine(
                                $"Type - {fieldItem.FieldType.GetType().ToString()}, Value to Validate - {key.Value.ToString()}, IsValid - {flag}");

                            Assert.True(flag);
                        }
                    }
                }
            }
        }

        [Fact]
        public void CustomFileCsvReaderTest()
        {
            string filename = "misto.csv";
            FileInfo fi = this.GetFileInfo(filename);
            int headerCount = 0;
            Assert.True(fi.Exists);

            using (StreamReader reader = new StreamReader(fi.FullName, false))
            {
                using (var csv = new CsvReader(reader,System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = ";";
                    int lineCounter = 0;

                    while (csv.Read())
                    {
                        if (lineCounter == 0)
                        {
                            lineCounter++;
                            continue;
                        }
                        else if (lineCounter == 1)
                        {
                            csv.ReadHeader();
                            headerCount = csv.Context.Record.Length;
                            lineCounter++;
                            continue;
                        }

                        ExpandoObject row = csv.GetRecord<dynamic>();

                        if (csv.Context.Record.Length != headerCount)
                        {
                            this.Output.WriteLine("Records Error");
                        }

                        foreach (KeyValuePair<string, object> key in row.ToList())
                        {
                            this.Output.WriteLine($"FieldName: {key.Key}, FieldValue = {key.Value.ToString()}");
                        }

                        lineCounter++;
                    }
                }
            }
        }
    }
}
