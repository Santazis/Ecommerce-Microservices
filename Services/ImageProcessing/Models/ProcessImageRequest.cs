namespace ImageProcessing.Models;

public record ProcessImageRequest(Guid ProductId,string TempUrl, int SortOrder);