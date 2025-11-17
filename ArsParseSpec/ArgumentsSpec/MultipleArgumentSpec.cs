using ArgParse;
using ArgParse.Attributes;

namespace ArsParseSpec.ArgumentsSpec
{
    public class MultipleArgumentSpec
    {
        private class ProgramArgs
        {

            [CmdParameter(Name = "-f", Description = "", Required = false, Multiple = true, Default = new int[] { })]
            public List<int> Flags { get; set; }
        }

        [Test]
        public void ShouldParseMultipleParametersToCollection()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-f", "1", "-f", "2"]);

            //When
            var pArgs = argParser.Take();

            //Then
            CollectionAssert.AreEquivalent(new int[] { 1, 2 }, pArgs.Flags);
        }

        [Test]
        public void ShouldReturnDefaultValueForCollection()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>([]);

            //When
            var pArgs = argParser.Take();

            //Then
            CollectionAssert.AreEquivalent(new int[] {}, pArgs.Flags);
        }
    }
}
