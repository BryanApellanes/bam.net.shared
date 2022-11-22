/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ClientKeySetPagedQuery: PagedQuery<ClientKeySetColumns, ClientKeySet>
    { 
		public ClientKeySetPagedQuery(ClientKeySetColumns orderByColumn,ClientKeySetQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}