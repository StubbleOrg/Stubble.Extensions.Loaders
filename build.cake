#tool "nuget:?package=ReportGenerator"

#tool nuget:?package=Codecov
#addin nuget:?package=Cake.Codecov
#addin nuget:?package=Cake.Coverlet

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testFramework = Argument("testFramework", "");
var framework = Argument("framework", "");
var runCoverage = Argument<bool>("runCoverage", true);

var buildDir = Directory("./src/Stubble.Extensions.Loaders/bin") + Directory(configuration);
var testBuildDir = Directory("./test/Stubble.Extensions.Loaders.Tests/bin") + Directory(configuration);

var artifactsDir = Directory("./artifacts/");
var coverageDirectory = Directory("./coverage-results");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(testBuildDir);
    CleanDirectory("./artifacts");
    CleanDirectory(coverageDirectory);
    CleanDirectory("./coverage-report");
    CleanDirectory("./test/Stubble.Extensions.Loaders.Tests/TestResults");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor) {
        DotNetCoreRestore("./src/Stubble.Extensions.Loaders/Stubble.Extensions.Loaders.csproj");
        DotNetCoreRestore("./test/Stubble.Extensions.Loaders.Tests/Stubble.Extensions.Loaders.Tests.csproj");
    } else {
        DotNetCoreRestore("./Stubble.Extensions.Loaders.sln");
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var setting = new DotNetCoreBuildSettings {
        Configuration = configuration
    };

    if(!string.IsNullOrEmpty(framework))
    {
        setting.Framework = framework;
    }

    var testSetting = new DotNetCoreBuildSettings {
        Configuration = configuration
    };

    if(!string.IsNullOrEmpty(testFramework))
    {
        testSetting.Framework = testFramework;
    }

    DotNetCoreBuild("./src/Stubble.Extensions.Loaders/", setting);
    DotNetCoreBuild("./test/Stubble.Extensions.Loaders.Tests/", testSetting);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    var testSettings = new DotNetCoreTestSettings {
        Configuration = configuration,
        NoBuild = true,
        Verbosity = DotNetCoreVerbosity.Quiet,
        Framework = testFramework,
        ArgumentCustomization = args =>
            args.Append("--logger:trx")
    };

    var coverletSettings = new CoverletSettings {
        CollectCoverage = runCoverage,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = coverageDirectory,
        CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
    };

    DotNetCoreTest("./test/Stubble.Extensions.Loaders.Tests/Stubble.Extensions.Loaders.Tests.csproj", testSettings, coverletSettings);

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        foreach(var file in GetFiles("./test/Stubble.Extensions.Loaders.Tests/TestResults/*"))
        {
            AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.MSTest);
            AppVeyor.UploadArtifact(file);
        }
    }
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        OutputDirectory = artifactsDir,
        NoBuild = true,
        Configuration = configuration,
    };

    DotNetCorePack("./src/Stubble.Extensions.Loaders/Stubble.Extensions.Loaders.csproj", settings);
});

Task("CodeCov")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var coverageFiles = GetFiles("./coverage-results/*.xml")
        .Select(f => f.FullPath)
        .ToArray();

    var settings = new CodecovSettings();

    if (AppVeyor.IsRunningOnAppVeyor) {
        var token = EnvironmentVariable("CODECOV_REPO_TOKEN");
        settings.Token = token;

        foreach(var file in coverageFiles)
        {
            settings.Files = new [] { file };

            // Upload coverage reports.
            Codecov(settings);
        }
    }
});

Task("CoverageReport")
    .IsDependentOn("Test")
    .Does(() =>
{
    ReportGenerator("./coverage-results/*.xml", "./coverage-report/");
});

Task("AppVeyor")
    .IsDependentOn("CodeCov");

Task("Travis")
    .IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);