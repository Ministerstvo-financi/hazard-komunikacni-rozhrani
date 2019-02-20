using System;
using System.Collections.Generic;
using System.Text;
using ValidationPilotServices.LoggerService;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class LogTests : CoreTest
    {
        public LogTests(ITestOutputHelper output) : base(output)
        {
        }

       
        [Fact]
        public void PackageValidationLoggerTest()
        {
            //PackageValidatorLogger logger = new PackageValidatorLogger();
            //logger.FatalError("Fatal Error");
            //logger.Debug("Debug Message");
            //logger.Info("Info Message");

            //Assert.Throws<ArgumentException>(() => logger.InvokeFatalError<ArgumentException>("Fatal Exception invoked"));
        }

    }
}
