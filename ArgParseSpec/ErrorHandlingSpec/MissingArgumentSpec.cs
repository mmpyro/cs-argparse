using ArgParse;
using ArgParse.Attributes;

namespace ArgParseSpec.ErrorHandlingSpec
{
    public class MissingArgumentSpec
    {
        private class ProgramArgs
        {
            [CmdParameter(Name = "-i", Description = "Number of items", Required =false)]
            public int Int { get; set; }

            [CmdParameter(Name = "-d", Description = "Date", Required = true)]
            public DateTime Date { get; set; }
        }

        [Test]
        public void ShouldThrowsErrorWhenArgumentIsMissing()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-i", "10", "-d"]);

            //When
            var ex = Assert.Throws<ArgumentException>(() => argParser.Take());

            //Then
            Assert.That(ex.Message, Is.EqualTo("Value wasn't set for a parameter -d."));
        }

        [Test]
        public void ShouldThrowsErrorWhenHasInvalidValue()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-d", "-i", "10"]);

            //When
            var ex = Assert.Throws<FormatException>(() => argParser.Take());

            //Then
            Assert.That(ex.Message, Is.EqualTo("String '-i' was not recognized as a valid DateTime."));
        }
    }
}
