using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidationPilotServices.Infrastructure.Enums;


namespace ValidationPilotServices.DataTypes
{

    public delegate void ValidationErrorMessageDelegate(EnumValidationResult errorType, string currentField, int rowId, string errorMsg);
    
    public class Fields : IEnumerable<FieldItem>
    {
        /// <summary>
        /// GET; SET;
        /// Fields collection to validate files.
        /// </summary>
        public List<FieldItem> Items { get; set; } = new List<FieldItem>();

        public IEnumerator<FieldItem> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(string fileName,
            string order,
            string fieldName)
        {
            FieldItem item = new FieldItem
            {
                FileSource = fileName,
                FieldName = fieldName,
                Order = int.Parse(order)
            };

            this.Items.Add(item);
        }

        /// <summary>
        /// This function returns FieldItem item from Items collection selected
        /// by file name and field name.
        /// If this field doesn't exist in the Items collection, this function returns NULL.
        /// </summary>
        /// <param name="fileName">The file name where this field is validating.</param>
        /// <param name="fieldName">The field name to validate.</param>
        /// <returns>The Field Item selected from the collection by file name and field name.</returns>
        public FieldItem GetValidator(string fileName, string fieldName)
        {
            return this.Items.FirstOrDefault(p => p.FileSource.ToUpper().Equals(fileName.ToUpper()) && p.FieldName.ToUpper().Equals(fieldName.ToUpper()));
        }

        public bool ValidateFieldsComposition(ValidationErrorMessageDelegate ValidationErrorMessage, string fileName, string[] fieldsArray)
        {
            bool valid = true;
            List<FieldItem> collection =
                this.Items.Where(p => p.FileSource.ToUpper().Equals(fileName.ToUpper())).ToList();
            
            List<string> fieldsArrayUpper = fieldsArray.ToList().ConvertAll((s)=>s.ToUpper());

            if (collection.Count != fieldsArray.Length)
            {
                ValidationErrorMessage(EnumValidationResult.ERR_LINE_BAD_HEADER,"",1,$"The number of fields in file structure ({collection.Count}) and fields in the file received ({fieldsArray.Length}) are different.");
                valid = false;  
            }

            foreach (FieldItem field in collection)
            {
                if (!fieldsArrayUpper.Contains(field.FieldNameUpper))
                {
                    valid = false;
                    ValidationErrorMessage(EnumValidationResult.ERR_LINE_BAD_HEADER,"",1,$"The field {field.FieldName} is not presented in the file received; ");
                }
            }


            for ( var i = 0; i<fieldsArrayUpper.Count ; i++)
            {
                string field = fieldsArrayUpper[i];
                FieldItem value = collection.FirstOrDefault(p => p.FieldName.ToUpper().Equals(field.ToUpper()));
                if (value == null)
                {
                    valid = false;
                    ValidationErrorMessage(EnumValidationResult.ERR_LINE_BAD_HEADER,"",1,$"The field {field} of the file received is not presented in the structure; ");
                } else {
                    if ( value.Order != i+1 ){
                        valid = false;
                        ValidationErrorMessage(EnumValidationResult.ERR_LINE_BAD_HEADER,"",1,
                                $"The field {field} of the file received is at a bad position found at {i+1}, should be at {value.Order} ");
                    }
                }
            }
            return valid;
        }
    }
}
