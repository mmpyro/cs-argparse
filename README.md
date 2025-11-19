# ArgParse

## Project Description

ArgParse is a lightweight C# library designed for parsing command-line arguments using attributes. It simplifies the process of handling CLI inputs by allowing developers to define argument structures declaratively on class properties. Key features include support for parameters (both single and multiple values), flags, subcommands, automatic help generation, and built-in validation to ensure correct usage.

## Project Structure

The project is organized as follows:

- **ArgParse/**: The main library containing the core functionality.
  - `ArgParser.cs`: The primary parser class that processes command-line arguments.
  - **Attributes/**: Contains attribute classes used to decorate properties for argument parsing.
    - `CmdAttribute.cs`: Base attribute class with common properties like `Name`, `Description`, and `Required`.
    - `CmdParameterAttribute.cs`: Attribute for defining parameters, supports single or multiple values with optional defaults.
    - `CmdFlagAttribute.cs`: Attribute for boolean flags.
    - `CmdSubCommandAttribute.cs`: Attribute for defining subcommands.
    - Other attribute files as needed.
  - **Exceptions/**: Custom exception classes for handling parsing errors and validation issues.
    - `DefaultValueTypeMismatchException.cs`
    - `HelpRequestedException.cs`
    - `RequiredAttributeException.cs`
    - And other exception types.

- **ArgParseSpec/**: Test project with comprehensive specifications covering various features.
  - **ArgumentsSpec/**: Tests for basic argument parsing (strings, numbers, dates, etc.).
  - **FlagSpec/**: Tests for flag handling.
  - **SubCommandSpec/**: Tests for subcommand functionality.
  - **MultipleParameterSpec/**: Tests for multiple value parameters.
  - **DefaultValueSpec/**, **EdgeCasesSpec/**, **ErrorHandlingSpec/**, **HelpTextSpec/**, **TypeConversionSpec/**, **ValidationSpec/**: Additional test categories for specific behaviors.

- **Other files**:
  - `ArgParse.sln`: Visual Studio solution file.
  - `docker-compose.yml`: Docker Compose configuration for containerized testing or deployment.
  - `Dockerfile`: Docker image definition.
  - `.gitignore`: Git ignore rules.
  - `LICENSE`: Project license file.

## Use Cases

### Parameters (Single Values)

Parameters are used for arguments that require a value. Use the `CmdParameterAttribute` to define them.

```csharp
using ArgParse;
using ArgParse.Attributes;

public class ProgramArgs
{
    [CmdParameter(Name = "-n", Description = "Name of the program", Required = false)]
    public string Name { get; set; }
}

// Usage
var argParser = new ArgParser<ProgramArgs>(new[] { "-n", "benchmark.sh" });
var pArgs = argParser.Take();
Console.WriteLine(pArgs.Name); // Output: benchmark.sh
```

### Parameters (Multiple Values)

For parameters that can accept multiple values, set `Multiple = true` and use a `List<T>` property type.

```csharp
public class ProgramArgs
{
    [CmdParameter(Name = "-f", Description = "List of flags", Required = false, Multiple = true, Default = new int[] { })]
    public List<int> Flags { get; set; }
}

// Usage
var argParser = new ArgParser<ProgramArgs>(new[] { "-f", "1", "-f", "2" });
var pArgs = argParser.Take();
Console.WriteLine(string.Join(", ", pArgs.Flags)); // Output: 1, 2
```

### Flags

Flags are boolean options that don't require values. Use `CmdFlagAttribute`.

```csharp
public class ProgramArgs
{
    [CmdFlag(Name = "-v", Description = "Enable verbose logging", Default = false)]
    public bool Verbose { get; set; }
}

// Usage
var argParser = new ArgParser<ProgramArgs>(new[] { "-v" });
var pArgs = argParser.Take();
Console.WriteLine(pArgs.Verbose); // Output: True
```

### Subcommands

Subcommands allow grouping related options under a command name. Define subcommands using `CmdSubCommandAttribute` and mark subcommand classes with `ClassSubCommandAttribute`.

```csharp
public class ProgramArgs
{
    [CmdSubCommand(Name = "add", Description = "Add a user")]
    public AddSubCommand Add { get; set; }

    [CmdSubCommand(Name = "remove", Description = "Remove a user")]
    public RemoveSubCommand Remove { get; set; }
}

[ClassSubCommand]
public class AddSubCommand
{
    [CmdParameter(Name = "-n", Description = "Name of the user to add", Required = true)]
    public string UserName { get; set; }
}

[ClassSubCommand]
public class RemoveSubCommand
{
    [CmdParameter(Name = "-n", Description = "Name of the user to remove", Required = true)]
    public string UserName { get; set; }
}

// Usage
var argParser = new ArgParser<ProgramArgs>(new[] { "add", "-n", "John Doe" });
var pArgs = argParser.Take();
Console.WriteLine(pArgs.Add.UserName); // Output: John Doe
```

## Getting Started

1. Add the ArgParse library to your project.
2. Define a class with properties decorated with the appropriate attributes.
3. Create an instance of `ArgParser<T>` with your argument array.
4. Call `Take()` to parse the arguments and get the populated object.

For automatic help generation, run your application with `-h` or `--help`, or no arguments in certain configurations.