﻿using ResolveEditor.Common;
using ResolveEditor.DllWrapers;
using ResolveEditor.GameProject;
using ResolveEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResolveEditor.Components
{
    /// <summary>
    /// Represents a game entity within a scene, supporting renaming, enabling/disabling, and undo/redo functionality.
    /// </summary>
    [DataContract]
	[KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
    {
        private int _entityId = ID.INVALID_ID;
        public int EntityId
        {
            get { return _entityId; }
            set {
                if (_entityId != value)
                {
                    _entityId = value; 
                    OnPropertyChanged(nameof(EntityId));
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        EntityId = EngineAPI.CreateGameEntity(this);
                        Debug.Assert(ID.Is_Valid(_entityId));
                    }
                    else
                    {
                        EngineAPI.RemoveGameEntity(this);
                    }
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
		{
			get { return _isEnabled; }
			set { if (_isEnabled != value)
				{
					_isEnabled = value;
					OnPropertyChanged(nameof(IsEnabled));
				}
}
		}

		private string _name;
		[DataMember]
		public string Name
		{
			get { return _name; }
			set { if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}
        [DataMember]
        public Scene ParentScene { get; private set; }
        [DataMember (Name =nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
		public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

		[OnDeserialized]
        /// <summary>
        /// Initializes the component collection and command bindings after deserialization.
        /// </summary>
		void OnDeserialized(StreamingContext context)
		{
			if (_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}
        }
        /// <summary>
        /// Initializes a new GameEntity with a reference to its parent scene.
        /// </summary>
        public GameEntity(Scene scene)
		{
			Debug.Assert(scene != null);
			ParentScene = scene;
			_components.Add(new Transform(this));

			OnDeserialized(new StreamingContext());
		}
    }
    /// <summary>
    /// Represents a multi-selection entity that manages properties for selected GameEntities.
    /// </summary>
    abstract class MSEntity : ViewModelBase
	{
        //enables updates to selected entities
        private bool _enableUpdates = true;
        private bool? _isEnabled;
        [DataMember]
        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

		private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent> ();
		public ReadOnlyObservableCollection<IMSComponent> Components { get; }
		public List<GameEntity> SelectedEntities { get; }
        /// <summary>
        /// Returns the common float value across all entities for a specific property. 
        /// Returns null if values differ between entities.
        /// </summary>
        public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
		{
			var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
				if (!value.IsTheSameAs(getProperty(entity)))
				{
                    return null;
                }
            }
            return value;
		}
        /// <summary>
        /// Returns the common bool value across all entities for a specific property. 
        /// Returns null if values differ between entities.
        /// </summary>
        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }
            return value;
        }
        /// <summary>
        /// Returns the common string value across all entities for a specific property. 
        /// Returns null if values differ between entities.
        /// </summary>
        public static string? GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }
            return value;
        }
        /// <summary>
        /// Updates the properties of selected GameEntities when the MSEntity's properties change.
        /// </summary>
        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }
        /// <summary>
        /// Updates the multi-selection entity's properties (IsEnabled and Name) based on the selected entities' common values.
        /// </summary>
        /// <returns>Returns true after successfully updating the properties.</returns>
        protected virtual bool UpdateMSGameEntity()
		{
			IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
			Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

			return true;
		}
        /// <summary>
        /// Refreshes the properties of the multi-selection entity.
        /// </summary>
        public void Refresh()
		{
            _enableUpdates = false;
			UpdateMSGameEntity();
            _enableUpdates = true;
		}
        /// <summary>
        /// Initializes a new MSEntity with a list of selected GameEntities.
        /// </summary>
        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert (entities?.Any() == true);
			Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
			SelectedEntities = entities;
			PropertyChanged += (s, e) => { if(_enableUpdates) UpdateGameEntities(e.PropertyName); };
        }
    }
    /// <summary>
    /// Represents a multi-selection GameEntity, synchronizing the properties of multiple GameEntities.
    /// </summary>
    class MSGameEntity : MSEntity
    {
        /// <summary>
        /// Initializes a new MSGameEntity with a list of selected GameEntities and refreshes their properties.
        /// </summary>
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
