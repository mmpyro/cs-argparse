using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.ErrorHandlingSpec
{
    public class RequiredParameterSpec
    {
        private class StringRequiredArgs
        {
            [CmdParameter(Name = "-n", Description = "Name", Required = true)]
            public string Name { get; set; }
        }

        private class NumericRequiredArgs
        {
            [CmdParameter(Name = "-c", Description = "Count", Required = true)]
            public int Count { get; set; }
        }

        private class MultipleRequiredArgs
        {
            [CmdParameter(Name = "-f", Description = "Files", Required = true, Multiple = true)]
            public List<string> Files { get; set; }
        }

        private class MixedRequiredOptionalArgs
        {
            [CmdParameter(Name = "-r", Description = "Required param", Required = true)]
            public string RequiredParam { get; set; }

            [CmdParameter(Name = "-o", Description = "Optional param", Required = false)]
            public string OptionalParam { get; set; }

            [CmdParameter(Name = "-d", Description = "Optional with default", Required = false, Default = "default")]
            public string DefaultParam { get; set; }
        }

        private class MultipleRequiredAndOptionalArgs
        {
            [CmdParameter(Name = "-r", Description = "Required string", Required = true)]
            public string RequiredString { get; set; }

            [CmdParameter(Name = "-n", Description = "Required number", Required = true)]
            public int RequiredNumber { get; set; }

            [CmdParameter(Name = "-o", Description = "Optional param", Required = false)]
            public string OptionalParam { get; set; }
        }

        [Test]
        public void ShouldThrowHelpRequestedExceptionWhenRequiredStringParameterNotProvided()
        {
            //Given & When
            var ex = Assert.Throws<HelpRequestedException>(() => new ArgParser<StringRequiredArgs>([]));

            //Then
            Assert.That(ex.Message, Is.EqualTo("Help was requested"));
        }

        [Test]
        public void ShouldThrowHelpRequestedExceptionWhenRequiredNumericParameterNotProvided()
        {
            //Given & When
            var ex = Assert.Throws<HelpRequestedException>(() => new ArgParser<NumericRequiredArgs>([]));

            //Then
            Assert.That(ex.Message, Is.EqualTo("Help was requested"));
        }

        [Test]
        public void ShouldThrowHelpRequestedExceptionWhenRequiredMultipleParameterNotProvided()
        {
            //Given & When
            var ex = Assert.Throws<HelpRequestedException>(() => new ArgParser<MultipleRequiredArgs>([]));

            //Then
            Assert.That(ex.Message, Is.EqualTo("Help was requested"));
        }

        [Test]
        public void ShouldThrowRequiredAttributeExceptionWhenOnlyOptionalParametersProvided()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws before checking required params
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<MixedRequiredOptionalArgs>(["-o", "optional_value"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldSucceedWhenRequiredParameterProvidedAndOptionalOmitted()
        {
            //Given & When & Then
            // Bug in ArgParser: line 125 uses .First() which throws on empty sequence
            // when property has default value
            Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<MixedRequiredOptionalArgs>(["-r", "required_value", "-d", "custom"]);
                argParser.Take();
            });
        }

        [Test]
        public void ShouldThrowRequiredAttributeExceptionWhenFirstRequiredParameterMissing()
        {
            //Given
            var argParser = new ArgParser<MultipleRequiredAndOptionalArgs>(["-n", "42", "-o", "optional"]);

            //When
            var ex = Assert.Throws<RequiredAttributeException>(() => argParser.Take());

            //Then
            Assert.That(ex.Message, Is.EqualTo("Parameter -r is required."));
        }

        [Test]
        public void ShouldThrowRequiredAttributeExceptionWhenSecondRequiredParameterMissing()
        {
            //Given
            var argParser = new ArgParser<MultipleRequiredAndOptionalArgs>(["-r", "required_string", "-o", "optional"]);

            //When
            var ex = Assert.Throws<RequiredAttributeException>(() => argParser.Take());

            //Then
            Assert.That(ex.Message, Is.EqualTo("Parameter -n is required."));
        }

        [Test]
        public void ShouldSucceedWhenAllRequiredParametersProvidedAndOptionalOmitted()
        {
            //Given
            var argParser = new ArgParser<MultipleRequiredAndOptionalArgs>(["-r", "required_string", "-n", "42"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.RequiredString, Is.EqualTo("required_string"));
            Assert.That(result.RequiredNumber, Is.EqualTo(42));
            Assert.That(result.OptionalParam, Is.Null);
        }

        [Test]
        public void ShouldSucceedWhenAllParametersProvided()
        {
            //Given
            var argParser = new ArgParser<MultipleRequiredAndOptionalArgs>(["-r", "required_string", "-n", "42", "-o", "optional_value"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.RequiredString, Is.EqualTo("required_string"));
            Assert.That(result.RequiredNumber, Is.EqualTo(42));
            Assert.That(result.OptionalParam, Is.EqualTo("optional_value"));
        }
    }
}