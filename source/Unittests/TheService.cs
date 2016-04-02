﻿using FakeItEasy;
using IdentityServer3.Core.Resources;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Contrib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Sdk;
using IdSrvConstants = IdentityServer3.Core.Constants.LocalizationCategories;

namespace Unittests
{
    public class TheService
    {
        public TheService()
        {
            Debug.Listeners.Add(new DefaultTraceListener());
        }

        [Fact]
        public void HasTranslationsForAllPublicIds()
        {
            var availableCultures = GlobalizedLocalizationService.GetAvailableLocales();
        
            foreach (var availableCulture in availableCultures)
            {
                AssertTranslationExists(availableCulture, TestHelper.GetAllMessageIds(), IdSrvConstants.Messages);
                AssertTranslationExists(availableCulture, TestHelper.GetAllEventIds(), IdSrvConstants.Events);
                AssertTranslationExists(availableCulture, TestHelper.GetAllScopeIds(), IdSrvConstants.Scopes);
            }
        }

        [Fact]
        public void UsesFallbackWhenIsSet()
        {
            const string someid = "SomeId";
            var mock = new Fake<ILocalizationService>();
            mock.CallsTo(loc => loc.GetString(IdSrvConstants.Messages, someid)).Returns("fallbackValue");
            
            var service = new GlobalizedLocalizationService(new LocaleOptions{ Locale = "nb-NO", FallbackLocalizationService = mock.FakedObject});

            var result = service.GetString(IdSrvConstants.Messages, someid);
            Assert.Equal("fallbackValue", result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("notexisting")]
        public void ThrowsExceptionForUnknownLocales(string locale)
        {
            var options = new LocaleOptions
            {
                Locale = locale
            };
            Assert.Throws<ApplicationException>(() => new GlobalizedLocalizationService(options));
        }
    
        private static void AssertTranslationExists(string culture, IEnumerable<string> ids, string category)
        {
            var options = new LocaleOptions
            {
                Locale = culture
            };
            var service = new GlobalizedLocalizationService(options);
            var notFoundTranslations = new List<string>();
            
            foreach (var id in ids)
            {
                var localizedString = service.GetString(category, id);
                if (string.IsNullOrEmpty(localizedString))
                {
                    var errormsg = string.Format("Could not find translation of Id '{0}' in {1}", id,
                        string.IsNullOrEmpty(culture) ? "IdentityServers internals" : culture);
                    notFoundTranslations.Add(errormsg);

                }
                else
                {
                    string message = string.Format("{0} - {1}:{2} - {3}", culture, category, id, localizedString);
                    Trace.WriteLine(message);
                    Debug.WriteLine(message);
                    Console.WriteLine(message);
                    Assert.NotEqual("", localizedString.Trim());    
                }
                
            }
            if (notFoundTranslations.Any())
            {
                var concated = notFoundTranslations.Aggregate((x, y) => x + ", " + y);
                throw new AssertActualExpectedException("Some translation", "NOTHING!", concated );
            }
        }
    }

    internal static class TestHelper
    {
        public static IEnumerable<string> GetAllMessageIds()
        {
            return typeof(MessageIds).GetFields().Select( m => m.GetRawConstantValue().ToString());            
        }

        public static IEnumerable<string> GetAllEventIds()
        {
            return typeof(EventIds).GetFields().Select(m => m.GetRawConstantValue().ToString());
        }

        public static IEnumerable<string> GetAllScopeIds()
        {
            return typeof(ScopeIds).GetFields().Select(m => m.GetRawConstantValue().ToString());
        }
    }
}
