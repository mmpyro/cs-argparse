using ArgParse;
using ArgParse.Attributes;

namespace ArgParseSpec.SubCommandSpec
{
    public class SubCommandSpec
    {
        private class ProgramArgs
        {
            [CmdSubCommandAttribute(Name = "add", Description = "Add user")]
            public AddSubCommand Add { get; set; }

            [CmdSubCommandAttribute(Name = "remove", Description = "Remove user")]
            public RemoveSubCommand Remove { get; set; }
        }

        [ClassSubCommandAttribute]
        private class AddSubCommand
        {
          [CmdParameter(Name = "-n", Description = "Name of the user to add", Required =true)]
          public string UserName { get; set; }
        }

        [ClassSubCommandAttribute]
        private class RemoveSubCommand
        {
          [CmdParameter(Name = "-n", Description = "Name of the user to remove", Required =true)]
          public string UserName { get; set; }
        }

        [Test]
        public void ShouldProcessAddSubCommand()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["add", "-n", "John Doe"]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Add.UserName, Is.EqualTo("John Doe"));
            Assert.IsNull(pArgs.Remove);
        }

        [Test]
        public void ShouldProcessRemoveSubCommand()
        {
            //Given
            var argParser = new ArgParser<ProgramArgs>(["remove", "-n", "John Doe"]);

            //When
            var pArgs = argParser.Take();

            //Then
            Assert.That(pArgs.Remove.UserName, Is.EqualTo("John Doe"));
            Assert.IsNull(pArgs.Add);
        }
    }
}