namespace WorkFlowStages.Dto
{
    public class WorkFlowDto
    {
        public int? template_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? sequence_number { get; set; }
        public int? parallel { get; set; }
        public int? dependent_on { get; set; }
        public string stage_type { get; set; }
    }
}
