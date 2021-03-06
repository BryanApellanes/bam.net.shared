using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.CoreServices.WebHooks.Data.Dao
{
    public class WebHookDescriptorWebHookSubscriberCollection: DaoCollection<WebHookDescriptorWebHookSubscriberColumns, WebHookDescriptorWebHookSubscriber>
    { 
		public WebHookDescriptorWebHookSubscriberCollection(){}
		public WebHookDescriptorWebHookSubscriberCollection(Database db, DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public WebHookDescriptorWebHookSubscriberCollection(DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(table, dao, rc) { }
		public WebHookDescriptorWebHookSubscriberCollection(Query<WebHookDescriptorWebHookSubscriberColumns, WebHookDescriptorWebHookSubscriber> q, Bam.Net.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public WebHookDescriptorWebHookSubscriberCollection(Database db, Query<WebHookDescriptorWebHookSubscriberColumns, WebHookDescriptorWebHookSubscriber> q, bool load) : base(db, q, load) { }
		public WebHookDescriptorWebHookSubscriberCollection(Query<WebHookDescriptorWebHookSubscriberColumns, WebHookDescriptorWebHookSubscriber> q, bool load) : base(q, load) { }
    }
}