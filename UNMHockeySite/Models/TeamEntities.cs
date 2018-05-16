namespace UNMHockeySite.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TeamEntities : DbContext
    {
        public TeamEntities()
            : base("name=TeamEntities")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Goal> Goals { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Roster> Rosters { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<StatsYear> StatsYears { get; set; }
        public virtual DbSet<Season_Player> SeasonPlayer { get; set; }
        public virtual DbSet<Game_Player> GamePlayer { get; set; }
        public virtual DbSet<PlayerRequest> PlayerRequest { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<HomePageHit> HomePageHits { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Game>()
                .Property(e => e.Opponent)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Birthplace)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Height)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Position)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Bio)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Team_Role)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Major)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.SnapChatURL)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.FacebookURL)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.TwitterURL)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.InstagramURL)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Goals)
                .WithOptional(e => e.Player)
                .HasForeignKey(e => e.Assist1_PlayerID);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Goals1)
                .WithOptional(e => e.Player1)
                .HasForeignKey(e => e.Assist2_PlayerID);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Goals2)
                .WithRequired(e => e.Player2)
                .HasForeignKey(e => e.Goal_PlayerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Season>()
                .Property(e => e.Season_Duration)
                .IsUnicode(false);
        }
    }
}
