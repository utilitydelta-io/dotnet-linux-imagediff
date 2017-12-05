using System;

namespace UtilityDelta.ImageDiff
{
    public interface IImageDiffController
    {
        /// <summary>
        /// Take an image from each camera, storing the image
        /// in the application's working directory.
        /// </summary>
        /// <param name="cameras"></param>
        void TakeBaselineImages(int[] cameras);

        /// <summary>
        /// Take an image from each camera and then use
        /// ImageMagick compare to work out the difference between
        /// the baseline image an new image for each camera. 
        /// </summary>
        /// <param name="cameras"></param>
        /// <returns>Average pixel difference for the cameras</returns>
        double CalculateDifference(int[] cameras);
    }
}
