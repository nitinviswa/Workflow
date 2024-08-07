using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WorkFlowStages.Dto;
using WorkFlowStages.Model;

namespace WorkFlowStages.Services
{

    public class Validator
    {
        private readonly List<FormField> _formFields;
        private readonly Dictionary<string, string> _payload;

        public Validator(List<FormField> formFields, Dictionary<string, string> payload)
        {
            _formFields = formFields;
            _payload = payload;
        }

        public List<ValidationError> Validate()
        {
            var errors = new List<ValidationError>();

            foreach (var field in _formFields)
            {
                if (_payload.TryGetValue(field.Name, out var value))
                {
                    foreach (var validation in field.Validations)
                    {
                        if (validation.Value.Enabled)
                        {
                            switch (validation.Key)
                            {
                                case "required":
                                    if (validation.Value.Value is bool isRequired && isRequired && string.IsNullOrEmpty(value))
                                    {
                                        errors.Add(new ValidationError { FieldName = field.Name, Message = validation.Value.Message });
                                    }
                                    break;

                                case "maxLength":
                                    if (validation.Value.Value is int maxLength && value.Length > maxLength)
                                    {
                                        errors.Add(new ValidationError { FieldName = field.Name, Message = validation.Value.Message });
                                    }
                                    break;

                                case "minValue":
                                    if (validation.Value.Value is int minValue && int.TryParse(value, out int intValue) && intValue < minValue)
                                    {
                                        errors.Add(new ValidationError { FieldName = field.Name, Message = validation.Value.Message });
                                    }
                                    else if (validation.Value.Value is decimal minDecimalValue && decimal.TryParse(value, out decimal decimalValue) && decimalValue < minDecimalValue)
                                    {
                                        errors.Add(new ValidationError { FieldName = field.Name, Message = validation.Value.Message });
                                    }
                                    break;

                                case "pattern":
                                    if (validation.Value.Value is string pattern && !Regex.IsMatch(value, pattern))
                                    {
                                        errors.Add(new ValidationError { FieldName = field.Name, Message = validation.Value.Message });
                                    }
                                    break;

                                case "dateFormat":
                                    if (validation.Value.Value is string dateFormat && !DateTime.TryParseExact(value, dateFormat, null, System.Globalization.DateTimeStyles.None, out _))
                                    {
                                        errors.Add(new ValidationError { FieldName = field.Name, Message = validation.Value.Message });
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            return errors;
        }
    }

}
