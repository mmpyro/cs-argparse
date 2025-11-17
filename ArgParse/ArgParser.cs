using ArgParse.Attributes;
using ArgParse.Exceptions;
using System.Collections;

namespace ArgParse
{
    public class ArgParser<T> where T : class
    {
        private readonly List<string> _args;
        private readonly T _instance;

        public ArgParser(string[] args)
        {
            _args = [.. args];
            _instance = (T)Activator.CreateInstance(typeof(T));
        }

        private static void Validate()
        {
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
            //TODO: Implement it;
            throw new NotImplementedException();
        }

        public T Take()
        {
            Validate();
            foreach (var property in typeof(T).GetProperties())
            {
                foreach (var attr in property.GetCustomAttributes(true))
                {
                    switch (attr)
                    {
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
