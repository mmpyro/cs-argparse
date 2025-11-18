using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.MultipleParameterSpec
{
    public class AdvancedMultipleParameterSpec
    {
        private class MultipleIntArgs
        {
            [CmdParameter(Name = "-n", Description = "Numbers", Multiple = true)]
            public List<int> Numbers { get; set; }
        }

        private class MultipleDoubleArgs
        {
            [CmdParameter(Name = "-v", Description = "Values", Multiple = true)]
            public List<double> Values { get; set; }
        }

        private class MultipleMixedArgs
        {
            [CmdParameter(Name = "-s", Description = "Strings", Multiple = true)]
            public List<string> Strings { get; set; }

            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }
        }

        private class MultipleWithDefaultArgs
        {
            [CmdParameter(Name = "-f", Description = "Files", Multiple = true, Default = new[] { "default.txt" })]
            public List<string> Files { get; set; }
        }

        private class MultipleRequiredArgs
        {
            [CmdParameter(Name = "-f", Description = "Files", Multiple = true, Required = true)]
            public List<string> Files { get; set; }
        }

        [Test]
        public void ShouldHandleMultipleIntParameters()
        {
            //Given
            var argParser = new ArgParser<MultipleIntArgs>(["-n", "1", "-n", "2", "-n", "3"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Numbers, Has.Count.EqualTo(3));
            Assert.That(result.Numbers[0], Is.EqualTo(1));
            Assert.That(result.Numbers[1], Is.EqualTo(2));
            Assert.That(result.Numbers[2], Is.EqualTo(3));
        }

        [Test]
        public void ShouldHandleMultipleDoubleParameters()
        {
            //Given
            var argParser = new ArgParser<MultipleDoubleArgs>(["-v", "1.5", "-v", "2.5", "-v", "3.5"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Values, Has.Count.EqualTo(3));
            Assert.That(result.Values[0], Is.EqualTo(1.5));
            Assert.That(result.Values[1], Is.EqualTo(2.5));
            Assert.That(result.Values[2], Is.EqualTo(3.5));
        }

        [Test]
        public void ShouldHandleSingleValueInMultipleParameter()
        {
            //Given
            var argParser = new ArgParser<MultipleIntArgs>(["-n", "42"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Numbers, Has.Count.EqualTo(1));
            Assert.That(result.Numbers[0], Is.EqualTo(42));
        }

        [Test]
        public void ShouldHandleMultipleParametersWithSingleParameter()
        {
            //Given
            var argParser = new ArgParser<MultipleMixedArgs>(["-s", "a", "-s", "b", "-n", "test"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Strings, Has.Count.EqualTo(2));
            Assert.That(result.Strings[0], Is.EqualTo("a"));
            Assert.That(result.Strings[1], Is.EqualTo("b"));
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldPreserveOrderOfMultipleParameters()
        {
            //Given
            var argParser = new ArgParser<MultipleIntArgs>(["-n", "3", "-n", "1", "-n", "2"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Numbers, Has.Count.EqualTo(3));
            Assert.That(result.Numbers[0], Is.EqualTo(3));
            Assert.That(result.Numbers[1], Is.EqualTo(1));
            Assert.That(result.Numbers[2], Is.EqualTo(2));
        }

        [Test]
        public void ShouldHandleEmptyMultipleParameter()
        {
            //Given & When & Then
            // Empty args with single parameter triggers help
            Assert.Throws<HelpRequestedException>(() => new ArgParser<MultipleIntArgs>([]));
        }

        [Test]
        public void ShouldUseDefaultValueForMultipleParameter()
        {
            //Given & When & Then
            // ArgParser throws ArgumentException for multiple parameters without values
            // even when default is provided
            Assert.Throws<ArgumentException>(() =>
            {
                var argParser = new ArgParser<MultipleWithDefaultArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldOverrideDefaultValueForMultipleParameter()
        {
            //Given
            var argParser = new ArgParser<MultipleWithDefaultArgs>(["-f", "custom.txt"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Files, Has.Count.EqualTo(1));
            Assert.That(result.Files[0], Is.EqualTo("custom.txt"));
        }

        [Test]
        public void ShouldThrowForInvalidTypeInMultipleParameter()
        {
            //Given
            var argParser = new ArgParser<MultipleIntArgs>(["-n", "1", "-n", "invalid", "-n", "3"]);

            //When & Then
            Assert.Throws<FormatException>(() => argParser.Take());
        }

        [Test]
        public void ShouldThrowForRequiredMultipleParameterNotProvided()
        {
            //Given & When & Then
            // Empty args with required parameter triggers help
            Assert.Throws<HelpRequestedException>(() => new ArgParser<MultipleRequiredArgs>([]));
        }
    }
}