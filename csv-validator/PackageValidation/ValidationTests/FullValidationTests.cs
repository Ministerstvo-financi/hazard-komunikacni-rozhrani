using System;
using System.Collections.Generic;
using System.Text;
using ValidationPilotServices.Infrastructure;
using ValidationPilotServices.Infrastructure.Enums;
using Xunit;
using Xunit.Abstractions;
using PackageValidation;

namespace ValidationPilotTests
{
    public class FullValidationTests : CoreTest
    {
        public FullValidationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CreateValidationResultTest()
        {
            PackageValidation.Program.Main(new string[]{"DataSource/28934929-M-2019061208-L-01"});
        }

        [Fact]
        public void TestV(){
            PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-L-01"});
        }

        [Fact]
        public void TestFile(){
            //PackageValidation.Program.Main(new string[]{"DataSource/28934929-M-201906-B-cas01-01/"});
            //PackageValidation.Program.Main(new string[]{"DataSource/28934929-M-201906-B-cas01-01"});

            //PackageValidation.Program.Main(new string[]{"DataSource/30123456-M-201902-Z-456-01"});

            // PackageValidation.Program.Main(new string[]{"DataSource/14613549-V-2018051416-K-01","provozovatel.csv"});
            //PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-K-01"});
            //PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-K-02"});
            //PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019063016-K-01"});
            PackageValidation.Program.Main(new string[]{"/home/rasekl/buffer/27625231-V-2019070816-T-01"});
            //PackageValidation.Program.Main(new string[]{"/home//buffer/47252481-V-2019051500-T-02"});
            // PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-R-01"});
            // PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-B-01"});
            // PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-T-01"});
            // PackageValidation.Program.Main(new string[]{"DataSource/28934929-V-2019061208-Z-01"});
        }
    }
}
