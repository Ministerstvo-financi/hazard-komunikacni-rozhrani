using System;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader.Profiles;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class DecimalNumberTests : CoreTest
    {
        public DecimalNumberTests(ITestOutputHelper output) : base(output)
        {
        }

    
        [Fact]
        public void DecimalNumberType_FromProfilerTest()
        {
            FileStructureProfile source = this.GetProfileForDecimal();
            BaseValidator field = source.ValidatorFieldBuilder();

            bool flag = field is DecimalNumber;
            Assert.True(flag);
        }

        [Theory]
        [InlineData("D_GPS","34,4567")]
        [InlineData("D_GPS","34,45673")]
        [InlineData("D_GPS","34,456756")]
        [InlineData("D_GPS","34,4567677")]
        [InlineData("D_GPS","0,4567677")]
        [InlineData("D_GPS","1,4567677")]
        [InlineData("D_GPS","11,4567677")]
        [InlineData("D_GPS","-11,4567677")]
        [InlineData("D_GPS","-1,4567677")]
        [InlineData("D_GPS","0,0000")]
        [InlineData("D_MONEY","111,45")]
        [InlineData("D_MONEY","-11,4")]
        [InlineData("D_MONEY","11111111111,")]
        [InlineData("D_MONEY","11111111111")]
        [InlineData("D_MONEY","0")]
        [InlineData("D_COURSE","34,456")]
        [InlineData("D_COURSE","34,45")]
        [InlineData("D_COURSE","34,4")]
        [InlineData("D_COURSE","34,")]
        [InlineData("D_COURSE","34")]
        public void PasseDecimal(string domainType, string value){
            var num = new DecimalNumber(domainType);
            num.Value =value;
            Assert.True(num.IsValid);
        }

        [Theory]
        [InlineData("D_GPS","34,456")]
        [InlineData("D_GPS","34,45")]
        [InlineData("D_GPS","34,4")]
        [InlineData("D_GPS","34,")]
        [InlineData("D_GPS","34")]
        [InlineData("D_GPS","0")]
        [InlineData("D_MONEY","0,09809")]
        [InlineData("D_COURSE","34,45600")]
        [InlineData("D_COURSE","xxx")]
        [InlineData("D_COURSE","-dslkfj")]
        [InlineData("D_COURSE","-ll56")]
        [InlineData("D_COURSE","-")]
        public void FailedDecimal(string domainType, string value){
            var num = new DecimalNumber(domainType);
            num.Value =value;
            Assert.False(num.IsValid);
        }



        [Theory]
        [InlineData("10")]
        [InlineData("1234567890")]
        [InlineData("-1234567890")]
        [InlineData("123456789012")]
        [InlineData("-123456789012")]
        [InlineData("0")]
        [InlineData("-12345,67")]
        [InlineData("1234567,8")]
        [InlineData("-123456,")]
        public void DecimalNumber_FromProfiler_PassedTest(string value)
        {
            FileStructureProfile source = this.GetProfileForDecimal();
            BaseValidator field = source.ValidatorFieldBuilder();
            field.Value = value;

            bool flag = field.IsValid;
            Assert.True(flag);
        }

        [Theory]
        [InlineData("")]
        [InlineData("10")]
        [InlineData("1234567890")]
        [InlineData("-1234567890")]
        [InlineData("123456789012")]
        [InlineData("-123456789012")]
        [InlineData("0")]
        [InlineData("-12345,67890")]
        [InlineData("1234567,8901")]
        [InlineData("-123456,78901")]
        public void DecimalNumber_FromProfiler_NullablePassedTest(string value)
        {
            FileStructureProfile source = this.GetProfileForNullableDecimal();
            BaseValidator field = source.ValidatorFieldBuilder();
            field.IsNullable = true;
            field.Value = value;

            bool flag = field.IsValid;
            Assert.True(flag);
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678.90")]
        [InlineData("-12345678900000000000000000000000")]
        [InlineData("12345678901200000000000000000000")]
        [InlineData("-1234567890%12")]
        [InlineData("Z")]
        public void DecimalNumber_FromProfiler_FiledTest(string value)
        {
            FileStructureProfile source = this.GetProfileForDecimal();
            BaseValidator field = source.ValidatorFieldBuilder();
            field.Value = value;

            bool flag = field.IsValid;
            Assert.False(flag);
        }
        
        #region STUFF METHODS AND FUNCTIONS

        private FileStructureProfile GetProfileForDecimal()
        {
            return new FileStructureProfile
            {
                FieldType = "DecimalNumber"
            };
        }

        private FileStructureProfile GetProfileForNullableDecimal()
        {
            return new FileStructureProfile
            {
                FieldType = "DecimalNumber",
                IsNullable = "Y"
            };
        }

        #endregion
    }
}
