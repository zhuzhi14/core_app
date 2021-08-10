using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.DbBase
{
    public class LotteryDb:DbContext
    {
        public LotteryDb(DbContextOptions<LotteryDb> options) : base(options)
        {
        }

        public DbSet<LotteryUsers> LotteryUsers { get; set; }

    }
}