using ArgParse;
using ArgParse.Attributes;

namespace ArgParseSpec.ValidationSpec
{
    public class AttributeValidationSpec
    {
        private class NoAttributesArgs
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        private class MixedAttributesAndPlainPropertiesArgs
        {
            [CmdParameter(Name = "-n", Description = "Name")]
            public string Name { get; set; }

            public string PlainProperty { get; set; }
        }

        [ClassSubCommand]
        private class ValidSubCommand
        {
            [CmdParameter(Name = "-f", Description = "File")]
            public string File { get; set; }
        }

        private class SubCommandWithAttributeArgs
        {
            [CmdSubCommand(Name = "valid", Description = "Valid command")]
            public ValidSubCommand Valid { get; set; }
        }

        private class DuplicateParameterNamesArgs
        {
            [CmdParameter(Name = "-n", Description = "First name")]
            public string FirstName { get; set; }

            [CmdParameter(Name = "-n", Description = "Second name")]
            public string SecondName { get; set; }
        }

        [Test]
        public void ShouldHandleClassWithNoAttributes()
        {
            //Given
            var argParser = new ArgParser<NoAttributesArgs>([]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShouldIgnorePlainPropertiesWithoutAttributes()
        {
            //Given
            var argParser = new ArgParser<MixedAttributesAndPlainPropertiesArgs>(["-n", "test"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Name, Is.EqualTo("test"));
            Assert.That(result.PlainProperty, Is.Null);
        }

        [Test]
        public void ShouldHandleValidSubCommandWithAttribute()
        {
            //Given
            var argParser = new ArgParser<SubCommandWithAttributeArgs>(["valid", "-f", "test.txt"]);

            //When
            var result = argParser.Take();

            //Then
            Assert.That(result.Valid, Is.Not.Null);
            Assert.That(result.Valid.File, Is.EqualTo("test.txt"));
        }

        [Test]
        public void ShouldHandleDuplicateParameterNames()
        {
            //Given
            var argParser = new ArgParser<DuplicateParameterNamesArgs>(["-n", "value"]);

            //When
            var result = argParser.Take();

            //Then
            // Both properties should get the same value since they share the same parameter name
            Assert.That(result.FirstName, Is.EqualTo("value"));
            Assert.That(result.SecondName, Is.EqualTo("value"));
        }
    }
}