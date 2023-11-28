namespace DiaryConsoleApp1_Answer
{
    public class DiaryEntry
    {
        // 一意のID
        public int Id { get; set; }
        //日付
        public DateTime Date { get; set; }
        //内容
        public string Content { get; set; }
        //カテゴリ
        public string Category { get; set; }
    }
}
