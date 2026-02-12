namespace SignalBlaze
{
    public static class Utility
    {
        public static string Truncate(string text, int length = 1)
        {
            if (text.Length > length) {
                return text.Substring(0, length) + "...";
            }

            return text;
        }
    }
}
