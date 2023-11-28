namespace DiaryConsoleApp1
{
    internal class Constant
    {
        // 半角英数以外が含まれる場合、true
        public static bool ContainsNonAlphanumeric(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
