using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    [Table("lottery_users")]
    public class LotteryUsers
    {
        
        [Column("id")]
        [Description("id")]
        public int Id { get; set; }
        [Column("mobile")] 
        [Description("手机号")]
        public string Mobile { get; set; }
        [Column("user_id")] 
        [Description("用户id")]
        public int UserId { get; set; }
        [Column("status")] 
        public int Status { get; set; }
        [Column("type")]
        public int Type { get; set; }
        [Column("acq")]
        public int Acq { get; set; }
    }
}