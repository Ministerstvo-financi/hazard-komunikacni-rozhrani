using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.SchemaReader;
using ValidationPilotServices.SchemaReader.Profiles;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class FieldsStructureReaderTests : CoreTest
    {
        public FieldsStructureReaderTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ServiceReaderInstanceTest()
        {
            FieldsStructureReaderService service = new FieldsStructureReaderService("M");
            Assert.Equal("fields_structure.csv", service.FileName);
        }

        [Fact]
        public void ReadCsvFileToProfilerTest()
        {
            FieldsStructureReaderService service = new FieldsStructureReaderService("V");
            var source = service.GetCsvData();

            Assert.NotEmpty(source);

            foreach (FileStructureProfile item in source)
            {
                this.Output
                    .WriteLine($"Model: {item.Model}, " +
                               $"Condition: {item.Condition}, - " +
                               $"FileName: {item.File}, " +
                               $"Order: {item.Order}, " +
                               $"Field: {item.FieldName}, " +
                               $"KEY Field: {item.FieldNameKey}, " +
                               $"Type: {item.FieldType}, " +
                               $"Max Length: {item.MaxLength}");
            }
        }

        [Fact]
        public void GetFieldsStructureTest()
        {
            FieldsStructureReaderService service = new FieldsStructureReaderService("V");
            Fields fields = service.Get();

            Assert.NotNull(fields);
            Assert.NotEmpty(fields.Items);

            foreach (FieldItem item in fields.Items)
            {
                this.Output.WriteLine(
                    $"Name - {item.FieldName}, Type - {item.FieldType.GetType().ToString()}, Nullable Condition - {item.IsNullableCondition}");
            }

        }
    }
}
