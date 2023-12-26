namespace MeganUploadFiles.Models
{
    public class Bucket
    {
        public string Name { get; set; } = null!;
        public MemoryStream InputStream { get; set; } = null!;
        public string BucketName { get; set; } = null!;

    }
}
