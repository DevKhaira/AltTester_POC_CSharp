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

        [SetUp]
        public void Setup()
        {
            // Initialize AltDriver (AltTester connection)
            altDriver = new AltDriver(port: 13000);
            Console.WriteLine("AltDriver started on port 13000");
            
            StartRecording(); // Start video recording on Android device
        }

        [TearDown]
        public void Cleanup()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                StopRecording(); // Stop recording if test fails
                PullVideo(); // Pull video from device to PC
                Console.WriteLine($"Test failed. Video saved: {videoFilePath}");
            }
            else
            {
                StopRecording(deleteFile: true); // Delete video if test passes
            }
            
            if (altDriver != null)
            {
                altDriver.Stop();
                Console.WriteLine("AltDriver stopped");
            }
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
    string localDirectory = Path.Combine(Directory.GetCurrentDirectory(), "videos"); // Full path to 'videos' folder
    string localFileName = Path.GetFileName(videoFilePath); // Extracts just the filename
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
        Console.WriteLine($"Video pulled successfully to: {localFilePath}");
    }
    else
    {
        Console.WriteLine("⚠️ Video pull failed! Check if the file exists on the device.");
    }
}

    }
}
