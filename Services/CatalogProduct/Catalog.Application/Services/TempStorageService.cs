using Catalog.Application.Interfaces;

namespace Catalog.Application.Services;

public class TempStorageService : ITempStorageService
{
    private  string Folder = $"{Environment.CurrentDirectory}/images/temp";
    public TempStorageService()
    {
        Folder = Path.Combine(Environment.CurrentDirectory, "images", "temp");

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);
    }
    public async Task<string> SaveFilesToTempStorageAsync(Stream stream)
    {
        var fileName = Guid.NewGuid().ToString();
        var filePath = $"{Folder}/{fileName}";
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        return filePath;
    }
}