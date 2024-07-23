using System.ComponentModel.DataAnnotations;

namespace WorkFlowStages.Model
{
    public class Envelope
    {
        [Key]
        public int Id { get; set; }
        public string EnvelopeId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
