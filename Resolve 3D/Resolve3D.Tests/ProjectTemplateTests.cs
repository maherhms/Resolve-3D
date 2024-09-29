using System.IO;
using System.Xml.Serialization;
using Xunit;
using ResolveEditor.GameProject;

public class ProjectTemplateTests
{
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

}
