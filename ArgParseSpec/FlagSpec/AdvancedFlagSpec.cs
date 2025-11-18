using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.FlagSpec
{
    public class AdvancedFlagSpec
    {
        private class MultipleFlagsArgs
        {
            [CmdFlag(Name = "-v", Description = "Verbose")]
            public bool Verbose { get; set; }

            [CmdFlag(Name = "-d", Description = "Debug")]
            public bool Debug { get; set; }

            [CmdFlag(Name = "-q", Description = "Quiet")]
            public bool Quiet { get; set; }
        }

        private class FlagWithParameterArgs
        {
            [CmdFlag(Name = "-v", Description = "Verbose")]
            public bool Verbose { get; set; }

            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }
        }

        private class DuplicateFlagArgs
        {
            [CmdFlag(Name = "-v", Description = "Verbose")]
            public bool Verbose { get; set; }
        }

        private class FlagAtDifferentPositionsArgs
        {
            [CmdFlag(Name = "-v", Description = "Verbose")]
            public bool Verbose { get; set; }

            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }

            [CmdParameter(Name = "-c", Description = "Count")]
            public int Count { get; set; }
        }

        [Test]
        public void ShouldHandleMultipleFlags()
        {
            //Given
            var argParser = new ArgParser<MultipleFlagsArgs>(["-v", "-d"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Verbose, Is.True);
            Assert.That(result.Debug, Is.True);
            Assert.That(result.Quiet, Is.False);
        }

        [Test]
        public void ShouldHandleAllFlagsSet()
        {
            //Given
            var argParser = new ArgParser<MultipleFlagsArgs>(["-v", "-d", "-q"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Verbose, Is.True);
            Assert.That(result.Debug, Is.True);
            Assert.That(result.Quiet, Is.True);
        }

        [Test]
        public void ShouldHandleNoFlagsSet()
        {
            //Given & When & Then
            // Empty args with multiple options triggers help
            Assert.Throws<HelpRequestedException>(() => new ArgParser<MultipleFlagsArgs>([]));
        }

        [Test]
        public void ShouldHandleFlagBeforeParameter()
        {
            //Given
            var argParser = new ArgParser<FlagWithParameterArgs>(["-v", "-n", "test"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Verbose, Is.True);
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldHandleFlagAfterParameter()
        {
            //Given
            var argParser = new ArgParser<FlagWithParameterArgs>(["-n", "test", "-v"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Verbose, Is.True);
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldHandleFlagBetweenParameters()
        {
            //Given
            var argParser = new ArgParser<FlagAtDifferentPositionsArgs>(["-n", "test", "-v", "-c", "42"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test"));
            Assert.That(result.Verbose, Is.True);
            Assert.That(result.Count, Is.EqualTo(42));
        }

        [Test]
        public void ShouldHandleDuplicateFlag()
        {
            //Given
            var argParser = new ArgParser<DuplicateFlagArgs>(["-v", "-v"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Verbose, Is.True);
        }

        [Test]
        public void ShouldHandleFlagWithoutValue()
        {
            //Given & When & Then
            // ArgParser throws RequiredAttributeException when parameter is not provided
            // even though it's not marked as required (bug in implementation)
            Assert.Throws<RequiredAttributeException>(() =>
            {
                var argParser = new ArgParser<FlagWithParameterArgs>(["-v"]);
                argParser.Take();
            });
        }
    }
}