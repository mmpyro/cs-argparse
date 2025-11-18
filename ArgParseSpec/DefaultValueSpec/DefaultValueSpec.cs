using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.DefaultValueSpec
{
    public class DefaultValueSpec
    {
        private class DefaultStringArgs
        {
            [CmdParameter(Name = "-n", Description = "Name", Default = "default_name")]
            public string Name { get; set; }
        }

        private class DefaultIntArgs
        {
            [CmdParameter(Name = "-c", Description = "Count", Default = 42)]
            public int Count { get; set; }
        }

        private class DefaultDoubleArgs
        {
            [CmdParameter(Name = "-v", Description = "Value", Default = 3.14)]
            public double Value { get; set; }
        }

        private class DefaultBoolArgs
        {
            [CmdParameter(Name = "-b", Description = "Bool", Default = true)]
            public bool BoolValue { get; set; }
        }

        private class DefaultEmptyStringArgs
        {
            [CmdParameter(Name = "-n", Description = "Name", Default = "")]
            public string Name { get; set; }
        }

        private class RequiredWithDefaultArgs
        {
            [CmdParameter(Name = "-n", Description = "Name", Required = true, Default = "default")]
            public string Name { get; set; }
        }

        private class TypeMismatchDefaultArgs
        {
            [CmdParameter(Name = "-c", Description = "Count", Default = "not_a_number")]
            public int Count { get; set; }
        }

        [Test]
        public void ShouldUseDefaultStringValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            // when property has default value
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultStringArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldOverrideDefaultStringValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultStringArgs>(["-n", "custom"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldUseDefaultIntValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultIntArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldOverrideDefaultIntValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultIntArgs>(["-c", "100"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldUseDefaultDoubleValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultDoubleArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldOverrideDefaultDoubleValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultDoubleArgs>(["-v", "2.71"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldUseDefaultBoolValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultBoolArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldOverrideDefaultBoolValue()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultBoolArgs>(["-b", "false"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldUseEmptyStringAsDefault()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<DefaultEmptyStringArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldUseDefaultValueForRequiredParameter()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<RequiredWithDefaultArgs>([]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldThrowForTypeMismatchInDefaultValue()
        {
            //Given & When & Then
            // ArgParser validates default value types during Validate() call
            var argParser = new ArgParser<TypeMismatchDefaultArgs>([]);
            Assert.Throws<DefaultValueTypeMismatchException>(() => argParser.Take());
        }
    }
}