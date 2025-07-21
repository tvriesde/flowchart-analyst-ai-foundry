# 🤖 VisionAgent - Semantic Kernel ChatCompletionAgent for Image Analysis

A console application demonstrating **Semantic Kernel ChatCompletionAgent** with **Azure OpenAI** for intelligent image analysis using proper agent architecture.

## 🚀 Features

- **ChatCompletionAgent Pattern** - Uses Semantic Kernel's ChatCompletionAgent for proper agent behavior
- **Azure OpenAI Vision** - Powered by GPT-4o with vision capabilities  
- **Configuration-Based** - Settings stored in `appsettings.json` for easy management
- **Secure Authentication** - Uses AzureCliCredential (no hardcoded keys)
- **Agent Personality** - Structured responses with VisionAgent persona and signature
- **Multiple Image Formats** - Supports JPG, PNG, GIF, BMP, WebP, TIFF
- **Interactive Console** - Easy-to-use command-line interface
- **Kernel Cloning** - Agent-specific kernel configuration following best practices

## 📋 Prerequisites

1. **Azure OpenAI Service** with a vision-capable model (e.g., GPT-4o)
2. **.NET 8.0 SDK** installed
3. **Azure CLI** authentication set up
4. **Semantic Kernel 1.28.0** and **Agents.Core** packages (included in project)

## ⚡ Quick Setup

### 1. Configure Azure OpenAI Settings
Update `appsettings.json` with your Azure OpenAI resource details:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o"
  }
}
```

### 2. Authenticate with Azure
```bash
az login
```

### 3. Build and Run
```bash
dotnet build
dotnet run
```

## 🎯 Usage

1. **Run the application**: `dotnet run`
2. **Enter image path**: Provide the full path to an image file
3. **Ask questions**: Type your question or press Enter for "What's in this image?"
4. **View analysis**: VisionAgent will provide detailed analysis
5. **Continue or quit**: Analyze more images or type 'quit' to exit

### Example Session

```
🤖 Semantic Kernel VisionAgent
===============================
🔗 Using endpoint: https://your-resource.openai.azure.com/
🤖 Using deployment: gpt-4o
✅ VisionAgent 'VisionAgent' created successfully

Enter image file path (or 'quit' to exit): C:\Images\bicycle.jpg
Your question (or press Enter for 'What's in this image?'): 

🔍 VisionAgent is analyzing the image...

🤖 VisionAgent Response:
========================
VisionAgent Analysis: This image features a sleek, modern road bicycle with a predominantly black frame. The bicycle has drop-style handlebars typical of road bikes, allowing for aerodynamic riding positions. Key components visible include thin wheels with narrow tires optimized for road cycling, a multi-speed gear system with derailleur, and a high-positioned saddle designed for performance. The composition shows the bicycle against a plain white background, emphasizing its streamlined design and professional appearance suitable for cycling enthusiasts.

—VisionAgent
========================

Enter image file path (or 'quit' to exit): quit
👋 VisionAgent signing off!
```

## 🔧 Key Components

- **ChatCompletionAgent Pattern**: Proper agent implementation with instructions and kernel cloning
- **Configuration Management**: Uses `appsettings.json` for environment-specific settings
- **AgentGroupChat**: Handles conversation flow with image and text content
- **Secure Authentication**: AzureCliCredential for production-ready security
- **Vision Analysis**: Optimized settings for image analysis tasks
- **Agent Persona**: VisionAgent with consistent signature and structured responses
- **Error Handling**: Comprehensive error handling with helpful guidance

## 🛠️ Architecture

```
Program.cs
├── Configuration Loading (appsettings.json)
├── Kernel Creation (Azure OpenAI + AzureCliCredential)
├── ChatCompletionAgent Creation
│   ├── Agent Instructions & Personality
│   ├── Kernel Cloning for Agent-specific Config
│   └── OpenAI Execution Settings
├── Interactive Loop
│   ├── Image Input & Validation
│   ├── AgentGroupChat Creation
│   ├── Message Content (Text + Image)
│   └── Agent Response via chat.InvokeAsync()
└── Error Handling & Cleanup

Supporting Files:
├── appsettings.json (Azure OpenAI Configuration)
├── ImagesToAgent.csproj (NuGet Dependencies)
├── .gitignore (.NET Standard Git Ignore Rules)
└── Helper Methods (MIME Type, Format Validation)
```

## 📦 Dependencies

```xml
<PackageReference Include="Microsoft.SemanticKernel" Version="1.28.0" />
<PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="1.28.0-alpha" />
<PackageReference Include="Azure.Identity" Version="1.13.1" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
```

## 🔒 Security Features

- ✅ **No hardcoded credentials** - Uses AzureCliCredential with Azure CLI authentication
- ✅ **Configuration-based** - Sensitive settings in `appsettings.json` (can be excluded from source control)
- ✅ **Secure communication** - HTTPS connections to Azure OpenAI
- ✅ **Input validation** - File existence and format checks
- ✅ **Error handling** - Proper exception handling without credential exposure

## 🚨 Troubleshooting

| Error | Solution |
|-------|----------|
| **Configuration Error** | Update `appsettings.json` with valid Azure OpenAI settings |
| **401 Unauthorized** | Run `az login` to authenticate with Azure CLI |
| **404 Not Found** | Check endpoint URL and deployment name in config |
| **Rate limits/Quota** | Wait and retry, or check your Azure OpenAI quota |
| **File not found** | Use absolute file paths for images |
| **Unsupported format** | Use JPG, PNG, GIF, BMP, WebP, or TIFF files |
| **Agent creation fails** | Ensure Semantic Kernel packages are properly installed |

## 📚 References

- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Semantic Kernel Agents](https://learn.microsoft.com/semantic-kernel/concepts/ai-services/chat-completion/chat-completion-agent)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/)
- [AzureCliCredential Authentication](https://learn.microsoft.com/dotnet/api/azure.identity.azureclicredential)
- [GPT-4 Vision API](https://learn.microsoft.com/azure/ai-services/openai/concepts/gpt-with-vision)
- [.NET Configuration](https://learn.microsoft.com/dotnet/core/extensions/configuration)

---

This sample demonstrates Azure development best practices including:
- ✅ **Modern Agent Architecture** with ChatCompletionAgent
- ✅ **Configuration-based** settings management  
- ✅ **Secure authentication** with Azure CLI
- ✅ **Proper error handling** and user guidance
- ✅ **Clean code structure** with helper methods
- ✅ **Production-ready patterns** for Semantic Kernel development
