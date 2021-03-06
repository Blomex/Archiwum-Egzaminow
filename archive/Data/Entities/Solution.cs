using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace archive.Data.Entities
{
    public class Solution
    {
        [Key]
        public int Id { get; set; }

        public string AuthorId { get; set; }
        /* Author is not necessary, and making it required would cause problems with migrations. */
        public virtual ApplicationUser Author { get; set; }

        public string CachedContent { get; set; }

        public virtual IEnumerable<SolutionVersion> Versions { get; set; }
        public virtual SolutionVersion CurrentVersion { get; set; }

        public int TaskId { get; set; }
        
        public virtual Task Task { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public virtual ICollection<SolutionsFiles> Attachments { get; set; } = new HashSet<SolutionsFiles>();
    }
}