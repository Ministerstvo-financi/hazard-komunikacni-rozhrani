using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;

namespace ValidationPilotServices.Infrastructure.Conditions
{
    public delegate bool IsNullableFieldValidationHandler(BaseValidator field, string[] value);
}
