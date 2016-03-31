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
            Institute inst = new Institute
            {
                Name = "ИФМИТ",
                FolderName = "IFMIT",
                Specialities = null
            };

            string localPath = @"C:\Users\Anna\Documents\Лектор\";
            string[] ext = { "xls", "xlst" };

            //act
            bool flag = StorageProcessor.TryGetFileByFTP(inst.FolderName + @"/", localPath, ext);

            //assert
            Assert.AreEqual(true, flag);
        }
    }
}
