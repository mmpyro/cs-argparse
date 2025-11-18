using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;
using System;
using System.IO;

namespace ArgParseSpec.ArgumentsSpec
{
    public class HelpSubCommandSpec
    {
        private class ProgramArgsWithSubCommands
        {
            [CmdSubCommandAttribute(Name = "add", Description = "Add a new user")]
            public AddSubCommand Add { get; set; }

            [CmdSubCommandAttribute(Name = "remove", Description = "Remove an existing user")]
            public RemoveSubCommand Remove { get; set; }

            [CmdSubCommandAttribute(Name = "list", Description = "List all users")]
            public ListSubCommand List { get; set; }
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

        [ClassSubCommandAttribute]
        private class ListSubCommand
        {
            [CmdFlag(Name = "--verbose", Description = "Show detailed information")]
            public bool Verbose { get; set; }
        }

        [Test]
        public void ShouldShowSubCommandHelpWhenEmptyArrayProvided()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgsWithSubCommands>(Array.Empty<string>());

                //Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(output, Does.Contain("Usage: <command> [options]"));
                    Assert.That(output, Does.Contain("Available commands:"));
                    Assert.That(output, Does.Contain("add"));
                    Assert.That(output, Does.Contain("Add a new user"));
                    Assert.That(output, Does.Contain("remove"));
                    Assert.That(output, Does.Contain("Remove an existing user"));
                    Assert.That(output, Does.Contain("list"));
                    Assert.That(output, Does.Contain("List all users"));
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldShowSubCommandHelpWhen_H_Provided()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgsWithSubCommands>(["-h"]);

                // Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(output, Does.Contain("Usage: <command> [options]"));
                    Assert.That(output, Does.Contain("Available commands:"));
                    Assert.That(output, Does.Contain("add"));
                    Assert.That(output, Does.Contain("Add a new user"));
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldShowSubCommandHelpWhen_HelpProvided()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgsWithSubCommands>(["--help"]);

                // Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(output, Does.Contain("Usage: <command> [options]"));
                    Assert.That(output, Does.Contain("Available commands:"));
                    Assert.That(output, Does.Contain("remove"));
                    Assert.That(output, Does.Contain("Remove an existing user"));
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldFormatSubCommandHelpInTableFormat()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgsWithSubCommands>(Array.Empty<string>());

                // Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    // Check for table headers
                    Assert.That(output, Does.Contain("Name"));
                    Assert.That(output, Does.Contain("Description"));
                    
                    // Check for separator line (should contain dashes)
                    var lines = output.Split('\n');
                    var hasSeparatorLine = lines.Any(line => line.TrimStart().StartsWith("---"));
                    Assert.That(hasSeparatorLine, Is.True, "Expected a separator line in the table");
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldNotShowHelpForValidSubCommand()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When - valid subcommand should not trigger help
                var argParser = new ArgParser<ProgramArgsWithSubCommands>(["add", "-n", "John Doe"]);
                var result = argParser.Take();
                
                // Then - should not have printed help
                var output = stringWriter.ToString();
                Assert.That(output, Does.Not.Contain("Usage: <command> [options]"));
                Assert.That(result.Add.UserName, Is.EqualTo("John Doe"));
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldReturnSubCommandHelpTextFromStaticMethod()
        {
            //When
            var helpText = ArgParser<ProgramArgsWithSubCommands>.Help();

            //Then
            Assert.Multiple(() =>
            {
                Assert.That(helpText, Does.Contain("Usage: <command> [options]"));
                Assert.That(helpText, Does.Contain("Available commands:"));
                Assert.That(helpText, Does.Contain("add"));
                Assert.That(helpText, Does.Contain("Add a new user"));
                Assert.That(helpText, Does.Contain("remove"));
                Assert.That(helpText, Does.Contain("Remove an existing user"));
                Assert.That(helpText, Does.Contain("list"));
                Assert.That(helpText, Does.Contain("List all users"));
                
                // Check table formatting
                Assert.That(helpText, Does.Contain("Name"));
                Assert.That(helpText, Does.Contain("Description"));
            });
        }
    }
}