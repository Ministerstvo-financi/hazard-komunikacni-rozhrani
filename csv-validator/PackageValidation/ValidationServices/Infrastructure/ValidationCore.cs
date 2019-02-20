using System.Text;

namespace ValidationPilotServices.Infrastructure
{
    public class ValidationCore
    {
        private readonly StringBuilder _errorMessageBuilder = new StringBuilder();

        public bool IsValid { get; protected set; } = true;

        public string ErrorMessage => _errorMessageBuilder == null ? string.Empty : this._errorMessageBuilder.ToString();

        protected virtual void Clean()
        {
            this.IsValid = true;
            this._errorMessageBuilder.Clear();
        }

        protected virtual void AddErrorMessage(string message)
        {
            this.IsValid = false;
            this._errorMessageBuilder.AppendLine(message);
        }
    }
}
