using System.Text.RegularExpressions;

namespace DiaryConsoleApp1_Answer
{
    public class CommonValidation
    {
        // 半角英数字のみの場合trueを返す
        public static bool IsAlphaNumeric(string input)
        {
            return Regex.IsMatch(input, "^[a-zA-Z0-9]+$");
        }
    }
}
