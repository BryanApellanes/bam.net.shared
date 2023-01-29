/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Data;
using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server
{
    /// <summary>
    /// An abstract base class used to subscribe special handlers
    /// to server events
    /// </summary>
    public abstract class BamServerEventListener
    {
        // consider generating this file: see ...Server.Tests.ConsoleActions.ListServerEventsAndTypes
        public virtual void Initializing(BamAppServer bryanAppServer) { }

        public virtual void Initialized(BamAppServer bryanAppServer) { }

        public virtual void SchemaInitializing(BamAppServer bryanAppServer, SchemaInitializer schemaInitializer) { }

        public virtual void SchemaInitialized(BamAppServer bryanAppServer, SchemaInitializer schemaInitializer) { }

        public virtual void SchemasInitializing(BamAppServer bryanAppServer) { }

        public virtual void SchemasInitialized(BamAppServer bryanAppServer) { }

        public virtual void LoadingConf(BamAppServer bryanAppServer, BamConf bryanConf) { }

        public virtual void LoadedConf(BamAppServer bryanAppServer, BamConf bryanConf) { }

        public virtual void CreatingApp(BamAppServer bryanAppServer, AppConf appConf) { }

        public virtual void CreatedApp(BamAppServer bryanAppServer, AppConf appConf) { }

        public virtual void Responded(BamAppServer bryanAppServer, IHttpResponder iResponder, IRequest iRequest) { }
        
        public virtual void NotResponded(BamAppServer bryanAppServer, IRequest iRequest) { }

        public virtual void ResponderAdded(BamAppServer bryanAppServer, IHttpResponder iResponder) { }

        public virtual void SettingConf(BamAppServer bryanAppServer, BamConf bryanConf) { }

        public virtual void SettedConf(BamAppServer bryanAppServer, BamConf bryanConf) { }

        public virtual void SavedConf(BamAppServer bryanAppServer, BamConf bryanConf) { }

        public virtual void Starting(BamAppServer bryanAppServer) { }

        public virtual void Started(BamAppServer bryanAppServer) { }

        public virtual void Stopping(BamAppServer bryanAppServer) { }

        public virtual void Stopped(BamAppServer bryanAppServer) { }


    }
}
