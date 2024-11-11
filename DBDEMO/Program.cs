using FastReport;
using System.Data.SQLite;
using System.Text.Unicode;
namespace DBDEMO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //設定連線字串
            //Data Source=CompanyEmployees.db這指定了資料庫檔案的路徑和名稱
            //Version=3指定 SQLite 資料庫的版本，這裡是版本 3
            string connectionString = "Data Source=CompanyEmployees.db;Version=3;";
            //設定了控制台的輸出編碼為 UTF-8
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //SQLiteConnection是用來與資料庫進行互動的元件
            using (var connection = new SQLiteConnection(connectionString))
            {
                // connection.Open();用來開啟與資料庫的連線
                connection.Open();
                string creatTable = @"CREATE TABLE IF NOT EXISTS Employee ( 
                                                         Id INTEGER PRIMARY KEY ,
                                                         Name TEXT NOT NULL,
                                                         Salary INTEGER NOT NULL,
                                                          ManagerId INTEGER );";

                //SQLiteCommand(a,b)第一個參數主要是放SQL語句,第二個參數是SQLiteConnection物件用來與資料庫溝通的字串
                //這裡的 using 語法是為了確保即使發生異常，資源也能被正確釋放，避免資源泄漏。
                //例如，資料庫連線、檔案處理等，需要在不再使用時明確釋放資源。
                //當未釋放資源會導致記憶體和資源占用過多，影響效能，甚至導致應用程式崩潰。
                using (var command = new SQLiteCommand(creatTable, connection)) 
                {
                    //要用來執行 SQL 語句，這些語句不返回資料，而是用來進行資料庫的結構變更、資料更新、刪除操作或執行非查詢的 SQL 語句。
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table 'Employee' has been created.");
                }
                InsertWorker worker = new InsertWorker();
                worker.InserData();
                var workersWithManager = InsertWorker.ManagerIdNotNull();


                //if (workersWithManager.Count > 0)
                //{
                //    Console.WriteLine("Manager不為null的員工資料:");
                //    foreach (var w in workersWithManager)
                //    {
                //        Console.WriteLine($"員工編號: {w.Id} ,員工姓名: {w.Name} ,薪資:{w.Salary} ,經理編號:{w.ManagerId}");
                //    }
                //}
                //else 
                //{
                //    Console.WriteLine("沒有找到 ManagerId 不為 null 的員工。");
                //}
                //var searchResult = InsertWorker.SalaryHighThanManager();
                ////代表有資料
                //if (searchResult.Count > 0) 
                //{
                //    Console.WriteLine("⾮管理職且薪資⾼於該主管⼈員:");
                //    foreach(var result in searchResult)
                //    {
                //        Console.WriteLine($"Employee:{result}");
                //    }
                //}
                worker.ExportRreport();
            };
        }
    }
}
