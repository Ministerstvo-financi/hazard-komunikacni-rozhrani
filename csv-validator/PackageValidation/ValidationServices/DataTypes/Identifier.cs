using System;
using System.Text.RegularExpressions;

namespace ValidationPilotServices.DataTypes
{
    public class Identifier : BaseValidator
    {
        public Identifier()
        {
            this.pattern = new Regex(@"(\w|\-)+",RegexOptions.Compiled);
        }

        public Identifier(int minLength, int maxLength)
        {
            this.pattern = new Regex(@"(\w|\-){" + $"1,{maxLength}" + "}",RegexOptions.Compiled);
        }
    }
}
