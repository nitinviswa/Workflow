using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkFlowStages.Model
{
    public class FormField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public Dictionary<string, ValidationRule> Validations { get; set; }
        public bool Required { get; set; }
        public string Regex { get; set; }

    }

    public class ValidationRule
    {
        public bool Enabled { get; set; }
        public object Value { get; set; }
        public string Message { get; set; }
    }

    public class Rootobject
    {
        public Form form { get; set; }
    }

    public class Form
    {
        public FormField[] fields { get; set; }
    }

    public class Customer
    {
        public int Age { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
