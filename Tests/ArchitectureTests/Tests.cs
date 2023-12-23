using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Domain.dll";
    private const string ApplicationNamespace = "Application.dll";
    private const string InfrastructureNamespace = "Infrastructure.dll";
    private const string PresentationNamespace = "Presentation.dll";
    private const string WebNamespace = "AudioBaseAPI.dll";
    
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(DomainNamespace);

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PresentationNamespace,
            WebNamespace
        };
        
        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();
        
        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(ApplicationNamespace);

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace,
            WebNamespace
        };
        
        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();
        
        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(InfrastructureNamespace);

        var otherProjects = new[]
        {
            PresentationNamespace,
            WebNamespace
        };
        
        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();
        
        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Presentation_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(PresentationNamespace);

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            WebNamespace
        };
        
        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();
        
        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
}