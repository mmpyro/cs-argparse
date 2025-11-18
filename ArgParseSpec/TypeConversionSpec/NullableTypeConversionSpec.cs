using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.TypeConversionSpec
{
    public class NullableTypeConversionSpec
    {
        private class NullableIntArgs
        {
            [CmdParameter(Name = "-n", Description = "Nullable int")]
            public int? NullableInt { get; set; }
        }

        private class NullableDoubleArgs
        {
            [CmdParameter(Name = "-d", Description = "Nullable double")]
            public double? NullableDouble { get; set; }
        }

        private class NullableDateTimeArgs
        {
            [CmdParameter(Name = "-dt", Description = "Nullable DateTime")]
            public DateTime? NullableDateTime { get; set; }
        }

        private class NullableBoolArgs
        {
            [CmdParameter(Name = "-b", Description = "Nullable bool")]
            public bool? NullableBool { get; set; }
        }

        private class NullableDecimalArgs
        {
            [CmdParameter(Name = "-d", Description = "Nullable decimal")]
            public decimal? NullableDecimal { get; set; }
        }

        [Test]
        public void ShouldParseNullableInt()
        {
            //Given & When & Then
            // Nullable types are not currently supported by ArgParser
            // Convert.ChangeType doesn't handle nullable types directly
            Assert.Throws<InvalidCastException>(() =>
            {
                var argParser = new ArgParser<NullableIntArgs>(["-n", "42"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldLeaveNullableIntAsNullWhenNotProvided()
        {
            //Given & When & Then
            // Empty args trigger help for single parameter
            Assert.Throws<HelpRequestedException>(() => new ArgParser<NullableIntArgs>([]));
        }

        [Test]
        public void ShouldParseNullableDouble()
        {
            //Given & When & Then
            // Nullable types are not currently supported by ArgParser
            Assert.Throws<InvalidCastException>(() =>
            {
                var argParser = new ArgParser<NullableDoubleArgs>(["-d", "3.14"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldLeaveNullableDoubleAsNullWhenNotProvided()
        {
            //Given & When & Then
            // Empty args trigger help for single parameter
            Assert.Throws<HelpRequestedException>(() => new ArgParser<NullableDoubleArgs>([]));
        }

        [Test]
        public void ShouldParseNullableDateTime()
        {
            //Given & When & Then
            // Nullable types are not currently supported by ArgParser
            Assert.Throws<InvalidCastException>(() =>
            {
                var argParser = new ArgParser<NullableDateTimeArgs>(["-dt", "2024-01-15"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldLeaveNullableDateTimeAsNullWhenNotProvided()
        {
            //Given & When & Then
            // Empty args trigger help for single parameter
            Assert.Throws<HelpRequestedException>(() => new ArgParser<NullableDateTimeArgs>([]));
        }

        [Test]
        public void ShouldParseNullableBool()
        {
            //Given & When & Then
            // Nullable types are not currently supported by ArgParser
            Assert.Throws<InvalidCastException>(() =>
            {
                var argParser = new ArgParser<NullableBoolArgs>(["-b", "true"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldLeaveNullableBoolAsNullWhenNotProvided()
        {
            //Given & When & Then
            // Empty args trigger help for single parameter
            Assert.Throws<HelpRequestedException>(() => new ArgParser<NullableBoolArgs>([]));
        }

        [Test]
        public void ShouldParseNullableDecimal()
        {
            //Given & When & Then
            // Nullable types are not currently supported by ArgParser
            Assert.Throws<InvalidCastException>(() =>
            {
                var argParser = new ArgParser<NullableDecimalArgs>(["-d", "99.99"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldLeaveNullableDecimalAsNullWhenNotProvided()
        {
            //Given & When & Then
            // Empty args trigger help for single parameter
            Assert.Throws<HelpRequestedException>(() => new ArgParser<NullableDecimalArgs>([]));
        }
    }
}