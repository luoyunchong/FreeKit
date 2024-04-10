using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace IGeekFan.Localization.FreeSql.Models
{
    public class LocalCulture
    {
        public LocalCulture()
        {
        }

        public LocalCulture(string name, string displayName, ICollection<LocalResource> resources) : this(name,
            displayName)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public LocalCulture(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        [Column(IsIdentity = true, IsPrimary = true)]
        [Required]
        public long Id { get; set; }

        [Column(StringLength = 50)] [Required] public string Name { get; set; }


        [Column(StringLength = 50)] [Required] public string DisplayName { get; set; }

        public virtual ICollection<LocalResource> Resources { get; set; }
    }
}