using ArgParse;
using ArgParse.Attributes;

namespace ArsParseSpec.ArgumentsSpec
{
    public class StringArgumentSpec
    {
        private class ProgramArgs
        {
            [CmdParameter(Name = "-n", Description = "Name of the program", Required =false)]
            public string Name { get; set; }
        }


        [Test]
        public void ShouldParseStringParameterFromArgs()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-n", "benchmark.sh"]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Name, Is.EqualTo("benchmark.sh"));
        }

        [Test]
        public void ShouldNotThrowAnyExceptionWhenValueIsNotSet()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>([]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Name, Is.Null);
        }
    }
}
