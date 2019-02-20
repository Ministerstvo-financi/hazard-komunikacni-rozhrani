using System;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class DateAndTimeTests : CoreTest
    {
        public DateAndTimeTests(ITestOutputHelper output) : base(output)
        {
        }

        #region BASIC DATE AND TIME TESTS

        [Fact]
        public BaseValidator DateAndTime_Create_PassedTests()
        {
            Type type = DataTypeDefinitionExtensions.GetTypeByName("DateAndTime");
            Assert.NotNull(type);
            BaseValidator src = (BaseValidator)Activator.CreateInstance(type);
            Assert.NotNull(src);

            return src;
        }
        
        [Theory]
        [InlineData("", true)]
        [InlineData("2017-10-10T23:12:30.5Z", false)]
        [InlineData("2017-10-10T23:12:30.3+00:01", true)]
        [InlineData("2007-11-21T20:12:30.0-13:00", true)]
        // [InlineData("2005-12-31T23:59:60.6-08:00", true)] - leap second not supported
        public void DateAndTimePassedTest(string value, bool isNullable)
        {
            BaseValidator src = DateAndTime_Create_PassedTests();
            src.IsNullable = isNullable;
            src.Value = value;
            Console.WriteLine($"Tested value: {value}");
            Assert.True(src.IsValid);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("201-10-10T23:12:30.5Z", false)]
        [InlineData("2017-12-32T23:12:30.5Z", true)]
        [InlineData("2007-11-21T25:12:30.5Z", true)]
        public void DateAndTimeFailedTest(string value, bool isNullable)
        {
            BaseValidator src = DateAndTime_Create_PassedTests();
            src.IsNullable = isNullable;
            src.Value = value;
            Assert.False(src.IsValid);
        }

        #endregion





        [Theory]
        [InlineData("", true)]
        [InlineData("2017-10-10T23:12:30.5Z", false)]
        [InlineData("2017-10-10T23:12:30.5Z", true)]
        [InlineData("2007-11-21T20:12:30.5+01:00", true)]
        public void DateAndTimeFromBuilderPassedTest(string value, bool isNullable)
        {
            FileStructureProfile source = this.GetProfileForDateAndTime(isNullable);
            BaseValidator field = source.ValidatorFieldBuilder();
            field.Value = value;
            bool isValid = field.IsValid;

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("201-10-10T23:12:30.5Z", false)]
        [InlineData("2017-12-32T23:12:30.5Z", true)]
        [InlineData("2007-11-21T25:12:30.5Z", true)]
        public void DateAndTimeFromBuilderFailedTest(string value, bool isNullable)
        {
            FileStructureProfile source = this.GetProfileForDateAndTime(isNullable);
            BaseValidator field = source.ValidatorFieldBuilder();
            field.Value = value;
            bool isValid = field.IsValid;

            Assert.False(isValid);
        }

        #region STUFF METHODS AND FUNCTIONS

        private FileStructureProfile GetProfileForDateAndTime(bool isNullable)
        {
            return new FileStructureProfile
            {
                FieldType = "DateAndTime",
                IsNullable = isNullable ? "Y" : "N"
            };
        }

        #endregion
    }
}
