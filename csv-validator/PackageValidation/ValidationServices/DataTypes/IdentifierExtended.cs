using System;
using System.Text.RegularExpressions;

namespace ValidationPilotServices.DataTypes
{
    public class IdentifierExtended : BaseValidator
    {
        public IdentifierExtended()
        {
            this.pattern = new Regex(@"\w+(-.*)?",RegexOptions.Compiled);
        }

        public IdentifierExtended(int minLength, int maxLength)
        {
            this.pattern = new Regex(@"\w+(-.*)?",RegexOptions.Compiled);
        }
    }
}
