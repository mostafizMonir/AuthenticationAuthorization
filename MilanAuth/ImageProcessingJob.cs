namespace MilanAuth;

public class ImageProcessingJob
{

    public Guid JobId { get; set; } = Guid.NewGuid();
    public string OriginalImagePath { get; set; }
    public string? ProcessedImagePath { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? Error { get; set; }
    public int ProgressPercentage { get; set; }
}

public enum JobStatus { Pending, Processing, Completed, Failed }
