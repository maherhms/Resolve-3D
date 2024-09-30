using System.IO;
using System.Xml.Serialization;
using Xunit;
using ResolveEditor.GameProject;
using ResolveEditor.Utilities;
using System.Runtime.Serialization;
using System.Diagnostics;

public class ProjectTemplateTests
{
    [Fact]
    public void CreateProject_ShouldCreateProjectStructure()
    {
        var createdPath = string.Empty;
        // Arrange
        var template = new ProjectTemplate
        {
            ProjectType = "TestType",
            ProjectFile = "testFile.xml",
            Folders = new List<string> { "Assets", "Scripts", ".Resolve"},
            IconFilePath = "../../../TestData/Icon.png",
            ScreenshotFilePath = "../../../TestData/Screenshot.png",
            ProjectFilePath = "../../../TestData/template.xml"
        };

        var project = new NewProject
        {
            ProjectName = "TestProject",
            ProjectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\TestProjects\"
        };

        try
        {
            // Act
            createdPath = project.CreateProject(template);


            // Assert
            Assert.False(string.IsNullOrEmpty(createdPath), "Project creation failed.");
            Assert.True(Directory.Exists(createdPath), "Project folder was not created.");
            Assert.True(File.Exists(Path.Combine(createdPath, ".Resolve", "Icon.png")), "Icon was not copied.");
            Assert.True(File.Exists(Path.Combine(createdPath, ".Resolve", "Screenshot.png")), "Screenshot was not copied.");
            Assert.True(File.Exists(Path.Combine(createdPath, $"{project.ProjectName}.Resolve")), "Project file was not created.");


        }
        catch (Exception ex)
        {
            Assert.True(false, $"Exception occurred: {ex.Message}");
        }
        finally
        {
            // Cleanup
            if (createdPath != null && Directory.Exists(createdPath))
            {
                Directory.Delete(createdPath, true);
            }
        }
    }


    [Fact]
    public void Serializer_ShouldSerializeAndDeserialize_ProjectTemplate()
    {
        // Arrange
        var template = new ProjectTemplate
        {
            ProjectType = "Game",
            ProjectFile = "test.xml",
            Folders = new List<string> { "Assets", "Scenes" },
            IconFilePath = "icon.png",
            ScreenshotFilePath = "screenshot.png"
        };

        var path = "test_project_template.xml";

        // Act
        Serializer.ToFile(template, path);
        var deserializedTemplate = Serializer.FromFile<ProjectTemplate>(path);

        // Assert
        Assert.NotNull(deserializedTemplate);
        Assert.Equal(template.ProjectType, deserializedTemplate.ProjectType);
        Assert.Equal(template.ProjectFile, deserializedTemplate.ProjectFile);
        Assert.Equal(template.Folders.Count, deserializedTemplate.Folders.Count);

        // Cleanup
        if (File.Exists(path)) File.Delete(path);
    }

    [Fact]
    public void CanSerializeAndDeserializeTemplate()
    {
        // Arrange
        var template = new ProjectTemplate
        {
            ProjectType = "TestType",
            ProjectFile = "testFile.xml",
            Folders = new System.Collections.Generic.List<string> { "Assets", "Scripts" }
        };

        // Act
        var serializer = new XmlSerializer(typeof(ProjectTemplate));
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(stream, template);
            stream.Seek(0, SeekOrigin.Begin);

            var deserializedTemplate = (ProjectTemplate)serializer.Deserialize(stream);

            // Assert
            Assert.Equal(template.ProjectType, deserializedTemplate.ProjectType);
            Assert.Equal(template.ProjectFile, deserializedTemplate.ProjectFile);
            Assert.Equal(template.Folders.Count, deserializedTemplate.Folders.Count);
        }
    }

    [Fact]
    public void CanValidateProjectPath_ReturnsFalseOnInvalidCharacters()
    {
        // Arrange
        var project = new NewProject
        {
            ProjectName = "Invalid/Name"
        };

        // Act
        var result = project.ValidateProjectPath();

        // Assert
        Assert.False(result);
        Assert.Equal("Invalid Character(s) used in project name.", project.ErrorMsg);
    }

    [Fact]
    public void Serializer_ToFile_And_FromFile_Test()
    {
        // Arrange
        var testData = new TestData { Name = "John", Age = 30 };
        string _filePath = "testData.xml";

        // Act
        Serializer.ToFile(testData, _filePath);
        var deserializedData = Serializer.FromFile<TestData>(_filePath);

        // Assert
        Assert.NotNull(deserializedData);
        Assert.Equal(testData.Name, deserializedData.Name);
        Assert.Equal(testData.Age, deserializedData.Age);

        // Clean up
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [DataContract]
    public class TestData
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Age { get; set; }
    }
}
