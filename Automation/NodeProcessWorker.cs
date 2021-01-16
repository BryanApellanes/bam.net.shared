using Bam.Net.CommandLine;
using Bam.Net.Configuration;
using Bam.Net.Logging;
using EcmaScript.NET;
using UAParser;

namespace Bam.Net.Automation
{
    public class NodeProcessWorker : ProcessWorker, IHasRequiredProperties
    {
        public NodeProcessWorker()
        {
            NodePath = OSInfo.GetPath("node");
        }
        
        public string NodePath { get; }

        /// <summary>
        /// The name or full path to the command to execute.
        /// </summary>
        public override string CommandName
        {
            get => NodePath;
            set => Log.Warn("NodeProcessWorker.CommandName is read only, use ScriptArguments instead.");
        }

        public override string Arguments
        {
            get => string.Join(" ", new string[] {Script, ScriptArguments});
            set => Log.Warn("NodeProcessWorker.Arguments is read only, use ScriptArguments instead.");
        }

        /// <summary>
        /// Gets or sets the script argument passed to NodeJs.
        /// </summary>
        public string Script { get; set; }
        
        /// <summary>
        /// Gets or sets the arguments intended for the script.
        /// </summary>
        public string ScriptArguments { get; set; }
        
        protected override WorkState Do(WorkState currentWorkState)
        {
            this.CheckRequiredProperties();

            
            ProcessOutput output = NodePath.Start(string.Join(" ", new string[] {Script, ScriptArguments}));
            WorkState<ProcessOutput> result = new WorkState<ProcessOutput>(this, output)
            {
                Message = $"Node Script:({Script} {ScriptArguments}) exited with code {output.ExitCode}",
                PreviousWorkState = currentWorkState
            };
            return result;
        }

        public override string[] RequiredProperties => new string[] { "NodePath", "Script" };

        protected override string GetWorkStateMessage(WorkState currentWorkState, ProcessOutput processOutput)
        {
            return $"Node Script:({Script} {ScriptArguments}) exited with code {processOutput?.ExitCode}";
        }
    }
}