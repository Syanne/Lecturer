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
            Course.MyCourse.RootFolderPath = @"D:\";
            Course.MyCourse.GroupName = "4 ПІ";

            //act
            StorageProcessor.ProcessSchedule("IFMIT");

            //assert
            Assert.AreNotEqual(null, Course.MyCourse.Subjects);
        }

        [TestMethod]
        public void ProcessZip_Test()
        {
            //arrange
            Course.MyCourse.RootFolderPath = @"D:\";
            string subfolder = "IFMIT/PI/8/";
            string[] ext = { "zip" };
            string path = StorageProcessor.TryGetFileByFTP(subfolder, Course.MyCourse.RootFolderPath, ext);

            //act
            bool flag = StorageProcessor.ProcessZipFile(path, Course.MyCourse.RootFolderPath);

            //assert
            Assert.AreEqual(true, flag);
        }

        [TestMethod]
        public void GetSubjects_Test()
        {
            //arrange
            Course.MyCourse.Semester = "8";
            string filepath = @"D:\settings.xml";

            //act
            XMLProcessor xProc = new XMLProcessor(filepath);
            Course.MyCourse.Subjects = xProc.GetSubjectList();

            //assert
            int size = Course.MyCourse.Subjects.Count;
            Assert.AreEqual(6, size);
        }
    }
}
