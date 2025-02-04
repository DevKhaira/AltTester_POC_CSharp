using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using AltBy = AltTester.AltTesterUnitySDK.Driver.By;
using allinHole_tests_csharp.tests;
using System;

namespace allinhole_gameplay_csharp.tests
{
    public class AppSizeTest : BaseTest
    {
        [Test]
        public void TestInstalledAppSize()
        {
            try
            {
                string appPackage = "com.homagames.studio.allinhole"; 
                
                var getAppPathCommand = $"adb shell pm path {appPackage} | sed 's/package://'";
                var processPath = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"" + getAppPathCommand + "\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                processPath.Start();
                string apkPath = processPath.StandardOutput.ReadToEnd().Trim();
                processPath.WaitForExit();
                
                if (string.IsNullOrEmpty(apkPath))
                {
                    Console.WriteLine("Failed to retrieve APK path.");
                    Assert.Fail("APK path retrieval failed.");
                    return;
                }
                
                var getSizeCommand = $"adb shell stat -c %s {apkPath}";
                var processSize = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"" + getSizeCommand + "\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                processSize.Start();
                string sizeOutput = processSize.StandardOutput.ReadToEnd().Trim();
                processSize.WaitForExit();
                
                if (string.IsNullOrEmpty(sizeOutput))
                {
                    Console.WriteLine("Failed to retrieve app size.");
                    Assert.Fail("App size retrieval failed.");
                    return;
                }
                
                long fileSizeInBytes = long.Parse(sizeOutput);
                double fileSizeInMB = fileSizeInBytes / (1024.0 * 1024.0);
                Console.WriteLine($"Installed App Size: {fileSizeInMB:F2} MB");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get installed app size: " + ex.Message);
                Assert.Fail("Installed app size retrieval failed.");
            }
        }
    }
}
