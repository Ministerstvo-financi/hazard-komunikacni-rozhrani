using System.Text;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.SchemaReader;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class CoreTest
    {
        protected readonly ITestOutputHelper Output;

        public CoreTest(ITestOutputHelper output)
        {
            this.Output = output;
        }

        protected string GetArrayToString(string[] array)
        {
            StringBuilder bld = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                bld.Append($"{i} - {array[i]}, ");
            }

            return bld.ToString();
        }

        protected Fields GetFieldsStructureCollection(string model)
        {
            FieldsStructureReaderService service = new FieldsStructureReaderService(model);
            return service.Get();
        }
    }
}
