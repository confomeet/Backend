using System.Text;

namespace VideoProjectCore6.Utility
{
    public static class Uri {
        public static string CombineUri(params string[] parts) {
            if (parts.Length == 0)
                return "";
            if (parts.Length == 1)
                return parts[0];
            StringBuilder result = new();
            result.Append(parts[0].TrimEnd());
            for (int i = 1; i < parts.Length; ++i) {
                result.Append('/');
                result.Append(parts[i].Trim('/'));
            }
            return result.ToString();
        }
    }
}
