namespace MeganUploadFiles.Models
{
    public class AwsCredentials
    {
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string BucketName { get; set; } = "";
        public string Region { get; set; } = "";
        public string AwsSessionToken { get; set; } = "";

    }
}
