using Amazon.S3.Model;
using MeganUploadFiles.Models;

namespace MeganUploadFiles.Services
{
    public interface IStorageService
    {
        Task<List<Models.FileInfo>> GetS3BucketFiles(AwsCredentials awsCredentialsValues);
        Task<Response> UploadFileAsync(Bucket obj, AwsCredentials awsCredentialsValues);
        Task<byte[]> DownloadFileAsync(string file, AwsCredentials awsCredentialsValues);
        //Task<bool> DeleteFileAsync(string fileName, string versionId = "");
    }

}
