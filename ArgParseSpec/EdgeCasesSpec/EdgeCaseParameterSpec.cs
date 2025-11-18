using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.EdgeCasesSpec
{
    public class EdgeCaseParameterSpec
    {
        private class StringArgs
        {
            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }
        }

        private class IntArgs
        {
            [CmdParameter(Name = "-c", Description = "Count")]
            public int Count { get; set; }
        }

        private class DoubleArgs
        {
            [CmdParameter(Name = "-v", Description = "Value")]
            public double Value { get; set; }
        }

        private class MultipleStringArgs
        {
            [CmdParameter(Name = "-f", Description = "Files", Multiple = true)]
            public List<string> Files { get; set; }
        }

        [Test]
        public void ShouldHandleEmptyStringValue()
        {
            //Given
            var argParser = new ArgParser<StringArgs>(["-n", ""]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo(""));
        }

        [Test]
        public void ShouldHandleWhitespaceOnlyValue()
        {
            //Given
            var argParser = new ArgParser<StringArgs>(["-n", "   "]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("   "));
        }

        [Test]
        public void ShouldHandleSpecialCharactersInValue()
        {
            //Given
            var argParser = new ArgParser<StringArgs>(["-n", "test\"with'quotes"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test\"with'quotes"));
        }

        [Test]
        public void ShouldHandleBackslashesInValue()
        {
            //Given
            var argParser = new ArgParser<StringArgs>(["-n", "path\\to\\file"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("path\\to\\file"));
        }

        [Test]
        public void ShouldHandleVeryLongParameterValue()
        {
            //Given
            var longString = new string('a', 10000);
            var argParser = new ArgParser<StringArgs>(["-n", longString]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo(longString));
            Assert.That(result.Name.Length, Is.EqualTo(10000));
        }

        [Test]
        public void ShouldHandleUnicodeCharacters()
        {
            //Given
            var argParser = new ArgParser<StringArgs>(["-n", "Hello世界🌍"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("Hello世界🌍"));
        }

        [Test]
        public void ShouldHandleNonAsciiCharacters()
        {
            //Given
            var argParser = new ArgParser<StringArgs>(["-n", "Ñoño"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("Ñoño"));
        }

        [Test]
        public void ShouldThrowForInvalidIntegerConversion()
        {
            //Given
            var argParser = new ArgParser<IntArgs>(["-c", "not_a_number"]);

            //When & Then
            Assert.Throws<FormatException>(() => argParser.Take());
        }

        [Test]
        public void ShouldThrowForInvalidDoubleConversion()
        {
            //Given
            var argParser = new ArgParser<DoubleArgs>(["-v", "not_a_double"]);

            //When & Then
            Assert.Throws<FormatException>(() => argParser.Take());
        }

        [Test]
        public void ShouldHandleIntegerOverflow()
        {
            //Given
            var argParser = new ArgParser<IntArgs>(["-c", "2147483648"]); // int.MaxValue + 1

            //When & Then
            Assert.Throws<OverflowException>(() => argParser.Take());
        }

        [Test]
        public void ShouldHandleIntegerUnderflow()
        {
            //Given
            var argParser = new ArgParser<IntArgs>(["-c", "-2147483649"]); // int.MinValue - 1

            //When & Then
            Assert.Throws<OverflowException>(() => argParser.Take());
        }

        [Test]
        public void ShouldHandleMultipleParametersWithEmptyStrings()
        {
            //Given
            var argParser = new ArgParser<MultipleStringArgs>(["-f", "", "-f", "file.txt", "-f", ""]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Files, Has.Count.EqualTo(3));
            Assert.That(result.Files[0], Is.EqualTo(""));
            Assert.That(result.Files[1], Is.EqualTo("file.txt"));
            Assert.That(result.Files[2], Is.EqualTo(""));
        }

        [Test]
        public void ShouldHandleMultipleParametersWithWhitespace()
        {
            //Given
            var argParser = new ArgParser<MultipleStringArgs>(["-f", "  ", "-f", "file.txt"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Files, Has.Count.EqualTo(2));
            Assert.That(result.Files[0], Is.EqualTo("  "));
            Assert.That(result.Files[1], Is.EqualTo("file.txt"));
        }
    }
}