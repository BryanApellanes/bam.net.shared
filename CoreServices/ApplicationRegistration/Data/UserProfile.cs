using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.CoreServices.ApplicationRegistration.Data
{
    [Serializable]
    public class UserProfile : KeyedAuditRepoData
    {
        public ulong UserId { get; set; }
        public virtual User User { get; set; }

        public virtual List<UserSetting> UserSettings { get; set; }
    }
}
