using AutoMapper;
using System.Globalization;

namespace SoftnetManager.Modules.Identity.Application.MappingProfiles
{
    public class TitleCaseConverter : IValueConverter<string?, string>
    {
        public string Convert(string? sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(sourceMember))
                return sourceMember!;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sourceMember.ToLower().Trim());
        }
    }
}
