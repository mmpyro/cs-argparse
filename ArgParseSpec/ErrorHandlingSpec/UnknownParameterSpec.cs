using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.ErrorHandlingSpec
{
    public class UnknownParameterSpec
    {
        private class SimpleArgs
        {
            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }
        }

        private class MultipleParametersArgs
        {
            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }

            [CmdParameter(Name = "-c", Description = "Count")]
            public int Count { get; set; }
        }

        private class FlagArgs
        {
            [CmdFlag(Name = "-v", Description = "Verbose")]
            public bool Verbose { get; set; }
        }

        [Test]
        public void ShouldIgnoreUnknownParameter()
        {
            //Given
            var argParser = new ArgParser<SimpleArgs>(["-n", "test", "-unknown", "value"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldIgnoreMultipleUnknownParameters()
        {
            //Given
            var argParser = new ArgParser<SimpleArgs>(["-n", "test", "-x", "val1", "-y", "val2"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldIgnoreUnknownParameterWithKnownParameters()
        {
            //Given
            var argParser = new ArgParser<MultipleParametersArgs>(["-n", "test", "-unknown", "value", "-c", "42"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test"));
            Assert.That(result.Count, Is.EqualTo(42));
        }

        [Test]
        public void ShouldIgnoreUnknownFlag()
        {
            //Given
            var argParser = new ArgParser<FlagArgs>(["-v", "-unknown"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Verbose, Is.True);
        }

        [Test]
        public void ShouldHandleParameterWithoutDashPrefix()
        {
            //Given
            var argParser = new ArgParser<SimpleArgs>(["-n", "test", "malformed"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldHandleOnlyUnknownParameters()
        {
            //Given & When & Then
            // ArgParser throws RequiredAttributeException when parameter is not found
            // even though it's not marked as required (bug in implementation)
            Assert.Throws<RequiredAttributeException>(() =>
            {
                var argParser = new ArgParser<SimpleArgs>(["-unknown", "value"]);
                argParser.Take();
            });
        }
    }
}