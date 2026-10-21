using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiemTheWebSDK.Database
{
    [Table("LoginTables")]
    public class LoginAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(50)]
        public string LoginName { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        public int? Status { get; set; } = 0;

        public DateTime? Date { get; set; } = DateTime.UtcNow;

        public int? ActiveRoleID { get; set; } = 0;

        [MaxLength(50)]
        public string ActiveRoleName { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        public DateTime? TokenTimeExp { get; set; }

        [MaxLength(50)]
        public string AccessToken { get; set; }

        [MaxLength(100)]
        public string Note { get; set; }

        public int? LastServerLogin { get; set; } = 0;

        public DateTime? LastLoginTime { get; set; }

        [MaxLength(50)]
        public string LastIPLogin { get; set; }
    }

    [Table("ServerLists")]
    public class GameServerEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(100)]
        public string strServerName { get; set; }

        public int? nServerOrder { get; set; }

        public int? nServerPort { get; set; }

        public int? nStatus { get; set; }

        [MaxLength(50)]
        public string strURL { get; set; }

        public int? nServerID { get; set; }

        public int? nOnlineNum { get; set; }

        public int? HttpServicePort { get; set; }

        public DateTime? strMaintainStarTime { get; set; }

        public DateTime? strMaintainTerminalTime { get; set; }

        [MaxLength(100)]
        public string strMaintainTxt { get; set; }

        public int? isTestServer { get; set; }
    }

    [Table("GiftCodes")]
    public class GiftCodeEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        public int ServerID { get; set; } = 0;

        public int MaxActive { get; set; } = 1;

        public DateTime? TimeExp { get; set; }

        [MaxLength(500)]
        public string GoodsList { get; set; }
    }

    [Table("GiftCodeLogs")]
    public class GiftCodeLogEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        public int ServerID { get; set; }

        public int RoleID { get; set; }

        public int UserID { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;
    }

    public class SdkDbContext : DbContext
    {
        public DbSet<LoginAccount> LoginAccounts { get; set; }
        public DbSet<GameServerEntry> GameServers { get; set; }
        public DbSet<GiftCodeEntry> GiftCodes { get; set; }
        public DbSet<GiftCodeLogEntry> GiftCodeLogs { get; set; }

        public SdkDbContext(DbContextOptions<SdkDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LoginAccount>().HasIndex(u => u.LoginName).IsUnique();
        }
    }
}
