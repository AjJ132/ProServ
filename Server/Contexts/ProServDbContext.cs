using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProServ.Shared.Models.UserInfo;
using ProServ.Shared.Models.Workouts;
using Microsoft.AspNetCore.Identity;
using ProServ.Shared.Models.Coaches;

namespace ProServ.Server.Contexts
{
    public class ProServDbContext : IdentityDbContext
    {
        public ProServDbContext(DbContextOptions<ProServDbContext> options)
            : base(options)
        {
        }

        //User tables
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<UserTrackRecords> UserTrackRecords { get; set; }
        public DbSet<UserCoachingStyle> UserCoachingStyle { get; set; }
        public DbSet<UserSubscription> UserSubscription { get; set; }
        public DbSet<ProfileOnboarding> ProfileOnboarding { get; set; }
        public DbSet<ReportedInjuries> ReportedInjuries { get; set; }
        public DbSet<UserGoals> UserGoals { get; set; }
        public DbSet<UserInformation> UserInformation { get; set; }

        //All Workout Tables
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<AssignedWorkout> AssignedWorkouts { get; set; }
        public DbSet<WorkoutBlock> WorkoutBlocks { get; set; }
        public DbSet<WorkoutInfo> WorkoutInfos { get; set; }
        public DbSet<WorkoutHistory> WorkoutHistories { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<WorkoutReports> WorkoutReports { get; set; }

        //All team tables
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamInfo> TeamInfo { get; set; }
        public DbSet<TeamPackage> TeamPackage { get; set; }
        public DbSet<AllTeamPackages> AllTeamPackages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workout>()
                .HasMany(w => w.WorkoutBlocks)
                .WithOne(wb => wb.Workout)
                .HasForeignKey(wb => wb.WorkoutId);

            modelBuilder.Entity<Workout>()
                .HasOne(w => w.WorkoutInfo)
                .WithOne(wi => wi.Workout)
                .HasForeignKey<Workout>(wi => wi.WorkoutId);

            //All Team stuff
            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamInfo)
                .WithOne(ti => ti.Team)
                .HasForeignKey<Team>(ti => ti.TeamID);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamPackage)
                .WithOne(tp => tp.Team)
                .HasForeignKey<Team>(tp => tp.TeamID);

            //modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            //modelBuilder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
            //modelBuilder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });

            base.OnModelCreating(modelBuilder);
        }

    }
}

