namespace Assets.Scripts.Infrastructure.Requests.ImageService
{
    public class LoadImageRequest
    {
        public string FileName { get; set; }
        public string Url { get; set; }

        public LoadImageRequest(string fileName, string url)
        {
            FileName = fileName;
            Url = url;
        }
    }
}
