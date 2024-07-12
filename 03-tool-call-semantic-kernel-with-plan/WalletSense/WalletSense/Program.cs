using Microsoft.Extensions.Azure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Configuration
	.AddUserSecrets<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();

string openAIKey = app.Configuration["OpenAIAPIKey"] ?? string.Empty;
var skBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", openAIKey);
Kernel kernel = skBuilder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
kernel.Plugins.AddFromType<LightsPlugin>("Lights");

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
	ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var history = new ChatHistory();

app.MapGet("/healthcheck/", () =>
{
	return "OK";
});

app.MapPost("v1/chat/sessions/{sessionId}", async (ChatRequest request, string sessionId) =>
{
	history.AddUserMessage(request.Prompt);

	var result = await chatCompletionService.GetChatMessageContentAsync(
		history,
		executionSettings: openAIPromptExecutionSettings,
		kernel: kernel);

	history.AddMessage(result.Role, result?.Content ?? string.Empty);

	var lastHistory = history.Last();
	return new { role = lastHistory.Role.Label, message = lastHistory.Content };
})
.WithName("Chat")
.WithOpenApi();

app.Run();


public class ChatRequest
{
	public string Prompt { get; set; }
}

public class LightsPlugin
{
	// Mock data for the lights
	private readonly List<LightModel> lights = new()
   {
	  new LightModel { Id = 1, Name = "Table Lamp", IsOn = false },
	  new LightModel { Id = 2, Name = "Porch light", IsOn = false },
	  new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
   };

	[KernelFunction("get_lights")]
	[Description("Gets a list of lights and their current state")]
	[return: Description("An array of lights")]
	public async Task<List<LightModel>> GetLightsAsync()
	{
		return lights;
    }

	[KernelFunction("change_state")]
	[Description("Changes the state of the light")]
	[return: Description("The updated state of the light; will return null if the light does not exist")]
	public async Task<LightModel?> ChangeStateAsync(int id, bool isOn)
	{
		var light = lights.FirstOrDefault(light => light.Id == id);

		if (light == null)
		{
			return null;
		}

		// Update the light with the new state
		light.IsOn = isOn;

		return light;
	}
}

public class LightModel
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("is_on")]
	public bool? IsOn { get; set; }
}
