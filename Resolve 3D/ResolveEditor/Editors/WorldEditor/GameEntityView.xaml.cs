using ResolveEditor.Components;
using ResolveEditor.GameProject;
using ResolveEditor.Utilities;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResolveEditor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action _undoAction;
        private string _propertyName;
        public static GameEntityView Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                {
                    (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }
        /// <summary>
        /// Captures the current names of all selected entities and returns an Action 
        /// that reverts their names to the original values when invoked.
        /// </summary>
        private Action GetRanameAction()
        {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MSEntity).Refresh();
            });
        }
        /// <summary>
        /// Captures the current IsEnabled state of all selected entities and returns an Action 
        /// that reverts their IsEnabled state to the original values when invoked.
        /// </summary>
        private Action GetIsEnabledAction()
        {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                (DataContext as MSEntity).Refresh();
            });
        }
        /// <summary>
        /// Stores the current names of the selected entities when the name TextBox gains focus, 
        /// in preparation for potential undo/redo actions.
        /// </summary>
        private void OnName_TextBox_GotKeyFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _undoAction = GetRanameAction();
        }
        /// <summary>
        /// Captures the updated names and adds both undo and redo actions to the undo/redo system 
        /// when the name TextBox loses focus, allowing renaming to be reversible.
        /// </summary>
        private void OnName_TextBox_LostKeyFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                var redoAction = GetRanameAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction,redoAction, "Rename game entity"));
                _propertyName = null;
            }
            _undoAction = null;
        }
        /// <summary>
        /// Captures the IsEnabled state before and after a CheckBox click, then adds both undo and redo actions 
        /// to the undo/redo system, allowing the IsEnabled state of selected entities to be reversible.
        /// </summary>
        private void OnIsEnabled_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var undoAction = GetIsEnabledAction();
            var vm = DataContext as MSEntity;
            vm.IsEnabled = (sender as CheckBox).IsChecked == true;
            var redoAction = GetIsEnabledAction();
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction,
                vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }
    }
}
