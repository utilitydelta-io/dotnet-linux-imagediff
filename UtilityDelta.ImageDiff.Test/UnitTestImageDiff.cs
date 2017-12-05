using System;
using System.IO;
using Moq;
using UtilityDelta.Bash.Implementation;
using UtilityDelta.Bash.Interface;
using Xunit;

namespace UtilityDelta.ImageDiff.Test
{
    public class UnitTestImageDiff
    {
        [Fact]
        public void TestFailure()
        {
            var mockBash = new Mock<IBashRunner>();
            var process = new Mock<ProcessWrapper>();
            process.Setup(x => x.ExitCode).Returns(1);
            mockBash.Setup(x => x.RunCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(),
                It.IsAny<int?>())).Returns(process.Object);
            var service = new ImageDiffController(mockBash.Object);
            Assert.Throws<ExceptionNoFsWebCam>(() => service.TakeBaselineImages(new[] { 1, 2 }));
        }

        [Fact]
        public void TestSetBase()
        {
            var mockBash = new Mock<IBashRunner>();
            var process = new Mock<ProcessWrapper>();
            process.Setup(x => x.ExitCode).Returns(0);
            mockBash.Setup(x => x.RunCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(),
                It.IsAny<int?>())).Returns(process.Object);
            var service = new ImageDiffController(mockBash.Object);
            service.TakeBaselineImages(new[] { 1, 2 });
            mockBash.Verify(x => x.RunCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(),
                It.IsAny<int?>()), Times.Exactly(2));
        }

        [Fact]
        public void TestDiff()
        {
            var process = ProcessObj("3.55");
            var process2 = ProcessObj("3.99");

            var mockBash = new Mock<IBashRunner>();
            mockBash.Setup(x => x.RunCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(),
                It.IsAny<int?>())).Returns(new Mock<ProcessWrapper>().Object);
            mockBash.Setup(x => x.RunCommand("compare -fuzz 5% -metric AE baseline1.jpg diff1.jpg diffresult1.jpg", 
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(),It.IsAny<int?>())).Returns(process.Object);
            mockBash.Setup(x => x.RunCommand("compare -fuzz 5% -metric AE baseline2.jpg diff2.jpg diffresult2.jpg", It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<int?>())).Returns(process2.Object);

            var service = new ImageDiffController(mockBash.Object);
            var result = service.CalculateDifference(new[] { 1, 2 });
            mockBash.Verify(x => x.RunCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int?>(),
                It.IsAny<int?>()), Times.Exactly(4));
            Assert.Equal(3.77, result);
        }

        private static Mock<ProcessWrapper> ProcessObj(string value)
        {
            var process = new Mock<ProcessWrapper>();
            var mem = new MemoryStream();
            var intbytes = System.Text.Encoding.UTF8.GetBytes(value);
            mem.Write(intbytes, 0, intbytes.Length);
            mem.Position = 0;
            process.Setup(x => x.StandardOutput).Returns(new StreamReader(mem));
            process.Setup(x => x.ExitCode).Returns(0);
            return process;
        }
    }
}
