using System;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class ConfigReaderTests : CoreTest
    {
        public ConfigReaderTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ConfigReaderInstanceTest()
        {
            Assert.NotNull(ConfigReader.Config);
        }

        [Theory]
        [InlineData("SourceLocation")]
        [InlineData("SourceLocation:Folder")]
        [InlineData("SourceLocation:SchemaStructure")]
        [InlineData("SourceLocation:CodeBooks")]
        public void ConfigSectionExistsTest(string sectionName)
        {
            bool flag = ConfigReader.IsSectionExist(sectionName);
            Assert.True(flag);
        }

        [Theory]
        [InlineData("SourceLocation:Test")]
        [InlineData("Source:Folder")]
        [InlineData("SourceLocation:SchemaStructureId")]
        [InlineData("SourceLocation:CodeBs")]
        public void ConfigSectionDoesNotExistsTest(string sectionName)
        {
            bool flag = ConfigReader.IsSectionExist(sectionName);
            Assert.False(flag);
        }

        [Theory]
        [InlineData("SourceLocation:Folder", "SchemaSource")]
        [InlineData("SourceLocation:SchemaStructure:FileName", "fields_structure.csv")]
        [InlineData("SourceLocation:SchemaStructure:MaxLengthColumn", "MaxLength")]
        public void GetSectionFailedTest(string sectionName, string expected)
        {
            string value = ConfigReader.GetSectionStringValue(sectionName);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(typeof(FileStructureProfile), "Model", "Model")]
        [InlineData(typeof(FileStructureProfile), "Order", "ColumnId")]
        public void ReadClassAttributesParameters(Type source, string parameterName, string expected)
        {
            string value = source.GetMapPropertyFromConfig(parameterName);
            Assert.Equal(expected, value);
        }
    }
}
