using System;
using System.Text.RegularExpressions;

namespace ValidationPilotServices.DataTypes
{
    public class Identifier : BaseValidator
    {
        public Identifier()
        {
            this.pattern = new Regex(@"(\w|\-){1,20}",RegexOptions.Compiled);
        }

        public Identifier(string reg)
        {
            this.pattern = new Regex(reg ?? throw new ArgumentNullException($"Identifier type must has a regex pattern defined."),RegexOptions.Compiled);
        }

        public Identifier(int maxLength)
        {
            this.pattern = new Regex(@"(\w|\-){" + $"1,{maxLength}" + "}",RegexOptions.Compiled);
        }
    }
}
