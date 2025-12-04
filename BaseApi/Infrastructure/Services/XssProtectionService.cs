using BaseApi.Application.Common.Security;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

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
            @"url\s*\(",
            @"@import",
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

            // HTML encode
            sanitized = HttpUtility.HtmlEncode(sanitized);

            // Remove XSS patterns
            sanitized = _combinedXssRegex.Replace(sanitized, string.Empty);

            // Remove any remaining dangerous characters
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
                        property.SetValue(obj, SanitizeInput(value));
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

            return HttpUtility.HtmlEncode(output);
        }

        private string RemoveDangerousCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove null bytes and other dangerous control characters
            input = Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);

            return input;
        }
    }
}