using CodeChallenge;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
    private List<DirectoryResult> results = new List<DirectoryResult>();
    private bool isPaused = false;
    SearchManager searchManager;
    private UIManager uiManager;

    public MainWindow()
    {
      InitializeComponent();
      textBoxSearching.Visibility = Visibility.Hidden;
      textNoFiles.Visibility = Visibility.Hidden;
      searchManager = new SearchManager();
      uiManager = new UIManager(this, searchManager);
      textNoFiles.Visibility = Visibility.Hidden;
    }

    // Event handlers
    private async void ButtonSelectFolder_ClickAsync(object sender, RoutedEventArgs e)
    {
      var dialog = new FolderBrowserDialog();
      dialog.ShowDialog();
      textBoxFolder.Text = dialog.SelectedPath;
      dataGridResults.Items.Clear();
      buttonPauseResume.IsEnabled = true;
      uiManager.InitializeSearchingMessage();

      await searchManager.SearchAsync(dialog.SelectedPath);

      if (!searchManager.IsCancelledRequested())
      {
        buttonSearch.IsEnabled = true;
        uiManager.ShowCompletedSearchMessage();
        if (dataGridResults.Items.IsEmpty)
        {
          textNoFiles.Visibility = Visibility.Visible;
        }
      }
    }

    private async void ButtonPauseResume_Click(object sender, RoutedEventArgs e)
    {
      if (isPaused)
      {
        if (searchManager.HasDirectoriesToProcess())
        {
          searchManager.Resume();
          buttonPauseResume.Content = "Pause";
          isPaused = false;
          uiManager.InitializeSearchingMessage();
        }
        else
          uiManager.ShowCompletedSearchMessage();
      }
      else
      {
        searchManager.Pause();
        buttonPauseResume.Content = "Resume";
        uiManager.StopBlinkingTimer();
        isPaused = true;
        await Task.Delay(100);
      }
    }
  }
}
