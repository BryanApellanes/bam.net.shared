/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.CommandLine;
using Bam.Net.Configuration;

namespace Bam.Net.Automation
{
    /// <summary>
    /// Work done as a command line process
    /// </summary>
    public class ProcessWorker: Worker, IHasRequiredProperties
    {
        public ProcessWorker() : base() { }
        public ProcessWorker(string name) : base(name) { }
        public ProcessWorker(string name, string commandName, string arguments)
            : base(name)
        {
            CommandName = commandName;
            Arguments = arguments;
        }

        /// <summary>
        /// The name or full path to the command to execute.
        /// </summary>
        public virtual string CommandName { get; set; }
        public virtual string Arguments { get; set; }
        public virtual string WorkingDirectory { get; set; }
        
        public override string[] RequiredProperties => new string[] { "Name", "CommandLine" };

        protected override WorkState Do(WorkState currentWorkState)
        {
            this.CheckRequiredProperties();
            
            string startDir = Environment.CurrentDirectory;
            if (!string.IsNullOrEmpty(WorkingDirectory))
            {
                Environment.CurrentDirectory = WorkingDirectory;
            }
            ProcessOutput processOutput = CommandName.Start(Arguments);
            var result = CreateNextWorkState(currentWorkState, processOutput, GetWorkStateMessage(currentWorkState, processOutput));
            Environment.CurrentDirectory = startDir;
            return result;
        }

        protected WorkState<ProcessOutput> CreateNextWorkState(WorkState currentWorkState, ProcessOutput processOutput, string message)
        {
            WorkState<ProcessOutput> result = new WorkState<ProcessOutput>(this, processOutput)
            {
                Message = message,
                PreviousWorkState = currentWorkState
            };
            return result;
        }

        protected virtual string GetWorkStateMessage(WorkState currentWorkState, ProcessOutput processOutput)
        {
            return $"'{CommandName} {Arguments}' exited with code {processOutput.ExitCode}";
        }
    }
}
