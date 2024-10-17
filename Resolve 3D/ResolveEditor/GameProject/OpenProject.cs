using ResolveEditor.Common;
using ResolveEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ResolveEditor.GameProject
{
    /// <summary>
    /// Represents the data for a project, including its name, path, date, icon, and screenshot.
    /// </summary>
    [DataContract]
    public class ProjectData
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        [DataMember]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the path of the project.
        /// </summary>
        [DataMember]
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the date associated with the project.
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets the full path of the project, combining the project path, name, and extension.
        /// </summary>
        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }

        /// <summary>
        /// Gets or sets the icon of the project as a byte array.
        /// </summary>
        public Byte[] Icon { get; set; }

        /// <summary>
        /// Gets or sets the screenshot of the project as a byte array.
        /// </summary>
        public Byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }
    /// <summary>
    /// Reads project data from the specified project data path.
    /// If the file exists, deserializes the project data and orders it by date.
    /// Clears the current project list and adds valid projects with their icons and screenshots.
    /// </summary>

    internal class OpenProject
    {
        /// <summary>
        /// The path to the application data directory for ResolveEditor.
        /// </summary>
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ResolveEditor\";

        /// <summary>
        /// The path to the project data file.
        /// </summary>
        private static readonly string _projectDataPath;

        /// <summary>
        /// A collection of project data objects.
        /// </summary>
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();

        /// <summary>
        /// A read-only collection of project data objects.
        /// </summary>
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        /// <summary>
        /// Reads project data from the specified project data path.
        /// If the file exists, deserializes the project data and orders it by date.
        /// Clears the current project list and adds valid projects with their icons and screenshots.
        /// </summary>
        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
                _projects.Clear();
                foreach (var project in projects)
                {
                    if (File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Resolve\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.Resolve\Screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(x => x.Date).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }
        /// <summary>
        /// Opens a project based on the provided project data. 
        /// If the project exists, updates its date to the current date.
        /// If the project does not exist, adds it to the project list with the current date.
        /// Saves the project data and loads the project from its full path.
        /// </summary>
        /// <param name="data">The project data to open.</param>
        /// <returns>The loaded project.</returns>

        public static Project Open(ProjectData data)
        {
            ReadProjectData();
            var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
            if (project != null)
            {
                project.Date = DateTime.Now;
            }
            else
            {
                project = data;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }
            WriteProjectData();

            return Project.Load(project.FullPath);
        }
        /// <summary>
        /// Static constructor for the OpenProject class.
        /// Ensures the application data directory exists and creates it if it does not.
        /// Initializes the project data path and the read-only collection of projects.
        /// Reads the project data from the specified path.
        /// Logs any exceptions that occur during the process.
        /// </summary>

        static OpenProject()
        {
            try
            {
                if(!Directory.Exists(_applicationDataPath)) Directory.CreateDirectory(_applicationDataPath);
                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project data");
                throw;
            }
        }
    }
}
