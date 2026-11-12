namespace Lms.Domain;

public abstract class Entity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class User : Entity
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<CourseFacilitator> FacilitatedCourses { get; set; } = new List<CourseFacilitator>();
}

public sealed class Role : Entity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public sealed class UserRole : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public sealed class Category : Entity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

public sealed class Course : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<CourseFacilitator> Facilitators { get; set; } = new List<CourseFacilitator>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    public ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}

public sealed class CourseFacilitator : Entity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

public sealed class Enrollment : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTimeOffset EnrolledAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "Active";
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}

public sealed class Batch : Entity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly? StartsOn { get; set; }
    public DateOnly? EndsOn { get; set; }
    public ICollection<BatchMember> Members { get; set; } = new List<BatchMember>();
}

public sealed class BatchMember : Entity
{
    public Guid BatchId { get; set; }
    public Batch Batch { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTimeOffset JoinedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class CourseContent : Entity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = "Lesson";
    public string? ResourceUrl { get; set; }
    public int SortOrder { get; set; }
    public ICollection<ContentProgress> ProgressRecords { get; set; } = new List<ContentProgress>();
}

public sealed class ContentProgress : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid CourseContentId { get; set; }
    public CourseContent CourseContent { get; set; } = null!;
    public decimal ProgressPercent { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
}

public sealed class Question : Entity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Prompt { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "SingleChoice";
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
}

public sealed class QuestionOption : Entity
{
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}

public sealed class Exam : Entity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public decimal? PassingScorePercent { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
    public ICollection<ExamAttempt> Attempts { get; set; } = new List<ExamAttempt>();
}

public sealed class ExamQuestion : Entity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public decimal Points { get; set; } = 1;
    public int SortOrder { get; set; }
}

public sealed class ExamAttempt : Entity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTimeOffset StartedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SubmittedAtUtc { get; set; }
    public ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
    public Result? Result { get; set; }
}

public sealed class AttemptAnswer : Entity
{
    public Guid ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; } = null!;
    public Guid ExamQuestionId { get; set; }
    public ExamQuestion ExamQuestion { get; set; } = null!;
    public Guid? SelectedOptionId { get; set; }
    public QuestionOption? SelectedOption { get; set; }
    public string? AnswerText { get; set; }
}

public sealed class Result : Entity
{
    public Guid ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; } = null!;
    public decimal Score { get; set; }
    public decimal MaximumScore { get; set; }
    public bool Passed { get; set; }
    public DateTimeOffset GradedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Certificate : Entity
{
    public Guid EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTimeOffset IssuedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAtUtc { get; set; }
}

public sealed class Notification : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset? ReadAtUtc { get; set; }
}

public sealed class Announcement : Entity
{
    public Guid AuthorUserId { get; set; }
    public User Author { get; set; } = null!;
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset PublishedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class AuditLog : Entity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
}
