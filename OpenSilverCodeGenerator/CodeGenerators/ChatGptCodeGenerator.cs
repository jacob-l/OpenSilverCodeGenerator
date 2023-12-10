using Microsoft.Extensions.ObjectPool;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilverCodeGenerator.CodeGenerators
{
    internal class ChatGptCodeGenerator : ICodeGenerator
    {
        private const string Delimiter = "-----";
        private Conversation _chat;

        private Conversation InitializeChat(Settings settings)
        {
            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                Console.WriteLine("No Api Key provided");
                return null;
            }
            var api = new OpenAI_API.OpenAIAPI(settings.ApiKey);

            api.Chat.DefaultChatRequestArgs.MaxTokens = settings.MaxTokens;
            api.Chat.DefaultChatRequestArgs.Model = settings.ApiModel;
            Console.WriteLine(api.Chat.DefaultChatRequestArgs.Temperature);
            var chat = api.Chat.CreateConversation();
            chat.AppendSystemMessage(settings.Setup);

            // Additional model training
            // Give a few examples as user and assistant
            var examples = settings.Examples?.Split(new[] { Delimiter }, StringSplitOptions.None);
            if (examples != null)
            {
                for (var i = 0; i + 1 < examples.Length; i += 2)
                {
                    chat.AppendUserInput(examples[i]);
                    chat.AppendExampleChatbotOutput(examples[i + 1]);
                }
            }

            return chat;
        }

        public void Initialize(Settings settings)
        {
            _chat = InitializeChat(settings);
        }

        public async Task<string> Generate(string input)
        {
            _chat.AppendUserInput(input);
            var response = await _chat.GetResponseFromChatbotAsync();
            return response;
        }

        public bool IsReady => _chat != null;
    }
}
