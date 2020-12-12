using System.Collections.Generic;

namespace Bam.Net.Automation.SourceControl
{
    public class GitChangeLogSection
    {
        public GitChangeLogSection(string version, string name, string[] bullets = null)
        {
            this.Version = version;
            this.Name = name;
            this.Bullets = bullets;
        }
        
        public string Version { get; set; }
        public string Name { get; set; }
        public string[] Bullets { get; set; }

        public void AddBullet(string text)
        {
            Bullets = new List<string>(Bullets) {text}.ToArray();
        }
    }
}