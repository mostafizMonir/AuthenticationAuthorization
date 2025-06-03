namespace MilanAuth.Abstractions;

public interface IImageProcessingService
{
    Task<Guid> StartResizeJobAsync(IFormFile file);
    Task<ImageProcessingJob?> GetJobStatusAsync(Guid jobId);
}
