﻿using ResolveEditor.Common;
using ResolveEditor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ResolveEditor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {
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

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			if (_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}
		}

		public GameEntity(Scene scene)
		{
			Debug.Assert(scene != null);
			ParentScene = scene;
			_components.Add(new Transform(this));

			OnDeserialized(new StreamingContext());
		}
    }
}
