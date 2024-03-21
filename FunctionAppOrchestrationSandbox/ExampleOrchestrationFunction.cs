using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace FunctionAppOrchestrationSandbox
{
    public class ExampleOrchestrationFunction
    {
        private readonly ILogger<ExampleOrchestrationFunction> _logger;

        public ExampleOrchestrationFunction(ILogger<ExampleOrchestrationFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ExampleOrchestrationFunction_BeginOrchestration))]
        public async Task<string> ExampleOrchestrationFunction_BeginOrchestration(
            [OrchestrationTrigger] TaskOrchestrationContext context,
            CancellationToken cancellationToken = default)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(ExampleOrchestrationFunction));
            logger.LogInformation("Start processing.");

            var message = new ParentMessage
            {
                Message = "Parent message",
                Child = new ChildMessage
                {
                    Message = "Child message"
                }
            };

            var nextPageNumber = await context.CallActivityAsync<int?>(
                    nameof(ExampleActivity),
                    message
                );

            logger.LogInformation("Done processing.");
            return "Completed";
        }

        [Function(nameof(ExampleActivity))]
        public Task ExampleActivity(
            [ActivityTrigger] string message, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing message {message}.");
            return Task.CompletedTask;
        }

        [Function(nameof(ExampleOrchestrationFunction_HttpStart))]
        public async Task<HttpResponseData> ExampleOrchestrationFunction_HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("Function1_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ExampleOrchestrationFunction_BeginOrchestration));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }

    public class ParentMessage
    {
        public string Message { get; set; }
        public BaseMessage Child { get; set; }
    }

    [JsonDerivedType(typeof(ChildMessage), typeDiscriminator: (int)MessageTypeEnum.Child)]
    public abstract class BaseMessage
    {
        public abstract MessageTypeEnum MessageType { get; }
    }

    public class ChildMessage : BaseMessage
    {
        public override MessageTypeEnum MessageType => MessageTypeEnum.Child;
        public string Message { get; set; }
    }

    public enum MessageTypeEnum
    {
        Child
    }

}
