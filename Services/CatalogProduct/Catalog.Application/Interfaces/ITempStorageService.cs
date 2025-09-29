namespace Catalog.Application.Interfaces;

public interface ITempStorageService
{
    Task<string> SaveFilesToTempStorageAsync(Stream stream,string contentType,string fileName);
}