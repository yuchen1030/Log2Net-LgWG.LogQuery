using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LgWG.LogQuery.LogMonitor
{
    public class Log_SystemMonitor : Entity<long>
    {
        [Key]
        [Column(Order = 1)]
        public DateTime LogTime { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Log2Net.Models.SysCategory SystemID { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string ServerHost { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(20)]
        public string ServerIP { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OnlineCnt { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllVisitors { get; set; }

        [Key]
        [Column(Order = 7, TypeName = "float")]
        public double RunHours { get; set; }

        [Key]
        [Column(Order = 8, TypeName = "float")]
        public double CpuUsage { get; set; }

        [Key]
        [Column(Order = 9, TypeName = "float")]
        public double MemoryUsage { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProcessNum { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ThreadNum { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CurProcThreadNum { get; set; }

        [Key]
        [Column(Order = 13, TypeName = "float")]
        public double CurProcMem { get; set; }

        [Key]
        [Column(Order = 14, TypeName = "float")]
        public double CurProcMemUse { get; set; }

        [Key]
        [Column(Order = 15, TypeName = "float")]
        public double CurProcCpuUse { get; set; }

        [Key]
        [Column(Order = 16, TypeName = "float")]
        public double CurSubProcMem { get; set; }

        //[Column(TypeName = "xml")]
        public string PageViewNum { get; set; }

        //[Column(TypeName = "xml")]
        public string DiskSpace { get; set; }


        [StringLength(4000)]
        public string Remark { get; set; }
    }
}
