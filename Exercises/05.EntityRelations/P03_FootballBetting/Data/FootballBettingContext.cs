using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Team>(entity =>
                {
                    entity.HasKey(e => e.TeamId);

                    entity.Property(e => e.Initials)
                          .HasColumnType("CHAR(3)")
                          .IsRequired();

                    entity.HasOne(e => e.PrimaryKitColor)
                          .WithMany(pk => pk.PrimaryKitTeams)
                          .HasForeignKey(e => e.PrimaryKitColorId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.SecondaryKitColor)
                          .WithMany(pk => pk.SecondaryKitTeams)
                          .HasForeignKey(e => e.SecondaryKitColorId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Town)
                          .WithMany(t => t.Teams);
                });

            modelBuilder
                .Entity<Color>(entity =>
                {
                    entity.HasKey(e => e.ColorId);

                });

            modelBuilder
                .Entity<Town>(entity =>
                {
                    entity.HasKey(e => e.TownId);

                    entity.HasOne(e => e.Country)
                          .WithMany(c => c.Towns)
                          .HasForeignKey(e => e.CountryId);
                });

            modelBuilder
                .Entity<Game>(entity =>
                {
                    entity.HasKey(e => e.GameId);

                    entity.HasOne(e => e.HomeTeam)
                          .WithMany(t => t.HomeGames)
                          .HasForeignKey(e => e.HomeTeamId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.AwayTeam)
                          .WithMany(t => t.AwayGames)
                          .HasForeignKey(e => e.AwayTeamId)
                          .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder
                .Entity<PlayerStatistic>(entity =>
                {
                    entity.HasKey(e => new { e.GameId, e.PlayerId });

                    entity.HasOne(e => e.Player)
                          .WithMany(p => p.PlayerStatistics)
                          .HasForeignKey(e => e.PlayerId);

                    entity.HasOne(e => e.Game)
                         .WithMany(g => g.PlayerStatistics)
                         .HasForeignKey(e => e.GameId);
                });
        }
    }
}
