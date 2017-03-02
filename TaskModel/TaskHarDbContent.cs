using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Diagnostics;
using HttpEntry;
namespace TaskModel
{
   public class TaskHarDbContent:DbContext
    {
        public TaskHarDbContent() : base("local")
        {
            Configuration.AutoDetectChangesEnabled = false;
            base.Database.Log = (info) => Debug.WriteLine(info);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //兼容sql2008类型
            modelBuilder.Properties().Where(p => p.PropertyType == typeof(DateTime))
                    .Configure(p => p.HasColumnType("datetime2"));
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<HttpHar> HttpHar { get; set; }
        public DbSet<TaskHar> TaskHar { get; set; }
        public DbSet<TaskDetail> TaskDetail { get; set; }
    }
}
