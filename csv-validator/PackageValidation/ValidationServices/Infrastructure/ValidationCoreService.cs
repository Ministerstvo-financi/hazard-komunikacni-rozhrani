using System.Text;

namespace ValidationPilotServices.Infrastructure
{
    public class ValidationCoreService
    {

        protected readonly string error_message_template = "{0};{1};{2};{3};{4};\"{5}\"";

        protected int ErrorsCounter { get; set; } = 0;

        public bool IsValid { get; protected set; } = true;


        public ValidationCoreService()
        {
        }

        /// <summary>
        /// This procedure clean error messages stack.
        /// </summary>
        protected virtual void Clean()
        {
            this.IsValid = true;
            this.ErrorsCounter = 0;
        }

        protected virtual void LogError(string message)
        {
            this.IsValid = false;
            this.ErrorsCounter++;
            LoggerService.LoggerService.GetValidationErrorsLog().Error(message);
        }

        /// <summary>
        /// This procedure writes to log the message of INFO level.
        /// </summary>
        /// <param name="message">The message to write to log file.</param>
        protected virtual void LogInfo(string message)
        {
            LoggerService.LoggerService.GetValidationErrorsLog().Info(message);
        }

        protected virtual void LogDebug(string message)
        {
            LoggerService.LoggerService.GetValidationErrorsLog().Debug(message);
        }

        protected virtual void LogFatal(string message)
        {
            LoggerService.LoggerService.GetValidationErrorsLog().Fatal(message);
        }

        protected virtual void LogWarn(string message)
        {
            LoggerService.LoggerService.GetValidationErrorsLog().Warn(message);
        }
    }
}
