using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace OpenSilverCodeGenerator
{
    public partial class LoaderComponent : UserControl
    {
        private string[] Messages = new[]
        {
            "Loading...",
            "Looking for the best",
            "Comparing Results",
            "Generating Xaml",
            "Rendering..."
        };

        public LoaderComponent()
        {
            this.InitializeComponent();

            GenerateMessages();
        }

        public async void GenerateMessages()
        {
            var index = 0;
            while (true)
            {
                Message.Text = Messages[index++ % Messages.Length];
                await Task.Delay(5000);
            }
        }
    }
}
