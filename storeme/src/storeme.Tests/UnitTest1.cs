using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using storeme.Data;
using System.Threading;
using System.Diagnostics;

namespace storeme.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sstring = "asdasdadsasdasdadasdadsasdasddassssssssssssssssssssssssssssssssaa";
            var t = new Stopwatch();
            t.Start();
            var hash = storeme.Data.Encryption.EncryptionHelper.ComputeSecureHash(sstring);
            t.Stop();

            var x = t.ElapsedMilliseconds;
        }
    }
}
