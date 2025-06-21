public class QuizAttemptDto
{
    public Guid ResultId { get; set; }                 // ✅ NEW
    public Guid AssessmentId { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; }
    public DateTime AttemptDate { get; set; }          // ✅ Rename from SubmittedAt
    public string? AssessmentTitle { get; set; }       // ✅ NEW
    public string? UserName { get; set; }              // ✅ NEW
}
