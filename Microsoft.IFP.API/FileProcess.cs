using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml;

namespace Microsoft.IFP.API
{
    public class FileProcess
    {
        private readonly ILogger<FileProcess> _logger;

        public FileProcess(ILogger<FileProcess> logger)
        {
            _logger = logger;
        }

        [Function("FileProcess")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            return new OkObjectResult(ConvertCxmlToJson(requestBody));
        }

        public string ConvertCxmlToJson(string cXMl)
        {

            XmlReaderSettings settings = new()
            {
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 1024
            };

            using (XmlReader reader = XmlReader.Create(new StringReader(cXMl), settings))
            {
                XmlDocument doc = new XmlDocument();
                
                doc.Load(reader);

                return JsonConvert.SerializeXmlNode(doc.SelectSingleNode("cXML") ?? doc);
            }
        }

    }
}
