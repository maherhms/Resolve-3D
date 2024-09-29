using ResolveEditor.GameProject;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResolveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// Sets up event handlers for the Loaded and Closing events.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowsLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        private void OnMainWindowsLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowsLoaded;
            OpenProjectBrowserDialog();
        }
        /// <summary>
        /// Opens the project browser dialog. 
        /// If the dialog is closed or no data context is set, shuts down the application.
        /// Otherwise, unloads the current project and sets the data context to the selected project.
        /// </summary>
        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}