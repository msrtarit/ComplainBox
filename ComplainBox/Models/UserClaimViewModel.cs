using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComplainBox.Models
{
    public class UserClaimViewModel
    {
        public string ClaimType { get; set; }
        public string Value { get; set; }
        public IList<SelectListItem> Options { get; set; }
        
    }
}