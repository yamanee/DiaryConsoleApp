using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Asn1;
using DiaryConsoleApp2;

class Program
{
    static void Main(string[] args)
    {
        var diaryEntries = LoadDiaryEntriesFromCsv(args.Length > 0 ? args[0] : "diary.csv");

        while (true)
        {
            PrintMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddDiaryEntry(diaryEntries);
                    break;
                case "2":
                    DisplayDiaryEntries(diaryEntries);
                    break;
                case "3":
                    EditDiaryEntry(diaryEntries);
                    break;
                case "4":
                    DeleteDiaryEntry(diaryEntries);
                    break;
                case "5":
                    SearchByDateRange(diaryEntries);
                    break;
                case "6":
                    SearchByText(diaryEntries);
                    break;
                case "7":
                    SaveAndExit("diary.csv", diaryEntries);
                    break;
                default:
                    Console.WriteLine("無効な選択です。もう一度お試しください。");
                    break;
            }
        }
    }

    static void PrintMenu()
    {
        Console.WriteLine("日記アプリケーション");
        Console.WriteLine("1. 新しい日記を追加");
        Console.WriteLine("2. 日記を表示");
        Console.WriteLine("3. 日記を編集");
        Console.WriteLine("4. 日記を削除");
        Console.WriteLine("5. 日付検索");
        Console.WriteLine("6. 内容検索");
        Console.WriteLine("7. 終了");
    }

    static void AddDiaryEntry(List<DiaryEntry> entries)
    {
        int maxId = entries.Count > 0 ? entries.Max(entry => entry.Id) : 0;
        int newId = maxId + 1;

        var entry = CreateDiaryEntry(newId);

        if (entry != null)
        {
            entries.Add(entry);
            Console.WriteLine("日記が追加されました。");
        }
    }

    public static DiaryEntry CreateDiaryEntry(int newId)
    {
        Console.WriteLine("日付を入力してください (yyyy/MM/dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("無効な日付形式です。");
            return null;
        }

        Console.WriteLine("日記の内容を入力してください:");
        string content = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(content))
        {
            Console.WriteLine("日記の内容が無効です。日記は登録されません。");
            return null;
        }

        return new DiaryEntry { Id = newId, Date = date, Content = content };
    }

    static void DisplayDiaryEntries(List<DiaryEntry> entries)
    {
        entries.OrderBy(entry => entry.Date);

        Console.WriteLine("日記一覧:");
        foreach (var entry in entries)
        {
            Console.WriteLine($"（{entry.Id}）日付: {entry.Date.ToShortDateString()}");
            Console.WriteLine(entry.Content);
            Console.WriteLine();
        }
    }

    static void EditDiaryEntry(List<DiaryEntry> entries)
    {
        Console.WriteLine("編集する日記のIDを入力してください:");

        if (int.TryParse(Console.ReadLine(), out int editEntryId))
        {
            var entryToEdit = entries.FirstOrDefault(entry => entry.Id == editEntryId);
            if (entryToEdit != null)
            {
                Console.WriteLine($"現在の内容: {entryToEdit.Content}");
                Console.WriteLine("新しい内容を入力してください:");
                string content = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine("日記の内容が無効です。日記は登録されません。");
                    return;
                }

                entryToEdit.Content = content;
                Console.WriteLine($"日記 ID {editEntryId} が編集されました。");
            }
            else
            {
                Console.WriteLine("指定したIDの日記が見つかりませんでした。");
            }
        }
        else
        {
            Console.WriteLine("無効なIDが入力されました。");
        }
    }

    static void DeleteDiaryEntry(List<DiaryEntry> entries)
    {
        Console.WriteLine("削除する日記のIDを入力してください:");

        if (int.TryParse(Console.ReadLine(), out int entryId))
        {
            var entryToDelete = entries.FirstOrDefault(entry => entry.Id == entryId);
            if (entryToDelete != null)
            {
                entries.Remove(entryToDelete);
                Console.WriteLine($"日記 ID {entryId} が削除されました。");
            }
            else
            {
                Console.WriteLine("指定したIDの日記が見つかりませんでした。");
            }
        }
        else
        {
            Console.WriteLine("無効なIDが入力されました。");
        }
    }

    static void SearchByDateRange(List<DiaryEntry> entries)
    {
        Console.WriteLine("検索開始日を入力してください (yyyy/MM/dd):");

        if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("検索終了日を入力してください (yyyy/MM/dd):");

            if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                var matchingEntries = entries.Where(entry => entry.Date.Date >= startDate.Date && entry.Date.Date <= endDate.Date).ToList();

                if (matchingEntries.Count == 0)
                {
                    Console.WriteLine("指定した日付範囲内の日記は見つかりませんでした。");
                }
                else
                {
                    DisplayMatchingEntries(startDate, endDate, matchingEntries);
                }
            }
            else
            {
                Console.WriteLine("無効な日付形式です。");
            }
        }
        else
        {
            Console.WriteLine("無効な日付形式です。");
        }
    }

    static void SearchByText(List<DiaryEntry> entries)
    {
        Console.WriteLine("検索するテキストを入力してください:");
        string searchText = Console.ReadLine();

        if (!string.IsNullOrEmpty(searchText))
        {
            var matchingEntries = entries.Where(entry => entry.Content.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matchingEntries.Count == 0)
            {
                Console.WriteLine($"指定したテキスト「{searchText}」を含む日記は見つかりませんでした。");
            }
            else
            {
                DisplayMatchingEntries(searchText, matchingEntries);
            }
        }
        else
        {
            Console.WriteLine("検索テキストが無効です。");
        }
    }

    static void DisplayMatchingEntries(DateTime startDate, DateTime endDate, List<DiaryEntry> matchingEntries)
    {
        Console.WriteLine($"日記（{startDate.ToShortDateString()}-{endDate.ToShortDateString()}）:");
        DisplayEntries(matchingEntries);
    }

    static void DisplayMatchingEntries(string searchText, List<DiaryEntry> matchingEntries)
    {
        Console.WriteLine($"日記（「{searchText}」を含む）:");
        DisplayEntries(matchingEntries, true, searchText);
    }

    static void DisplayEntries(List<DiaryEntry> entries, bool highlight = false, string searchText = "")
    {
        foreach (var entry in entries)
        {
            Console.WriteLine($"日付: {entry.Date.ToShortDateString()}");
            if (highlight)
            {
                HighlightAndPrintText(entry.Content, searchText);
            }
            else
            {
                Console.WriteLine(entry.Content);
            }
            Console.WriteLine();
        }
    }

    //検索した文字を赤くして出力する処理
    public static void HighlightAndPrintText(string text, string searchTerm)
    {
        int startIndex = 0;

        while (true)
        {
            int index = text.IndexOf(searchTerm, startIndex, StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                Console.Write(text.Substring(startIndex));
                break;
            }

            Console.Write(text.Substring(startIndex, index - startIndex));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text.Substring(index, searchTerm.Length));
            Console.ResetColor();

            startIndex = index + searchTerm.Length;
        }

        Console.WriteLine();
    }

    static void SaveAndExit(string filePath, List<DiaryEntry> entries)
    {
        SaveDiaryEntriesToCsv(filePath, entries);
        Console.WriteLine("アプリケーションを終了します。");
        Environment.Exit(0);
    }

    //CSVファイル保存処理
    public static void SaveDiaryEntriesToCsv(string filePath, List<DiaryEntry> entries)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(entries);
        }
    }

    //CSVファイル読み込み処理
    public static List<DiaryEntry> LoadDiaryEntriesFromCsv(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                return csv.GetRecords<DiaryEntry>().ToList();
            }
        }
        else
        {
            return new List<DiaryEntry>();
        }
    }
}