# 🤖 VisionAgent - Semantic Kernel ChatCompletionAgent Sample

A simple console application demonstrating **Semantic Kernel ChatCompletionAgent** with **Azure OpenAI** for image analysis.

## ⚙️ Configuration

Update `appsettings.json` with your Azure OpenAI resource details:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o"
  }
}
```

## � Authentication

Uses **AzureCliCredential** for authentication. Run this command first:

```bash
az login
```

## � Usage

```bash
dotnet run
```

Enter an image file path when prompted and the VisionAgent will analyze it.
