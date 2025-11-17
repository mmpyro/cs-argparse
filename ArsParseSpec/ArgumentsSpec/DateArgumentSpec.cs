using ArgParse;
using ArgParse.Attributes;

namespace ArsParseSpec.ArgumentsSpec
{
    public class DateArgumentSpec
    {
        static DateTime dt = DateTime.Parse("");

        private class ProgramArgs
        {
            [CmdParameter(Name = "-d", Description = "Date", Default = "8/10/1990", Required =false)]
            public DateTime Date { get; set; }
        }

        [Test]
        public void ShouldTakeDateParameterFromArgs()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["-d", "8/10/1990"]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Date, Is.EqualTo(new DateTime(1990, 8, 10)));
        }


        [Test]
        public void ShouldTakeDefaultValueForParameter()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>([]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Date, Is.EqualTo(new DateTime(1990, 8, 10)));
        }
    }
}
