#tool "nuget:?package=coveralls.net"
#tool "nuget:https://nuget.org/api/v2/?package=ReportGenerator"

#addin "nuget:https://nuget.org/api/v2/?package=Cake.Coveralls"
#addin "nuget:https://nuget.org/api/v2/?package=Cake.Incubator"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testFramework = Argument("testFramework", "");
var framework = Argument("framework", "");
var runCoverage = Argument<bool>("runCoverage", true);

var buildDir = Directory("./src/Stubble.Extensions.Loaders/bin") + Directory(configuration);
var testBuildDir = Directory("./test/Stubble.Extensions.Loaders.Tests/bin") + Directory(configuration);

var artifactsDir = Directory("./artifacts/");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(testBuildDir);
    CleanDirectory("./artifacts");
    CleanDirectory("./coverage-results");
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
    Action<ICakeContext> testAction = tool => {
        tool.DotNetCoreTest("./test/Stubble.Extensions.Loaders.Tests/Stubble.Extensions.Loaders.Tests.csproj", new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true,
            Verbose = false,
            Framework = testFramework,
            ArgumentCustomization = args =>
                args.Append("--logger:trx")
        });
    };

    if(runCoverage || AppVeyor.IsRunningOnAppVeyor)
    {
        var path = new FilePath("./OpenCover-Experimental/OpenCover.Console.exe").MakeAbsolute(Context.Environment);

        Information(path.ToString());

        CreateDirectory("./coverage-results/");
        OpenCover(
            testAction,
            new FilePath(string.Format("./coverage-results/results.xml", DateTime.UtcNow)),
            new OpenCoverSettings {
                Register = "path32",
                SkipAutoProps = true,
                OldStyle = true,
                ToolPath = path,
                ReturnTargetCodeOffset = 0
            }
            .WithFilter("+[Stubble.Extensions.Loaders]*")
        );

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach(var file in GetFiles("./test/Stubble.Extensions.Loaders.Tests/TestResults/*"))
            {
                AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.MSTest);
                AppVeyor.UploadArtifact(file);
            }
        }
    } else {
        testAction(Context);
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

Task("Coveralls")
    .IsDependentOn("Pack")
    .Does(() =>
{
    if (!AppVeyor.IsRunningOnAppVeyor) return;

    var token = EnvironmentVariable("COVERALLS_REPO_TOKEN");

    CoverallsNet("./coverage-results/results.xml", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
    {
        RepoToken = token,
        CommitId = EnvironmentVariable("APPVEYOR_REPO_COMMIT"),
        CommitBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH"),
        CommitAuthor = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
        CommitEmail = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"),
        CommitMessage = EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE")
    });
});

Task("CoverageReport")
    .IsDependentOn("Test")
    .Does(() =>
{
    ReportGenerator("./coverage-results/*.xml", "./coverage-report/");
});

Task("AppVeyor")
    .IsDependentOn("Coveralls");

Task("Travis")
    .IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);