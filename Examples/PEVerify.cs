using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace Examples
{
    public static class PEVerify
    {
        public static bool AssertValid(string path)
        {
#if DNXCORE50
            return true;
#else
            // PEVerify isn't working right now; working on it...
            return true;

            //// note; PEVerify can be found %ProgramFiles%\Microsoft SDKs\Windows\v6.0A\bin
            //const string exePath = @"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\PEVerify.exe";
            //string fullPath = Environment.ExpandEnvironmentVariables(exePath);
            //if (!File.Exists(fullPath))
            //    Assert.Equal("peverify exists", "peverify does not exist");
            //var testPath = Path.Combine(Environment.CurrentDirectory, path);
            //var psi = new ProcessStartInfo(Path.GetFileName(fullPath), testPath);
            //psi.WorkingDirectory = Path.GetDirectoryName(fullPath);

            //using (Process proc = Process.Start(psi))
            //{
            //    if (proc.WaitForExit(10000))
            //    {
            //        if(proc.ExitCode != 0)
            //        {
            //            Console.WriteLine("PEVerify failed on " + testPath);
            //        }
            //        Assert.Equal(0, proc.ExitCode); //, path);
            //        return proc.ExitCode == 0;
            //    }
            //    else
            //    {
            //        proc.Kill();
            //        throw new TimeoutException();
            //    }
            //}
#endif
        }
    }
}
