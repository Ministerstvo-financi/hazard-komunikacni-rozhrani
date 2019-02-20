using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.Infrastructure.Extensions;

namespace ValidationPilotTests
{
    public class DataTypeDefenitionTests : CoreTest
    {
        public DataTypeDefenitionTests(ITestOutputHelper output) : base(output)
        {
        }

     
        [Theory]
        [InlineData("Identifier")]
        [InlineData("DecimalNumber")]
        [InlineData("IntegerNumber")]
        [InlineData("Text")]
        public void GetDataTypeTests(string typeName)
        {
            Type type = DataTypeDefinitionExtensions.GetTypeByName(typeName);

            Assert.NotNull(type);

            this.Output.WriteLine(type.FullName);
        }

       
    }
}
