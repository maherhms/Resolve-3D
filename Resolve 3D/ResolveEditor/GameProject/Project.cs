﻿using ResolveEditor.Common;
using ResolveEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ResolveEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".resolve";
        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path {  get; private set; }
        public string FullPath => @$"{Path}{Name}\{Name}{Extension}";
        [DataMember (Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }
        private Scene _activeScene;
        [DataMember]
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        public static Project Current => Application.Current.MainWindow.DataContext as Project;
        public static UndoRedo UndoRedo { get; } = new UndoRedo();
        public ICommand UndoCommand {  get; private set; }
        public ICommand RedoCommand {  get; private set; }
        public ICommand AddSceneCommand {  get; private set; }
        public ICommand RemoveSceneCommand {  get; private set; }
        public ICommand SaveCommand {  get; private set; }

        private void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }
        public void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }
        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        public void Unload()
        {

        }
        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
        }

        /// <summary>
        /// Reinitializes fields and commands after deserialization:
        /// - Rebuilds read-only Scenes collection and updates the UI.
        /// - Sets the ActiveScene to the first active scene.
        /// - Adds commands for adding/removing scenes with Undo/Redo functionality.
        /// - Initializes commands for undo, redo, and save operations.
        /// </summary>

        [OnDeserialized]
        private void OnDeseralized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddSceneCommand = new RelayCommand<Object>(x =>
            {
            AddScene($"New Scene {_scenes.Count()}");
            var newScene = _scenes.Last();
            var sceneIndex = _scenes.Count - 1;
            UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveScene(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex,x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<Object>(x => UndoRedo.Undo());
            RedoCommand = new RelayCommand<Object>(x => UndoRedo.Redo());
            SaveCommand = new RelayCommand<Object>(x => Project.Save(this));
        }


        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeseralized(new StreamingContext());
        }
    }
}
