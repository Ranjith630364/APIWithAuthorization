using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using MeganUploadFiles.Models;
using System.Net;
using Amazon;
//using Amazon.S3.Configurations;

namespace MeganUploadFiles.Services
{
    public class StorageService: IStorageService
    {

        public StorageService()
        {
        }

        public async Task<List<Models.FileInfo>> GetS3BucketFiles(AwsCredentials awsCredentialsValues)
        {
            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };

            var fileNames = new List<Models.FileInfo>();
            var sortedList = new List<Models.FileInfo>();
            try
            {
                // initialise client
                using var _awsS3Client = new AmazonS3Client(credentials, config);

                var result = await _awsS3Client.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request
                {
                    BucketName = "filesdotnet",
                });
                               

                foreach (var s3Object in result.S3Objects)
                {
                    var fileInfo = new Models.FileInfo();
                    fileInfo.Id = s3Object.Key;
                    fileInfo.CreatedDate = s3Object.LastModified;
                    fileNames.Add(fileInfo);
                }

                sortedList = fileNames.OrderByDescending(item => item.CreatedDate).ToList();


                return sortedList;
            }
            catch (AmazonS3Exception ex)
            {
                return fileNames;
            }
        }


         public async Task<Response> UploadFileAsync(Bucket obj, AwsCredentials awsCredentialsValues)
        {
             var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };

            var response = new Response();
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                // initialise client
                using var _awsS3Client = new AmazonS3Client(credentials, config);
                
                // initialise the transfer/upload tools
                var transferUtility = new TransferUtility(_awsS3Client);

                // initiate the file upload
                await transferUtility.UploadAsync(uploadRequest);

                response.StatusCode = 201;
                response.Message = $"{obj.Name} has been uploaded sucessfully";
            }
            catch (AmazonS3Exception s3Ex)
            {
                response.StatusCode = (int)s3Ex.StatusCode;
                response.Message = s3Ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<byte[]> DownloadFileAsync(string file, AwsCredentials awsCredentialsValues)
        {
            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };
            
            MemoryStream ms = null;

            var response = new Response();
            try
            {
              
                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = "filesdotnet",
                    Key = file
                };

                // initialise client
                using var _awsS3Client = new AmazonS3Client(credentials, config);

                using (var result = await _awsS3Client.GetObjectAsync(getObjectRequest))
                {
                    if (result.HttpStatusCode == HttpStatusCode.OK)
                    {
                        using (ms = new MemoryStream())
                        {
                            await result.ResponseStream.CopyToAsync(ms);
                        }
                    }
                }

                if (ms is null || ms.ToArray().Length < 1)
                    throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));

                return ms.ToArray();

            }
            catch (Exception)
            {
                throw;
            }
        }

        //Task<Response> IStorageService.UploadFileAsync(Bucket obj, AwsCredentials awsCredentialsValues)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<byte[]> IStorageService.DownloadFileAsync(string file)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<bool> IStorageService.DeleteFileAsync(string fileName, string versionId)
        //{
        //    throw new NotImplementedException();
        //}

        //        public async Task<byte[]> DownloadFileAsync(string file)
        //{
        //    MemoryStream ms = null;

        //    try
        //    {
        //        GetObjectRequest getObjectRequest = new GetObjectRequest
        //        {
        //            BucketName = _bucketName,
        //            Key = file
        //        };

        //        using (var response = await _awsS3Client.GetObjectAsync(getObjectRequest))
        //        {
        //            if (response.HttpStatusCode == HttpStatusCode.OK)
        //            {
        //                using (ms = new MemoryStream())
        //                {
        //                    await response.ResponseStream.CopyToAsync(ms);
        //                }
        //            }
        //        }

        //        if (ms is null || ms.ToArray().Length < 1)
        //            throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));

        //        return ms.ToArray();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

    }
}





