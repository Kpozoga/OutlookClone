using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace OutlookClone.Services  
    {
        // shamelessly taken from
        // https://www.c-sharpcorner.com/article/upload-files-in-azure-blob-storage-using-asp-net-core/
        public class BlobStorageService  
        {  
            private string accessKey;  
      
            public BlobStorageService(IConfiguration configuration)
            {
                accessKey = configuration.GetConnectionString("BlobStorageAccessKey");
            }  
      
            public string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType)  
            {  
                try  
                {  
      
                    var _task = Task.Run(() => UploadFileToBlobAsync(strFileName, fileData, fileMimeType));  
                    _task.Wait();  
                    var fileUrl = _task.Result;  
                    return fileUrl;  
                }  
                catch (Exception ex)  
                {  
                    throw ex;  
                }  
            }  
      
            public async void DeleteBlobData(string fileUrl)  
            {  
                var uriObj = new Uri(fileUrl);  
                var BlobName = Path.GetFileName(uriObj.LocalPath);  
      
                var cloudStorageAccount = CloudStorageAccount.Parse(accessKey);  
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();  
                var strContainerName = "uploads";  
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);  
      
                var pathPrefix = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/";  
                var blobDirectory = cloudBlobContainer.GetDirectoryReference(pathPrefix);  
                // get block blob refarence    
                var blockBlob = blobDirectory.GetBlockBlobReference(BlobName);  
      
                // delete blob from container        
                await blockBlob.DeleteAsync();  
            }  
      
      
            private string GenerateFileName(string fileName)  
            {  
                var strFileName = string.Empty;  
                var strName = fileName.Split('.');  
                strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];  
                return strFileName;  
            }  
      
            private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType)  
            {  
                try  
                {  
                    var cloudStorageAccount = CloudStorageAccount.Parse(accessKey);  
                    var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();  
                    var strContainerName = "uploads";  
                    var cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);  
                    var fileName = GenerateFileName(strFileName);  
      
                    if (await cloudBlobContainer.CreateIfNotExistsAsync())  
                    {  
                        await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });  
                    }  
      
                    if (fileName != null && fileData != null)  
                    {  
                        var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);  
                        cloudBlockBlob.Properties.ContentType = fileMimeType;  
                        await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);  
                        return cloudBlockBlob.Uri.AbsoluteUri;  
                    }  
                    return "";  
                }  
                catch (Exception ex)  
                {  
                    throw (ex);  
                }  
            }  
        }  
    }
