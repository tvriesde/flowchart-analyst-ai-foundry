# 🤖 VisionAgent - Semantic Kernel ChatCompletionAgent Sample

A simple console application demonstrating **Semantic Kernel ChatCompletionAgent** with **Azure AI Foundry** for image analysis.

Requires an Azure AI Foundry resource with a **Vision Agent** and **Review Agent**. Define these agents to your liking in AI Foundry, and use the agent IDs in the configuration.

## ⚙️ Configuration

Update `appsettings_example.json` with your Azure Foundry resource details:

```json
{
  "AzureAI": {
    "Endpoint": "https://<your-custom-endpoint>.cognitiveservices.azure.com/",
    "DeploymentName": "gpt-4o"
  },
  "AzureAIProject": {
    "Endpoint": "https://<your-custom-endpoint>.services.ai.azure.com/api/projects/<your-project-id>",
    "ReviewAgentId": "",
    "VisionAgentId": ""
  }
}
Rename to appsettings.json

## � Authentication

Uses **AzureCliCredential** for authentication. Run this command first:

```bash
az login
```

## � Usage

```bash
dotnet run
```

Modify the script imagePath variable to an image containing a flowchart of your choice

==========================
https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-types/azure-ai-agent?pivots=programming-language-csharp