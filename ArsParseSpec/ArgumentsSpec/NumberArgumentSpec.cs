using ArgParse;
using ArgParse.Attributes;

namespace ArsParseSpec.ArgumentsSpec
{
    public class NumberArgumentSpec
    {
        private class ProgramArgs
        {
            [CmdParameter(Name = "-i", Description = "Number of items")]
            public int Int { get; set; }

            [CmdParameter(Name = "-b", Description = "Number of items")]
            public byte Byte { get; set; }

            [CmdParameter(Name = "-d", Description = "Number of items")]
            public double Double { get; set; }

            [CmdParameter(Name = "-f", Description = "Number of items")]
            public float Float { get; set; }
        }

        [Test]
        public void ShouldParseIntParameterFromArgs()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-i", "1000", "-b", "100", "-d", "1.2", "-f", "7.8"]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Int, Is.EqualTo(1000));
            Assert.That(pArgs.Byte, Is.EqualTo(100));
            Assert.That(pArgs.Double, Is.EqualTo(1.2));
            Assert.That(pArgs.Float, Is.EqualTo(7.8f));
        }

    }
}
