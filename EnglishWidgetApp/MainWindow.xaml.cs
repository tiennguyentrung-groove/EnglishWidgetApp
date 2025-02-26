using Azure;
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Configuration;
using System.Drawing;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EnglishWidgetApp
{
    public class SentencePair
    {
        public string english { get; set; }
        public string vietnamese { get; set; }
    }

    public partial class MainWindow : Window
    {
        private System.Timers.Timer _timer;
        private List<string> _dailySentences = new();
        private int _currentSentenceIndex = 0;
        private const int MaxSentencesPerDay = 5;
        private NotifyIcon _trayIcon;

        public MainWindow()
        {
            InitializeComponent();

            // Enable dragging by mouse click
            MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            };

            // Setup System Tray Icon
            SetupTrayIcon();

            // Load sentences and start displaying them
            LoadDailySentences().ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowNextSentence();
                });
            });

            SetTimer();
        }

        private void SetupTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/icon.ico", 40, 40), // Change this to your app's icon
                Visible = true,
                Text = "English Widget"
            };

            var contextMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show Widget", null, (s, e) => ShowWidget());
            var hideItem = new ToolStripMenuItem("Hide Widget", null, (s, e) => HideWidget());
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) => ExitApplication());

            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(hideItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowWidget()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            });
        }

        private void HideWidget()
        {
            Application.Current.Dispatcher.Invoke(() => Hide());
        }

        private void ExitApplication()
        {
            _trayIcon.Dispose();
            Application.Current.Shutdown();
        }

        private void SetTimer()
        {
            _timer = new System.Timers.Timer(300000); // Change every 5 minutes
            _timer.Elapsed += (s, e) => ShowNextSentence();
            _timer.Start();
        }

        private async void ShowNextSentence()
        {
            if (_dailySentences.Count == 0)
            {
                await LoadDailySentences();
                if (_dailySentences.Count == 0)
                {
                    return;
                }
            }

            if (_currentSentenceIndex >= _dailySentences.Count)
            {
                _currentSentenceIndex = 0;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                EnglishSentence.Text = _dailySentences[_currentSentenceIndex];
            });

            _currentSentenceIndex++;
        }

        private async Task LoadDailySentences()
        {
            if (_dailySentences.Count > 0) return;

            try
            {
                var openAiKey = ConfigurationManager.AppSettings["AzureOpenAIKey"];
                var endpoint = ConfigurationManager.AppSettings["AzureEndpoint"];
                var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(openAiKey));

                var completion = await client.GetChatClient("gpt-4o-mini").CompleteChatAsync(
                    new SystemChatMessage("You are a helpful assistant that helps people learn English"),
                    new UserChatMessage($"Give me {MaxSentencesPerDay} short, common English conversational sentences in business, with Vietnamese translations. Output as json string without json wrapper. Each output object has 2 fields: english, vietnamese")
                );

                var jsonOutput = completion.Value.Content[0].Text;
                var sentences = JsonSerializer.Deserialize<List<SentencePair>>(jsonOutput);
                if (sentences != null)
                {
                    foreach (var s in sentences)
                    {
                        _dailySentences.Add($"{s.english}\n{s.vietnamese}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sentences: {ex.Message}");
            }
        }


    }
}
