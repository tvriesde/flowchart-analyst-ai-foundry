using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;

#pragma warning disable SKEXP0110 // Suppress experimental API warnings for Agents

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get Azure OpenAI configuration from appsettings
var endpoint = configuration["AzureOpenAI:Endpoint"];
var deploymentName = configuration["AzureOpenAI:DeploymentName"];
var projectEndpoint = configuration["AzureAIProject:Endpoint"];
var reviewAgentId = configuration["AzureAIProject:ReviewAgentId"];
var visionAgentId = configuration["AzureAIProject:VisionAgentId"];

PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient(projectEndpoint, new AzureCliCredential());

//get Agents
PersistentAgent reviewAgentDefinition = await client.Administration.GetAgentAsync(reviewAgentId);
PersistentAgent visionAgentDefinition = await client.Administration.GetAgentAsync(visionAgentId);

AzureAIAgent reviewAgent = new(reviewAgentDefinition, client);
AzureAIAgent visionAgent = new(visionAgentDefinition, client);

if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName))
{
    Console.WriteLine("❌ Configuration Error: Please configure Azure OpenAI settings in appsettings.json");
    Console.WriteLine("Required settings:");
    Console.WriteLine("- AzureOpenAI:Endpoint");
    Console.WriteLine("- AzureOpenAI:DeploymentName");
    return;
}

Console.WriteLine("🤖 Semantic Kernel VisionAgent");
Console.WriteLine("===============================");
Console.WriteLine($"🔗 Using endpoint: {endpoint}");
Console.WriteLine($"🤖 Using deployment: {deploymentName}");

try
{
    // Create kernel with Azure OpenAI
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        new AzureCliCredential()); // Uses Azure CLI authentication

    var kernel = kernelBuilder.Build();

    var imagePath = "flowchart.png";    
    // Read image file
    var imageBytes = await File.ReadAllBytesAsync(imagePath);
    var mimeType = GetMimeType(Path.GetExtension(imagePath));

    // Create a chat for agent interaction
    AgentGroupChat chat =
                new(visionAgent, reviewAgent)
                { 
                    ExecutionSettings =
                        new()
                        {
                            // Use the custom ApprovalTerminationStrategy to stop when ReviewAgent says "APPROVED"
                            TerminationStrategy =
                                new ApprovalTerminationStrategy()
                                {
                                    // Only the review agent may approve.
                                    Agents = [reviewAgent],
                                    // Limit total number of turns
                                    MaximumIterations = 3,
                                }
                        },
                };

            // Add user message with image content to the chat
            var messageContent = new ChatMessageContentItemCollection
            {
                //new TextContent($"User Question: {question}"),
                new ImageContent(imageBytes, mimeType)
            };

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, messageContent));

            // Get agent's response
            Console.WriteLine("\n🤖 Group chat starting:");
            Console.WriteLine("========================");
            await foreach (var message in chat.InvokeAsync())
            {
                if (message.Role == AuthorRole.Assistant)
                {
                    Console.Write(message.Content);
                }
            }

            Console.WriteLine("\n========================\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ VisionAgent initialization error: {ex.Message}");
        Console.WriteLine("\n📋 Setup Checklist:");
        Console.WriteLine("1. Configure Azure OpenAI settings in appsettings.json:");
        Console.WriteLine("   - AzureOpenAI:Endpoint: Your Azure OpenAI endpoint URL");
        Console.WriteLine("   - AzureOpenAI:DeploymentName: Your vision-capable model deployment");
        Console.WriteLine("2. Authenticate with Azure: az login");
        Console.WriteLine("3. Ensure your model supports vision (e.g., gpt-4o)");
        Console.WriteLine("4. Verify your Azure OpenAI resource is accessible");
        Console.WriteLine();
        Console.WriteLine("📖 Example appsettings.json:");
        Console.WriteLine("{");
        Console.WriteLine("  \"AzureOpenAI\": {");
        Console.WriteLine("    \"Endpoint\": \"https://myresource.openai.azure.com/\",");
        Console.WriteLine("    \"DeploymentName\": \"gpt-4o\"");
        Console.WriteLine("  }");
        Console.WriteLine("}");
    }

// Helper method to determine MIME type from file extension
static string GetMimeType(string extension) => extension.ToLowerInvariant() switch
{
    ".jpg" or ".jpeg" => "image/jpeg",
    ".png" => "image/png",
    ".gif" => "image/gif",
    ".bmp" => "image/bmp",
    ".webp" => "image/webp",
    ".tiff" or ".tif" => "image/tiff",
    _ => "image/jpeg"
};


sealed class ApprovalTerminationStrategy : TerminationStrategy
{
    // Terminate when the final message contains the term "APPROVED"
    protected override Task<bool> ShouldAgentTerminateAsync(Microsoft.SemanticKernel.Agents.Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        => Task.FromResult(history[history.Count - 1].Content?.Contains("APPROVED", StringComparison.OrdinalIgnoreCase) ?? false);
}