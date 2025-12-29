namespace BaseApi.Infrastructure.Services
{
    public interface ISpamFilterService
    {
        bool IsSpam(string message, string email, string name);
    }

    public class SpamFilterService : ISpamFilterService
    {
        private readonly string[] _spamWords = new[]
        {
            "viagra", "casino", "gambling", "forex", "bitcoin", "cryptocurrency",
            "loan", "credit", "debt", "prize", "winner", "congratulations",
            "click here", "urgent", "limited time", "act now", "free money"
        };

        private readonly string[] _suspiciousPatterns = new[]
        {
            @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+",
            @"\b\w*\d{8,}\w*\b", // Uzun sayý dizileri
            @"[A-Z]{10,}", // Uzun büyük harf dizileri
            @"(.)\1{4,}" // Ayný karakterin 5+ kez tekrarý
        };

        public bool IsSpam(string message, string email, string name)
        {
            var combinedText = $"{message} {email} {name}".ToLowerInvariant();

            // Spam kelimeleri kontrol et
            foreach (var spamWord in _spamWords)
            {
                if (combinedText.Contains(spamWord.ToLowerInvariant()))
                {
                    return true;
                }
            }

            // Þüpheli pattern'leri kontrol et
            foreach (var pattern in _suspiciousPatterns)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(combinedText, pattern))
                {
                    return true;
                }
            }

            // Aþýrý tekrar kontrol et
            if (HasExcessiveRepetition(message))
            {
                return true;
            }

            // E-posta domain kontrolü
            if (IsSuspiciousEmailDomain(email))
            {
                return true;
            }

            return false;
        }

        private bool HasExcessiveRepetition(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 3) return false;

            var wordCount = words.GroupBy(w => w.ToLowerInvariant())
                                 .ToDictionary(g => g.Key, g => g.Count());

            // Ayný kelimenin %50'den fazla tekrarlanmasý
            return wordCount.Any(kvp => kvp.Value > words.Length / 2);
        }

        private bool IsSuspiciousEmailDomain(string email)
        {
            var suspiciousDomains = new[]
            {
                "10minutemail.com", "tempmail.org", "guerrillamail.com",
                "mailinator.com", "throwaway.email"
            };

            if (string.IsNullOrWhiteSpace(email)) return false;

            var domain = email.Split('@').LastOrDefault()?.ToLowerInvariant();
            return suspiciousDomains.Contains(domain);
        }
    }
}