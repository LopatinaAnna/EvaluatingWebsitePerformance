using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvaluatingWebsitePerformance.Data.Entities
{
    public class User
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public List<BaseRequest> Requests { get; set; } = new List<BaseRequest>();
    }
}