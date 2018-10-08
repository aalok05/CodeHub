namespace CodeHub.Helpers
{
    public static class StringHelper
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string @string)
        {
            return string.IsNullOrEmpty(@string) || string.IsNullOrWhiteSpace(@string);
        }
    }
}
