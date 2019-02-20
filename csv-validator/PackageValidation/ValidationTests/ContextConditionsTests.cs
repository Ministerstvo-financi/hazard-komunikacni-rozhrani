using System;
using System.Linq;
using System.Reflection;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Conditions;
using ValidationPilotServices.Infrastructure.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class ContextConditionsTests : CoreTest
    {
        public ContextConditionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetExtensionsMembersTests()
        {
            MethodInfo[] info = typeof(ContextConditionsExtensions).GetMethods();
            Assert.True(info.Any());
        }

        [Fact]
        public void GetExtensionConditionPassedTest()
        {
            string methodName = "withinPackageTimespan";
            MethodInfo method = typeof(ContextConditionsExtensions)
                .GetMethods()
                .FirstOrDefault(p => p.GetCustomAttributes<ContextConditionAttribute>()
                    .FirstOrDefault(a => a.MethodToProcess.Equals(methodName.ToUpper()))!= null);


            Assert.NotNull(method);
            FieldItem item = this.GetFieldItem();
        }

        [Theory]
        [InlineData("DigitsNumber", 2)]
        [InlineData("LengthEquals", 102)]
        public void PredefinedMethodRegExpressionTest(string exp, int param)
        {
            string expression = $"{exp}({param})";
            bool isPredFlag = ContextConditionsExtensions.IsPredefinedConditionExpression(expression);
            Assert.True(isPredFlag);

            int argument = ContextConditionsExtensions.GetPredefinedConditionArgument(expression);
            Assert.True(argument == param);

            string fn = ContextConditionsExtensions.GetPredefinedConditionFunction(expression);
            Assert.Equal(exp, fn);
        }

        #region MyRegion


        public FieldItem GetFieldItem()
        {
            return new FieldItem
            {
                FieldName = "TestAgain",
                FieldType = new Identifier()
            };
        }

        #endregion
    }
}
