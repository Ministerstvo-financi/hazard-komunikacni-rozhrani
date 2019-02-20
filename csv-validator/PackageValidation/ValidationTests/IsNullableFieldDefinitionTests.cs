using System.Text.RegularExpressions;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.Infrastructure.Enums;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    /// <summary>
    /// Expression Collection:
    ///     @FieldName.IsEmpty()            -
    ///     @FieldName.NotEmpty()
    ///     @FieldName.InRange(value1, value2,...)
    ///     @FieldName.NotInRange(value1, value2,...)
    /// </summary>
    public class IsNullableFieldDefinitionTests : CoreTest
    {
        public IsNullableFieldDefinitionTests(ITestOutputHelper output) : base(output)
        {
        }

        #region Field Is Empty Condition Parse

        private readonly string FieldIsEmptyConditionString = @"([A-Za-z0-9]{1,}).(IsEmpty\(\))";
        
        //[Fact]
        //public void IsEmptyFileProcessingTest()
        //{
        //    //create package
        //    PackageType package = new PackageType(EnumModelReport.DAILY, this.PackageNumber);
        //    package.Ini();

        //    Assert.True(package.IsValid);

        //    //dataReader initialize
        //    DataReaderService service = new DataReaderService(package);
        //    service.ReadFileData(this.TestFileName);

        //    this.Output.WriteLine(service.ErrorMessage);

        //    Assert.True(service.IsValid);
        //}

        [Theory]
        [InlineData("{0}.IsEmpty()", "Code")]
        [InlineData("{0}.IsEmpty()", "LocationCode")]
        [InlineData("{0}.IsEmpty()", "IDMainGameType")]
        public void FieldIsEmptyExpressionValid(string template, string value)
        {
            string regString = string.Format(template, value);
            Regex regex = new Regex(this.FieldIsEmptyConditionString, RegexOptions.Compiled);
            Match match = regex.Match(regString, 0, regString.Length);

            this.Output.WriteLine($"Groups Count: {match.Groups.Count}, " +
                                  $"Expression: {regString}");

            Assert.True(match.Groups.Count > 2);
            Assert.Equal(value, match.Groups[1].Value);
            Assert.Equal("IsEmpty()", match.Groups[2].Value);
        }

        [Theory]
        [InlineData("{0}.IsEty()", "Code")]
        [InlineData("{0}.IsEmpty(m)", "LocationCode")]
        public void FieldIsEmptyExpressionNotValid(string template, string value)
        {
            string regString = string.Format(template, value);
            Regex regex = new Regex(this.FieldIsEmptyConditionString, RegexOptions.Compiled);
            Match match = regex.Match(regString, 0, regString.Length);

            this.Output.WriteLine($"Groups Count: {match.Groups.Count}, " +
                                  $"Expression: {regString}");

            Assert.False(match.Groups.Count > 2);
            Assert.NotEqual(value, match.Groups[1].Value);
            Assert.NotEqual("IsEmpty", match.Groups[2].Value);
        }

        #endregion

        #region FIELD IN CONDITION PARSE

        private readonly string FieldInConditionString = @"([A-Za-z0-9]{1,}).InRange(\(((\w|,?)*)\))";

        [Theory]
        [InlineData("{0}.InRange(12,y,T,IR,h1,h)", "Account", 6)]
        [InlineData("{0}.InRange(12,Y,T)", "Cash", 3)]
        [InlineData("{0}.InRange(1)", "OutField", 1)]
        public void FieldInExpressionParseTest(string input, string field, int count)
        {
            int minGroups = 2;
            string inputValue = string.Format(input, field);
            Regex regex = new Regex(this.FieldInConditionString, RegexOptions.Compiled);
            Match match = regex.Match(inputValue, 0, inputValue.Length);

            Assert.True(match.Groups.Count >= minGroups);

            string fieldValue = match.Groups[1].Value;
            string[] array = match.Groups[3].Value.Split(",");

            Assert.True(array.Length == count);
            Assert.Equal(field, fieldValue);

            string s = this.GetArrayToString(array);
            this.Output.WriteLine(s);
        }

        #endregion
    }
}
