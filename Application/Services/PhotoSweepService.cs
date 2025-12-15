using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;

namespace IdeonBack.Application.Services;

/// <summary>
/// Servicio para gestionar PhotoSweep (limpieza de fotos)
/// </summary>
public class PhotoSweepService
{
    private readonly IPhotoRepository _photoRepository;

    public PhotoSweepService(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    /// <summary>
    /// Registrar una nueva foto del usuario
    /// </summary>
    public async Task<Photo> RegisterPhotoAsync(string userId, string uri, DateTime? dateTaken = null)
    {
        var photo = new Photo
        {
            UserId = userId,
            Uri = uri,
            DateTaken = dateTaken ?? DateTime.UtcNow
        };

        return await _photoRepository.CreateAsync(photo);
    }

    /// <summary>
    /// Marcar foto como mantenida
    /// </summary>
    public async Task<Photo> KeepPhotoAsync(string photoId)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId);
        if (photo == null)
            throw new InvalidOperationException("Foto no encontrada");

        photo.KeepStatus = true;
        photo.ReviewedAt = DateTime.UtcNow;

        return await _photoRepository.UpdateAsync(photo);
    }

    /// <summary>
    /// Marcar foto como eliminada
    /// </summary>
    public async Task<Photo> DeletePhotoAsync(string photoId)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId);
        if (photo == null)
            throw new InvalidOperationException("Foto no encontrada");

        photo.KeepStatus = false;
        photo.ReviewedAt = DateTime.UtcNow;

        return await _photoRepository.UpdateAsync(photo);
    }

    /// <summary>
    /// Recuperar una foto de la papelera
    /// </summary>
    public async Task<Photo> RecoverPhotoAsync(string photoId)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId);
        if (photo == null)
            throw new InvalidOperationException("Foto no encontrada");

        photo.KeepStatus = null;
        photo.ReviewedAt = null;

        return await _photoRepository.UpdateAsync(photo);
    }

    /// <summary>
    /// Obtener fotos sin revisar de un usuario
    /// </summary>
    public async Task<IEnumerable<Photo>> GetUnreviewedPhotosAsync(string userId)
    {
        return await _photoRepository.GetUnreviewedByUserIdAsync(userId);
    }

    /// <summary>
    /// Obtener últimas fotos eliminadas (papelera)
    /// </summary>
    public async Task<IEnumerable<Photo>> GetDeletedPhotosAsync(string userId, int limit = 5)
    {
        return await _photoRepository.GetDeletedByUserIdAsync(userId, limit);
    }

    /// <summary>
    /// Obtener estadísticas de limpieza
    /// </summary>
    public async Task<PhotoSweepStats> GetStatsAsync(string userId)
    {
        var allPhotos = await _photoRepository.GetByUserIdAsync(userId);
        var deletedCount = await _photoRepository.GetDeletedCountByUserIdAsync(userId);
        
        // Estimación: asumimos ~3MB por foto promedio
        const long avgPhotoSize = 3 * 1024 * 1024;
        var spaceFreed = deletedCount * avgPhotoSize;

        return new PhotoSweepStats
        {
            TotalPhotos = allPhotos.Count(),
            ReviewedPhotos = allPhotos.Count(p => p.KeepStatus != null),
            KeptPhotos = allPhotos.Count(p => p.KeepStatus == true),
            DeletedPhotos = deletedCount,
            EstimatedSpaceFreed = spaceFreed
        };
    }

    /// <summary>
    /// Eliminar permanentemente fotos marcadas como eliminadas
    /// </summary>
    public async Task PermanentlyDeletePhotosAsync(string userId)
    {
        var deletedPhotos = await _photoRepository.GetDeletedByUserIdAsync(userId, int.MaxValue);
        
        foreach (var photo in deletedPhotos)
        {
            await _photoRepository.DeleteAsync(photo.Id);
        }
    }
}

/// <summary>
/// Estadísticas de PhotoSweep
/// </summary>
public class PhotoSweepStats
{
    public int TotalPhotos { get; set; }
    public int ReviewedPhotos { get; set; }
    public int KeptPhotos { get; set; }
    public int DeletedPhotos { get; set; }
    public long EstimatedSpaceFreed { get; set; }
}
