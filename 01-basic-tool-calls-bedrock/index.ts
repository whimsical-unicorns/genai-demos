import readline from 'readline';
import { ChatAgent } from './agent';

var cl = readline.createInterface(process.stdin, process.stdout);
export function question(q): Promise<string> {
    return new Promise((res, rej) => {
        cl.question(q, answer => {
            res(answer);
        })
    });
};

async function main() {
    // simple "chat interface"
    const agent = new ChatAgent();
    let prompt: string = null!;
    while (true) {
        prompt = await question("You: ");

        if (prompt && prompt !== 'exit') {
            console.log(`Prompt: ${prompt}`);

            const answer = await agent.getResponse(prompt);
            console.log('Answer:', answer);
        } else {
            break;
        }
    }

    cl.close();
}


main();