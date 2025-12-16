using ContentVersionsPOC.Data.Enums;

namespace ContentVersionsPOC.Data.Models
{
    public class LanguageBranch
    {
        public Guid ContentId { get; set; }
        public ContentRoot ContentRoot { get; set; }
        public LanguageBranchEnum LanguageBranchEnum { get; set; } // Part of Composite Key

        // The current "Live" version pointer
        public Guid ActiveVersionId { get; set; }
        public virtual ContentVersion ActiveVersion { get; set; }

        // NEW: The collection of historical versions for ONLY this language
        public virtual ICollection<ContentVersion> Versions { get; set; } = new List<ContentVersion>();
    }
}
