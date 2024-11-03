using ResolveEditor.Common;
using ResolveEditor.Components;
using ResolveEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResolveEditor.GameProject
{
    /// <summary>
    /// Represents a scene within a project, including its name, project reference, and active status.
    /// </summary>
    [DataContract]
    class Scene : ViewModelBase
    {
        private string _name;

        /// <summary>
        /// Gets or sets the name of the scene.
        /// </summary>
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets the project associated with the scene.
        /// </summary>
        [DataMember]
        public Project Project { get; private set; }

        private bool _isActive;

        /// <summary>
        /// Gets or sets a value indicating whether the scene is active.
        /// </summary>
        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        [DataMember (Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }
        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        private void AddGameEntity(GameEntity entity, int index = -1)
        {
            Debug.Assert(!_gameEntities.Contains(entity));
            entity.IsActive = IsActive;
            if (index == -1)
            {
                _gameEntities.Add(entity);
            }
            else
            {
                _gameEntities.Insert(index, entity);
            }
        }

        private void RemoveGameEntity(GameEntity entity)
        {
            Debug.Assert(_gameEntities.Contains(entity));
            entity.IsActive = false;
            _gameEntities.Remove(entity);
        }
        /// <summary>
        /// Reinitializes fields after deserialization:
        /// - Rebuilds the read-only GameEntities collection and updates the UI.
        /// - Sets up the AddGameEntity and RemoveGameEntity commands with Undo/Redo functionality.
        /// - Ensures added entities can be undone and removed entities can be redone, preserving action history.
        /// </summary>

        [OnDeserialized]
        private void OnDeseralized(StreamingContext context)
        {
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }
            foreach (GameEntity entity in _gameEntities)
            {
                entity.IsActive = true;
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var entityIndex = _gameEntities.Count - 1;
                Project.UndoRedo.Add(new UndoRedoAction(
                        () => RemoveGameEntity(x),
                        () => AddGameEntity(x,entityIndex),
                        $"Add {x.Name} to {Name}"));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityIndex = _gameEntities.IndexOf(x);
                RemoveGameEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGameEntity(x, entityIndex),
                    () => RemoveGameEntity(x),
                    $"Remove {x.Name}"));
            });
        }

        /// <summary>
        /// Initializes a new instance of the Scene class with the specified project and name.
        /// </summary>
        /// <param name="project">The project associated with the scene.</param>
        /// <param name="name">The name of the scene.</param>
        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;

            OnDeseralized(new StreamingContext());
        }
    }

}
