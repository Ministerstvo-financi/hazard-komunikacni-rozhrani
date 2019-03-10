using System;
using System.Collections.Generic;
using System.Text;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class DateTests : CoreTest
    {
        public DateTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public BaseValidator Date_Create_PassedTests()
        {
            Type type = DataTypeDefinitionExtensions.GetTypeByName("Date");
            Assert.NotNull(type);

            BaseValidator src = (BaseValidator)Activator.CreateInstance(type);
            Assert.NotNull(src);
            return src;
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("2017-10-10", false)]
        [InlineData("2017-02-02", true)]
        [InlineData("2007-11-21", true)]
        // [InlineData("2005-12-31T23:59:60.6-08:00", true)] - leap second not supported
        public void DatePassedTest(string value, bool isNullable)
        {
            BaseValidator src = Date_Create_PassedTests();
            src.IsNullable = isNullable;
            src.Value = value;
            Console.WriteLine($"Tested value: {value}");
            Assert.True(src.IsValid);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("2017-10-10T10:30:10.0+01:00", false)]
        [InlineData("xxx", true)]
        [InlineData("2007-40-21", true)]
        // [InlineData("2005-12-31T23:59:60.6-08:00", true)] - leap second not supported
        public void DateFailedTest(string value, bool isNullable)
        {
            BaseValidator src = Date_Create_PassedTests();
            src.IsNullable = isNullable;
            src.Value = value;
            Console.WriteLine($"Tested value: {value}");
            Assert.False(src.IsValid);
        }



    }
}
