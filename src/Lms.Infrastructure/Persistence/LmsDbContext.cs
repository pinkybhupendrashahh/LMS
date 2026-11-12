using Lms.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lms.Infrastructure.Persistence;

public sealed class LmsDbContext(DbContextOptions<LmsDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseFacilitator> CourseFacilitators => Set<CourseFacilitator>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<BatchMember> BatchMembers => Set<BatchMember>();
    public DbSet<CourseContent> CourseContents => Set<CourseContent>();
    public DbSet<ContentProgress> ContentProgress => Set<ContentProgress>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamQuestion> ExamQuestions => Set<ExamQuestion>();
    public DbSet<ExamAttempt> ExamAttempts => Set<ExamAttempt>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512);
            entity.HasIndex(x => x.Email).IsUnique();
        });
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
            entity.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => new { x.ParentCategoryId, x.Name }).IsUnique();
            entity.HasOne(x => x.ParentCategory).WithMany(x => x.Children).HasForeignKey(x => x.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => new { x.CategoryId, x.Title });
            entity.HasOne(x => x.Category).WithMany(x => x.Courses).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CourseFacilitator>(entity =>
        {
            entity.HasIndex(x => new { x.CourseId, x.UserId }).IsUnique();
            entity.HasOne(x => x.Course).WithMany(x => x.Facilitators).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.User).WithMany(x => x.FacilitatedCourses).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.Property(x => x.Status).HasMaxLength(24).IsRequired();
            entity.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
            entity.HasIndex(x => new { x.CourseId, x.Status });
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Course).WithMany(x => x.Enrollments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.HasIndex(x => new { x.CourseId, x.Code }).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_Batches_DateRange", "[EndsOn] IS NULL OR [StartsOn] IS NULL OR [EndsOn] >= [StartsOn]"));
        });
        modelBuilder.Entity<BatchMember>(entity =>
        {
            entity.HasIndex(x => new { x.BatchId, x.UserId }).IsUnique();
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CourseContent>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(32).IsRequired();
            entity.Property(x => x.ResourceUrl).HasMaxLength(2048);
            entity.HasIndex(x => new { x.CourseId, x.SortOrder });
        });
        modelBuilder.Entity<ContentProgress>(entity =>
        {
            entity.Property(x => x.ProgressPercent).HasPrecision(5, 2);
            entity.HasIndex(x => new { x.UserId, x.CourseContentId }).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_ContentProgress_Percent", "[ProgressPercent] >= 0 AND [ProgressPercent] <= 100"));
        });
        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(x => x.Prompt).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.QuestionType).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => x.CourseId);
        });
        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.Property(x => x.Text).HasMaxLength(1000).IsRequired();
            entity.HasIndex(x => new { x.QuestionId, x.SortOrder }).IsUnique();
        });
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PassingScorePercent).HasPrecision(5, 2);
            entity.HasIndex(x => new { x.CourseId, x.Title });
            entity.ToTable(t => t.HasCheckConstraint("CK_Exams_PassingScore", "[PassingScorePercent] IS NULL OR ([PassingScorePercent] >= 0 AND [PassingScorePercent] <= 100)"));
            entity.ToTable(t => t.HasCheckConstraint("CK_Exams_TimeLimit", "[TimeLimitMinutes] IS NULL OR [TimeLimitMinutes] > 0"));
        });
        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.Property(x => x.Points).HasPrecision(8, 2);
            entity.HasIndex(x => new { x.ExamId, x.QuestionId }).IsUnique();
            entity.HasIndex(x => new { x.ExamId, x.SortOrder }).IsUnique();
            entity.HasOne(x => x.Question).WithMany(x => x.ExamQuestions).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(t => t.HasCheckConstraint("CK_ExamQuestions_Points", "[Points] > 0"));
        });
        modelBuilder.Entity<ExamAttempt>(entity =>
        {
            entity.HasIndex(x => new { x.ExamId, x.UserId, x.StartedAtUtc });
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<AttemptAnswer>(entity =>
        {
            entity.Property(x => x.AnswerText).HasMaxLength(4000);
            entity.HasIndex(x => new { x.ExamAttemptId, x.ExamQuestionId }).IsUnique();
            entity.HasOne(x => x.ExamQuestion).WithMany().HasForeignKey(x => x.ExamQuestionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.SelectedOption).WithMany().HasForeignKey(x => x.SelectedOptionId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Result>(entity =>
        {
            entity.Property(x => x.Score).HasPrecision(10, 2);
            entity.Property(x => x.MaximumScore).HasPrecision(10, 2);
            entity.HasIndex(x => x.ExamAttemptId).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_Results_Score", "[Score] >= 0 AND [Score] <= [MaximumScore]"));
        });
        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.Property(x => x.CertificateNumber).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.CertificateNumber).IsUnique();
            entity.HasIndex(x => x.EnrollmentId).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_Certificates_Expiry", "[ExpiresAtUtc] IS NULL OR [ExpiresAtUtc] >= [IssuedAtUtc]"));
        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => new { x.UserId, x.ReadAtUtc });
        });
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Body).HasMaxLength(8000).IsRequired();
            entity.HasIndex(x => new { x.CourseId, x.PublishedAtUtc });
            entity.HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorUserId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EntityName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.EntityId).HasMaxLength(80);
            entity.Property(x => x.Details).HasMaxLength(4000);
            entity.HasIndex(x => x.OccurredAtUtc);
            entity.HasIndex(x => new { x.UserId, x.OccurredAtUtc });
        });
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(x => x.TokenHash).HasMaxLength(256).IsRequired();
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.ExpiresAtUtc });
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
