// Controllers/ImagesController.cs

using Microsoft.AspNetCore.Mvc;
using MilanAuth;
using MilanAuth.Abstractions;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageProcessingService _imageService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        IImageProcessingService imageService,
        ILogger<ImagesController> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    [HttpPost("resize")]
    public async Task<IActionResult> ResizeImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        { return BadRequest("No file uploaded");}

        try
        {
            var jobId = await _imageService.StartResizeJobAsync(file);
            return Accepted(new { jobId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting resize job");
            return StatusCode(500, "Error processing your request");
        }
    }

    [HttpGet("status/{jobId}")]
    public async Task<IActionResult> GetStatus(Guid jobId)
    {
        var job = await _imageService.GetJobStatusAsync(jobId);

        if (job == null)
        { return NotFound();}

        return Ok(new
        {
            job.JobId,
            job.Status,
            job.ProgressPercentage,
            job.CreatedAt,
            job.CompletedAt,
            job.Error,
            ResultUrl = job.Status == JobStatus.Completed
                ? Url.Action("GetImage", new { jobId = job.JobId })
                : null
        });
    }

    [HttpGet("result/{jobId}")]
    public IActionResult GetImage(Guid jobId)
    {
        // Implement actual image retrieval
        // For demo, just return the path
        return Ok(new { message = "Image would be served here" });
    }
}
