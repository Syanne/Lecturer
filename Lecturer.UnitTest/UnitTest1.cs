using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lecturer.Data.Entities;
using Lecturer.Data.Processor;

namespace Lecturer.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetFileByFTP_Test()
        {
            //arrange
            string subfolder = @"IFMIT/";
            string localPath = @"C:\Users\Anna\Documents\Лектор\";
            string[] ext = { "xls", "xlst" };

            //act
            string path = StorageProcessor.TryGetFileByFTP(subfolder, localPath, ext);

            //assert
            Assert.AreNotEqual(null, path);
        }

        [TestMethod]
        public void ProcessZip_Test()
        {
            string subfolder = @"IFMIT/PI/8/";
            string localPath = @"C:\Users\Anna\Documents\Лектор\8\";
            string[] ext = { "zip" };

            //act
            string path = StorageProcessor.TryGetFileByFTP(subfolder, localPath, ext);
            bool flag = StorageProcessor.ProcessZipFile(path, localPath);
            //assert
            Assert.AreNotEqual(null, path);
            Assert.AreEqual(true, flag);
        }
    }
}
