using System.Reflection;
using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_External_Layers()
    {
        // Assemble: Inspect the Domain assembly
        var domainAssembly = typeof(Invoice).Assembly;
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        // Assert: Ensure domain does not reference API, Infrastructure, or UI layers
        var forbiddenDependencies = new[]
           {
      "InvoiceApp.Api",
      "Microsoft.AspNetCore",
      "Microsoft.EntityFrameworkCore"
    };

        foreach (var dependency in referencedAssemblies)
        {
            foreach (var forbidden in forbiddenDependencies)
            {
                Assert.DoesNotContain(forbidden, dependency.Name, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public void Domain_Project_Declares_No_Dependencies()
    {
        var projectFile = FindDomainProjectFile();
        var declaredDependencies = System.Xml.Linq.XDocument.Load(projectFile)
          .Descendants()
          .Where(element => element.Name.LocalName is "ProjectReference" or "PackageReference" or "FrameworkReference")
          .Select(element => element.Attribute("Include")?.Value ?? element.ToString())
          .ToList();

        Assert.True(declaredDependencies.Count == 0,
          "InvoiceApp.Domain must stay dependency-free but declares: " + string.Join(", ", declaredDependencies));
    }

    private static string FindDomainProjectFile()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "InvoiceApp.Domain", "InvoiceApp.Domain.csproj");
            if (File.Exists(candidate)) return candidate;
            directory = directory.Parent;
        }

        throw new FileNotFoundException("Could not locate InvoiceApp.Domain.csproj above " + AppContext.BaseDirectory);
    }

    [Fact]
    public void Domain_Entities_Must_Not_Have_Public_Setters_On_Core_State()
    {
        var entityType = typeof(Invoice);
        var statusProperty = entityType.GetProperty(nameof(Invoice.Status));

        Assert.NotNull(statusProperty);
        Assert.True(statusProperty.SetMethod == null || !statusProperty.SetMethod.IsPublic,
          "Invoice.Status should only be modified via domain methods, not public setters.");
    }
}