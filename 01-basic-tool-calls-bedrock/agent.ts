import { config } from 'dotenv';
import { BedrockChat } from '@langchain/community/chat_models/bedrock';
import { ChatPromptTemplate } from "@langchain/core/prompts";
import { InMemoryChatMessageHistory } from "@langchain/core/chat_history"
import { z } from "zod";
import { createToolCallingAgent, AgentExecutor } from "langchain/agents";
import { DynamicStructuredTool } from "@langchain/core/tools";
import { RunnableWithMessageHistory } from "@langchain/core/runnables";
import { ChainValues } from "@langchain/core/utils/types";

export class ChatAgent {
    private model: BedrockChat;
    private prompt: ChatPromptTemplate;
    private agentExecutor: AgentExecutor;
    private agentWithHistory: RunnableWithMessageHistory<Record<string, any>, ChainValues>;
    private history: InMemoryChatMessageHistory = new InMemoryChatMessageHistory();

    constructor() {
        config(); // load .env file

        if (!process?.env?.AWSAccessKeyId || !process?.env?.AWSSecretAccessKey) {
            console.error('AWSAccessKeyId or AWSSecretAccessKey not found in .env file');
            process.exit(1);
        }

        this.model = new BedrockChat({
            model: "anthropic.claude-3-sonnet-20240229-v1:0",
            temperature: 0.5,
            region: "us-east-1",
            credentials: {
                accessKeyId: process.env.AWSAccessKeyId as string,
                secretAccessKey: process.env.AWSSecretAccessKey as string,
            },
        });

        this.prompt = ChatPromptTemplate.fromMessages([
            ["system", "You are a helpful assistant"],
            ["placeholder", "{chat_history}"],
            ["human", "{input}"],
            ["placeholder", "{agent_scratchpad}"],
        ]);

        const agent = createToolCallingAgent({
            llm: this.model,
            tools: [this.addTool, this.getVectorDataFake, this.getCurrentDateTime],
            prompt: this.prompt,
            streamRunnable: false // work around streaming issue with tools
        });

        this.agentExecutor = new AgentExecutor({
            agent,
            tools: [this.addTool, this.getVectorDataFake, this.getCurrentDateTime],
            // verbose: true,
            returnIntermediateSteps: true,
        });

        this.agentWithHistory = new RunnableWithMessageHistory({
            runnable: this.agentExecutor,
            getMessageHistory: async (_sessionId) => this.history, // we ignore sessions for simplicity
            inputMessagesKey: 'input',
            historyMessagesKey: 'chat_history',
        });
    }

    public async getResponse(input: string): Promise<any> {
        const config = {
            configurable: {
                sessionId: 'default', // it's required, but we ignore it for simplicity
            },
        };

        const result = await this.agentWithHistory.invoke({ input }, config);
        console.log('------ Agent result -----------', result);

        return result.output;
    }

    private addTool = new DynamicStructuredTool({
        name: "add",
        description: "Add two integers together.",
        schema: z.object({
            firstInt: z.number(),
            secondInt: z.number(),
        }),
        func: async ({ firstInt, secondInt }) => {
            console.log('------ Function addTool called with: -----------', { firstInt, secondInt });
            return (firstInt + secondInt).toString();
        },
    });

    private getVectorDataFake = new DynamicStructuredTool({
        name: "listCustomerPreferences",
        description: "List customer preferences document text snippets, search by provided attribute, e.g. color.",
        schema: z.object({
            preferenceAttribute: z.string(),
        }),
        func: async ({ preferenceAttribute }) => {
            console.log('------ Function getVectorDataFake called with: -----------', { preferenceAttribute });
            return "30% of our customers prefer the blue color. 47% prefer the red color. 23% prefer the green color.";
        },
    });

    private getCurrentDateTime = new DynamicStructuredTool({
        name: "getCurrentDateTime",
        description: "Get the current date and time.",
        schema: z.object({}),
        func: async () => {
            console.log('------ Function getCurrentDateTime called -----------');
            return new Date().toISOString();
        },
    });
}