using Newtonsoft.Json;
using OpenSilverCodeGenerator.CodeGenerators;
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
        private Settings _settings;

        private readonly ChatGptCodeGenerator _chatGptCodeGenerator = new ChatGptCodeGenerator();

        private readonly ICodeGenerator _currentCodeGenerator;

        public MainPage()
        {
            this.InitializeComponent();

            OpenSilver.Interop.ExecuteJavaScriptVoid("console.clear()");
            // Enter construction logic here...
            _settings = RestoreSettings();
            _chatGptCodeGenerator.Initialize(_settings);
            _currentCodeGenerator = _chatGptCodeGenerator;
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
                if (!_currentCodeGenerator.IsReady || _settings == null)
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
            if (!_currentCodeGenerator.IsReady)
            {
                return;
            }
            LoaderOverlay.Visibility = Visibility.Visible;
            try
            {
                var response = await _currentCodeGenerator.Generate(request);
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
                _chatGptCodeGenerator.Initialize(_settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot save settings: {ex.Message}");
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            _chatGptCodeGenerator.Initialize(_settings);
        }
    }
}
