# General
- this is folder of general demos on how to run models locally
- primary target are programmatic aproaches, e.g. llama.cpp or Transformers in python
- tools such as Ollama, LlamaFile and others are out of scope
- model tip: https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/resolve/main/Phi-3-mini-4k-instruct-q4.gguf?download=true

# LLamaFile
- https://github.com/Mozilla-Ocho/llamafile
- e.g. `.\llamafile-0.8.17.exe -m C:\temp\llava-v1.6-mistral-7b.Q5_K_M.gguf -ngl 999`

# LLamaSharp
- https://github.com/SciSharp/LLamaSharp/
- running without CUDA installed:
    - simply download https://github.com/ggerganov/llama.cpp/releases/download/b4255/cudart-llama-bin-win-cu12.4-x64.zip
    - extract the contents into ...\genai-demos\04-local-models\local-ai-service\local-ai-console\bin\Debug\net8.0
- good tips: https://github.com/SciSharp/LLamaSharp/issues/350