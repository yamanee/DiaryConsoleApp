using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryConsoleApp1
{
    public class DiaryEntry
    {
        public int Id { get; set; } // 一意のID
        //日付
        public DateTime Date { get; set; }
        //内容
        public string Content { get; set; }
    }
}
