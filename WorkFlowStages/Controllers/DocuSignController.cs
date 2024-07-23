using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkFlowStages.Model;
using WorkFlowStages.Services;

namespace WorkFlowStages.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class DocuSignController : ControllerBase
    {
        private readonly DocuSignService _docuSignService;
        private readonly string _docFilePath = "C:\\Users\\a\\source\\repos\\WorkFlowStages\\WorkFlowStages\\Attachment\\World_Wide_Corp_salary.docx";
        
        public DocuSignController(IConfiguration configuration)
        {
            _docuSignService = new DocuSignService(configuration);
        }

        [HttpPost("send")]
        public IActionResult SendEnvelope([FromBody] ESignRequest request)
        {
            string documentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(_docFilePath));
            var envelopeSummary = _docuSignService.CreateEnvelope(request.SignerEmail, request.SignerName, documentBase64, request.DocumentName);
            return Ok(envelopeSummary);
        }
    }
}
