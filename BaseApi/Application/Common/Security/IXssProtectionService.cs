namespace BaseApi.Application.Common.Security
{
    public interface IXssProtectionService
    {
        string SanitizeInput(string input);
        T SanitizeObject<T>(T obj) where T : class;
        bool ContainsXss(string input);
        string EncodeOutput(string output);
    }
}