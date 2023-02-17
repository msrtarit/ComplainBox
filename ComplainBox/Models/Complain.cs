using System;

namespace ComplainBox.Models
{
    public class Complain
    {
        public Guid Id { get; set; }
        public string ComplainedBy { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string ComplainType { get; set; }
        public string ComplainTo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ComplainTime { get; set; }
        public bool IsResolved { get; set; }
        public string Reference { get; set; }
    }
}