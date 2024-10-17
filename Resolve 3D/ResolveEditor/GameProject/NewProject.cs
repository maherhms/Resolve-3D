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
using ResolveEditor.Common;
using ResolveEditor.Utilities;

namespace ResolveEditor.GameProject
{
    [DataContract]
    /// <summary>
    /// Represents a template for a project, including its type, file, folders, icon, and screenshot.
    /// </summary>
    public class ProjectTemplate
    {
        /// <summary>
        /// Gets or sets the type of the project.
        /// </summary>
        [DataMember]
        public string ProjectType { get; set; }

        /// <summary>
        /// Gets or sets the project file.
        /// </summary>
        [DataMember]
        public string ProjectFile { get; set; }

        /// <summary>
        /// Gets or sets the list of folders associated with the project.
        /// </summary>
        [DataMember]
        public List<string> Folders { get; set; }

        /// <summary>
        /// Gets or sets the icon of the project as a byte array.
        /// </summary>
        public Byte[] Icon { get; set; }

        /// <summary>
        /// Gets or sets the screenshot of the project as a byte array.
        /// </summary>
        public Byte[] Screenshot { get; set; }

        /// <summary>
        /// Gets or sets the file path of the project's icon.
        /// </summary>
        public string IconFilePath { get; set; }

        /// <summary>
        /// Gets or sets the file path of the project's screenshot.
        /// </summary>
        public string ScreenshotFilePath { get; set; }

        /// <summary>
        /// Gets or sets the file path of the project file.
        /// </summary>
        public string ProjectFilePath { get; set; }
    }

    public class NewProject : ViewModelBase
    {
        //TODO: get path from the installation location
        private readonly string _templatePath = @"..\..\ResolveEditor\ProjectTemplates\";

        private string _projectName = "NewProject";
        public string ProjectName
        {
            get { return _projectName; }
            set { if (_projectName != value) { 
                    _projectName = value; 
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName)); 
                } }
        }
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\ResolveProjects\";
        public string ProjectPath
        {
            get { return _projectPath; }
            set { if (_projectPath != value) { 
                    _projectPath = value; 
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                } }
        }
        /// <summary>
        /// reflects the validation of the project path
        /// </summary>
        private bool _isValid;
        public bool IsValid 
        {
            get { return _isValid; }
            set { if (_isValid != value) { _isValid = value; OnPropertyChanged(nameof(IsValid)); } }
        }

        private string _errorMsg;
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { if (_errorMsg != value) { _errorMsg = value; OnPropertyChanged(nameof(ErrorMsg)); } }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        /// <summary>
        /// Validates the project path and name, ensuring they meet specific criteria:
        /// - Project name is not empty and contains no invalid characters.
        /// - Project path is not empty, contains no invalid characters, and does not include spaces or brackets.
        /// - The project directory does not already exist with contents.
        /// </summary>
        /// <returns>True if the project path and name are valid; otherwise, false.</returns>
        public bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += $@"{ProjectName}\";

            IsValid=false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Type in a project name.";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid Character(s) used in project name.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Select a valid project folder.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invalid Character(s) used in project path.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project folder already exists and is not empty.";
            }
            else
            {
                ErrorMsg = string.Empty;
                IsValid = true;
            }

            return IsValid;
        }
        /// <summary>
        /// Creates a new project based on the provided template.
        /// Validates the project path and ensures it ends with a directory separator.
        /// Creates the project directory and any specified subfolders.
        /// Copies the icon and screenshot files to the project directory.
        /// Formats and writes the project XML file.
        /// Returns the project path if successful, or an empty string if an error occurs.
        /// </summary>
        /// <param name="template">The project template to use for creating the project.</param>
        /// <returns>The path of the created project, or an empty string if an error occurs.</returns>

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!IsValid)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }
                var dirinfo = new DirectoryInfo(path + @".Resolve\");
                dirinfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "Screenshot.png")));

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                projectXml = string.Format(projectXml,ProjectName,ProjectPath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXml);
                return path;
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the NewProject class.
        /// Populates the ProjectTemplates collection by reading and deserializing template files.
        /// For each template, reads the icon and screenshot files and sets their paths.
        /// Adds the populated templates to the project templates collection.
        /// Validates the project path.
        /// Logs any exceptions that occur during the process.
        /// </summary>
        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                //get all template.xml
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var file in templateFiles)
                {
                    //from each template.xml we deserialize information and create template and populate it
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    //get icon path using template directory name and append to it icon file name and then read bytes info of the file
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file),"Icon.Png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    //get screenshot path using template directory name and append to it icon file name and then read bytes info of the file
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplates.Add(template);
                }
                ValidateProjectPath();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project templates");
                throw;
            }
        }
    }
}
