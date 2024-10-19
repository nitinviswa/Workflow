using Microsoft.AspNetCore.Mvc;
using WorkFlowStages.Services;
using WorkFlowStages.Model;
using Microsoft.Extensions.Configuration;
using System.Text;
using DocuSign.Core.Client;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;

namespace WorkFlowStages.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocuSignController : ControllerBase
    {
        private readonly DocuSignService _docuSignService;
        private readonly IConfiguration _configuration;

        public DocuSignController(DocuSignService docuSignService, IConfiguration configuration)
        {
            _docuSignService = docuSignService;
            _configuration = configuration;
        }

        [HttpPost("SendEnvelope")]
        public IActionResult SendEnvelope([FromBody] ESignRequest request)
        {
            try
            {
                var envelopeSummary = _docuSignService.CreateEnvelopeAsync(request.SignerEmail, request.SignerName, request.DocumentName);
                return Ok(envelopeSummary);
                //For test second branch..
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetToken()
        {
            //for test second branch...
            string token = string.Empty;
            var apiClient = new DocuSignClient("https://demo.docusign.net/restapi");
            var rsaKey = System.IO.File.ReadAllBytes("./private.key");
            var scope = new List<string>
            {
                "signature","impersonation"
            };

            OAuth.OAuthToken authToken = apiClient.RequestJWTUserToken("46d57c69-cc2f-48c0-9fdd-75ca4b31b411",
                "5952e3f5-6cdb-442f-83eb-a77b8b35af20",
                "account-d.docusign.com",
                rsaKey,
                3600,scope);

            token = authToken.access_token;
            return Ok(token);
        }

        [HttpPost("send-envelope")]
        public async Task<IActionResult> SendEnvelope()
        {
            var result = await _docuSignService.SaveDocuSignWithSignatureImageAsync();

            if (result.Message.Equals("Envelope with signature image sent and saved successfully"))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("envelopes")]
        public async Task<IActionResult> GetEnvelopes()
        {
            var result = await _docuSignService.GetDocuSignAsync();

            if (result.Message.Equals("Envelopes retrieved successfully"))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}    

