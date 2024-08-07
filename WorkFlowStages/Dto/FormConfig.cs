namespace WorkFlowStages.Dto
{
    public class FormConfig
    {
        public FormDefinition Form { get; set; }

        public class FormDefinition
        {
            public List<FieldDefinition> Fields { get; set; }
        }

        public class FieldDefinition
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Label { get; set; }
            public ValidationDefinition Validations { get; set; }
        }

        public class ValidationDefinition
        {
            public RequiredValidation Required { get; set; }
            public MaxLengthValidation MaxLength { get; set; }
            public DateFormatValidation DateFormat { get; set; }
        }

        public class RequiredValidation
        {
            public bool Enabled { get; set; }
            public bool Value { get; set; }
            public string Message { get; set; }
        }

        public class MaxLengthValidation
        {
            public bool Enabled { get; set; }
            public int Value { get; set; }
            public string Message { get; set; }
        }

        public class DateFormatValidation
        {
            public bool Enabled { get; set; }
            public string Value { get; set; }
            public string Message { get; set; }
        }
    }

}
