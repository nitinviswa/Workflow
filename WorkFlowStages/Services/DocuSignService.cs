using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace WorkFlowStages.Services
{
    public class DocuSignService
    {
        private readonly IConfiguration _configuration;

        public DocuSignService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAccessToken()
        {
            var apiClient = new ApiClient($"https://{_configuration["DocuSign:OAuthBasePath"]}");

            // Read the entire private key file content
            string privateKeyContent = File.ReadAllText(_configuration["DocuSign:PrivateKeyFile"]);

            // Remove headers, footers, and new lines to get the Base64 content
            string header = "-----BEGIN RSA PRIVATE KEY-----";
            string footer = "-----END RSA PRIVATE KEY-----";
            string base64Content = privateKeyContent.Replace(header, "")
                                                    .Replace(footer, "")
                                                    .Replace("\r", "")
                                                    .Replace("\n", "")
                                                    .Trim();

            byte[] privateKeyBytes;
            try
            {
                // Convert the cleaned Base64 content to byte array
                privateKeyBytes = Convert.FromBase64String(base64Content);
            }
            catch (FormatException ex)
            {
                throw new Exception("Invalid Base-64 string in the private key.", ex);
            }

            OAuth.OAuthToken oAuthToken;
            try
            {
                // Request JWT User Token
                oAuthToken = apiClient.RequestJWTUserToken(
                    _configuration["DocuSign:IntegrationKey"],
                    _configuration["DocuSign:UserId"],
                    "account-d.docusign.com",
                    privateKeyBytes,
                    1
                );
            }
            catch (ApiException ex)
            {
                throw new Exception("Error while requesting JWT user token.", ex);
            }

            return oAuthToken.access_token;
        }

        public EnvelopeSummary CreateEnvelope(string signerEmail, string signerName, string documentBase64, string documentName)
        {
            var apiClient = new ApiClient($"https://{_configuration["DocuSign:OAuthBasePath"]}");
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + GetAccessToken());

            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document",
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = documentBase64,
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
                                        XPosition = "100",
                                        YPosition = "150"
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