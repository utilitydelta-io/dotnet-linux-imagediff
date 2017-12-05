using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityDelta.Bash.Interface;

namespace UtilityDelta.ImageDiff
{
    public class ImageDiffController : IImageDiffController
    {
        private const string FsWebCamParams = "-S 30 -r 1280x720 --no-banner";
        private const int FsWebCamTimeout = 200;
        private const int FsWebCamRetrys = 5;
        private readonly IBashRunner _bashRunner;

        public ImageDiffController(IBashRunner bashRunner)
        {
            _bashRunner = bashRunner;
        }

        public void TakeBaselineImages(int[] cameras)
        {
            try
            {
                Parallel.For(0, cameras.Length, (i) =>
                {
                    var command = $"fswebcam -d /dev/video{cameras[i]} {FsWebCamParams} {BaselineImage(cameras[i])}";
                    var process = _bashRunner.RunCommand(
                        command,
                        Environment.CurrentDirectory, true, FsWebCamTimeout, FsWebCamRetrys);
                    if (process.ExitCode != 0) throw new ExceptionNoFsWebCam($"Could not take base images with fswebcam. Command: {command}");
                });
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }

        public double CalculateDifference(int[] cameras)
        {
            var compareResults = new Double[cameras.Length];
            try
            {
                Parallel.For(0, cameras.Length, (i) =>
                {
                    var fswebcamCommand = $"fswebcam -d /dev/video{cameras[i]} {FsWebCamParams} {DiffImage(cameras[i])}";
                    var processTakeImage = _bashRunner.RunCommand(
                        fswebcamCommand,
                        Environment.CurrentDirectory, true, FsWebCamTimeout, FsWebCamRetrys);
                    if (processTakeImage.ExitCode != 0) throw new ExceptionNoFsWebCam($"Could not take diff images with fswebcam. Command: {fswebcamCommand}");

                    var compareCommand = $"compare -fuzz 5% -metric AE {BaselineImage(cameras[i])} {DiffImage(cameras[i])} diffresult{cameras[i]}.jpg";
                    var processDiff = _bashRunner.RunCommand(
                        compareCommand,
                        Environment.CurrentDirectory, true, 500, null);
                    if (processDiff.ExitCode != 0) throw new ExceptionNoCompare($"Could not perform image diff with ImageMagick compare. Command: {compareCommand}");

                    compareResults[i] = Convert.ToDouble(processDiff.StandardOutput.ReadToEnd());
                });
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }

            return compareResults.Average();
        }

        private static string DiffImage(int camera) => $"diff{camera}.jpg";
        private static string BaselineImage(int camera) => $"baseline{camera}.jpg";
    }
}
