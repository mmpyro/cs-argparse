using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.HelpTextSpec
{
    public class HelpTextSpec
    {
        [ClassSubCommand]
        private class AddCommand
        {
            [CmdParameter(Name = "-f", Description = "File to add")]
            public string File { get; set; }
        }

        private class EmptyArgs
        {
        }

        private class LongDescriptionArgs
        {
            [CmdParameter(Name = "-d", Description = "This is a very long description that should be handled properly by the help text generator without causing any formatting issues")]
            public string Description { get; set; }
        }

        private class SpecialCharsDescriptionArgs
        {
            [CmdParameter(Name = "-s", Description = "Special chars: \"quotes\", 'apostrophes', <brackets>")]
            public string Special { get; set; }
        }

        private class AllAttributeTypesArgs
        {
            [CmdParameter(Name = "-n", Description = "Name parameter")]
            public string Name { get; set; }

            [CmdParameter(Name = "-c", Description = "Count parameter")]
            public int Count { get; set; }

            [CmdFlag(Name = "-v", Description = "Verbose flag")]
            public bool Verbose { get; set; }
        }

        private class SubCommandArgs
        {
            [CmdSubCommand(Name = "add", Description = "Add command")]
            public AddCommand Add { get; set; }
        }

        private class DefaultValueArgs
        {
            [CmdParameter(Name = "-n", Description = "Name", Default = "default_name")]
            public string Name { get; set; }
        }

        [Test]
        public void ShouldGenerateHelpForEmptyClass()
        {
            //Given & When
            var help = ArgParser<EmptyArgs>.Help();

            //Then
            Assert.That(help, Does.Contain("Usage:"));
            Assert.That(help, Does.Contain("Options:"));
        }

        [Test]
        public void ShouldGenerateHelpWithLongDescription()
        {
            //Given & When
            var help = ArgParser<LongDescriptionArgs>.Help();

            //Then
            Assert.That(help, Does.Contain("-d"));
            Assert.That(help, Does.Contain("very long description"));
        }

        [Test]
        public void ShouldGenerateHelpWithSpecialCharacters()
        {
            //Given & When
            var help = ArgParser<SpecialCharsDescriptionArgs>.Help();

            //Then
            Assert.That(help, Does.Contain("-s"));
            Assert.That(help, Does.Contain("Special chars"));
        }

        [Test]
        public void ShouldGenerateHelpWithAllAttributeTypes()
        {
            //Given & When
            var help = ArgParser<AllAttributeTypesArgs>.Help();

            //Then
            Assert.That(help, Does.Contain("-n"));
            Assert.That(help, Does.Contain("Name parameter"));
            Assert.That(help, Does.Contain("-c"));
            Assert.That(help, Does.Contain("Count parameter"));
            Assert.That(help, Does.Contain("-v"));
            Assert.That(help, Does.Contain("Verbose flag"));
        }

        [Test]
        public void ShouldGenerateHelpForSubCommands()
        {
            //Given & When
            var help = ArgParser<SubCommandArgs>.Help();

            //Then
            Assert.That(help, Does.Contain("add"));
            Assert.That(help, Does.Contain("Add command"));
            Assert.That(help, Does.Contain("Available commands:"));
        }

        [Test]
        public void ShouldShowHelpWhenHelpFlagProvided()
        {
            //Given & When & Then
            var ex = Assert.Throws<HelpRequestedException>(() => new ArgParser<AllAttributeTypesArgs>(["-h"]));
            Assert.That(ex.Message, Is.EqualTo("Help was requested"));
        }

        [Test]
        public void ShouldShowHelpWhenLongHelpFlagProvided()
        {
            //Given & When & Then
            var ex = Assert.Throws<HelpRequestedException>(() => new ArgParser<AllAttributeTypesArgs>(["--help"]));
            Assert.That(ex.Message, Is.EqualTo("Help was requested"));
        }

        [Test]
        public void ShouldGenerateHelpWithDefaultValue()
        {
            //Given & When
            var help = ArgParser<DefaultValueArgs>.Help();

            //Then
            Assert.That(help, Does.Contain("-n"));
            Assert.That(help, Does.Contain("Name"));
        }
    }
}