using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityDelta.ImageDiff
{
    public class ExceptionNoFsWebCam : Exception
    {
        public ExceptionNoFsWebCam(string message) : base(message)
        {
            
        }
    }
}
