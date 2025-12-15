using IdeonBack.API.DTOs;
using IdeonBack.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdeonBack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotoSweepController : ControllerBase
{
    private readonly PhotoSweepService _photoSweepService;

    public PhotoSweepController(PhotoSweepService photoSweepService)
    {
        _photoSweepService = photoSweepService;
    }

    /// <summary>
    /// Registrar una nueva foto
    /// </summary>
    [HttpPost("photos")]
    public async Task<ActionResult<PhotoResponse>> RegisterPhoto([FromBody] RegisterPhotoRequest request)
    {
        var photo = await _photoSweepService.RegisterPhotoAsync(
            request.UserId,
            request.Uri,
            request.DateTaken
        );

        return Ok(MapPhotoToResponse(photo));
    }

    /// <summary>
    /// Obtener fotos sin revisar
    /// </summary>
    [HttpGet("users/{userId}/unreviewed")]
    public async Task<ActionResult<List<PhotoResponse>>> GetUnreviewedPhotos(string userId)
    {
        var photos = await _photoSweepService.GetUnreviewedPhotosAsync(userId);
        var response = photos.Select(MapPhotoToResponse).ToList();
        
        return Ok(response);
    }

    /// <summary>
    /// Marcar foto como mantenida
    /// </summary>
    [HttpPost("photos/{photoId}/keep")]
    public async Task<ActionResult<PhotoResponse>> KeepPhoto(string photoId)
    {
        try
        {
            var photo = await _photoSweepService.KeepPhotoAsync(photoId);
            return Ok(MapPhotoToResponse(photo));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Marcar foto como eliminada
    /// </summary>
    [HttpPost("photos/{photoId}/delete")]
    public async Task<ActionResult<PhotoResponse>> DeletePhoto(string photoId)
    {
        try
        {
            var photo = await _photoSweepService.DeletePhotoAsync(photoId);
            return Ok(MapPhotoToResponse(photo));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Recuperar foto de la papelera
    /// </summary>
    [HttpPost("photos/{photoId}/recover")]
    public async Task<ActionResult<PhotoResponse>> RecoverPhoto(string photoId)
    {
        try
        {
            var photo = await _photoSweepService.RecoverPhotoAsync(photoId);
            return Ok(MapPhotoToResponse(photo));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener fotos eliminadas (papelera)
    /// </summary>
    [HttpGet("users/{userId}/deleted")]
    public async Task<ActionResult<List<PhotoResponse>>> GetDeletedPhotos(string userId, [FromQuery] int limit = 5)
    {
        var photos = await _photoSweepService.GetDeletedPhotosAsync(userId, limit);
        var response = photos.Select(MapPhotoToResponse).ToList();
        
        return Ok(response);
    }

    /// <summary>
    /// Obtener estadísticas de limpieza
    /// </summary>
    [HttpGet("users/{userId}/stats")]
    public async Task<ActionResult<PhotoStatsResponse>> GetStats(string userId)
    {
        var stats = await _photoSweepService.GetStatsAsync(userId);
        
        return Ok(new PhotoStatsResponse
        {
            TotalPhotos = stats.TotalPhotos,
            ReviewedPhotos = stats.ReviewedPhotos,
            KeptPhotos = stats.KeptPhotos,
            DeletedPhotos = stats.DeletedPhotos,
            EstimatedSpaceFreed = stats.EstimatedSpaceFreed,
            FormattedSpaceFreed = FormatBytes(stats.EstimatedSpaceFreed)
        });
    }

    /// <summary>
    /// Eliminar permanentemente fotos marcadas
    /// </summary>
    [HttpDelete("users/{userId}/permanent-delete")]
    public async Task<IActionResult> PermanentlyDeletePhotos(string userId)
    {
        await _photoSweepService.PermanentlyDeletePhotosAsync(userId);
        return Ok(new { message = "Fotos eliminadas permanentemente" });
    }

    private PhotoResponse MapPhotoToResponse(Domain.Entities.Photo photo)
    {
        return new PhotoResponse
        {
            Id = photo.Id,
            UserId = photo.UserId,
            Uri = photo.Uri,
            DateTaken = photo.DateTaken,
            KeepStatus = photo.KeepStatus,
            ReviewedAt = photo.ReviewedAt
        };
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
