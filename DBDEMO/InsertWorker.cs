using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastReport;
using System.Data;
using FastReport.Export.PdfSimple;


namespace DBDEMO
{
    public class InsertWorker
    {
        /// <summary>
        /// 新增資料
        /// </summary>
        public void InserData()
        {
            var worKer = new[]
           {
                new Worker{Id=1,Name="Joe",Salary=70000,ManagerId=3},
                new Worker{Id=2,Name="Henry",Salary=80000, ManagerId=4},
                new Worker{Id=3,Name="Sam",Salary=60000,ManagerId=null},
                new Worker{Id=4,Name="Max",Salary=90000,ManagerId =null},
            };
            //設定連線字串
            string connectionString = "Data Source=CompanyEmployees.db;Version=3;";

            // 使用 using 語法來創建和處理 SQLite 連線，並確保連線結束後正確釋放資源
            //using 是 C# 中的一個語法結構，用來確保在程式執行完畢後正確釋放資源，避免資源泄漏。
            //using 語句可以自動幫助你管理物件的生命周期，並在作用域結束時自動呼叫 Dispose() 方法而Dispose() 是釋放資源的方法。
            using (var conection = new SQLiteConnection(connectionString))
            {
                //開啟與 SQLite 資料庫的連線。如果連線成功，後續的資料操作才能執行。
                conection.Open();
                //INSERT OR IGNORE為如果要插入的資料已經存在（例如 Id 欄位唯一），則該筆資料會被忽略，不會報錯。
                //@參數佔位符，這些會被替換為具體的值，這些值會來自 worKer 這個參數。
                string sql = @"INSERT OR IGNORE INTO  Employee (Id,Name, Salary, ManagerId) VALUES ( @Id, @Name, @Salary, @ManagerId)";

                try
                {
                    //Dapper的Execute方法是用來執行不返回任何資料的 SQL 語句例如INSERT、UPDATE、DELETE,
                    //Execute 方法的返回值是 int，代表受影響的行數
                    conection.Execute(sql, worKer);
                    Console.WriteLine("資料已成功加入 (若資料已存在，則被忽略)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"發生錯誤: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// ManagerId不為null
        /// </summary>
        /// <returns></returns>
        public static DataTable ManagerIdNotNull()
        {
            string connectionString = "Data Source=CompanyEmployees.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {

                connection.Open();
                string querySql = "SELECT * FROM Employee WHERE  ManagerId IS NOT NULL";
                DataTable table = new DataTable();
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(querySql, connection))
                {
                    adapter.Fill(table);
                }
                //Query是用來查詢的方法,querySql為要執行的sql語法，並將每行資料映射到 Worker 類型的物件
                return table;
            }
        }
        /// <summary>
        /// 薪資高於經理
        /// </summary>
        /// <returns></returns>
        public static DataTable SalaryHighThanManager()
        {
            string connectionString = "Data Source=CompanyEmployees.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string querySql = @"SELECT E.Name AS Employee 
                                                      FROM Employee E 
                                                      JOIN Employee e2 
                                                      ON E.ManagerId = e2.Id
                                                      WHERE E.ManagerId IS NOT NULL 
                                                      AND E.Salary > e2.Salary";
                DataTable table = new DataTable();
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(querySql, connection))
                {
                    adapter.Fill(table);
                }
                return table;
            }
        }
        public void ExportRreport()
        {
            Report report = new Report();
            string reportFilePath = @"C:\Users\user\Desktop\DBDEMO\DBDEMO\Report.frx";
            report.Load(reportFilePath);

            DataTable managerIdDataNotNull = ManagerIdNotNull();
            DataTable salaryHighEmployee = SalaryHighThanManager();

            report.RegisterData(managerIdDataNotNull, "Employee");
            report.RegisterData(salaryHighEmployee, "Employee");

            report.Prepare();


            string outPutPdfPath = @"C:\Users\user\Desktop\DBDEMO\HighSalaryEmployeeReport.pdf";
            PDFSimpleExport pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, outPutPdfPath);
            Console.WriteLine($"報表已匯出到: {outPutPdfPath}");

        }

    }
}
