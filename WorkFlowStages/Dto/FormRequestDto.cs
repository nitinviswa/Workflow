using System.Text.Json.Serialization;

namespace WorkFlowStages.Dto
{
    public class FormRequestDto
    {
        public string FirstName { get; set; }
        public string Dob { get; set; } 
        public string Email { get; set; }
        public int Age { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
