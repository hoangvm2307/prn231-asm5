using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChildTracking.Repositories.Models;
using ChildTracking.SoapService.MVC.Models;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace ChildTracking.SoapService.MVC.Services
{
    public class ChildSoapClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl;

        public ChildSoapClient(IHttpClientFactory httpClientFactory, string endpointUrl)
        {
            _httpClient = httpClientFactory.CreateClient();
            _endpointUrl = endpointUrl;
        }

        public async Task<List<Child>> GetAllChildrenAsync()
        {
            var soapEnvelope = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
           <soapenv:Header/>
           <soapenv:Body>
              <tem:GetAll/>
           </soapenv:Body>
        </soapenv:Envelope>";

            var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };

            request.Headers.Add("SOAPAction", "http://tempuri.org/IChildSoapService/GetAll");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Parse XML and convert to Child objects
            return ParseChildrenFromSoapResponse(responseString);
        }

        private List<Child> ParseChildrenFromSoapResponse(string soapResponse)
        {
            var children = new List<Child>();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsManager.AddNamespace("a", "http://schemas.datacontract.org/2004/07/ChildTracking.Repositories.Models");

            var childNodes = xmlDoc.SelectNodes("//a:Child", nsManager);

            if (childNodes != null)
            {
                foreach (XmlNode node in childNodes)
                {
                    try
                    {
                        var child = new Child
                        {
                            ChildId = Guid.Parse(GetNodeValue(node, "a:ChildId", nsManager)),
                            FullName = GetNodeValue(node, "a:FullName", nsManager),
                            DateOfBirth = DateTime.Parse(GetNodeValue(node, "a:DateOfBirth", nsManager)),
                            Gender = GetNodeValue(node, "a:Gender", nsManager),
                            BloodType = GetNodeValue(node, "a:BloodType", nsManager),
                            MedicalConditions = GetNodeValue(node, "a:MedicalConditions", nsManager),
                            Allergies = GetNodeValue(node, "a:Allergies", nsManager),
                            Notes = GetNodeValue(node, "a:Notes", nsManager)
                        };

                        children.Add(child);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing child: {ex.Message}");
                    }
                }
            }

            return children;
        }

        private string GetNodeValue(XmlNode parentNode, string xpath, XmlNamespaceManager nsManager)
        {
            var node = parentNode.SelectSingleNode(xpath, nsManager);
            return node?.InnerText ?? string.Empty;
        }
    }
}