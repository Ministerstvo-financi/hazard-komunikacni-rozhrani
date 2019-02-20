using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ValidationPilotServices.DataReader;

namespace ValidationPilotServices.DataTypes
{
    public class CodeBookCollection : IEnumerable<CodeBookItem>
    {
        public List<CodeBookItem> Items { get; set; } = new List<CodeBookItem>();

        public void Add(string key, string value, string description)
        {
            this.Items.Add(new CodeBookItem{CodeKey = key, CodeValue = value, Description = description});
        }

        public bool IsValidValue(string key, string value, out string message)
        {
            message = string.Empty;
            CodeBookItem item = this.Items.FirstOrDefault(p => p.CodeKey.Equals(key) && p.CodeValue.Equals(value));
            if (item == null)
            {
                message = $"The value requested doesn't exist in code book {key}";
                return false;
            }

            return true;
        }

        public IEnumerator<CodeBookItem> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
