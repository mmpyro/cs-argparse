using ArgParse.Attributes;
using ArgParse.Exceptions;
using System.Collections;
using System.Linq;

namespace ArgParse
{
    public class ArgParser<T> where T : class
    {
        private readonly List<string> _args;
        private readonly T _instance;

        public ArgParser(string[] args)
        {
            _args = new List<string>(args);
            _instance = (T)Activator.CreateInstance(typeof(T));
            
            // Check for help request
            if (ShouldShowHelp(_args.ToArray()))
            {
                var helpText = Help();
                Console.WriteLine(helpText);
                throw new HelpRequestedException("Help was requested");
            }
        }
        
        private static bool ShouldShowHelp(string[] args)
        {
            // Show help if:
            // 1. Explicit help flags are provided
            if (args.Contains("-h") || args.Contains("--help"))
                return true;
            
            // 2. Empty array - show help based on parameter configuration
            if (args.Length == 0)
            {
                var properties = typeof(T).GetProperties();
                
                // Check if class has only subcommands
                var subCommands = properties
                    .SelectMany(p => p.GetCustomAttributes(true))
                    .OfType<CmdSubCommandAttribute>()
                    .ToList();
                
                var parameters = properties
                    .SelectMany(p => p.GetCustomAttributes(true))
                    .OfType<CmdParameterAttribute>()
                    .ToList();
                
                var flags = properties
                    .SelectMany(p => p.GetCustomAttributes(true))
                    .OfType<CmdFlagAttribute>()
                    .ToList();
                
                // If class has only subcommands, show help
                if (subCommands.Any() && !parameters.Any() && !flags.Any())
                    return true;
                
                var totalOptions = parameters.Count + flags.Count;
                
                // Show help if there are required parameters without defaults
                if (parameters.Any(attr => attr.Required && attr.Default == null))
                    return true;
                
                // Show help if there are multiple options (2 or more), indicating a complex CLI
                if (totalOptions >= 2)
                    return true;
                
                // Don't show help for single optional parameter
                return false;
            }
            
            return false;
        }

        private static void Validate()
        {
            var allProperties = typeof(T).GetProperties();
            
            // Check for mixing CmdSubCommandAttribute with CmdFlagAttribute or CmdParameterAttribute
            var subCommandProperties = allProperties
                .Where(p => p.GetCustomAttributes(true).OfType<CmdSubCommandAttribute>().Any())
                .ToList();
            
            var flagProperties = allProperties
                .Where(p => p.GetCustomAttributes(true).OfType<CmdFlagAttribute>().Any())
                .ToList();
            
            var parameterProperties = allProperties
                .Where(p => p.GetCustomAttributes(true).OfType<CmdParameterAttribute>().Any())
                .ToList();
            
            // If there are subcommand properties, ensure there are no flag or parameter properties
            if (subCommandProperties.Any())
            {
                if (flagProperties.Any())
                {
                    throw new InvalidOperationException(
                        $"Cannot mix CmdSubCommandAttribute with CmdFlagAttribute. " +
                        $"Properties with CmdSubCommandAttribute: {string.Join(", ", subCommandProperties.Select(p => p.Name))}. " +
                        $"Properties with CmdFlagAttribute: {string.Join(", ", flagProperties.Select(p => p.Name))}.");
                }
                
                if (parameterProperties.Any())
                {
                    throw new InvalidOperationException(
                        $"Cannot mix CmdSubCommandAttribute with CmdParameterAttribute. " +
                        $"Properties with CmdSubCommandAttribute: {string.Join(", ", subCommandProperties.Select(p => p.Name))}. " +
                        $"Properties with CmdParameterAttribute: {string.Join(", ", parameterProperties.Select(p => p.Name))}.");
                }
            }
            
            // Existing validation for default values
            var properties = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(true).Where(t => t is CmdParameterAttribute).Cast<CmdParameterAttribute>().Any(t => t.Default != null)).Select(p => new {Info = p, Attribute = p.GetCustomAttributes(true).Where(t => t is CmdParameterAttribute).Cast<CmdParameterAttribute>().Single()});
            foreach(var item in properties)
            {
                if (item.Info.PropertyType != typeof(DateTime))
                {
                    if (!item.Attribute.Multiple && item.Attribute.Default.GetType() != item.Info.PropertyType)
                    {
                        throw new DefaultValueTypeMismatchException($"Default type {item.Attribute.Default.GetType().Name} doesn't match for a property {item.Info.Name} that is {item.Info.GetType().Name} type.");
                    }
                    else if (!item.Attribute.Default.GetType().IsArray || item.Attribute.Default.GetType().GetElementType() != item.Info.PropertyType.GenericTypeArguments.First())
                    {
                        var collectionType = item.Info.PropertyType.GenericTypeArguments.First();
                        throw new DefaultValueTypeMismatchException($"Default type {item.Attribute.Default.GetType().Name} doesn't match for a property {item.Info.Name} that is List<{collectionType}> type.");
                    }
                }
            }
        }

        public static string Help()
        {
            var helpText = new System.Text.StringBuilder();
            var properties = typeof(T).GetProperties();
            
            // Check if class has only subcommands
            var subCommandEntries = new List<(string name, string description)>();
            var helpEntries = new List<(string name, string description, string type)>();
            
            foreach (var property in properties)
            {
                foreach (var attr in property.GetCustomAttributes(true))
                {
                    switch (attr)
                    {
                        case CmdSubCommandAttribute subCommand:
                            subCommandEntries.Add((subCommand.Name, subCommand.Description ?? ""));
                            break;
                        case CmdParameterAttribute parameter:
                            helpEntries.Add((parameter.Name, parameter.Description ?? "", "parameter"));
                            break;
                        case CmdFlagAttribute flag:
                            helpEntries.Add((flag.Name, flag.Description ?? "", "flag"));
                            break;
                    }
                }
            }
            
            // If class has only subcommands, display subcommands help
            if (subCommandEntries.Count > 0 && helpEntries.Count == 0)
            {
                helpText.AppendLine("Usage: <command> [options]");
                helpText.AppendLine();
                helpText.AppendLine("Available commands:");
                
                var maxNameLength = Math.Max(subCommandEntries.Max(e => e.Item1.Length), 4); // "Name" header
                var maxDescriptionLength = Math.Max(subCommandEntries.Max(e => e.Item2.Length), 11); // "Description" header
                
                // Header
                var nameHeader = "Name".PadRight(maxNameLength);
                var descHeader = "Description".PadRight(maxDescriptionLength);
                helpText.AppendLine($"{nameHeader} {descHeader}");
                helpText.AppendLine(new string('-', maxNameLength + maxDescriptionLength + 1));
                
                // Entries
                foreach (var entry in subCommandEntries)
                {
                    var nameEntry = entry.Item1.PadRight(maxNameLength);
                    var descEntry = entry.Item2.PadRight(maxDescriptionLength);
                    helpText.AppendLine($"{nameEntry} {descEntry}");
                }
            }
            else
            {
                // Display regular options help
                helpText.AppendLine("Usage: [options]");
                helpText.AppendLine();
                helpText.AppendLine("Options:");
                
                // Format as table
                if (helpEntries.Count > 0)
                {
                    var maxNameLength = Math.Max(helpEntries.Max(e => e.Item1.Length), 4); // "Name" header
                    var maxDescriptionLength = Math.Max(helpEntries.Max(e => e.Item2.Length), 11); // "Description" header
                    
                    // Header
                    var nameHeader = "Name".PadRight(maxNameLength);
                    var descHeader = "Description".PadRight(maxDescriptionLength);
                    helpText.AppendLine($"{nameHeader} {descHeader}");
                    helpText.AppendLine(new string('-', maxNameLength + maxDescriptionLength + 1));
                    
                    // Entries
                    foreach (var entry in helpEntries)
                    {
                        var nameEntry = entry.Item1.PadRight(maxNameLength);
                        var descEntry = entry.Item2.PadRight(maxDescriptionLength);
                        helpText.AppendLine($"{nameEntry} {descEntry}");
                    }
                }
            }
            
            return helpText.ToString();
        }

        public T Take()
        {
            Validate();
            
            // First, check for subcommands
            var subCommandProperties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttributes(true).OfType<CmdSubCommandAttribute>().Any())
                .ToList();
            
            if (subCommandProperties.Any() && _args.Count > 0)
            {
                var firstArg = _args[0];
                foreach (var property in subCommandProperties)
                {
                    var subCommandAttr = property.GetCustomAttributes(true)
                        .OfType<CmdSubCommandAttribute>()
                        .FirstOrDefault();
                    
                    if (subCommandAttr != null && subCommandAttr.Name == firstArg)
                    {
                        // Found matching subcommand
                        var subCommandType = property.PropertyType;
                        var subCommandInstance = Activator.CreateInstance(subCommandType);
                        
                        // Remove the subcommand name from args
                        var subCommandArgs = _args.Skip(1).ToArray();
                        
                        // Parse the subcommand's parameters
                        foreach (var subProp in subCommandType.GetProperties())
                        {
                            foreach (var subAttr in subProp.GetCustomAttributes(true))
                            {
                                if (subAttr is CmdParameterAttribute parameter)
                                {
                                    if (!parameter.Multiple)
                                    {
                                        try
                                        {
                                            object value = null;
                                            int index = Array.IndexOf(subCommandArgs, parameter.Name);
                                            if (index != -1 && index + 1 < subCommandArgs.Length)
                                                value = subCommandArgs[index + 1];
                                            else if (!parameter.Required && parameter.Default != null)
                                                value = parameter.Default;
                                            else if (parameter.Required)
                                                throw new RequiredAttributeException($"Parameter {parameter.Name} is required.");
                                            
                                            if (value != null)
                                                subProp.SetValue(subCommandInstance, Convert.ChangeType(value, subProp.PropertyType));
                                        }
                                        catch (ArgumentOutOfRangeException ex)
                                        {
                                            throw new ArgumentException($"Value wasn't set for a parameter {parameter.Name}.", ex);
                                        }
                                    }
                                }
                                else if (subAttr is CmdFlagAttribute flag)
                                {
                                    int index = Array.IndexOf(subCommandArgs, flag.Name);
                                    subProp.SetValue(subCommandInstance, index != -1);
                                }
                            }
                        }
                        
                        // Set the subcommand instance on the main instance
                        property.SetValue(_instance, subCommandInstance);
                        
                        // Return early since we processed a subcommand
                        return _instance;
                    }
                }
            }
            
            // If no subcommand matched, process regular parameters and flags
            foreach (var property in typeof(T).GetProperties())
            {
                foreach (var attr in property.GetCustomAttributes(true))
                {
                    switch (attr)
                    {
                        case CmdSubCommandAttribute:
                            // Skip subcommand properties in regular processing
                            break;
                            
                        case CmdParameterAttribute:
                            {
                                CmdParameterAttribute parameter = (CmdParameterAttribute)attr;
                                if (!parameter.Multiple)
                                {
                                    try
                                    {
                                        var instanceProp = _instance.GetType().GetProperty(property.Name);
                                        object value = null;
                                        int index = _args.IndexOf(parameter.Name);
                                        if (index != -1)
                                            value = _args[index + 1];
                                        else if (!parameter.Required && parameter.Default != null)
                                            value = parameter.Default;
                                        else if(parameter.Required)
                                            throw new RequiredAttributeException($"Parameter {parameter.Name} is required.");
                                        instanceProp.SetValue(_instance, Convert.ChangeType(value, instanceProp.PropertyType));
                                    }
                                    catch (ArgumentOutOfRangeException ex)
                                    {
                                        throw new ArgumentException($"Value wasn't set for a parameter {parameter.Name}.", ex);
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        var list = new List<string>(_args);
                                        var instance_prop = _instance.GetType().GetProperty(property.Name);
                                        var listType = typeof(List<>);
                                        var collectionType = instance_prop.PropertyType.GenericTypeArguments.First();
                                        var constructedListType = listType.MakeGenericType(collectionType);
                                        var value = (IList)Activator.CreateInstance(constructedListType);

                                        int index = list.IndexOf(parameter.Name);
                                        while (index != -1)
                                        {
                                            value.Add(Convert.ChangeType(list[index + 1], collectionType));
                                            list.RemoveAt(index + 1);
                                            list.RemoveAt(index);
                                            index = list.IndexOf(parameter.Name);
                                        }
                                        if (value.Count > 0)
                                            instance_prop.SetValue(_instance, value);
                                        else if (!parameter.Required && parameter.Default != null)
                                            instance_prop.SetValue(_instance, Activator.CreateInstance(constructedListType, parameter.Default));
                                        else if(parameter.Required)
                                            throw new ArgumentException($"Cannot find {parameter.Name}");
                                    }
                                    catch (ArgumentOutOfRangeException ex)
                                    {
                                        throw new ArgumentException($"Value wasn't set for a parameter {parameter.Name}.", ex);
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }
                                break;
                            }

                        case CmdFlagAttribute:
                            {
                                CmdFlagAttribute flag = attr as CmdFlagAttribute;
                                int index = _args.IndexOf(flag.Name);
                                var instance_prop = _instance.GetType().GetProperty(property.Name);
                                instance_prop.SetValue(_instance, index != -1);
                                break;
                            }
                    }
                }
            }

            return _instance;
        }
    }
}
