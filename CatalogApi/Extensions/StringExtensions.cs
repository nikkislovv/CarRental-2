namespace CatalogApi.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalazeFirstLetter(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
