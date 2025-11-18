using ArgParse;
using ArgParse.Attributes;

namespace ArgParseSpec.TypeConversionSpec
{
    public class NumericTypeConversionSpec
    {
        private class ByteArgs
        {
            [CmdParameter(Name = "-b", Description = "Byte value")]
            public byte ByteValue { get; set; }
        }

        private class SByteArgs
        {
            [CmdParameter(Name = "-sb", Description = "SByte value")]
            public sbyte SByteValue { get; set; }
        }

        private class ShortArgs
        {
            [CmdParameter(Name = "-s", Description = "Short value")]
            public short ShortValue { get; set; }
        }

        private class UShortArgs
        {
            [CmdParameter(Name = "-us", Description = "UShort value")]
            public ushort UShortValue { get; set; }
        }

        private class LongArgs
        {
            [CmdParameter(Name = "-l", Description = "Long value")]
            public long LongValue { get; set; }
        }

        private class ULongArgs
        {
            [CmdParameter(Name = "-ul", Description = "ULong value")]
            public ulong ULongValue { get; set; }
        }

        private class UIntArgs
        {
            [CmdParameter(Name = "-ui", Description = "UInt value")]
            public uint UIntValue { get; set; }
        }

        private class DecimalArgs
        {
            [CmdParameter(Name = "-d", Description = "Decimal value")]
            public decimal DecimalValue { get; set; }
        }

        private class CharArgs
        {
            [CmdParameter(Name = "-c", Description = "Char value")]
            public char CharValue { get; set; }
        }

        private class BoolArgs
        {
            [CmdParameter(Name = "-b", Description = "Bool value")]
            public bool BoolValue { get; set; }
        }

        [Test]
        public void ShouldParseByte()
        {
            //Given
            var argParser = new ArgParser<ByteArgs>(["-b", "255"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.ByteValue, Is.EqualTo(255));
        }

        [Test]
        public void ShouldThrowForByteOverflow()
        {
            //Given
            var argParser = new ArgParser<ByteArgs>(["-b", "256"]);

            //When & Then
            Assert.Throws<OverflowException>(() => argParser.Take());
        }

        [Test]
        public void ShouldParseSByte()
        {
            //Given
            var argParser = new ArgParser<SByteArgs>(["-sb", "-128"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.SByteValue, Is.EqualTo(-128));
        }

        [Test]
        public void ShouldThrowForSByteOverflow()
        {
            //Given
            var argParser = new ArgParser<SByteArgs>(["-sb", "128"]);

            //When & Then
            Assert.Throws<OverflowException>(() => argParser.Take());
        }

        [Test]
        public void ShouldParseShort()
        {
            //Given
            var argParser = new ArgParser<ShortArgs>(["-s", "32767"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.ShortValue, Is.EqualTo(32767));
        }

        [Test]
        public void ShouldThrowForShortOverflow()
        {
            //Given
            var argParser = new ArgParser<ShortArgs>(["-s", "32768"]);

            //When & Then
            Assert.Throws<OverflowException>(() => argParser.Take());
        }

        [Test]
        public void ShouldParseUShort()
        {
            //Given
            var argParser = new ArgParser<UShortArgs>(["-us", "65535"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.UShortValue, Is.EqualTo(65535));
        }

        [Test]
        public void ShouldThrowForUShortOverflow()
        {
            //Given
            var argParser = new ArgParser<UShortArgs>(["-us", "65536"]);

            //When & Then
            Assert.Throws<OverflowException>(() => argParser.Take());
        }

        [Test]
        public void ShouldParseLong()
        {
            //Given
            var argParser = new ArgParser<LongArgs>(["-l", "9223372036854775807"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.LongValue, Is.EqualTo(9223372036854775807));
        }

        [Test]
        public void ShouldParseULong()
        {
            //Given
            var argParser = new ArgParser<ULongArgs>(["-ul", "18446744073709551615"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.ULongValue, Is.EqualTo(18446744073709551615));
        }

        [Test]
        public void ShouldParseUInt()
        {
            //Given
            var argParser = new ArgParser<UIntArgs>(["-ui", "4294967295"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.UIntValue, Is.EqualTo(4294967295));
        }

        [Test]
        public void ShouldParseDecimal()
        {
            //Given
            var argParser = new ArgParser<DecimalArgs>(["-d", "123.456"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.DecimalValue, Is.EqualTo(123.456m));
        }

        [Test]
        public void ShouldParseChar()
        {
            //Given
            var argParser = new ArgParser<CharArgs>(["-c", "A"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.CharValue, Is.EqualTo('A'));
        }

        [Test]
        public void ShouldThrowForMultipleCharacters()
        {
            //Given
            var argParser = new ArgParser<CharArgs>(["-c", "AB"]);

            //When & Then
            Assert.Throws<FormatException>(() => argParser.Take());
        }

        [Test]
        public void ShouldParseBoolTrue()
        {
            //Given
            var argParser = new ArgParser<BoolArgs>(["-b", "true"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.BoolValue, Is.True);
        }

        [Test]
        public void ShouldParseBoolFalse()
        {
            //Given
            var argParser = new ArgParser<BoolArgs>(["-b", "false"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.BoolValue, Is.False);
        }

        [Test]
        public void ShouldParseBoolOne()
        {
            //Given & When & Then
            // ArgParser doesn't support "1" as boolean, only "true"/"false"
            Assert.Throws<FormatException>(() =>
            {
                var argParser = new ArgParser<BoolArgs>(["-b", "1"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldParseBoolZero()
        {
            //Given & When & Then
            // ArgParser doesn't support "0" as boolean, only "true"/"false"
            Assert.Throws<FormatException>(() =>
            {
                var argParser = new ArgParser<BoolArgs>(["-b", "0"]);
                argParser.Take();
            });
        }
    }
}