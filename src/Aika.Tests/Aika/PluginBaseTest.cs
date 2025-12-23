using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Versioning;
using System.Reflection;
using System.Text.Json;

namespace Aika.Tests.Aika;

[TestFixture]
public class PluginBaseTests
{
    private Mock<ILogger<TestPlugin>> _loggerMock;
    private string _testPluginDirectory;
    private const string TestManifestFileName = "plugin.json";

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<TestPlugin>>();
        _testPluginDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testPluginDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testPluginDirectory))
        {
            Directory.Delete(_testPluginDirectory, recursive: true);
        }
    }

    [Test]
    public async Task InitAsync_LoadsManifestFromFile_Successfully()
    {
        // Arrange
        var manifest = new CustomPluginManifest
        {
            Id = "test.plugin",
            Name = "Test Plugin",
            Version = "1.0.0",
            PluginConfiguration = new CustomPluginConfig
            {
                MagicName = "lcma",
                MagicNumber = 20
            },
            Dependences = [
                new ()
                {
                    PluginId = "abc",
                    Version = VersionRange.Parse("[1.0.0, 2.0.0]")
                }
            ]
        };

        var manifestPath = Path.Combine(_testPluginDirectory, TestManifestFileName);
        var json = JsonSerializer.Serialize(manifest);
        await File.WriteAllTextAsync(manifestPath, json);

        var plugin = new TestPlugin(_loggerMock.Object, _testPluginDirectory);
        var services = new ServiceCollection();

        // Act
        await plugin.InitAsync(services, CancellationToken.None);
        var config = PluginBase.PluginManifest?.GetConfiguration<CustomPluginConfig>();

        // Assert
        Assert.That(PluginBase.PluginManifest, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(PluginBase.PluginManifest.FullId, Is.EqualTo("test.plugin@1.0.0"));
            Assert.That(PluginBase.PluginManifest.Name, Is.EqualTo("Test Plugin"));
            Assert.That(PluginBase.PluginManifest.Version.ToString(), Is.EqualTo("1.0.0"));
        });

        Assert.That(config, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(config.MagicName, Is.EqualTo("lcma"));
            Assert.That(config.MagicNumber, Is.EqualTo(20));
        });
    }

    public class CustomPluginManifest
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Version { get; set; }

        public required List<PluginDependence> Dependences { get; set; }

        public CustomPluginConfig? PluginConfiguration { get; set; }
    }

    public class CustomPluginConfig
    {
        public int MagicNumber {  get; set; }
        public string? MagicName { get; set; }
    }

    public class TestPlugin : PluginBase
    {
        private readonly string _testDirectory;

        public TestPlugin(ILogger<TestPlugin> logger, string testDirectory)
            : base(logger)
        {
            _testDirectory = testDirectory;
        }

        public override async Task InitAsync(IServiceCollection services, CancellationToken cancellationToken)
        {
            PluginManifest = await LoadManifestFromTestDirectory(cancellationToken);
        }

        private async Task<PluginManifest> LoadManifestFromTestDirectory(CancellationToken cancellationToken)
        {
            var pathToManifest = Path.Combine(_testDirectory, PluginManifestFileName);

            var json = await File.ReadAllTextAsync(pathToManifest, cancellationToken);
            var manifest = JsonSerializer.Deserialize<PluginManifest>(json);

            if (manifest is not null)
                return manifest;

            throw new InvalidOperationException("Failed to load plugin manifest.");
        }
    }
}

