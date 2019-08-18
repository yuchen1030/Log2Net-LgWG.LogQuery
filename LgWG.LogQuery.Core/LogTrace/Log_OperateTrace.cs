using Abp.Domain.Entities;
using Log2Net.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace LgWG.LogQuery.LogTrace
{


    // Log2Net.Models.Log_OperateTrace 对应
    public partial class Log_OperateTrace : Entity<long>
    {
        [Key]
        [Column(Order = 1)]
        public DateTime LogTime { get; set; }

        [StringLength(100)]
        public string UserID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(200)]
        public string UserName { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public LogType LogType { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public SysCategory SystemID { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(20)]
        public string ServerHost { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(20)]
        public string ServerIP { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(20)]
        public string ClientHost { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(20)]
        public string ClientIP { get; set; }

        [StringLength(100)]
        public string TabOrModu { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(4000)]
        public string Detail { get; set; }

        [StringLength(4000)]
        public string Remark { get; set; }
    }


}
