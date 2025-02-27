using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace SNSReceiverApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SNSController(IAmazonSimpleNotificationService snsService, ILogger<SNSController> logger) : ControllerBase
    {
        public static string SNS_TOPIC_ARN = Environment.GetEnvironmentVariable("SNS_TOPIC_ARN") ?? throw new ArgumentNullException("SNS_TOPIC_ARN is not set");

        [HttpGet("publish")]
        public async Task<ActionResult> PublishMessage(MessageDto request)
        {
            try
            {
                var response = await snsService.PublishAsync(SNS_TOPIC_ARN, JsonConvert.SerializeObject(request));
                logger.LogInformation(JsonConvert.SerializeObject(response?.ResponseMetadata));
                return Ok(response?.ResponseMetadata);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    public class MessageDto
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
    }
}
