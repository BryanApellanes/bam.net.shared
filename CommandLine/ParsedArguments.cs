/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bam.Net.CommandLine
{
    public enum ArgumentParseStatus
    {
        Invalid,
        Success,
        Error
    }

    /// <summary>
    /// Class used to parse command line arguments.  All arguments are 
    /// assumed to be in the format /&lt;name&gt;:&lt;value&gt; or an ArgumentException is thrown 
    /// during parsing.
    /// </summary>
    public class ParsedArguments
    {
        public const string DefaultArgPrefix = "/";

        public ParsedArguments(string[] args, string[] validArgNames)
            : this(args, ArgumentInfo.FromStringArray(validArgNames))
        {
        }

        public ParsedArguments(string[] args, ArgumentInfo[] validArgumentInfos) : this(DefaultArgPrefix, args, validArgumentInfos)
        {
        }

        /// <summary>
        /// Instantiate a ParsedArguments from the command line args specified to the current process.
        /// </summary>
        public ParsedArguments() : this(Environment.GetCommandLineArgs())
        {
        }
        
        public ParsedArguments(string[] args) : this(DefaultArgPrefix, args, ArgumentInfo.FromArgs(args))
        {
        }
        
        public ParsedArguments(string argPrefix, string[] args, ArgumentInfo[] validArgumentInfos)
        {
            this.OriginalStrings = args;
            this.parsedArguments = new Dictionary<string, string>();
            if (args.Length == 0)
            {
                this.Status = ArgumentParseStatus.Success;
                this.Message = "No arguments";
                return;
            }
            ArgumentInfoHash validArguments = new ArgumentInfoHash(validArgumentInfos);
            
            foreach (string argument in args)
            {
                string arg = argument.Trim();

                if (!arg.StartsWith(argPrefix) || !(arg.Length > 1))
                {
                    Console.WriteLine($"attach debugger: {Process.GetCurrentProcess().Id}");
                    Console.ReadLine();
                    Message = $"Unrecognized argument format: {arg}\r\n\r\nAll Args:\r\n{string.Join("\r\n", args)}";
                    Status = ArgumentParseStatus.Error;
                }
                else
                {
                    string[] nameValue = arg.Substring(1, arg.Length - 1).Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    string name = string.Empty;
                    if (nameValue.Length > 0)
                    {
                        name = nameValue[0];
                    }

                    // allow ":" in arg value
                    if (nameValue.Length > 2)
                    {
                        int startIndex = arg.IndexOf(":") + 1;
                        nameValue = new string[] { name, arg.Substring(startIndex, arg.Length - startIndex) };
                    }

                    if (nameValue.Length == 1 && validArguments[name] != null)
                    {
						if (validArguments[name].AllowNullValue)
						{
							parsedArguments.Add(name, "");
						}
						else
						{
							Message = "No value specified for " + name;
							Status = ArgumentParseStatus.Error;
						}
                    }

                    if (nameValue.Length == 2)
                    {
                        if (validArguments[name] == null)
                        {
                            Message = "Invalid argument name specified: " + name;
                            Status = ArgumentParseStatus.Error;
                        }
                        else
                        {
                            if (parsedArguments.ContainsKey(name))
                            {
                                parsedArguments[name] = nameValue[1];
                            }
                            else
                            {
                                parsedArguments.Add(name, nameValue[1]);
                            }
                        }
                    }

                }
            }

            if (Status != ArgumentParseStatus.Error)
            {
                Status = ArgumentParseStatus.Success;
            }
        }

		public void EnsureArgumentValue(string argument, string message = "Required argument value not specified")
		{
			EnsureArgument(argument);
			Args.ThrowIf(string.IsNullOrEmpty(this[argument]), message);
		}

		public void EnsureArgument(string argument, string message = "Required argument not specified")
		{
			Args.ThrowIf(!Contains(argument), message);
		}

        public bool Contains(string argumentToLookFor)
        {
            return parsedArguments.ContainsKey(argumentToLookFor);
        }

        public string Message { get; set; }
        public ArgumentParseStatus Status { get; set; }
        public string[] OriginalStrings { get; private set; }
        readonly Dictionary<string, string> parsedArguments;
        public string this[string name]
        {
            get => parsedArguments.ContainsKey(name) ? parsedArguments[name] : null;
            set => parsedArguments[name] = value;
        }

        public string[] Keys => parsedArguments.Keys.ToArray();

        public int Length => parsedArguments.Count;

        private static ParsedArguments _current;
        static readonly object _currentLock = new object();
        public static ParsedArguments Current => _currentLock.DoubleCheckLock(ref _current, () => new ParsedArguments());
    }
}
