using Bam.Net.Incubation;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// Represents a ClassName and MethodName referenced for execution.
    /// </summary>
    public class InvocationTargetInfo
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public static InvocationTargetInfo ResolveInvocationTarget(string path, Incubator serviceProvider, ProxyAlias[] proxyAliases)
        {
            InvocationTargetInfo result = new InvocationTargetInfo();

            Queue<string> split = new Queue<string>(path.DelimitSplit("/", "."));
            while (split.Count > 0)
            {
                string currentChunk = split.Dequeue();

                if (string.IsNullOrEmpty(result.ClassName))
                {
                    if (!serviceProvider.HasClass(currentChunk) && proxyAliases != null)
                    {
                        ProxyAlias alias = proxyAliases.FirstOrDefault(pa => pa.Alias.Equals(currentChunk));
                        if (alias != null)
                        {
                            result.ClassName = alias.ClassName;
                        }
                        else
                        {
                            result.ClassName = currentChunk;
                        }
                    }
                    else
                    {
                        result.ClassName = currentChunk;
                    }
                }
                else if (string.IsNullOrEmpty(result.MethodName))
                {
                    result.MethodName = currentChunk;
                }
            }

            return result;
        }
    }
}
