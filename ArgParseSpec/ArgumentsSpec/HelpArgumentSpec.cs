using ArgParse;
using ArgParse.Attributes;
using ArgParse.Exceptions;
using System;
using System.IO;

namespace ArgParseSpec.ArgumentsSpec
{
    public class HelpArgumentSpec
    {
        private class ProgramArgs
        {
            [CmdParameter(Name = "-n", Description = "Name of the program", Required = false)]
            public string Name { get; set; }

            [CmdParameter(Name = "-v", Description = "Verbose output", Required = false, Multiple = true)]
            public List<string> Verbose { get; set; }

            [CmdFlag(Name = "--debug", Description = "Enable debug mode")]
            public bool Debug { get; set; }
        }

        [Test]
        public void ShouldShowHelpWhenEmptyArrayProvided()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgs>(Array.Empty<string>());

                //Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(output, Does.Contain("Usage: [options]"));
                    Assert.That(output, Does.Contain("Options:"));
                    Assert.That(output, Does.Contain("-n"));
                    Assert.That(output, Does.Contain("Name of the program"));
                    Assert.That(output, Does.Contain("-v"));
                    Assert.That(output, Does.Contain("Verbose output"));
                    Assert.That(output, Does.Contain("--debug"));
                    Assert.That(output, Does.Contain("Enable debug mode"));
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldShowHelpWhen_H_Provided()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgs>(["-h"]);

                // Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(output, Does.Contain("Usage: [options]"));
                    Assert.That(output, Does.Contain("Options:"));
                    Assert.That(output, Does.Contain("-n"));
                    Assert.That(output, Does.Contain("Name of the program"));
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldShowHelpWhen_HelpProvided()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgs>(["--help"]);

                // Then - this should have already thrown HelpRequestedException
                Assert.Fail("Expected HelpRequestedException to be thrown");
            }
            catch (HelpRequestedException)
            {
                // This is expected
                var output = stringWriter.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(output, Does.Contain("Usage: [options]"));
                    Assert.That(output, Does.Contain("Options:"));
                    Assert.That(output, Does.Contain("--debug"));
                    Assert.That(output, Does.Contain("Enable debug mode"));
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldFormatHelpInTableFormat()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When
                var argParser = new ArgParser<ProgramArgs>(Array.Empty<string>());

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
        public void ShouldNotShowHelpForNormalArguments()
        {
            //Given
            var stringWriter = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(stringWriter);

            try
            {
                //When - normal arguments should not trigger help
                var argParser = new ArgParser<ProgramArgs>(["-n", "test"]);
                var result = argParser.Take();
                
                // Then - should not have printed help
                var output = stringWriter.ToString();
                Assert.That(output, Does.Not.Contain("Usage: [options]"));
                Assert.That(result.Name, Is.EqualTo("test"));
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ShouldReturnHelpTextFromStaticMethod()
        {
            //When
            var helpText = ArgParser<ProgramArgs>.Help();

            //Then
            Assert.Multiple(() =>
            {
                Assert.That(helpText, Does.Contain("Usage: [options]"));
                Assert.That(helpText, Does.Contain("Options:"));
                Assert.That(helpText, Does.Contain("-n"));
                Assert.That(helpText, Does.Contain("Name of the program"));
                Assert.That(helpText, Does.Contain("-v"));
                Assert.That(helpText, Does.Contain("Verbose output"));
                Assert.That(helpText, Does.Contain("--debug"));
                Assert.That(helpText, Does.Contain("Enable debug mode"));
                
                // Check table formatting
                Assert.That(helpText, Does.Contain("Name"));
                Assert.That(helpText, Does.Contain("Description"));
            });
        }
    }
}