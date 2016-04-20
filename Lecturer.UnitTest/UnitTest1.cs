using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lecturer.Data.Entities;
using Lecturer.Data.Processor;

namespace Lecturer.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetSchedule_Test()
        {
            //arrange
            Cource.MyCource.RootFolderPath = @"D:\";
            Cource.MyCource.GroupName = "4 ПІ";

            //act
            StorageProcessor.ProcessSchedule("IFMIT");

            //assert
            Assert.AreNotEqual(null, Cource.MyCource.Subjects);
        }

        [TestMethod]
        public void ProcessZip_Test()
        {
            //arrange
            Cource.MyCource.RootFolderPath = @"D:\";
            string subfolder = "IFMIT/PI/8/";
            string[] ext = { "zip" };
            string path = StorageProcessor.TryGetFileByFTP(subfolder, Cource.MyCource.RootFolderPath, ext);

            //act
            bool flag = StorageProcessor.ProcessZipFile(path, Cource.MyCource.RootFolderPath);

            //assert
            Assert.AreEqual(true, flag);
        }
    }
}
