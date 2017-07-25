using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HsgCommunications
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DeleteOldLogFiles()
        {
            var path = @"c:\temp";
            var file = "StarRezIsoUpdater.txt";

            var Log = new Log(path, file);
            

            using (var f = File.CreateText($@"{path}\000000 {file}"))
            {
                f.WriteLine("Test");
            }

            File.SetLastWriteTime($@"{path}\000000 {file}", DateTime.Now.AddDays(-5));

            Log.DeleteOldFiles(3);

            var match = Directory.GetFiles(path)
                .Select(f => new FileInfo(f))
                .Where(f => f.Name == $"000000 {file}").ToList();

            Assert.IsTrue(match.Count == 0);
        }

        [TestMethod]
        public void SendEmail()
        {
            var e = new Email()
            {
                Host = "smtpgate.email.arizona.edu",
                EnableSSL = false,
                From = "ggracia@email.arizona.edu",
                To = "ggracia@email.arizona.edu",
                Subject = "Test",
                Body = "This is a test",
                BodyIsHTML = true,
                Port = 25 
            };

            try
            {
                e.SendEmail();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
