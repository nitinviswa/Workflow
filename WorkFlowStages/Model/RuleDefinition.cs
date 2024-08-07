namespace WorkFlowStages.Model
{
    public class RuleSet
    {
        public List<RuleDefinition> Rules { get; set; }
    }

    public class RuleDefinition
    {
        public string Name { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<ActionDefinition> Actions { get; set; }
    }

    public class Condition
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }

    public class ActionDefinition
    {
        public string Message { get; set; }
    }

}
