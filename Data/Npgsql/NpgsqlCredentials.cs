/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Data.Npgsql
{
    public class NpgsqlCredentials : DatabaseCredentials
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
