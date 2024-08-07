using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using Microsoft.Extensions.Configuration;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkFlowStages.Dto;
using WorkFlowStages.Model;
using Envelope = WorkFlowStages.Model.Envelope;
using Microsoft.EntityFrameworkCore;

namespace WorkFlowStages.Services
{
    public class DocuSignService
    {
        private readonly IConfiguration _configuration;
        private readonly Data.AppContext _context;


        public DocuSignService(IConfiguration configuration, Data.AppContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private string GetAccessToken()
        {
            var privateKeyPath = _configuration["DocuSign:PrivateKeyFile"];
            var privateKeyBytes = File.ReadAllBytes(privateKeyPath);

            var clientId = _configuration["DocuSign:IntegrationKey"];
            var userId = _configuration["DocuSign:UserId"];
            var authServer = _configuration["DocuSign:OAuthBasePath"];
            var expiresInHours = int.Parse(_configuration["DocuSign:ExpiresInHours"]);
            var scopes = new List<string> { "signature", "impersonation" };

            var docuSignClient = new ApiClient(authServer);
            OAuth.OAuthToken authToken = docuSignClient.RequestJWTUserToken(clientId, userId, authServer, privateKeyBytes, expiresInHours, scopes);

            string accessToken = authToken.access_token;
            docuSignClient.SetOAuthBasePath(authServer);
            OAuth.UserInfo userInfo = docuSignClient.GetUserInfo(authToken.access_token);

            var account = userInfo.Accounts.FirstOrDefault();
            if (account == null)
            {
                throw new Exception("No DocuSign account found for the user.");
            }

            return accessToken;
        }

        public async Task<ResponceModel<Envelope>> SaveDocuSignWithSignatureImageAsync()
        {
            try
            {
                var apiClient = new ApiClient("https://demo.docusign.net/restapi");
                apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + GetAccessToken());


                var envelopeDefinition = new EnvelopeDefinition
                {
                    EmailSubject = "Please sign this document",
                    Documents = new List<Document>
                         {
                             new Document
                             {
                                 DocumentBase64 = Convert.ToBase64String(await File.ReadAllBytesAsync("C:\\Users\\a\\Downloads\\Test 2.pdf")),
                                 Name = "Test Document",
                                 FileExtension = "pdf",
                                 DocumentId = "1"
                             }
                         },
                    Recipients = new Recipients
                    {
                        Signers = new List<Signer>
                        {
                                 new Signer
                                 {
                                     Email = "nitin.vishvakarma@enablistar.com",
                                     Name = "Nitin",
                                     RecipientId = "1",
                                     Tabs = new Tabs
                                     {
                                         SignHereTabs = new List<SignHere>
                                         {
                                             new  SignHere
                                             {
                                                 DocumentId = "1",
                                                 PageNumber = "1",
                                                 XPosition = "445",
                                                 YPosition = "792",
                                                 ScaleValue = "0.5"
                                             }
                                         }
                                     }
                                 }
                        }
                    },
                    Status = "sent"
                };

                EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
                var envelopeSummary = await envelopesApi.CreateEnvelopeAsync(_configuration["DocuSign:AccountId"], envelopeDefinition);

                var envelope = new Envelope
                {
                    EnvelopeId = envelopeSummary.EnvelopeId,
                    Status = envelopeSummary.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _context.envelopes.Add(envelope);
                await _context.SaveChangesAsync();

                return new ResponceModel<Envelope>
                {
                    Data = envelope,
                    Message = "Envelope with signature image sent and saved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponceModel<Envelope>
                {
                    Data = null,
                    Message = $"Error: {ex.Message}"
                };
            }
        }


        private void AddSignatureImageToPdf(string inputPdfPath, string outputPdfPath, string imagePath, int xPosition, int yPosition)
        {
            using (PdfDocument document = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify))
            {
                PdfPage page = document.Pages[0]; // Assuming the signature goes on the first page
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XImage image = XImage.FromFile(imagePath);

                gfx.DrawImage(image, xPosition, yPosition);
                document.Save(outputPdfPath);
            }
        }

        public async Task<ResponceModel<List<Envelope>>> GetDocuSignAsync()
        {
            try
            {
                var envelopes = await _context.envelopes.ToListAsync();
                return new ResponceModel<List<Envelope>>
                {
                    Data = envelopes,
                    Message = "Envelopes retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponceModel<List<Envelope>>
                {
                    Data = null,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<EnvelopeSummary> CreateEnvelopeAsync(string signerEmail, string signerName, string documentName)
        {

            var apiClient = new ApiClient("https://demo.docusign.net/restapi");
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + GetAccessToken());

            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document",
                Documents = new List<Document>
                   {
                       new Document
                       {
                           DocumentBase64 = Convert.ToBase64String(await File.ReadAllBytesAsync("C:\\Users\\a\\Downloads\\" + documentName)),
                           Name = documentName,
                           FileExtension = "pdf",
                           DocumentId = "1"
                       }
                   },
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                       {
                           new Signer
                           {
                               Email = signerEmail,
                               Name = signerName,
                               RecipientId = "1",
                               RoutingOrder = "1",
                               Tabs = new Tabs
                               {
                                   SignHereTabs = new List<SignHere>
                                   {
                                       new SignHere
                                       {
                                             DocumentId = "1",
                                      PageNumber = "1",
                                      XPosition = "445",
                                      YPosition = "792",
                                      ScaleValue = "0.5"
                                       }
                                   }
                               }
                           }
                       }
                },
                Status = "sent"
            };

            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(_configuration["DocuSign:AccountId"], envelopeDefinition);
            return envelopeSummary;
        }
    }
}
