namespace ProductService.Data.Images
{
    public interface IImageProcessor
    {
        Task<bool> ProcessImageAsync(string base64, string productId, string fileName);
        Task<string> ConvertImageAsync(byte[] bytes);
        Task<bool> DeleteImageAsync(string imageURL);
    }
}
