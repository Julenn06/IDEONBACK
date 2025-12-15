using IdeonBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para IDEON - CrateDB
/// </summary>
public class IdeonDbContext : DbContext
{
    public IdeonDbContext(DbContextOptions<IdeonDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Photo> Photos { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<RoomPlayer> RoomPlayers { get; set; } = null!;
    public DbSet<Round> Rounds { get; set; } = null!;
    public DbSet<RoundPhoto> RoundPhotos { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;
    public DbSet<MatchResult> MatchResults { get; set; } = null!;
    public DbSet<AppSettings> AppSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.Username).HasColumnName("username").IsRequired();
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
        });

        // Photo
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.ToTable("photos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Uri).HasColumnName("uri").IsRequired();
            entity.Property(e => e.DateTaken).HasColumnName("date_taken");
            entity.Property(e => e.KeepStatus).HasColumnName("keep_status");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Photos)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Room
        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.Code).HasColumnName("code").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status")
                .HasConversion<string>();
            entity.Property(e => e.RoundsTotal).HasColumnName("rounds_total");
            entity.Property(e => e.SecondsPerRound).HasColumnName("seconds_per_round");
            entity.Property(e => e.NsfwAllowed).HasColumnName("nsfw_allowed");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // RoomPlayer
        modelBuilder.Entity<RoomPlayer>(entity =>
        {
            entity.ToTable("room_players");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.RoomId).HasColumnName("room_id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.JoinedAt).HasColumnName("joined_at");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Players)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.RoomPlayers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Round
        modelBuilder.Entity<Round>(entity =>
        {
            entity.ToTable("rounds");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.RoomId).HasColumnName("room_id").IsRequired();
            entity.Property(e => e.RoundNumber).HasColumnName("round_number").IsRequired();
            entity.Property(e => e.PromptPhrase).HasColumnName("prompt_phrase").IsRequired();
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Rounds)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RoundPhoto
        modelBuilder.Entity<RoundPhoto>(entity =>
        {
            entity.ToTable("round_photos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.RoundId).HasColumnName("round_id").IsRequired();
            entity.Property(e => e.PlayerId).HasColumnName("player_id").IsRequired();
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url").IsRequired();
            entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at");

            entity.HasOne(e => e.Round)
                .WithMany(r => r.RoundPhotos)
                .HasForeignKey(e => e.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Player)
                .WithMany(p => p.RoundPhotos)
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Vote
        modelBuilder.Entity<Vote>(entity =>
        {
            entity.ToTable("votes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.RoundId).HasColumnName("round_id").IsRequired();
            entity.Property(e => e.VoterPlayerId).HasColumnName("voter_player_id").IsRequired();
            entity.Property(e => e.VotedPlayerId).HasColumnName("voted_player_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Round)
                .WithMany(r => r.Votes)
                .HasForeignKey(e => e.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.VoterPlayer)
                .WithMany(p => p.VotesCast)
                .HasForeignKey(e => e.VoterPlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.VotedPlayer)
                .WithMany(p => p.VotesReceived)
                .HasForeignKey(e => e.VotedPlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MatchResult
        modelBuilder.Entity<MatchResult>(entity =>
        {
            entity.ToTable("match_results");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.RoomId).HasColumnName("room_id").IsRequired();
            entity.Property(e => e.WinnerPlayerId).HasColumnName("winner_player_id").IsRequired();
            entity.Property(e => e.TotalRounds).HasColumnName("total_rounds");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Room)
                .WithOne(r => r.MatchResult)
                .HasForeignKey<MatchResult>(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.WinnerPlayer)
                .WithMany()
                .HasForeignKey(e => e.WinnerPlayerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // AppSettings
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.ToTable("app_settings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.DarkMode).HasColumnName("dark_mode");
            entity.Property(e => e.Notifications).HasColumnName("notifications");
            entity.Property(e => e.Language).HasColumnName("language");

            entity.HasOne(e => e.User)
                .WithOne(u => u.AppSettings)
                .HasForeignKey<AppSettings>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
