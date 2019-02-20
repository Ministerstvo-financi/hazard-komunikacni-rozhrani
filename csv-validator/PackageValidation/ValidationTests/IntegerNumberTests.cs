using System;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class IntegerNumberTests : CoreTest
    {
        public IntegerNumberTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public BaseValidator IntegerNumber_Create_PassedTests()
        {
            Type type = DataTypeDefinitionExtensions.GetTypeByName("IntegerNumber");
            Assert.NotNull(type);
            BaseValidator src = (BaseValidator) Activator.CreateInstance(type);
            Assert.NotNull(src);

            return src;
        }

        [Theory]
        [InlineData("10")]
        [InlineData("1234567890")]
        [InlineData("-1234567890")]
        public void IntegerNumber_NotNullable_PassedTests(string value)
        {
            BaseValidator src = this.IntegerNumber_Create_PassedTests();
            src.IsNullable = false;
            src.Value = value;

            Assert.True(src.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("10")]
        [InlineData("1234567890")]
        [InlineData("-1234567890")]
        public void IntegerNumber_Nullable_PassedTests(string value)
        {
            BaseValidator src = this.IntegerNumber_Create_PassedTests();
            src.IsNullable = true;
            src.Value = value;

            Assert.True(src.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("-")]
        [InlineData("2147483650")]
        [InlineData("-2147483670")]
        [InlineData("-2147t48360")]
        [InlineData("-21.90")]
        [InlineData("21,90")]
        public void IntegerNumber_NotNullable_FailedTests(string value)
        {
            BaseValidator src = this.IntegerNumber_Create_PassedTests();
            src.IsNullable = false;
            src.Value = value;

            Assert.False(src.IsValid);
        }

        [Theory]
        [InlineData("2147483650")]
        [InlineData("-2147483670")]
        [InlineData("-2147t48360")]
        [InlineData("-21.90")]
        [InlineData("21,90")]
        public void IntegerNumber_Nullable_FailedTests(string value)
        {
            BaseValidator src = this.IntegerNumber_Create_PassedTests();
            src.IsNullable = false;
            src.Value = value;

            Assert.False(src.IsValid);
        }
    }
}
