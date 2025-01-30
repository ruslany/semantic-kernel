// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GettingStarted;

/// <summary>
/// This example shows how to create and use a <see cref="Kernel"/>.
/// </summary>
public sealed class Step1_Create_Kernel(ITestOutputHelper output) : BaseTest(output)
{
    /// <summary>
    /// Show how to create a <see cref="Kernel"/> and use it to execute prompts.
    /// </summary>
    [Fact]
    public async Task CreateKernelAsync()
    {
        // Create a kernel with OpenAI chat completion
        Kernel kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                endpoint: TestConfiguration.AzureOpenAI.Endpoint,
                deploymentName: TestConfiguration.AzureOpenAI.DeploymentName,
                apiKey: TestConfiguration.AzureOpenAI.ApiKey)
            .Build();

        // Example 1. Invoke the kernel with a prompt and display the result
        output.WriteLine(await kernel.InvokePromptAsync("What color is the sky?"));
        output.WriteLine();

        // Example 2. Invoke the kernel with a templated prompt and display the result
        KernelArguments arguments = new() { { "topic", "sea" } };
        output.WriteLine(await kernel.InvokePromptAsync("What color is the {{$topic}}?", arguments));
        output.WriteLine();

        // Example 3. Invoke the kernel with a templated prompt and stream the results to the display
        await foreach (var update in kernel.InvokePromptStreamingAsync("What color is the {{$topic}}? Provide a detailed explanation.", arguments))
        {
            output.Write(update);
        }

        output.WriteLine(string.Empty);

        // Example 4. Invoke the kernel with a templated prompt and execution settings
        arguments = new(new OpenAIPromptExecutionSettings { MaxTokens = 500, Temperature = 0.5 }) { { "topic", "dogs" } };
        output.WriteLine(await kernel.InvokePromptAsync("Tell me a story about {{$topic}}", arguments));

        // Example 5. Invoke the kernel with a templated prompt and execution settings configured to return JSON
#pragma warning disable SKEXP0010
        arguments = new(new OpenAIPromptExecutionSettings { ResponseFormat = "json_object" }) { { "topic", "chocolate" } };
        output.WriteLine(await kernel.InvokePromptAsync("Create a recipe for a {{$topic}} cake in JSON format", arguments));
    }
}
