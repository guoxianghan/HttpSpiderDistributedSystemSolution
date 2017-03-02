using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpEntry;
namespace TaskModel
{
    public class TaskDetail
    {
        public int ID { get; set; }
        public Guid Guid { get; set; }
        public Status Status { get; set; }
        /// <summary>
        /// 优先级从0开始排序
        /// </summary>
        public int Priority { get; set; }
        public string Description { get; set; }
        public HarEntry HarEntry { get; set; }

        #region 日志
        public DateTime CreatTime { get; set; }
        public string CreatUserID { get; set; }
        public string CreatUserName { get; set; }

        public DateTime? UpdateTime { get; set; }
        public string UpdateUserID { get; set; }
        public string UpdateUserName { get; set; }

        public bool IsDelete { get; set; }
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        #endregion
    }
}
