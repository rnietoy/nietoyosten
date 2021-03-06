﻿using System;
using System.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace NietoYostenMvc.Code
{
    public class ImageStorage
    {
        private ImageStorage()
        {
            
        }

        public static ImageStorage GetInstance()
        {
            return new ImageStorage();
        }

        private CloudStorageAccount StorageAccount
        {
            get
            {
                const string account = "nietoyosten";
                string key = ConfigurationManager.AppSettings["STORAGE_ACCOUNT_KEY"];
                string connectionString = $"DefaultEndpointsProtocol=https;AccountName={account};AccountKey={key}";
                return CloudStorageAccount.Parse(connectionString);
            }
        }

        private CloudBlockBlob GetCloudBlockBlob(string fullName)
        {
            // Create the blob client and reference the container
            CloudBlobClient blobClient = this.StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("pictures");

            return container.GetBlockBlobReference(fullName);
        }

        public void Upload(string sourceFileName, string destFullName)
        {
            CloudBlockBlob blockBlob = this.GetCloudBlockBlob(destFullName);
            blockBlob.Properties.ContentType = "image/jpeg";
            blockBlob.UploadFromFile(sourceFileName, FileMode.Open);
        }

        public void Delete(string fullName)
        {
            CloudBlockBlob blockBlob = this.GetCloudBlockBlob(fullName);
            blockBlob.Delete();
        }

        public bool TryDelete(string fullName)
        {
            bool success = false;
            
            try
            {
                this.Delete(fullName);
                success = true;
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException)
            {
                // do nothing
            }

            return success;
        }
    }
}