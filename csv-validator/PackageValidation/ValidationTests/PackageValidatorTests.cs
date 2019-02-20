using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class PackageValidatorTests : CoreTest
    {
        private readonly string remote_regex = @"^([A-Za-z0-9_]{1,20})\-(V{1})\-([0-9]{10})\-([a-zA-Z]{1})\-([0-9]{2})$";
        private readonly string daily_regex = @"^([A-Za-z0-9_]{1,20})\-(M{1})\-([0-9]{6})\-([a-zA-Z]{1})\-([A-Za-z0-9_]{1,50})\-([0-9]{2})$";

        public PackageValidatorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("20020020-V-2017012108-L-01")]
        [InlineData("OP-V-2017012308-R-01")]
        [InlineData("97_12ID-V-2018012108-B-05")]
        [InlineData("D-V-2018062516-t-21")]
        public void PackageNumberRemoteParseFuncPassedTest(string source)
        {
            int groups = 6;
            Regex regex = new Regex(this.remote_regex, RegexOptions.Compiled);
            Match match = regex.Match(source, 0, source.Length);

            Assert.Equal(groups, match.Groups.Count);
        }

        [Theory]
        [InlineData("V-2017012108-L-01")]
        [InlineData("")]
        [InlineData("97_12ID-V-2018012108-B-M5")]
        [InlineData("D-V-2018062516-1-21")]
        [InlineData("D-V-20X8062516-C-21")]
        [InlineData("D-V-20180625-C-21")]
        public void PackageNumberRemoteParseFuncFailedTest(string source)
        {
            int groups = 6;
            Regex regex = new Regex(this.remote_regex, RegexOptions.Compiled);
            Match match = regex.Match(source, 0, source.Length);

            Assert.NotEqual(groups, match.Groups.Count);
        }

        [Theory]
        [InlineData("900MMJ400-M-201701-L-900MMJ400_8956655-01")]
        [InlineData("OP-M-201701-R-11111111-01")]
        [InlineData("97_12ID-M-201801-B-NHJUJ-05")]
        [InlineData("D-M-201806-T-1-21")]
        public void PackageNumberDailyParseFuncPassedTest(string source)
        {
            int groups = 7;
            Regex regex = new Regex(this.daily_regex, RegexOptions.Compiled);
            Match match = regex.Match(source, 0, source.Length);

            Assert.Equal(groups, match.Groups.Count);
        }
        
    }
}
