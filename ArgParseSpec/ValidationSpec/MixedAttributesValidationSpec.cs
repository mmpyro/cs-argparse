using ArgParse;
using ArgParse.Attributes;

namespace ArgParseSpec.ValidationSpec
{
    public class MixedAttributesValidationSpec
    {
        private class InvalidMixedSubCommandAndParameter
        {
            [CmdSubCommandAttribute(Name = "add", Description = "Add user")]
            public AddSubCommand Add { get; set; }

            [CmdParameter(Name = "-n", Description = "Name parameter", Required = false)]
            public string Name { get; set; }
        }

        private class InvalidMixedSubCommandAndFlag
        {
            [CmdSubCommandAttribute(Name = "add", Description = "Add user")]
            public AddSubCommand Add { get; set; }

            [CmdFlag(Name = "-v", Description = "Verbose flag")]
            public bool Verbose { get; set; }
        }

        private class InvalidMixedSubCommandParameterAndFlag
        {
            [CmdSubCommandAttribute(Name = "add", Description = "Add user")]
            public AddSubCommand Add { get; set; }

            [CmdParameter(Name = "-n", Description = "Name parameter", Required = false)]
            public string Name { get; set; }

            [CmdFlag(Name = "-v", Description = "Verbose flag")]
            public bool Verbose { get; set; }
        }

        private class ValidSubCommandOnly
        {
            [CmdSubCommandAttribute(Name = "add", Description = "Add user")]
            public AddSubCommand Add { get; set; }

            [CmdSubCommandAttribute(Name = "remove", Description = "Remove user")]
            public RemoveSubCommand Remove { get; set; }
        }

        private class ValidParameterAndFlagOnly
        {
            [CmdParameter(Name = "-n", Description = "Name parameter", Required = false)]
            public string Name { get; set; }

            [CmdFlag(Name = "-v", Description = "Verbose flag")]
            public bool Verbose { get; set; }
        }

        [ClassSubCommandAttribute]
        private class AddSubCommand
        {
            [CmdParameter(Name = "-n", Description = "Name of the user to add", Required = true)]
            public string UserName { get; set; }
        }

        [ClassSubCommandAttribute]
        private class RemoveSubCommand
        {
            [CmdParameter(Name = "-n", Description = "Name of the user to remove", Required = true)]
            public string UserName { get; set; }
        }

        [Test]
        public void ShouldThrowExceptionWhenMixingSubCommandAndParameter()
        {
            //Given & When & Then
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<InvalidMixedSubCommandAndParameter>(["add", "-n", "test"]);
                argParser.Take();
            });

            Assert.That(ex.Message, Does.Contain("Cannot mix CmdSubCommandAttribute with CmdParameterAttribute"));
            Assert.That(ex.Message, Does.Contain("Add"));
            Assert.That(ex.Message, Does.Contain("Name"));
        }

        [Test]
        public void ShouldThrowExceptionWhenMixingSubCommandAndFlag()
        {
            //Given & When & Then
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<InvalidMixedSubCommandAndFlag>(["add", "-n", "test"]);
                argParser.Take();
            });

            Assert.That(ex.Message, Does.Contain("Cannot mix CmdSubCommandAttribute with CmdFlagAttribute"));
            Assert.That(ex.Message, Does.Contain("Add"));
            Assert.That(ex.Message, Does.Contain("Verbose"));
        }

        [Test]
        public void ShouldThrowExceptionWhenMixingSubCommandParameterAndFlag()
        {
            //Given & When & Then
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var argParser = new ArgParser<InvalidMixedSubCommandParameterAndFlag>(["add", "-n", "test"]);
                argParser.Take();
            });

            Assert.That(ex.Message, Does.Contain("Cannot mix CmdSubCommandAttribute with"));
        }

        [Test]
        public void ShouldNotThrowExceptionWhenUsingOnlySubCommands()
        {
            //Given
            var argParser = new ArgParser<ValidSubCommandOnly>(["add", "-n", "John Doe"]);

            //When & Then
            Assert.DoesNotThrow(() =>
            {
                var pArgs = argParser.Take();
                Assert.That(pArgs.Add.UserName, Is.EqualTo("John Doe"));
            });
        }

        [Test]
        public void ShouldNotThrowExceptionWhenUsingOnlyParametersAndFlags()
        {
            //Given
            var argParser = new ArgParser<ValidParameterAndFlagOnly>(["-n", "test", "-v"]);

            //When & Then
            Assert.DoesNotThrow(() =>
            {
                var pArgs = argParser.Take();
                Assert.That(pArgs.Name, Is.EqualTo("test"));
                Assert.That(pArgs.Verbose, Is.True);
            });
        }
    }
}