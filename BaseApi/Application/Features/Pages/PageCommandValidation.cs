using BaseApi.Domain.Entities;

namespace BaseApi.Application.Features.Pages
{
    internal static class PageCommandValidation
    {
        public static string? Validate(
            string title,
            string slug,
            string content,
            string? summary,
            string? featuredImageUrl,
            int template,
            int status,
            int visibility,
            string? metaTitle,
            string? metaDescription,
            string? keywords,
            string? canonicalUrl)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "Sayfa basligi zorunludur.";

            if (title.Length > 200)
                return "Sayfa basligi en fazla 200 karakter olabilir.";

            if (string.IsNullOrWhiteSpace(slug))
                return "Slug zorunludur.";

            if (slug.Length > 300)
                return "Slug en fazla 300 karakter olabilir.";

            if (content == null)
                return "Icerik zorunludur.";

            if (content?.Length > 5000)
                return "Icerik en fazla 5000 karakter olabilir.";

            if (summary?.Length > 1000)
                return "Ozet en fazla 1000 karakter olabilir.";

            if (featuredImageUrl?.Length > 1000)
                return "Gorsel adresi en fazla 1000 karakter olabilir.";

            if (!Enum.IsDefined(typeof(PageTemplate), template))
                return "Gecersiz sayfa sablonu.";

            if (!Enum.IsDefined(typeof(PageStatus), status))
                return "Gecersiz sayfa durumu.";

            if (!Enum.IsDefined(typeof(PageVisibility), visibility))
                return "Gecersiz gorunurluk degeri.";

            if (metaTitle?.Length > 200)
                return "Meta title en fazla 200 karakter olabilir.";

            if (metaDescription?.Length > 500)
                return "Meta description en fazla 500 karakter olabilir.";

            if (keywords?.Length > 500)
                return "Keywords en fazla 500 karakter olabilir.";

            if (canonicalUrl?.Length > 500)
                return "Canonical URL en fazla 500 karakter olabilir.";

            return null;
        }
    }
}
