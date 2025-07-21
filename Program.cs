using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

#pragma warning disable SKEXP0110 // Suppress experimental API warnings for Agents

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get Azure OpenAI configuration from appsettings
var endpoint = configuration["AzureOpenAI:Endpoint"];
var deploymentName = configuration["AzureOpenAI:DeploymentName"];

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

    // Create VisionAgent using ChatCompletionAgent
    var visionAgent = CreateVisionAgent(kernel);

    Console.WriteLine($"✅ VisionAgent '{visionAgent.Name}' created successfully");
    Console.WriteLine();

    // Interactive loop - Agent conversation
    while (true)
    {
        Console.Write("Enter image file path (or 'quit' to exit): ");
        var input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            break;

        if (!File.Exists(input))
        {
            Console.WriteLine("❌ File not found. Please try again.");
            continue;
        }

        if (!IsSupportedImageFormat(input))
        {
            Console.WriteLine("❌ Unsupported image format. Supported: JPG, PNG, GIF, BMP, WEBP, TIFF");
            continue;
        }

        try
        {
            Console.Write("Your question (or press Enter for 'What's in this image?'): ");
            var question = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(question))
                question = "What's in this image?";

            Console.WriteLine("🔍 VisionAgent is analyzing the image...");

            // Read image file
            var imageBytes = await File.ReadAllBytesAsync(input);
            var mimeType = GetMimeType(Path.GetExtension(input));

            // Create agent group chat
            var chat = new AgentGroupChat();

            // Add user message with image content to the chat
            var messageContent = new ChatMessageContentItemCollection
            {
                new TextContent($"User Question: {question}"),
                new ImageContent(imageBytes, mimeType)
            };

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, messageContent));

            // Get agent's response
            Console.WriteLine("\n🤖 VisionAgent Response:");
            Console.WriteLine("========================");
            
            await foreach (var message in chat.InvokeAsync(visionAgent))
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
            Console.WriteLine($"❌ VisionAgent encountered an error: {ex.Message}");
            
            if (ex.Message.Contains("401") || ex.Message.Contains("Unauthorized"))
            {
                Console.WriteLine("🔐 Authentication issue - run: az login");
            }
            else if (ex.Message.Contains("404") || ex.Message.Contains("NotFound"))
            {
                Console.WriteLine("🔍 Check your endpoint and deployment name");
            }
            else if (ex.Message.Contains("rate") || ex.Message.Contains("quota"))
            {
                Console.WriteLine("⏱️  Rate limit or quota exceeded - try again later");
            }
            
            Console.WriteLine();
        }
    }

    Console.WriteLine("👋 VisionAgent signing off!");
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

// Create VisionAgent using ChatCompletionAgent pattern
ChatCompletionAgent CreateVisionAgent(Kernel kernel)
{
    // Clone kernel instance to allow for agent specific configuration
    Kernel agentKernel = kernel.Clone();

    // Define agent instructions for vision analysis
    const string agentInstructions = """
        You are VisionAgent, an expert image analysis AI assistant. 
        Your role is to analyze images carefully and provide detailed, accurate descriptions.
        
        When analyzing images, focus on:
        - Main subjects and objects in the image
        - Colors, textures, and composition details
        - Any text visible in the image
        - Setting, environment, or background
        - Activities or actions taking place
        - Notable features or interesting elements
        
        Always be objective, factual, and thorough in your analysis.
        Structure your response clearly and be comprehensive.
        Always start your response with "VisionAgent Analysis:" and end with your signature "—VisionAgent"
        """;

    // Create the ChatCompletionAgent
    return new ChatCompletionAgent()
    {
        Name = "VisionAgent",
        Instructions = agentInstructions,
        Kernel = agentKernel,
        Arguments = new KernelArguments(
            new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 2000,
                Temperature = 0.1, // Low temperature for consistent, factual responses
                TopP = 0.95,
                FrequencyPenalty = 0.0,
                PresencePenalty = 0.0
            })
    };
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

// Helper method to check supported image formats
static bool IsSupportedImageFormat(string filePath)
{
    var extension = Path.GetExtension(filePath).ToLowerInvariant();
    return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".tiff" or ".tif";
}
