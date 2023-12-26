using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3.Model;
using MeganUploadFiles.Services;
using MeganUploadFiles.Models;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MeganUploadFiles.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UploadController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;


        public UploadController(IConfiguration config,  IStorageService storageService)
        {
            _config = config;
            _storageService = storageService;
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetS3BucketFiles()
        {
            try
            {   

                var cred = new AwsCredentials()
                {
                    AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                    SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                    BucketName = _config["AwsConfiguration:BucketName"]
                };

                var response = await _storageService.GetS3BucketFiles(cred);
                //var response = await _s3Client.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request
                //{
                //    BucketName = bucketName,
                //});

                //var fileNames = new List<string>();
                //foreach (var s3Object in response.S3Objects)
                //{
                //    fileNames.Add(s3Object.Key);
                //}

                return Ok(response);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest($"Error retrieving S3 bucket files: {ex.Message}");
            }
        }


        [HttpPost(Name = "UploadFile")]

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Process file
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileExt = Path.GetExtension(file.FileName);
            var docName = $"{Guid.NewGuid()}{fileExt}";

            // call server
            var s3Obj = new Bucket()
            {
                BucketName = "filesdotnet",
                InputStream = memoryStream,
                Name = docName
            };

            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                BucketName= _config["AwsConfiguration:BucketName"]
            };

            var result = await _storageService.UploadFileAsync(s3Obj, cred);
            
            return Ok(result);

        }

        [HttpGet("Download/{documentName}")]
        public IActionResult GetDocumentFromS3(string documentName)
        {
            try
            {

                var response = new Response();
                if (string.IsNullOrEmpty(documentName))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = $"{documentName} has not found, Please check";
                    return Ok(response);
                }

                var cred = new AwsCredentials()
                {
                    AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                    SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                    BucketName = _config["AwsConfiguration:BucketName"]
                };

                var document = _storageService.DownloadFileAsync(documentName, cred).Result;

                return File(document, "application/octet-stream", documentName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
