using ResolveEditor.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ResolveEditor.GameProject
{
    /// <summary>
    /// Represents a scene within a project, including its name, project reference, and active status.
    /// </summary>
    [DataContract]
    public class Scene : ViewModelBase
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
        }
    }

}
