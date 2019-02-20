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
    }
}
