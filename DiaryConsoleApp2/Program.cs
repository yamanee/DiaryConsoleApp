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
        //CSVファイルから日記を読み込む
        var diaryEntries = LoadDiaryEntriesFromCsv(args.Length > 0 ? args[0] : "diary.csv");

        //メニューの表示選択
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
                    Console.WriteLine("無効な選択です。もう一度お試しください。\r\n");
                    break;
            }
        }
    }

    /// <summary>
    /// メニューの表示
    /// </summary>
    static void PrintMenu()
    {
        Console.WriteLine("< 日記アプリケーション　メニュー >");
        Console.WriteLine("使用する機能の数字を入力してください。");
        Console.WriteLine("1. 新しい日記を追加");
        Console.WriteLine("2. 日記を表示");
        Console.WriteLine("3. 日記を編集");
        Console.WriteLine("4. 日記を削除");
        Console.WriteLine("5. 日付検索");
        Console.WriteLine("6. 内容検索");
        Console.WriteLine("7. 終了");
    }

    /// <summary>
    /// 日記の追加
    /// </summary>
    static void AddDiaryEntry(List<DiaryEntry> entries)
    {
        //新しいIDの採番
        int maxId = entries.Count > 0 ? entries.Max(entry => entry.Id) : 0;
        int newId = maxId + 1;

        var entry = CreateDiaryEntry(newId);

        if (entry != null)
        {
            entries.Add(entry);
            Console.WriteLine("日記が追加されました。\r\n");
        }
    }

    /// <summary>
    /// 日記データの作成
    /// </summary>
    public static DiaryEntry CreateDiaryEntry(int newId)
    {
        Console.WriteLine("日付を入力してください (yyyy/MM/dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("無効な日付形式です。\r\n");
            return null;
        }

        Console.WriteLine("日記の内容を入力してください:");
        string content = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(content))
        {
            Console.WriteLine("日記の内容が無効です。日記は登録されません。\r\n");
            return null;
        }

        return new DiaryEntry { Id = newId, Date = date, Content = content };
    }

    /// <summary>
    /// 日記の表示
    /// </summary>
    static void DisplayDiaryEntries(List<DiaryEntry> entries)
    {
        Console.WriteLine("日記一覧:");

        DisplayEntries(entries);
    }

    /// <summary>
    /// 日記の編集
    /// </summary>
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
                    Console.WriteLine("日記の内容が無効です。日記は登録されません。\r\n");
                    return;
                }

                entryToEdit.Content = content;
                Console.WriteLine($"日記 ID {editEntryId} が編集されました。\r\n");
            }
            else
            {
                Console.WriteLine("指定したIDの日記が見つかりませんでした。\r\n");
            }
        }
        else
        {
            Console.WriteLine("無効なIDが入力されました。\r\n");
        }
    }

    /// <summary>
    /// 日記の削除
    /// </summary>
    static void DeleteDiaryEntry(List<DiaryEntry> entries)
    {
        Console.WriteLine("削除する日記のIDを入力してください:");

        if (int.TryParse(Console.ReadLine(), out int entryId))
        {
            var entryToDelete = entries.FirstOrDefault(entry => entry.Id == entryId);
            if (entryToDelete != null)
            {
                entries.Remove(entryToDelete);
                Console.WriteLine($"日記 ID {entryId} が削除されました。\r\n");
            }
            else
            {
                Console.WriteLine("指定したIDの日記が見つかりませんでした。\r\n");
            }
        }
        else
        {
            Console.WriteLine("無効なIDが入力されました。\r\n");
        }
    }

    /// <summary>
    /// 日記の日付検索
    /// </summary>
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
                    Console.WriteLine("指定した日付範囲内の日記は見つかりませんでした。\r\n");
                }
                else
                {
                    Console.WriteLine($"日記一覧（{startDate.ToShortDateString()}-{endDate.ToShortDateString()}）:");
                    DisplayEntries(matchingEntries);
                }
            }
            else
            {
                Console.WriteLine("無効な日付形式です。\r\n");
            }
        }
        else
        {
            Console.WriteLine("無効な日付形式です。\r\n");
        }
    }

    /// <summary>
    /// 日記の内容検索
    /// </summary>
    static void SearchByText(List<DiaryEntry> entries)
    {
        Console.WriteLine("検索するテキストを入力してください:");
        string searchText = Console.ReadLine();

        if (!string.IsNullOrEmpty(searchText))
        {
            var matchingEntries = entries.Where(entry => entry.Content.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matchingEntries.Count == 0)
            {
                Console.WriteLine($"指定したテキスト「{searchText}」を含む日記は見つかりませんでした。\r\n");
            }
            else
            {
                Console.WriteLine($"日記一覧（「{searchText}」を含む）:");
                DisplayEntries(matchingEntries);
            }
        }
        else
        {
            Console.WriteLine("検索テキストが無効です。\r\n");
        }
    }


    /// <summary>
    /// 検索結果の表示
    /// </summary>
    static void DisplayEntries(List<DiaryEntry> entries)
    {
        //日付の昇順に並び変える
        entries= entries.OrderBy(entry => entry.Date).ToList();

        foreach (var entry in entries)
        {
            Console.WriteLine($"（{entry.Id}）日付: {entry.Date.ToShortDateString()}");
            Console.WriteLine(entry.Content);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// CSVファイルに保存、アプリを終了
    /// </summary>
    static void SaveAndExit(string filePath, List<DiaryEntry> entries)
    {
        SaveDiaryEntriesToCsv(filePath, entries);
        Console.WriteLine("アプリケーションを終了します。\r\n");
        Environment.Exit(0);
    }

    /// <summary>
    /// CSVファイルに保存
    /// </summary>
    public static void SaveDiaryEntriesToCsv(string filePath, List<DiaryEntry> entries)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(entries);
        }
    }

    /// <summary>
    /// CSVファイルから日記データを読み込み、リストに追加
    /// </summary>
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