using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResolveEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialg.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserDialogLoaded;
        }

        private void OnProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserDialogLoaded;
            if (!OpenProject.Projects.Any())
            {
                openProjectButton.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;
                OnToggleButton_Click(createProjectButton, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Handles the click event for toggle buttons. 
        /// Toggles the state of the openProjectButton and createProjectButton, 
        /// and adjusts the margin of the browser content accordingly.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == openProjectButton)
            {
                if (createProjectButton.IsChecked == true)
                {
                    createProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(0);
                }
                openProjectButton.IsChecked = true;
            }
            else
            {
                if (sender == createProjectButton)
                {
                    if (openProjectButton.IsChecked == true)
                    {
                        openProjectButton.IsChecked = false;
                        browserContent.Margin = new Thickness(-800,0,0,0);
                    }
                    createProjectButton.IsChecked = true;
                }
            }
        }
    }
}
