namespace Bam.Net.Automation.SourceControl
{
    public class GitChangeLog
    {
        public GitChangeLog(string introduction = null, string version = "0.0.1")
        {
            this.Introduction = introduction;
            this.Features = new GitChangeLogSection(version, "Features");
            this.Updates = new GitChangeLogSection(version, "Updates");
            this.Additions = new GitChangeLogSection(version, "Additions");
            this.Fixes = new GitChangeLogSection(version, "Fixes");
        }
        
        /// <summary>
        /// Gets or sets the introduction paragraph for this change log.
        /// </summary>
        public string Introduction { get; set; }
        
        public GitChangeLogSection Features { get; set; }
        
        public GitChangeLogSection Updates { get; set; }
        
        public GitChangeLogSection Additions { get; set; }
        
        public GitChangeLogSection Fixes { get; set; }
    }
}