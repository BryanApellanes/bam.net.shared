/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Web
{
    /// <summary>
    /// An alias for a proxied service class, typically
    /// the name of the client side javascript variable
    /// </summary>
    public class ProxyAlias
    {
        public ProxyAlias() { }

        public ProxyAlias(string alias, Type typeToAlias)
        {
            this.Alias = alias;
			this.Type = typeToAlias;
        }

        public string Alias { get; set; }
        
		public string ClassName 
		{
			get => Type.Name;
		}

		public Type Type
		{
			get;
			set;
		}

		public override string ToString()
		{
			return $"{Alias}.{ClassName}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ProxyAlias alias = obj as ProxyAlias;
			if (alias != null)
			{
				return alias.ToString().Equals(this.ToString());
			}
			else
			{
				return base.Equals(obj);
			}
		}
    }
}
