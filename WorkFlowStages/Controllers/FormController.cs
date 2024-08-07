using Microsoft.AspNetCore.Mvc;
using WorkFlowStages.Services;
using WorkFlowStages.Dto;
using System.Threading.Tasks;
using System.Linq;
using WorkFlowStages.Model;
using System.Text.Json;
using Newtonsoft.Json;


namespace WorkFlowStages.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly SchemaValidator _schemaValidator;
        private readonly ValidationService _validationService;

        public FormController(SchemaValidator schemaValidator, ValidationService validationService)
        {
            _schemaValidator = schemaValidator;
            _validationService = validationService;
        }

        [HttpPost]
        [Route("validateForm")]
        public ActionResult<List<ValidationError>> Validate([FromBody] FormRequestDto formRequestDto)
        {
            // Load validation JSON
            string validationJson = System.IO.File.ReadAllText("./validation.json");
            Rootobject formFields = JsonConvert.DeserializeObject<Rootobject>(validationJson);

            var errors = _validationService.Validate(formRequestDto, formFields);
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            var customer = new Customer
            {
                Age = formRequestDto.Age,
                TotalIncome = formRequestDto.TotalIncome
            };

            /*_validationService.EvaluateBusinessRules(customer);
            bool isLoanEligible = _validationService.EvaluateDecisionTable(customer);
*/
           // return Ok(new { LoanEligible = isLoanEligible });
           return Ok(errors);
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] Dictionary<string, string> formData)
        {
            var isValid = _schemaValidator.Validate(formData, out var errors);
            if (!isValid)
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "Form data is valid" });
        }
    }
}