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
    public void Domain_Entities_Must_Not_Have_Public_Setters_On_Core_State()
    {
        var entityType = typeof(Invoice);
        var statusProperty = entityType.GetProperty(nameof(Invoice.Status));

        Assert.NotNull(statusProperty);
        Assert.True(statusProperty.SetMethod == null || !statusProperty.SetMethod.IsPublic,
            "Invoice.Status should only be modified via domain methods, not public setters.");
    }
}