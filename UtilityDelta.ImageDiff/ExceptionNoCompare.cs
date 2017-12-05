using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityDelta.ImageDiff
{
    public class ExceptionNoCompare : Exception
    {
        public ExceptionNoCompare(string message) : base(message)
        {

        }
    }
}
