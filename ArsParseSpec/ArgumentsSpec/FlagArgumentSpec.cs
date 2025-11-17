using ArgParse;
using ArgParse.Attributes;

namespace ArsParseSpec.ArgumentsSpec
{
    public class FlagArgumentSpec
    {
        private class ProgramArgs
        {
            [CmdFlag(Name = "-v", Description = "Enable debug logging", Default = false)]
            public bool Debug { get; set; }
        }

        [Test]
        public void ShouldParseFlagFromArgs()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-v"]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Debug, Is.True);
        }
    }
}
