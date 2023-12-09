using Newtonsoft.Json;
using OpenAI_API.Chat;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace OpenSilverCodeGenerator
{
    public partial class MainPage : Page
    {
        private const string Delimiter = "-----";

        private Conversation _chat;
        private Settings _settings;

        public MainPage()
        {
            this.InitializeComponent();

            OpenSilver.Interop.ExecuteJavaScriptVoid("console.clear()");
            // Enter construction logic here...
            _settings = RestoreSettings();
            _chat = InitializeChat(_settings);
        }


        private void Render(string xaml)
        {
            // Convert the XAML string into an UIElement
            UIElement element = XamlReader.Load($"<UserControl xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">{xaml}</UserControl>") as UIElement;

            // Set the generated UIElement to the ContentControl to be displayed
            RenderedXamlView.Content = element;
        }

        private async void XamlInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_chat == null || _settings == null)
                {
                    return;
                }
                for (var i = 0; i < _settings.MaxAttempts; i++)
                {
                    try
                    {
                        await GenerateXaml(XamlInput.Text);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cannot draw: {ex.Message}");
                    }
                }

                RenderedXamlView.Content = new TextBlock() { Text = $"Cannot generate xaml. Please, try another input", Foreground = new SolidColorBrush(Colors.Red) };
            }
        }

        private string ExtractXaml(string request)
        {
            var code = "```";
            if (request.Contains(code))
            {
                request = request.Substring(request.IndexOf(code) + code.Length);
                request = request.Substring(0, request.LastIndexOf(code));
            }

            request = request.Substring(request.IndexOf("<Grid"));
            var closing = "Grid>";
            request = request.Substring(0, request.LastIndexOf(closing) + closing.Length);

            return request;
        }

        private async Task GenerateXaml(string request)
        {
            if (_chat == null)
            {
                return;
            }
            LoaderOverlay.Visibility = Visibility.Visible;
            try
            {
                _chat.AppendUserInput(request);
                string response = await _chat.GetResponseFromChatbotAsync();
                response = ExtractXaml(response);
                XamlDisplay.Text = response;
                Render(response);
            }
            finally
            {
                LoaderOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private Settings RestoreSettings()
        {
            var settings = new Settings();
            var settingsJson = OpenSilver.Interop.ExecuteJavaScriptGetResult<string>("localStorage.getItem('settings')");
            if (!string.IsNullOrEmpty(settingsJson))
            {
                settings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            }

            Password.Password = settings.ApiKey ?? "";
            ApiModel.Text = (settings.ApiModel ?? "").Trim();
            MaxTokens.Text = settings.MaxTokens.ToString();
            MaxAttempts.Text = settings.MaxAttempts.ToString();
            Setup.Text = (settings.Setup ?? "").Trim();
            Examples.Text = (settings.Examples ?? "").Trim();

            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                MessageBox.Show("Enter the API Key on the setting tab");
            }

            return settings;
        }

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
            var examples = settings.Examples?.Split(new [] { Delimiter }, StringSplitOptions.None);
            if (examples != null)
            {
                for (var i = 0; i + 1 < examples.Length; i+=2)
                {
                    chat.AppendUserInput(examples[i]);
                    chat.AppendExampleChatbotOutput(examples[i + 1]);
                }
            }

            return chat;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = new Settings
                {
                    ApiKey = Password.Password,
                    ApiModel = ApiModel.Text.Trim(),
                    MaxTokens = Convert.ToInt32(MaxTokens.Text),
                    MaxAttempts = Convert.ToInt32(MaxAttempts.Text),
                    Setup = Setup.Text.Trim(),
                    Examples = Examples.Text.Trim()
                };

                OpenSilver.Interop.ExecuteJavaScriptVoid("localStorage.setItem('settings', $0)", JsonConvert.SerializeObject(settings));
                _settings = settings;
                _chat = InitializeChat(_settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot save settings: {ex.Message}");
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            _chat = InitializeChat(_settings);
        }
    }
}
