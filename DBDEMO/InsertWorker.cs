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
            //設定連線字串
            string connectionString = "Data Source=CompanyEmployees.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {

                connection.Open();
                string querySql = "SELECT * FROM Employee WHERE  ManagerId IS NOT NULL";
                DataTable table = new DataTable();
                //SQLiteDataAdapter是一個工具，用來將SQL查詢的填充到DataTable中
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(querySql, connection))
                {
                    //adapter.Fill會執行前面定義的 SQL 查詢，並將查詢結果填入 table 物件
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
                //********注意!!!當只有select某個欄位或某些欄位，而要產出報表前要去frx檔留存只需要的欄位，其他不需要的可以先刪除
                string querySql = @"SELECT E.Name 
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
        /// <summary>
        /// 匯出報表
        /// </summary>
        public void ExportRreport()
        {
            //Report 類通常來自於報表生成工具（如 FastReport）。這個物件代表了報表的整體結構和邏輯。
            Report report = new Report();

            //reportFilePath為frx檔的位置
            string reportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report.frx"); ;
          
            //加載指定的報表模板 (.frx 文件) 到 report 物件中。這樣報表就能夠根據模板來生成內容。
            report.Load(reportFilePath);

            //ManagerIdNotNull()、SalaryHighThanManager()把這些方法所得到的datatable結果存到managerIdDataNotNull、salaryHighEmployee 變數中
            DataTable managerIdDataNotNull = ManagerIdNotNull();
            DataTable salaryHighEmployee = SalaryHighThanManager();

            //設定資料表名稱
            managerIdDataNotNull.TableName = "Employee";
            salaryHighEmployee.TableName = "EmployeeHighSalary";

            //註冊資料RegisterData方法作用是將資料註冊到報表中，讓報表引擎能夠使用這些資料來生成報表
            //其中參數部分()要放要註冊的資料(datatable)與資料名稱(可自行設定但若上面有先設定資料表名稱，需一致性)
            report.RegisterData(managerIdDataNotNull, "Employee");
            report.RegisterData(salaryHighEmployee, "EmployeeHighSalary");

            //此report為先前設的變數代表整個報表，FindObject為Report 物件的一個方法，會搜尋報表模板中名稱為Data1的物件
            //如果有搜尋到會返回DataBand物件，= report.GetDataSource("Employee");這行會得到資料源為Employee的資料(Datatable)
            //一整句的意思就是將DataBand與資料源Employee的資料(Datatable)綁定在一起
            ((DataBand)report.Report.FindObject("Data1")).DataSource= report.GetDataSource("Employee");
            ((DataBand)report.Report.FindObject("Data2")).DataSource = report.GetDataSource("EmployeeHighSalary");

            //是生成報表內容的必要步驟，它會確保所有資料都被正確處理並顯示在報表中。只有在這一步完成後，報表才可以進行匯出或顯示。
            report.Prepare();

            //outPutPdfPath指定了匯出報表的目標路徑和檔案名稱
            //Path.Combine()用來將多個字串合併成一個有效的檔案路徑
            //AppDomain(應用程式域)代表了程式的執行域（也就是應用程式的執行環境）。它提供了關於當前應用程式的資訊，例如執行程式的位置、設置等。
            //CurrentDomain(當前應用程式域)指的是應用程式當前運行時所處的執行環境
            //BaseDirectory(BaseDirectory)該應用程式正在執行的目錄位置; 而返回的值
            //ex:如果在window中且應用程式在 C:\MyApp\ 資料夾裡返回值將是 C:\MyApp\
            string outPutPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report.pdf"); ;

            //PDFSimpleExport 物件，用來將報表匯出為 PDF 格式
            PDFSimpleExport pdfExport = new PDFSimpleExport();

            //Export方法是將報表內容轉換並匯出成指定格式，其中第一個參數是指定了要將報表匯出成 PDF 格式，第二個為指定匯出 PDF 檔案儲存的路徑
            report.Export(pdfExport, outPutPdfPath);
            Console.WriteLine($"報表已匯出到: {outPutPdfPath}");
        }

    }
}
