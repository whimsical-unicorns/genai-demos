using LLama;
using LLama.Common;

Console.WriteLine("Please enter the model path (or press Enter to skip):");
string? modelPath = Console.ReadLine();
modelPath = string.IsNullOrWhiteSpace(modelPath) ? @"C:\temp\llm_models_weights\Phi-3-mini-4k-instruct-q4.gguf" : modelPath;

LLama.Native.NativeLibraryConfig.All.WithLogCallback((l, x) => Console.WriteLine($"[{l}]\t{x}"));
LLama.Native.NativeLibraryConfig.All.DryRun(out var loadedLlamaNative, out var loadedLlavaNative);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"LLama Native Library: {loadedLlamaNative}");
Console.WriteLine($"LLava Native Library: {loadedLlavaNative}");
Console.ResetColor();

var parameters = new ModelParams(modelPath)
{
	ContextSize = 1024,
	GpuLayerCount = 50,
	MainGpu = 0
};

using var model = LLamaWeights.LoadFromFile(parameters);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Model loaded successfully! Inferencing...");
Console.ResetColor();

StatelessExecutor statelessExecutor = new(model, parameters);
InferenceParams inverenceParams = new() { MaxTokens = 1024, AntiPrompts = ["User:"] };

string? prompt = null;
do
{
	Console.WriteLine("Please enter the prompt (or press Enter to skip):");
	prompt = Console.ReadLine();
	if (!string.IsNullOrWhiteSpace(prompt))
	{
		var result = statelessExecutor.InferAsync(prompt, inverenceParams);
		await foreach (var text in result)
		{
			Console.Write(text);
		}
	}
	Console.WriteLine();
} while (!string.IsNullOrWhiteSpace(prompt));

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Inference completed!");
Console.ResetColor();

Console.ReadLine();