using MilanAuth.Abstractions;
using MilanAuth.Data;

namespace MilanAuth.Services;

public class ImageProcessingService : IImageProcessingService
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(
        AppDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<ImageProcessingService> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }

    public async Task<Guid> StartResizeJobAsync(IFormFile file)
    {
        var jobId = Guid.NewGuid();
        var originalPath = await StoreOriginalImage(file, jobId);

        var job = new ImageProcessingJob
        {
            JobId = jobId,
            OriginalImagePath = originalPath,
            Status = JobStatus.Pending
        };

        _dbContext.ImageJobs.Add(job);
        await _dbContext.SaveChangesAsync();

        // Start background processing
        _ = ProcessImageInBackground(jobId);

        return jobId;
    }

    public async Task<ImageProcessingJob?> GetJobStatusAsync(Guid jobId)
    {
        return await _dbContext.ImageJobs.FindAsync(jobId);
    }

    private async Task ProcessImageInBackground(Guid jobId)
    {
        var job = await _dbContext.ImageJobs.FindAsync(jobId);
        if (job == null) {return; }

         

        job.Status = JobStatus.Processing;
        await _dbContext.SaveChangesAsync();

        try
        {
            // Simulate processing steps with progress updates
            await UpdateProgress(jobId, 20);
            await Task.Delay(10000); // Simulate work

            await UpdateProgress(jobId, 50);
            await Task.Delay(15000); // Simulate work

            var resultPath = await ResizeImage(job.OriginalImagePath, jobId);

            job.ProcessedImagePath = resultPath;
            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.ProgressPercentage = 100;

            _logger.LogInformation("Completed processing job {JobId}", jobId);
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.Error = ex.Message;
            _logger.LogError(ex, "Failed to process image job {JobId}", jobId);
        }
        finally
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task UpdateProgress(Guid jobId, int progress)
    {
        var job = await _dbContext.ImageJobs.FindAsync(jobId);
        if (job != null)
        {
            job.ProgressPercentage = progress;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<string> StoreOriginalImage(IFormFile file, Guid jobId)
    {
        // Use the application's base directory for uploads
        var basePath = AppContext.BaseDirectory;
        var uploadsPath = Path.Combine(basePath, "uploads", jobId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, "original" + Path.GetExtension(file.FileName));

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    private async Task<string> ResizeImage(string originalPath, Guid jobId)
    {
        // Implement your actual image resizing logic here
        // This is just a simulation
        await Task.Delay(15000);

        var uploadsPath = Path.GetDirectoryName(originalPath);
        var resizedPath = Path.Combine(uploadsPath, "resized.jpg");

        // In reality, you would:
        // 1. Load the image from originalPath
        // 2. Resize it using ImageSharp/SkiaSharp/etc.
        // 3. Save to resizedPath

        return resizedPath;
    }
}
