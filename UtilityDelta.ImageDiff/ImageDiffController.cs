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
                for (var i = 0; i < cameras.Length; i++)
                {
                    var command = $"fswebcam -d /dev/video{cameras[i]} {FsWebCamParams} {BaselineImage(cameras[i])}";
                    var process = _bashRunner.RunCommand(
                        command,
                        Environment.CurrentDirectory, true, null, FsWebCamRetrys);
                    if (process.ExitCode != 0) throw new ExceptionNoFsWebCam($"Could not take base images with fswebcam. Command: {command}");
                };
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
                for (var i = 0; i < cameras.Length; i++)
                {
                    var fswebcamCommand = $"fswebcam -d /dev/video{cameras[i]} {FsWebCamParams} {DiffImage(cameras[i])}";
                    var processTakeImage = _bashRunner.RunCommand(
                        fswebcamCommand,
                        Environment.CurrentDirectory, true, null, FsWebCamRetrys);
                    if (processTakeImage.ExitCode != 0) throw new ExceptionNoFsWebCam($"Could not take diff images with fswebcam. Command: {fswebcamCommand}");

                    var compareCommand = $"compare -fuzz 5% -metric AE {BaselineImage(cameras[i])} {DiffImage(cameras[i])} diffresult{cameras[i]}.jpg";
                    var processDiff = _bashRunner.RunCommand(
                        compareCommand,
                        Environment.CurrentDirectory, true, null, null);
                    //HACK: This call comes out as an error, but actually its ok
                    compareResults[i] = Convert.ToDouble(processDiff.StandardError.ReadToEnd());
                }
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
