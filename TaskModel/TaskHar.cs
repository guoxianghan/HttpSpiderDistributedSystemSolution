using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Diagnostics; 
namespace TaskModel
{
    public class TaskHar
    { 
        public int ID { get; set; }
        public Guid Guid { get; set; }
        public Status Status { get; set; }
        /// <summary>
        /// 优先级从0开始排序
        /// </summary>
        public int Priority { get; set; }

        public string WebName { get; set; }
        public string Description { get; set; }
        public string ErrMsg { get; set; }
        /// <summary>
        /// 任务调度表达式
        /// </summary>
        public string QuartzExpression{get;set;}

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
        public List<TaskDetail> TaskDetails { get; set; }
        #endregion

    }

    /// <summary>
    /// 优先级
    /// </summary>
    public enum Priority
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Eleven,
        Twelve

    }
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum Status
    {
        None,
        Success,
        Failed
    }
}
