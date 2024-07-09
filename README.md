# Intro and basics
- https://en.wikipedia.org/wiki/Large_language_model
- All the below is just my summary and observation, use with caution

## Key takeaways
- LLMs try to guess the "best next token" (i.e. a word or part of word)
- Base model doesn't try to answer the question, it tries to finish a document that happened to end with "Question... Answer: "
- So we make it "imitate a document" to get it perform a task
- A. Karpathy: "LLM's don't WANT to succeed, a human wants to"
- High Level talk I like: https://www.youtube.com/watch?v=bZQun8Y4L2A

## True strength
- ChatBot is just the beginning (e.g. ChatGPT)
- Main strength of LLMs emerge when we:
    - Give them access to "real world" (e.g. APIs, code execution)
    - Provide them with real-time context (e.g. RAG, "grounding")
    - Use them for semantic searching (e.g. Embeddings)
    - Give them "memory" (e.g. option to save Embedding)
    - Use them as multi-agent system (to compete, cooperate, plan, "brainstorm")

# Tool calls, intelligent automation basics

## Intro
- We "explain" to model it can call a tool
- Model itself doesn't call anything, it tells us to execute the tool for it
- It expects the answer from tool to proceed with its main task, "to imitate a document"
- This means we can execute ANY code on behalf of LLM

## Typical round-trip
1. (Our) APP send a "prompt" and "tools descriptions"
2. LLM decides to use one of the tools (as part of token generation)
3. It returns a response, but instead of "answer" it sends arguments for the tool
4. APP passes these arguments to the tool and "executes" it
5. APP returns result of the tool to LLM with next prompt (and history)
6. LLM returns answer based on whole context

## Use - case examples
- Provide tools for complex Math or other areas where LLMs do not excel (yet?)
- Let LLM send a mail or Teams chat message
- Connect existing applications to LLM via APIs
    - And let LLM automate workflows across multiple of them
    - Or let it gather data from all of them for user prompt
    - Or let it be the only UI for them
    - Or let it write and execute its own code on top of the APIs on demand
- Link one LLM with another and let them collaborate in simple way
    - Link it with "Coder" LLM that will prepare and execute code for it
- Connect LLM to physical world by allowing it to manipulate a robotic arm or a factory conveyor
- Provide LLM with Internet / Google access
- Connect it to your smart home, let it turn off lights or understand what person knocking your door wants (and send it to you via e-mail)

## Takeaways
- Tools let LLM **DO** things, not just talk about them
- Even simple "zero-shot" tool prompt can lead LLM to plan multiple actions
- Smarter frameworks allow separate "planning" and "execution" phases for more complex workflows

### Libraries, tools, frameworks, tips, links
- OpenAI docs https://platform.openai.com/docs/guides/function-calling
- Amazon Bedrock Agent docs https://aws.amazon.com/bedrock/agents/
    - Bedrock samples https://github.com/aws-samples/amazon-bedrock-workshop
- PartyRock, a simple LLM playground for inspiration https://partyrock.aws/
- LangChain ecosystem [Python, Javascript...] https://www.langchain.com/
- Semantic Kernel [C#, Java, Python...] https://github.com/microsoft/semantic-kernel
- Autogen https://github.com/microsoft/autogen
- Polyglot Notebooks for VS Code https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode