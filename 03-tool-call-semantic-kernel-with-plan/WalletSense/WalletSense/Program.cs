using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using WalletSense;

#region .NET Minimal API default stuff + cors
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

app.MapGet("/healthcheck/", () =>
{
	return "OK";
});
#endregion

#region Semantic Kernel "standard" setup
// to initialize user-secrets run `dotnet user-secrets init` and `dotnet user-secrets set "OpenAIAPIKey" "your-key"`
string openAIKey = app.Configuration["OpenAIAPIKey"] ?? string.Empty;

// setup the kernel "core"
var skBuilder = Kernel.CreateBuilder()
	.AddOpenAIChatCompletion("gpt-4o", openAIKey);
Kernel kernel = skBuilder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>(); // get service to send and receive messages
var history = new ChatHistory(); // keep track of the conversation; only one in this case

// define basic settings for tool calls
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
	ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
	Temperature = 0.5,
};
#endregion

// add the plugins / tools / functions / connectors
kernel.Plugins.AddFromType<AccountPlugin>("Account");
//kernel.Plugins.AddFromType<LightsPlugin>("Lights"); // to try demo from docs

// configure system prompt; this can be dynamic based on user's settings
history.AddSystemMessage("""
	You are 'Wallet Sense', a smart wallet and financial assistant for users on the go.
	User configured you to help them save more and avoid unnecessary expenses.
	You should analyze their recent transactions and predicted spending, and if the planned transaction is unnecessary, convince them to avoid it.
	Do NOT recapitulate the transactions or current balance. Answer in short recommendations for given transaction planned.
	""");

// in this example we simply ignore the sessionId
app.MapPost("v1/chat/sessions/{sessionId}", async (ChatRequest request, string sessionId) =>
{
	history.AddUserMessage(request.Prompt ?? string.Empty); // track history

	// main "magic" happens here
	// get the response from AI
	// let Kernel handle the conversation and tool calls and execution
	var result = await chatCompletionService.GetChatMessageContentAsync(
		history,
		executionSettings: openAIPromptExecutionSettings,
		kernel: kernel);

	history.AddMessage(result.Role, result?.Content ?? string.Empty); // track history

	var lastHistory = history.Last();
	return new ChatResponse { Role = lastHistory.Role.Label, Message = lastHistory.Content ?? string.Empty };
})
.WithName("Chat")
.WithOpenApi();

app.Run();

#region models and other formals
public class ChatRequest
{
	public string? Prompt { get; set; }
}

public class ChatResponse
{
	public string Role { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
}
#endregion
