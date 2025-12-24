using BaseApi.Application.Common.Security;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BaseApi.Infrastructure.Services
{
    public class XssProtectionService : IXssProtectionService
    {
        private readonly List<string> _xssPatterns = new()
        {
            @"<script[\s\S]*?>[\s\S]*?</script>",
            @"<iframe[\s\S]*?>[\s\S]*?</iframe>",
            @"<object[\s\S]*?>[\s\S]*?</object>",
            @"<embed[\s\S]*?>[\s\S]*?</embed>",
            @"<link[\s\S]*?>",
            @"<meta[\s\S]*?>",
            @"<style[\s\S]*?>[\s\S]*?</style>",
            @"javascript\s*:",
            @"vbscript\s*:",
            @"data\s*:",
            @"on\w+\s*=",
            @"expression\s*\(",
            @"alert\s*\(",
            @"confirm\s*\(",
            @"prompt\s*\(",
            @"eval\s*\(",
            @"setTimeout\s*\(",
            @"setInterval\s*\(",
            @"function\s*\(",
            @"window\.",
            @"document\.",
            @"location\.",
            @"cookie",
            @"innerHTML",
            @"outerHTML"
        };

        private readonly Regex _combinedXssRegex;

        public XssProtectionService()
        {
            var combinedPattern = string.Join("|", _xssPatterns);
            _combinedXssRegex = new Regex(combinedPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sanitized = input.Trim();

            // XSS pattern'leri temizle - HTML encode yapma!
            sanitized = _combinedXssRegex.Replace(sanitized, string.Empty);

            // Sadece tehlikeli script attribute'larýný temizle
            sanitized = RemoveDangerousAttributes(sanitized);

            // Dangerous karakterleri temizle ama Türkçe karakterleri koru
            sanitized = RemoveDangerousCharacters(sanitized);

            return sanitized;
        }

        public T SanitizeObject<T>(T obj) where T : class
        {
            if (obj == null) return obj;

            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(obj) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        // AllowHtmlContent attribute'ý var mý kontrol et
                        var hasAllowHtml = property.GetCustomAttribute<AllowHtmlContentAttribute>() != null;

                        if (hasAllowHtml)
                        {
                            // HTML içeren alanlar için hafif sanitization
                            property.SetValue(obj, SanitizeHtmlContent(value));
                        }
                        else
                        {
                            // Normal alanlar için XSS temizleme ama encoding yok
                            property.SetValue(obj, SanitizeInput(value));
                        }
                    }
                }
            }

            return obj;
        }

        public bool ContainsXss(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return _combinedXssRegex.IsMatch(input);
        }

        public string EncodeOutput(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
                return string.Empty;

            // Output encoding kaldýr - frontend'de gerekirse yapýlýr
            return output;
        }

        private string SanitizeHtmlContent(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
                return string.Empty;

            // Sadece tehlikeli script'leri temizle, HTML tag'lari koru
            var sanitized = htmlContent;

            // Tehlikeli pattern'leri temizle
            var dangerousPatterns = new List<string>
            {
                @"<script[\s\S]*?>[\s\S]*?</script>",
                @"javascript\s*:",
                @"on\w+\s*=[\s\S]*?(?=[>\s])",
                @"alert\s*\(",
                @"eval\s*\(",
                @"setTimeout\s*\(",
                @"setInterval\s*\("
            };

            foreach (var pattern in dangerousPatterns)
            {
                sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.IgnoreCase);
            }

            return sanitized;
        }


        private string RemoveDangerousAttributes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sadece tehlikeli onclick, onload gibi event handler'larý kaldýr
            var dangerousAttributes = new List<string>
            {
                @"on\w+\s*=[\s\S]*?(?=[>\s])",
                @"javascript\s*:",
                @"vbscript\s*:"
            };

            foreach (var pattern in dangerousAttributes)
            {
                input = Regex.Replace(input, pattern, string.Empty, RegexOptions.IgnoreCase);
            }

            return input;
        }

        private string RemoveDangerousCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sadece null byte ve control karakterleri kaldýr
            // Türkçe karakterleri (ç, ð, ý, ö, þ, ü) koru
            input = Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);

            return input;
        }




    }


}



