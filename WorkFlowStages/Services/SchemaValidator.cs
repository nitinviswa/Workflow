using Newtonsoft.Json;
using System.Globalization;
using WorkFlowStages.Dto;
namespace WorkFlowStages.Services
{
        public class SchemaValidator
        {
        private readonly FormConfig _formConfig;

        public SchemaValidator()
        {
            _formConfig = GetFormConfigAsync().GetAwaiter().GetResult();
        }

        private async Task<FormConfig> GetFormConfigAsync()
        {
            var formConfigEntity = File.ReadAllText("./validation.json");
            return formConfigEntity != null
                ? JsonConvert.DeserializeObject<FormConfig>(formConfigEntity)
                : null;
        }

        public bool Validate(Dictionary<string, string> formData, out List<string> errors)
        {
            errors = new List<string>();

            foreach (var field in _formConfig.Form.Fields)
            {
                if (formData.TryGetValue(field.Name, out var value))
                {
                    // Validate Required
                    if (field.Validations.Required.Enabled && string.IsNullOrWhiteSpace(value))
                    {
                        errors.Add(field.Validations.Required.Message);
                    }

                    // Validate MaxLength
                    if (field.Validations.MaxLength != null &&  field.Validations.MaxLength.Enabled && value.Length > field.Validations.MaxLength.Value)
                    {
                        errors.Add(field.Validations.MaxLength.Message);
                    }

                    // Validate DateFormat
                    if (field.Type == "date" && field.Validations.DateFormat.Enabled && !DateTime.TryParseExact(value, field.Validations.DateFormat.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        errors.Add(field.Validations.DateFormat.Message);
                    }
                }
                else if (field.Validations.Required.Enabled)
                {
                    // Field is required but not provided
                    errors.Add(field.Validations.Required.Message);
                }
            }
            return !errors.Any();
        }
    }
}
