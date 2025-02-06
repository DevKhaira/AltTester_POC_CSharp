using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace allinHole_tests_csharp.tests
{
    public class BaseTest
    {
        public AltDriver altDriver;
        private string videoFilePath;

        private string baseUrl;
        private string username;
        private string password;

        public int caseId;

        private TestRailClient testRailClient;
        private int testRunId;

        [SetUp]
        public void Setup()
        {
            // Initialize TestRail client
            baseUrl = "https://homagames2.testrail.io";
            username = "romain.jaubert@homagames.com";
            password = "T28.0kcKJFhxb4Qyr1SY";
            testRailClient = new TestRailClient(baseUrl, username, password);
            testRunId = 963; // Replace with the actual test run ID
            altDriver = new AltDriver(port: 13000);
            Console.WriteLine("AltDriver started on port 13000");
            
            StartRecording(); // Start video recording on Android device
        }

        [TearDown]
    public void Cleanup()
    {
        int statusId = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed ? 5 : 1;
        string comment = statusId == 5 ? $"Test failed. Video attached: {videoFilePath}" : "Test passed successfully.";
        testRailClient.QueueTestResult(caseId, statusId, comment);

        if (statusId == 5 && !string.IsNullOrEmpty(videoFilePath))
        {
            PullVideo();
        }
        
        if (altDriver != null)
        {
            altDriver.Stop();
            Console.WriteLine("AltDriver stopped");
        }
    }

    [OneTimeTearDown]
    public void FinalizeTestRun()
    {
        testRailClient.SubmitResults(testRunId);
    }

        private void StartRecording()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            videoFilePath = $"/sdcard/FailedTest_{timestamp}.mp4";

            Process adbStart = new Process();
            adbStart.StartInfo.FileName = "adb";
            adbStart.StartInfo.Arguments = $"shell screenrecord --time-limit 180 \"{videoFilePath}\"";
            adbStart.StartInfo.UseShellExecute = false;
            adbStart.Start();
        }

        private void StopRecording(bool deleteFile = false)
        {
            Process adbStop = new Process();
            adbStop.StartInfo.FileName = "adb";
            adbStop.StartInfo.Arguments = "shell killall -INT screenrecord";
            adbStop.StartInfo.UseShellExecute = false;
            adbStop.Start();

            if (deleteFile)
            {
                Process adbDelete = new Process();
                adbDelete.StartInfo.FileName = "adb";
                adbDelete.StartInfo.Arguments = $"shell rm \"{videoFilePath}\"";
                adbDelete.StartInfo.UseShellExecute = false;
                adbDelete.Start();
            }
        }


       private void PullVideo()
        {
            if (string.IsNullOrEmpty(videoFilePath))
            {
                Console.WriteLine("❌ Error: videoFilePath is null or empty! Skipping video pull.");
                return; // Stop execution to prevent a crash
            }

            string localDirectory = Path.Combine(Directory.GetCurrentDirectory(), "videos"); // Full path to 'videos' folder
            string localFileName = Path.GetFileName(videoFilePath); // Extract just the filename
            string localFilePath = Path.Combine(localDirectory, localFileName); // Correct local file path

            // Create the directory if it doesn't exist
            if (!Directory.Exists(localDirectory))
            {
                Directory.CreateDirectory(localDirectory);
                Console.WriteLine($"Created directory: {localDirectory}");
            }

            Console.WriteLine($"Pulling video from {videoFilePath} to {localFilePath}");

            Process adbPull = new Process();
            adbPull.StartInfo.FileName = "adb";
            adbPull.StartInfo.Arguments = $"pull \"{videoFilePath}\" \"{localFilePath}\"";
            adbPull.StartInfo.UseShellExecute = false;
            adbPull.Start();
            adbPull.WaitForExit(); // Ensures ADB finishes pulling the file before proceeding

            // Check if the file was successfully copied
            if (File.Exists(localFilePath))
            {
                Console.WriteLine($"✅ Video pulled successfully to: {localFilePath}");
            }
            else
            {
                Console.WriteLine("⚠️ Video pull failed! Check if the file exists on the device.");
            }

        }

        

    }
}
