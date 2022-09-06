using System.Collections.Generic;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Extensions;
using PERLS.DataImplementation.Models;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// The Landing provider.
    /// </summary>
    public class LandingProvider : ILandingProvider
    {
        /// <summary>
        /// Gets the landing data.
        /// </summary>
        /// <returns>The landing data.</returns>
        public IEnumerable<ILandingData> GetLandingData()
        {
            return new List<ILandingData>
            {
                new LandingPageData(StringsSpecific.OnboardingPageOneTitleLabel, StringsSpecific.OnboardingPageOneDescriptionLabel.AddAppName(), "PERLS.Data.Resources.onboarding_one.svg"),
                new LandingPageData(StringsSpecific.OnboardingPageTwoTitleLabel, StringsSpecific.OnboardingPageTwoDescriptionLabel.AddAppName(), "PERLS.Data.Resources.onboarding_two.svg"),
                new LandingPageData(StringsSpecific.OnboardingPageThreeTitleLabel, StringsSpecific.OnboardingPageThreeDescriptionLabel, "PERLS.Data.Resources.onboarding_three.svg"),
            };
        }
    }
}
