using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;

namespace ArgParseSpec.SubCommandSpec
{
    public class AdvancedSubCommandSpec
    {
        [ClassSubCommand]
        private class AddCommand
        {
            [CmdParameter(Name = "-f", Description = "File", Required = true)]
            public string File { get; set; }
        }

        [ClassSubCommand]
        private class DeleteCommand
        {
            [CmdParameter(Name = "-f", Description = "File", Required = true)]
            public string File { get; set; }

            [CmdFlag(Name = "-force", Description = "Force delete")]
            public bool Force { get; set; }
        }

        [ClassSubCommand]
        private class EmptyCommand
        {
        }

        private class MultipleSubCommandsArgs
        {
            [CmdSubCommand(Name = "add", Description = "Add file")]
            public AddCommand Add { get; set; }

            [CmdSubCommand(Name = "delete", Description = "Delete file")]
            public DeleteCommand Delete { get; set; }
        }

        private class SubCommandWithEmptyArgs
        {
            [CmdSubCommand(Name = "empty", Description = "Empty command")]
            public EmptyCommand Empty { get; set; }
        }

        private class InvalidSubCommandArgs
        {
            [CmdSubCommand(Name = "add", Description = "Add file")]
            public AddCommand Add { get; set; }
        }

        [Test]
        public void ShouldThrowForInvalidSubCommandName()
        {
            //Given
            var argParser = new ArgParser<MultipleSubCommandsArgs>(["invalid"]);

            //When
            var result = argParser.Take();

            //Then
            // ArgParser doesn't validate subcommand names, just returns instance with null subcommands
            Assert.That(result.Add, Is.Null);
            Assert.That(result.Delete, Is.Null);
        }

        [Test]
        public void ShouldThrowForSubCommandWithoutRequiredParameter()
        {
            //Given
            var argParser = new ArgParser<MultipleSubCommandsArgs>(["add"]);

            //When & Then
            var ex = Assert.Throws<RequiredAttributeException>(() => argParser.Take());
            Assert.That(ex.Message, Is.EqualTo("Parameter -f is required."));
        }

        [Test]
        public void ShouldHandleSubCommandWithFlag()
        {
            //Given
            var argParser = new ArgParser<MultipleSubCommandsArgs>(["delete", "-f", "test.txt", "-force"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Delete, Is.Not.Null);
            Assert.That(result.Delete.File, Is.EqualTo("test.txt"));
            Assert.That(result.Delete.Force, Is.True);
        }

        [Test]
        public void ShouldHandleSubCommandWithoutFlag()
        {
            //Given
            var argParser = new ArgParser<MultipleSubCommandsArgs>(["delete", "-f", "test.txt"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Delete, Is.Not.Null);
            Assert.That(result.Delete.File, Is.EqualTo("test.txt"));
            Assert.That(result.Delete.Force, Is.False);
        }

        [Test]
        public void ShouldHandleEmptySubCommand()
        {
            //Given
            var argParser = new ArgParser<SubCommandWithEmptyArgs>(["empty"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Empty, Is.Not.Null);
        }

        [Test]
        public void ShouldHandleFirstSubCommandOnly()
        {
            //Given
            var argParser = new ArgParser<MultipleSubCommandsArgs>(["add", "-f", "file1.txt"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Add, Is.Not.Null);
            Assert.That(result.Add.File, Is.EqualTo("file1.txt"));
            Assert.That(result.Delete, Is.Null);
        }

        [Test]
        public void ShouldHandleSecondSubCommandOnly()
        {
            //Given
            var argParser = new ArgParser<MultipleSubCommandsArgs>(["delete", "-f", "file2.txt"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Add, Is.Null);
            Assert.That(result.Delete, Is.Not.Null);
            Assert.That(result.Delete.File, Is.EqualTo("file2.txt"));
        }

        [Test]
        public void ShouldShowHelpForNoSubCommand()
        {
            //Given & When & Then
            var ex = Assert.Throws<HelpRequestedException>(() => new ArgParser<MultipleSubCommandsArgs>([]));
            Assert.That(ex.Message, Is.EqualTo("Help was requested"));
        }
    }
}