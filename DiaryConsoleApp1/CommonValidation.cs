namespace DiaryConsoleApp1
{
    public class CommonValidation
    {
        // 半角英数字のみの場合trueを返す
        public static bool IsAlphaNumeric(string input)
        {
            foreach (char c in input)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
