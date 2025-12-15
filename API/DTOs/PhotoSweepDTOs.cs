namespace IdeonBack.API.DTOs;

// ============ PHOTOSWEEP DTOs ============

public class RegisterPhotoRequest
{
    public Guid UserId { get; set; }
    public string Uri { get; set; } = string.Empty;
    public DateTime? DateTaken { get; set; }
}

public class PhotoResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Uri { get; set; } = string.Empty;
    public DateTime? DateTaken { get; set; }
    public bool? KeepStatus { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class PhotoStatsResponse
{
    public int TotalPhotos { get; set; }
    public int ReviewedPhotos { get; set; }
    public int KeptPhotos { get; set; }
    public int DeletedPhotos { get; set; }
    public long EstimatedSpaceFreed { get; set; }
    public string FormattedSpaceFreed { get; set; } = string.Empty;
}
