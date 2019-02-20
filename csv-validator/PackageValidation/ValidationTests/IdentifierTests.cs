using System;
using System.Reflection;
using System.Text.RegularExpressions;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class IdentifierTests : CoreTest
    {
        private readonly string _regExpression = @"(\w|\-){1,20}";

        public IdentifierTests(ITestOutputHelper output) : base(output)
        {
        }

        #region IDENTIFIER DATA TYPE REG EXPRESSION STRIING PARSER

        [Theory]
        [InlineData("AA_-OO1290")]
        [InlineData("_tttOverdrive1280238")]
        [InlineData("12345678901234567890")]
        public void RegExpressionCorrectTest(string value)
        {
            Regex regex = new Regex(this._regExpression);
            Match match = regex.Match(value, 0, value.Length);

            bool flag = match.Success && (match.Value.Equals(value));
            Assert.True(flag);
        }

        /// <summary>
        /// This procedure tests the regular string to validate identifier data
        /// type when input value has wrong format.
        /// </summary>
        /// <param name="value">The string to convert into identifier data type.</param>
        [Theory]
        [InlineData("AA_!-OO1290")]
        [InlineData("_tttOverdrive*12802389023-")]
        [InlineData("%_tttOverdrive*12802389023-")]
        public void RegExpressionFailTest(string value)
        {
            Regex regex = new Regex(this._regExpression);
            Match match = regex.Match(value, 0, value.Length);

            bool flag = match.Success && (match.Value.Equals(value));
            Assert.False(flag);
        }

        #endregion

        #region IDENTIFIER  TESTS - LENGTH 20

        /// <summary>
        /// This procedure tests the identifier data tape (not nullable) with
        /// failed result.
        /// </summary>
        /// <param name="value">The value is not valid identifier format.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("123456789012-3456789012345")]
        [InlineData("AAA!k243)___%")]
        [InlineData("AD89#")]
        public void Identifier_20_NotNullable_FailTest(string value)
        {
            Identifier src = new Identifier { IsNullable = false };
            src.Value = value;

            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");
            Assert.False(src.IsValid);
        }

        /// <summary>
        /// This procedure tests the identifier data tape (nullable) with
        /// failed result.
        /// </summary>
        /// <param name="value">The value is not valid identifier format.</param>
        [Theory]
        [InlineData("123456789012-3456789012345")]
        [InlineData("AAA!k243)___%")]
        [InlineData("AD89#")]
        public void Identifier_20_Nullable_FailTest(string value)
        {
            Identifier src = new Identifier();
            src.IsNullable = true;
            src.Value = value;

            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");
            Assert.False(src.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("12345678901234567890")]
        [InlineData("TEST-67-555_8")]
        [InlineData("AD_6")]
        public void Identifier_20_Nullable_PassTest(string value)
        {
            Identifier src = new Identifier();
            src.IsNullable = true;
            src.Value = value;

            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");

            Assert.True(src.IsValid);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12345678901234567890")]
        [InlineData("TEST-67-555_8")]
        [InlineData("AD_6")]
        public void Identifier_20_NotNullable_PassTest(string value)
        {
            Identifier src = new Identifier();
            src.IsNullable = false;
            src.Value = value;

            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");

            Assert.True(src.IsValid);
        }

        #endregion


        [Theory]
        [InlineData("123456789012345678901234567890123456789012345678901234567890")]
        [InlineData("A@@@AA!k243)___%")]
        [InlineData("AD89#")]
        public void Identifier_50_Nullable_FailTest(string value)
        {
            Identifier src = new Identifier(50);
            src.IsNullable = true;
            src.Value = value;

            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");

            Assert.False(src.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("123456789012345678901234567890123456789012345678901234567890")]
        [InlineData("A@@@AA!k243)___%")]
        [InlineData("AD89#")]
        public void Identifier_50_NotNullable_FailTest(string value)
        {
            Identifier src = new Identifier(50);
            src.IsNullable = false;
            src.Value = value;

            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");

            Assert.False(src.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1")]
        [InlineData("12345678901234567890123-5_7890AsVtr67890Ynio567890")]
        [InlineData("TEST-67-555_8")]
        [InlineData("AD_6")]
        public void Identifier_50_Nullable_PassTest(string value)
        {
            Identifier src = new Identifier(50);
            src.IsNullable = true;
            src.Value = value;
            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");

            Assert.True(src.IsValid);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12345678901234567890123-5_7890AsVtr67890Ynio567890")]
        [InlineData("TEST-67-555_8")]
        [InlineData("AD_6")]
        public void Identifier_50_NotNullable_PassTest(string value)
        {
            Identifier src = new Identifier(50);
            src.IsNullable = false;
            src.Value = value;
            this.Output.WriteLine($"Source Value = {value}, Target Value = {src.Value}");

            Assert.True(src.IsValid);
        }
        
        [Theory]
        [InlineData(20, "1")]
        [InlineData(20, "12345678901234567890")]
        [InlineData(20, "TEST-67-555_8")]
        [InlineData(20, "AD_6")]
        [InlineData(50, "1")]
        [InlineData(50, "12345678901234567890123-5_7890AsVtr67890YnIo567890")]
        [InlineData(50, "TEST-67-555_8")]
        [InlineData(50, "AD_6")]
        public void IdentifierFromType_PassedTests(int len, string value)
        {
            string dataType = "Identifier";
            Type type = DataTypeDefinitionExtensions.GetTypeByName(dataType);

            Assert.NotNull(type);
            if (Activator.CreateInstance(type, new object[] {len}) is BaseValidator src)
            {
                src.Value = value;
                Assert.True(src.IsValid);
            }
        }
    }
}
