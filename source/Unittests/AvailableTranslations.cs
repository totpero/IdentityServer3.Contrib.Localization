using System.Linq;
using IdentityServer3.Core.Services.Contrib;
using Xunit;

namespace Unittests
{
    public class AvailableTranslations
    {
        [Theory]
        [InlineData("Default")]
        [InlineData("pirate")]
        [InlineData("de-DE")]
        [InlineData("es-AR")]
        [InlineData("fr-FR")]
        [InlineData("nb-NO")]
        [InlineData("sv-SE")]
        [InlineData("tr-TR")]
        [InlineData("ro-RO")]
        [InlineData("nl-NL")]
        [InlineData("zh-CN")]
        [InlineData("da-DK")]
        [InlineData("ru-RU")]
        [InlineData("pt-BR")]
        [InlineData("cs-CZ")]
        [InlineData("it-IT")]
        [InlineData("pl-PL")]
        [InlineData("sk-SK")]
        public void ContainsLocales(string locale)
        {
            Assert.Contains(GlobalizedLocalizationService.GetAvailableLocales(), s => s.Equals(locale));
        }

        [Fact]
        public void HasCorrectCount()
        {
            Assert.Equal(18, GlobalizedLocalizationService.GetAvailableLocales().Count());
        }
    }
}