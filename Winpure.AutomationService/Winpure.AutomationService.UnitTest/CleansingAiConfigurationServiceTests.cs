using System.Collections.Generic;
using NUnit.Framework;
using WinPure.Configuration.DependencyInjection;
using WinPure.Configuration.Models.Configuration;
using WinPure.Configuration.Repository;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Service;
using WinPure.Configuration.Models.Database;
using System.Threading.Tasks;
using System.Linq;

namespace WinPure.Configuration.Tests;

[TestFixture]
public class CleansingAiConfigurationServiceTests
{
    private InMemoryRepo _repo;
    private ICleansingAiConfigurationService _service;

    [SetUp]
    public void SetUp()
    {
        _repo = new InMemoryRepo();
        _service = new WinPureConfigurationDependency.CleansingAiConfigurationService(_repo);
    }

    [Test]
    public void SyncAiTypes_RemovesMissing_UpdatesExisting_AddsNew()
    {
        // Arrange existing in repo
        _repo.Add(new CleansingAiConfigurationEntity { AiType = "A", Options = new CleanAiFieldOptions { Comma = true }, MappedFields = new List<CleanAiMappedField> { new() { Name = "F1", MapType = CleanAiMapType.Exact, Precision = 100 } } });
        _repo.Add(new CleansingAiConfigurationEntity { AiType = "B", Options = new CleanAiFieldOptions { Dots = true } });
        _repo.Add(new CleansingAiConfigurationEntity { AiType = "C" });

        var incoming = new List<CleansingAiFieldType>
        {
            new() { AiType = "A", Options = new CleanAiFieldOptions { Comma = false, Hyphens = true }, MappedFields = new List<CleanAiMappedField>{ new(){ Name="F2", MapType=CleanAiMapType.Fuzzy, Precision=80 } } }, // update
            new() { AiType = "D", Options = new CleanAiFieldOptions { Apostrophes = true } }, // new
        };

        // Act
        _service.SyncAiTypes(incoming);

        var all = _service.GetAllConfigurations().Result;

        // Assert
        Assert.That(all, Has.Count.EqualTo(2), "Should only contain A and D after sync.");
        var a = all.Find(x => x.AiType == "A");
        var d = all.Find(x => x.AiType == "D");
        Assert.That(a, Is.Not.Null);
        Assert.That(d, Is.Not.Null);
        Assert.That(a.Options.Comma, Is.False, "A options should be updated (Comma false)");
        Assert.That(a.Options.Hyphens, Is.True, "A options should be updated (Hypens true)");
        Assert.That(a.MappedFields, Has.Count.EqualTo(1));
        Assert.That(a.MappedFields[0].Name, Is.EqualTo("F2"));
        Assert.That(a.MappedFields[0].MapType, Is.EqualTo(CleanAiMapType.Fuzzy));
        Assert.That(a.MappedFields[0].Precision, Is.EqualTo(80));
        Assert.That(d.Options.Apostrophes, Is.True, "D should have Apostrophes true");
    }

    // Simple in-memory repo implementation
    private class InMemoryRepo : ICleansingAiConfigurationRepository
    {
        private readonly List<CleansingAiConfigurationEntity> _store = new();

        public Task<List<CleansingAiConfigurationEntity>> GetCleansingAiConfigurationsAsync() => Task.FromResult(_store.ToList());
        public Task<CleansingAiConfigurationEntity> GetByNameAsync(string name) => Task.FromResult(_store.FirstOrDefault(x => x.AiType == name));
        public void Add(object entity) => _store.Add((CleansingAiConfigurationEntity)entity);
        public void Delete(object entity) => _store.Remove((CleansingAiConfigurationEntity)entity);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
