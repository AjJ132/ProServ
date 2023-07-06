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
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserTrackRecords> UserTrackRecords { get; set; }
        public virtual DbSet<UserCoachingStyle> UserCoachingStyle { get; set; }
        public virtual DbSet<UserSubscription> UserSubscription { get; set; }
        public virtual DbSet<ProfileOnboarding> ProfileOnboarding { get; set; }
        public virtual DbSet<ReportedInjuries> ReportedInjuries { get; set; }
        public virtual DbSet<UserGoals> UserGoals { get; set; }
        public virtual DbSet<UserInformation> UserInformation { get; set; }

        //All Workout Tables
        public virtual DbSet<Workout> Workouts { get; set; }
        public virtual DbSet<AssignedWorkout> AssignedWorkouts { get; set; }
        public virtual DbSet<WorkoutBlock> WorkoutBlocks { get; set; }
        public virtual DbSet<WorkoutInfo> WorkoutInfos { get; set; }
        public virtual DbSet<WorkoutHistory> WorkoutHistories { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<WorkoutReports> WorkoutReports { get; set; }

        //All team tables
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamInfo> TeamInfo { get; set; }
        public virtual DbSet<TeamPackage> TeamPackage { get; set; }
        public virtual DbSet<AllTeamPackages> AllTeamPackages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workout>()
                .HasMany(w => w.WorkoutBlocks)
                .WithOne(wb => wb.Workout)
                .HasForeignKey(wb => wb.WorkoutId)
                .IsRequired(false);

            modelBuilder.Entity<Workout>()
                .HasOne(w => w.WorkoutInfo)
                .WithOne(wi => wi.Workout)
                .HasForeignKey<Workout>(wi => wi.WorkoutId);

			modelBuilder.Entity<WorkoutBlock>()
                .HasMany(wb => wb.Parameters)
                .WithOne(p => p.WorkoutBlock)
                .HasForeignKey(p => p.BlockId)
                .IsRequired(false);

			//All Team stuff
			modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamInfo)
                .WithOne(ti => ti.Team)
                .HasForeignKey<Team>(ti => ti.TeamID);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamPackage)
                .WithOne(tp => tp.Team)
                .HasForeignKey<Team>(tp => tp.TeamID);

           
            base.OnModelCreating(modelBuilder);
        }

    }
}

