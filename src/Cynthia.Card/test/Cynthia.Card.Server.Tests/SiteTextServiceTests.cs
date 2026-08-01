using System.Globalization;
using Cynthia.Card.Server.Services;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public class SiteTextServiceTests
    {
        [Theory]
        [InlineData("en-US", true)]
        [InlineData("zh-CN", false)]
        [InlineData("", false)]
        public void EnglishDetectionUsesTheFullCultureName(
            string cultureName,
            bool expected)
        {
            Assert.Equal(
                expected,
                SiteTextService.IsEnglishCulture(new CultureInfo(cultureName)));
        }
    }
}
