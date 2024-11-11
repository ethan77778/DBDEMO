using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDEMO
{
    public class Worker
    {
        /// <summary>
        /// 編號
        /// </summary>
       public int Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 薪資
        /// </summary>
        public int Salary { get; set; }
        /// <summary>
        /// 經理編號
        /// </summary>
        public int? ManagerId { get; set; }
    }
}
