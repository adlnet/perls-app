#!/usr/bin/env cake

// Required namespaces

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Resources.NetStandard;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Cake.Common.Xml;
using Cake.Common.Tools.XUnit;
using Cake.Git.Clog;
using GoogleApi;
using GoogleApi.Entities.Translate.Common.Enums.Extensions;
using GoogleApi.Entities.Translate.Translate.Response;
using LibGit2Sharp;

// Tools and addins

#addin nuget:?package=Cake.Android.Adb&version=3.2.0
#addin nuget:?package=Cake.Android.AvdManager&version=2.2.0
#addin nuget:?package=Cake.AppCenter&version=2.0.1
#addin nuget:?package=Cake.AppleSimulator&version=0.2.0
#addin nuget:?package=Cake.AWS.S3&version=1.0.0&loaddependencies=true
#addin nuget:?package=Cake.Badge&version=1.0.0.2
#addin nuget:?package=Cake.Coverlet&version=2.5.4
#addin nuget:?package=Cake.DotNetTool.Module&version=1.1.0
#addin nuget:?package=Cake.FileHelpers&version=4.0.1
#addin nuget:?package=Cake.Git.Clog&version=1.0.0.2
#addin nuget:?package=Cake.Json&version=6.0.1
#addin nuget:?package=Cake.Plist&version=0.7.0
#addin nuget:?package=GoogleApi&version=3.10.9
#tool nuget:?package=LibGit2Sharp.NativeBinaries&version=2.0.315-alpha.0.9
#addin nuget:?package=LibGit2Sharp&version=0.27.0-preview-0175
#addin nuget:?package=Newtonsoft.Json&version=12.0.2
#addin nuget:?package=Portable.BouncyCastle&version=1.8.1.3
#addin nuget:?package=ResXResourceReader.NetStandard&version=1.0.1
#tool nuget:?package=GitVersion.CommandLine&version=5.0.1
#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1
#tool nuget:?package=xunit.runner.console&version=2.4.1

// Enum options

enum Configuration
{
    Debug,
    Release,
}

enum Flavor
{
    Dev,
    CI,
    Canary,
    Release,
}

enum Platform
{
    Android,
    iPhone,
    iPhoneSimulator,
}

public class BuildFlagsConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (!(value is string stringValue))
        {
            throw new Exception($"Unsupported source value '{value}' for build flag");
        }
        
        var parsed = stringValue.Split("|");
        var allBuildFlags = Enum.GetValues(typeof(BuildFlags)).Cast<BuildFlags>();
        var providedBuildFlags = parsed.Select(flag => Enum.Parse<BuildFlags>(flag));
        var parsedBuildFlags = (parsed.Contains("All") ? allBuildFlags : providedBuildFlags).Aggregate(BuildFlags.None, (a, b) => a |= b);
        return parsedBuildFlags;
    }
}

[Flags]
[TypeConverter(typeof(BuildFlagsConverter))]
enum BuildFlags
{
    // No flags.
    None = 0,
    // Whether or not to show the set server screen.
    TeamEntry = 1 << 0,
    // Whether or not to enable offline cached providers.
    OfflineAccess = 1 << 1,
    // Whether or not to enable the media tab.
    Podcasts = 1 << 2,
    // Whether or not to show the terms of use page.
    TermsOfUse = 1 << 3,
    // Whether or not to enable the feedback screen.
    FeedbackAccess = 1 << 4,
    // Whether or not to enable the stats subtab.
    StatsAccess = 1 << 5,
    // Whether or not to enable the interests subtab.
    InterestsAccess = 1 << 6,
    // Whether or not to enable the support setting.
    SupportSetting = 1 << 7,
    // Whether to prefer accessing the app with a local account.
    PrefersLocalAuthentication = 1 << 8,
    // Whether or not to enable the notes subtab.
    NotesAccess = 1 << 9,
    // Whether or not to enable the groups setting.
    GroupsSetting = 1 << 10,
    // Whether or not to enable the account option in settings.
    AccountSetting = 1 << 11,
    // Whether or not to have tag following.
    TagFollowing = 1 << 12,
    // Whether or not to have the privacy policy.
    PrivacyPolicySetting = 1 << 13,
    // Downloads, enables, and compiles against the UnityFramework file.
    EnableUnityFramework = 1 << 14,
}

enum APSEnvironment
{
    Development,
    Production,
}

enum AndroidPackageFormat
{
    Apk,
    Aab,
}

enum AndroidLinkTool
{
    None,
    Proguard,
    R8,
}

// Constants

var defaultGuid = new Guid().ToString();
const string GitHashKey = "GIT_HASH";
const string VersionKey = "VERSION";
const string BundleHashKey = "BUNDLE_HASH";
const string BundleSizeKey = "BUNDLE_SIZE";
const string readableReaderLine = "<value>System.Resources.NetStandard.ResXResourceReader, System.Resources.NetStandard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</value>";
const string appReadyReaderLine = "<value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>";
const string readableWriterLine = "<value>System.Resources.NetStandard.ResXResourceWriter, System.Resources.NetStandard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</value>";
const string appReadyWriterLine = "<value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>";

// Handles an argument provided to the script as an argument (e.g. `./build.sh --example=flag`) OR as a YAML property
// If the `yamlFile` parameter is null or whitespace, will default to only return a script argument
// If a path to a YAML file is provided, this will look for a key with a matching name and then attempt
// to return the value associated with that key using a type converter to match the desired type.
T ArgumentOrYaml<T>(string name, T defaultValue, string yamlFile = null)
{
    if (string.IsNullOrWhiteSpace(yamlFile))
    {
        return Argument(name, defaultValue);
    }

    if (!FileExists(yamlFile))
    {
        throw new Exception($"Unable to locate YAML file {yamlFile}");
    }

    var lines = FileReadLines(yamlFile);

    foreach (var line in lines)
    {
        // ignore comments
        if (line.StartsWith('#'))
        {
            continue;
        }

        var split = line.Split(':', 2);

        if (split.Count() != 2)
        {
            Warning($"Ignoring line {line}");
            continue;
        }

        var key = split[0].Trim();
        var val = split[1].Trim();

        // handle environment variables defined in YAML
        if (val.StartsWith("${") && val.EndsWith("}"))
        {
            var envName = val.TrimStart(new [] { '$', '{' }).TrimEnd('}');
            val = Environment.GetEnvironmentVariable(envName);

            if (string.IsNullOrWhiteSpace(val))
            {
                throw new Exception($"Failed to read environment variable {envName}");
            }
        }

        if (key == name)
        {
            try
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(val);
            }
            catch
            {
                throw new Exception($"Failed to convert \"{val}\" with key \"{key}\"");
            }
        }
    }

    Debug($"Unable to find property {name} in {yamlFile}");
    return Argument(name, defaultValue);
}

// Parameters and arguments

// A path to a YAML file containing arguments to parse.
readonly string yaml = Argument(nameof(yaml), string.Empty);

// The task to perform; see the <code>Task()</code> top-level invocations below to see what the options are.
readonly string task = ArgumentOrYaml(nameof(task), "Build", yaml);

// Whether to build in Debug or Release. This is generally passed on to MSBuild or dotnet.
readonly Configuration configuration = ArgumentOrYaml(nameof(configuration), Configuration.Debug, yaml);

// The build flavor, which is injected into the app Constants file and is used for the App Center URL slug.
readonly Flavor flavor = ArgumentOrYaml(nameof(flavor), Flavor.Dev, yaml);

// Determines the build platform, see options in the <code>Platform</code> enum above.
readonly Platform platform = ArgumentOrYaml(nameof(platform), Platform.iPhone, yaml);

/// Determines the marketing to apply; this will copy the contents of the Assets folder onto the repo.
/// When marketing is set to <code>Marketing.None</code>, the marketing task is effectively skipped, and no copying is performed.
readonly string marketing = ArgumentOrYaml(nameof(marketing), string.Empty, yaml);

// Specifies features that should be enabled in the built application.
readonly BuildFlags buildFlags = ArgumentOrYaml(nameof(buildFlags), BuildFlags.None, yaml);

/// The display name of the application.
readonly string appName = ArgumentOrYaml(nameof(appName), "App", yaml);

// The name of the project. Used for App Center download link environment variables and as part of the App Center project name.
readonly string projectName = ArgumentOrYaml(nameof(projectName), string.Empty, yaml);

// Release notes to upload to AppCenter.
// CI builds will prefix this with the git branch for pruning purposes. See the DeployCleanup task.
readonly string releaseNotes = ArgumentOrYaml(nameof(releaseNotes), string.Empty, yaml);

// The default server with which the app should communicate.
readonly string defaultServer = ArgumentOrYaml(nameof(defaultServer), "https://example.com/", yaml);

// The OAuth client ID for the server.
readonly string clientId = ArgumentOrYaml(nameof(clientId), string.Empty, yaml);

// The OAuth client secret for the server.
readonly string clientSecret = ArgumentOrYaml(nameof(clientSecret), string.Empty, yaml);

// The package ID to set in the plist or manifest for the app bundle.
readonly string packageIdentifier = ArgumentOrYaml(nameof(packageIdentifier), "com.example.app", yaml);

// Used just for the Android version code.
readonly int buildNumber = ArgumentOrYaml(nameof(buildNumber), 1, yaml);

// The name of the App Center URL slug to which to upload. If left empty, this script assumes that the slug is in project-flavor-platform format.
string appCenterApp = ArgumentOrYaml(nameof(appCenterApp), string.Empty, yaml);

// The App Center team prefix (part of the app URL). Unlikely to change.
readonly string appCenterPrefix = ArgumentOrYaml(nameof(appCenterPrefix), string.Empty, yaml);

// The group name to which to share this release in App Center.
// For store releases, this should be set to the store group, e.g. "App Store Connect Users"
readonly string appCenterGroupName = ArgumentOrYaml(nameof(appCenterGroupName), string.Empty, yaml);

// Path to a file to which post-build environment variables should be written.
readonly string envInjectFile = ArgumentOrYaml(nameof(envInjectFile), "build/app.properties", yaml);

// Whether or not to ignore some errors in the build process. Mostly for debugging.
readonly bool force = ArgumentOrYaml(nameof(force), false, yaml);

// Whether or not to upload to an app store during the deploy task.
readonly bool uploadToStore = ArgumentOrYaml(nameof(uploadToStore), false, yaml);

// The name of the codesign key to use when building for iOS.
readonly string codesignKey = ArgumentOrYaml(nameof(codesignKey), string.Empty, yaml);

// The name of the codesign provision to use when building for iOS.
readonly string codesignProvision = ArgumentOrYaml(nameof(codesignProvision), string.Empty, yaml);

// The folder to which bundles should be built, relative to the root directory.
readonly string buildFolder = ArgumentOrYaml(nameof(buildFolder), "build", yaml);

// Whether or not to use the shared runtime. Should be true for debug builds, false for release.
readonly bool androidUseSharedRuntime = ArgumentOrYaml(nameof(androidUseSharedRuntime), true, yaml);

// Whether or not to embed assemblies into the APK. This will almost always be true.
readonly bool embedAssembliesIntoApk = ArgumentOrYaml(nameof(embedAssembliesIntoApk), true, yaml);

// Whether or not to skip determining the App Center ID, even when not set.
readonly bool skipAppCenterId = ArgumentOrYaml(nameof(skipAppCenterId), false, yaml);

// Whether or not to completely restore git state after execution completes. Moderately dangerous!
readonly bool gitReset = ArgumentOrYaml(nameof(gitReset), false, yaml);

// Whether or not to badge app icons. Probably enabled for CI/Canary, and disabled for production.
readonly bool badgeIcons = ArgumentOrYaml(nameof(badgeIcons), false, yaml);

// Whether or not to badge in-app graphics (icon_no_shadow*). Requires 'badgeIcons' to be enabled also.
readonly bool inAppBadges = ArgumentOrYaml(nameof(inAppBadges), false, yaml);

// Enable to read the changelog start from an existing environment injection file.
readonly bool startChangelogFromEnvInject = ArgumentOrYaml(nameof(startChangelogFromEnvInject), false, yaml);

// Enable to skip configuring Firebase files for this build.
readonly bool skipFirebase = ArgumentOrYaml(nameof(skipFirebase), false, yaml);

// Enable to skip running tests on build.
readonly bool skipTests = ArgumentOrYaml(nameof(skipTests), false, yaml);

// Enable writing the git hash to the environment injection file.
// Disabled in build configurations that require a previous hash for the changelog and don't want to overwrite it.
readonly bool writeGitHash = ArgumentOrYaml(nameof(writeGitHash), true, yaml);

// Set to skip the clean step.
readonly bool noClean = ArgumentOrYaml(nameof(noClean), false, yaml);

// Set to skip the restore step.
readonly bool noRestore = ArgumentOrYaml(nameof(noRestore), false, yaml);

// The ref from which to start the changelog.
string changelogStart = ArgumentOrYaml(nameof(changelogStart), string.Empty, yaml);

// The ref to which to end the changelog.
string changelogEnd = ArgumentOrYaml(nameof(changelogEnd), string.Empty, yaml);

// The location of the changelog file to write.
readonly string changelogFile = ArgumentOrYaml(nameof(changelogFile), string.Empty, yaml);

// Specifies the filename of the keystore file. If this is specified, it's assumed you'll provide the other signing properties.
readonly string androidSigningKeyStore = ArgumentOrYaml(nameof(androidSigningKeyStore), string.Empty, yaml);

// Specifies the alias for the key in the keystore.
readonly string androidSigningKeyAlias = ArgumentOrYaml(nameof(androidSigningKeyAlias), string.Empty, yaml);

// Specifies the password to the Android keystore.
readonly string androidSigningStorePass = ArgumentOrYaml(nameof(androidSigningStorePass), string.Empty, yaml);

// Specifies the password of the key within the keystore file.
readonly string androidSigningKeyPass = ArgumentOrYaml(nameof(androidSigningKeyPass), string.Empty, yaml);

// Specifies the APS environment to include in entitlements.
readonly APSEnvironment apsEnvironment = ArgumentOrYaml(nameof(apsEnvironment), APSEnvironment.Development, yaml);

// The valid localizations for this build.
readonly string[] localizations = ArgumentOrYaml(nameof(localizations), string.Empty, yaml).Split(",");

// The Google API Key, used for translations.
readonly string googleAPIKey = ArgumentOrYaml(nameof(googleAPIKey), string.Empty, yaml);

// The  App Center token for uploading events, crashes, and so on. Injected into the application at build time.
// If this value is left as `defaultGuid`, it will be determined during the AppCenterId task.
string appCenterId = ArgumentOrYaml(nameof(appCenterId), defaultGuid, yaml);

// The token to use when uploading apps to App Center.
readonly string appCenterToken = ArgumentOrYaml(nameof(appCenterToken), string.Empty, yaml);

// The root Android SDK folder. Sometimes Cake can't find it on build servers.
readonly string androidSdkRoot = ArgumentOrYaml(nameof(androidSdkRoot), string.Empty, yaml);

// The Android package format. APK is still most common, but AAB bundles can be used on Google Play.
readonly AndroidPackageFormat androidPackageFormat = ArgumentOrYaml(nameof(androidPackageFormat), AndroidPackageFormat.Apk, yaml);

// The Android link tool. R8 is newer & better, but Proguard is safer. If none, no DEX linking is performed.
readonly AndroidLinkTool androidLinkTool = ArgumentOrYaml(nameof(androidLinkTool), AndroidLinkTool.None, yaml);

// The name of the company to include in assemblies.
readonly string companyName = ArgumentOrYaml(nameof(companyName), string.Empty, yaml);

// The copyright to include in assemblies.
readonly string copyright = ArgumentOrYaml(nameof(copyright), string.Empty, yaml);

// The path that is opened when the user taps on the Support option in settings.
readonly string supportPath = ArgumentOrYaml(nameof(supportPath), "https://example.com/", yaml);

// The path to a legal info page.
readonly string legalInfoPath = ArgumentOrYaml(nameof(legalInfoPath), string.Empty, yaml);

// This value will be included in the user agent string for web requests.
readonly string userAgent = ArgumentOrYaml(nameof(userAgent), string.Empty, yaml);

// The server to be pre-filled on debug builds.
readonly string debugDefaultServer = ArgumentOrYaml(nameof(debugDefaultServer), string.Empty, yaml);

// The redirect URI for OAuth.
readonly string oauthRedirectUri = ArgumentOrYaml(nameof(oauthRedirectUri), "https://example.com/", yaml);

// The allowed host name for URI team names.
readonly string allowedHost = ArgumentOrYaml(nameof(allowedHost), "https://example.com", yaml);

// The default reachability URI.
readonly string defaultReachabilityUri = ArgumentOrYaml(nameof(defaultReachabilityUri), "https://example.com", yaml);

// The expected Unity message.
readonly string unityMessageLabel = ArgumentOrYaml(nameof(unityMessageLabel), "ExampleUnityMessage", yaml);

// The access key for downloading remote files from AWS S3.
readonly string awsAccessKey = ArgumentOrYaml(nameof(awsAccessKey), string.Empty, yaml);

// The secret key for downloading remote files from AWS S3.
readonly string awsSecretKey = ArgumentOrYaml(nameof(awsSecretKey), string.Empty, yaml);

// The bucket in AWS S3 from which to download remote files.
readonly string awsBucketName = ArgumentOrYaml(nameof(awsBucketName), string.Empty, yaml);

// The files in AWS S3 that should be downloaded to the native references folder.
readonly string[] awsFrameworkFilenames = FilterNullOrWhiteSpace(ArgumentOrYaml(nameof(awsFrameworkFilenames), string.Empty, yaml).Split(",")).ToArray();

// The files in AWS S3 that should be downloaded to the assets folder.
readonly string[] awsAssetFilenames = FilterNullOrWhiteSpace(ArgumentOrYaml(nameof(awsAssetFilenames), string.Empty, yaml).Split(",")).ToArray();

// A flag for testing that ensures the constants file will be unchanged.
readonly bool changeNoConstants = ArgumentOrYaml(nameof(changeNoConstants), false, yaml);

// Derived global parameters

var root = MakeAbsolute(new DirectoryPath("./"));
var isReleaseBuild = configuration == Configuration.Release;
var isApplePlatform = platform == Platform.iPhone || platform == Platform.iPhoneSimulator;
var solution = GetFiles($"./*.sln").First();
var folder = new {
    Apple = "PERLS.iOS",
    Android = "PERLS.Android",
    Assets = "Assets",
    Core = "PERLS",
    Data = "PERLS.Data",
    Impl = "PERLS.DataImplementation",
    Test = $"PERLS.Tests",
    Temp = $"tmp",
    Unity = "UnityLibrary.iOS"
};
var project = new {
    Apple = $"./{folder.Apple}/PERLS.iOS.csproj",
    Android = $"./{folder.Android}/PERLS.Android.csproj",
    Core = $"./{folder.Core}/PERLS.csproj",
    Data = $"./{folder.Data}/PERLS.Data.csproj",
    Impl = $"./{folder.Impl}/PERLS.DataImplementation.csproj",
    Test = $"./{folder.Test}/PERLS.Tests.csproj",
    Unity = $"./{folder.Unity}/UnityLibrary.iOS.csproj"
};
var extension = isApplePlatform 
    ? "ipa"
    : $"{androidPackageFormat}".ToLower();
var buildPath = root.Combine(new DirectoryPath(buildFolder));
var netAssemblyInfoLocation = File($"./{(isApplePlatform ? folder.Apple : folder.Android)}/Properties/AssemblyInfo.cs");
var appCenterNewline = "\r\n\r\n"; // I'm not sure why, but CRLF x2 is required (specifically for install.appcenter.ms)
var assemblyName = ParseProject(isApplePlatform ? project.Apple : project.Android).AssemblyName;

// Parameters to be defined later

var appVersion = "0.0.0";
var assemblyVersion = "0.0.0.0";
var packageVersion = "0.0.0.0-feat";
var fullSemVersion = "0.0.0-feat.1+1";
var infoVersion = "0.0.0-feat.1+1.Sha";
var branchName = string.Empty;
var changelog = string.Empty;
var gitSha = string.Empty;
var injectedEnvironmentVariables = new Dictionary<string, string>();
FilePath symbolsArchivePath = null;
FilePath bundlePath = null;

// Global methods

string EscapeNewLines(string str) => str.Replace("\n", "\\n");
string UnescapeNewLines(string str) => str.Replace("\\n", "\n");

IEnumerable<KeyValuePair<string, string>> ParseEnvInject(string filename)
{
    var readLines = FileReadLines(filename);
    var result = new Dictionary<string, string>();
    string previousKey = null;

    foreach (var line in readLines)
    {
        var split = line.Split("=");

        if (split.Count() != 2)
        {
            if (string.IsNullOrWhiteSpace(previousKey))
            {
                Warning($"Unexpected format for line: {line}");
            }
            else
            {
                result[previousKey] = $"{result[previousKey]}\n{UnescapeNewLines(line)}";
            }
            
            continue;
        }
        
        previousKey = split[0];
        result[split[0]] = UnescapeNewLines(split[1]);
    }

    return result;
}

IEnumerable<StatusEntry> RepoFileStatuses(string folder)
{
    var repo = new Repository($"{root}");
            
    foreach (var item in repo.RetrieveStatus())
    {
        yield return item;
    }
}

// The next two methods are useful for getting the Resx files to be able to be read by the ResxResourceReader nuget.
// If these are not used, there will be build errors.
string PrepareResxStringForTranslation(string resx)
{
    var readResx = resx.Replace(appReadyReaderLine, readableReaderLine);
    return readResx.Replace(appReadyWriterLine, readableWriterLine);
}

string PrepareResxStringForApp(string resx)
{
    var readResx = resx.Replace(readableReaderLine, appReadyReaderLine);
    return readResx.Replace(readableWriterLine, appReadyWriterLine);
}

IEnumerable<Translation> TranslateSingleBlock(IEnumerable<string> toTranslate, string targetLanguage, bool useNeuralMachineTranslation)
{
    if (toTranslate.Count() > 128)
    {
        throw new Exception("Limit each translation block to 128 strings or less.");
    }

    var request = new GoogleApi.Entities.Translate.Translate.Request.TranslateRequest
    {
        Target = targetLanguage.FromCode(),
        Qs = toTranslate,
        Source = GoogleApi.Entities.Translate.Common.Enums.Language.English,
        Key = googleAPIKey,
        Model = useNeuralMachineTranslation 
            ? GoogleApi.Entities.Translate.Common.Enums.Model.Nmt
            : GoogleApi.Entities.Translate.Common.Enums.Model.Base,
    };

    var response = GoogleTranslate.Translate.Query(request);

    if (response.Data.Translations is not ICollection<Translation> translationsList)
    {
        throw new Exception("Unexpected API response");
    }

    if (translationsList.Count() != toTranslate.Count())
    {
        throw new Exception($"The number of returned translations {translationsList.Count()} does not equal the number of requested translations {toTranslate.Count()}. This is likely due to a duplicate string for two separate keys.");
    }

    return translationsList;
}

IEnumerable<Translation> TranslateMultipleBlocks(IEnumerable<string> text, string targetLanguage)
{
    var currentTranslations = new List<Translation>();
    var toTranslate = text;

    while (toTranslate.Count() > 0)
    {
        var thisBlock = toTranslate.Take(128);
        toTranslate = toTranslate.Skip(128);

        try
        {
            var result1 = TranslateSingleBlock(thisBlock, targetLanguage, true);
            currentTranslations.AddRange(result1);
        }
        catch
        {
            var result2 = TranslateSingleBlock(thisBlock, targetLanguage, false);
            currentTranslations.AddRange(result2);
        }
    }

    return currentTranslations;
}

string TrimStart(string str, string toTrim)
{
    var output = str;

    while (output.StartsWith(toTrim))
    {
        output = output.Substring(toTrim.Length);
    }

    return output;
}

string MakeRelative(string path)
{
    return TrimStart(path, $"{root}");
}

bool DirectoriesExist(IEnumerable<string> directories)
{
    foreach (var dir in directories)
    {
        if (!DirectoryExists(dir))
        {
            return false;
        }
    }

    return true;
}

bool FilesExist(IEnumerable<string> files)
{
    foreach (var file in files)
    {
        if (!FileExists(file))
        {
            return false;
        }
    }

    return true;
}

IEnumerable<string> FilterNullOrWhiteSpace(IEnumerable<string> source)
{
    foreach (var el in source)
    {
        if (!string.IsNullOrWhiteSpace(el))
        {
            yield return el;
        }
    }
}

// a replacement for an identical function in Cake.Git (not ARM supported)
bool GitHasUncommitedChanges(DirectoryPath path)
{
    return new Repository($"{path}")
        .RetrieveStatus()
        .IsDirty;
}

// a replacement for a comparable class in Cake.Git (not ARM supported)
class GitCommit
{
    public GitCommit(Commit commit)
    {
        Sha = commit.Sha;
    }

    public string Sha { get; }
}

// a replacement for an identical function in Cake.Git (not ARM supported)
ICollection<GitCommit> GitLog(string path, int count)
{
    return new Repository($"{path}")
        .Commits
        .Take(count)
        .Select(commit => new GitCommit(commit))
        .ToList();
}

// Tasks

// Cleans folders containing cached data.
Task("Clean")
    .Does(() =>
    {
        if (noClean)
        {
            Warning("Skipping clean step.");
            return;
        }

        CleanDirectories(GetDirectories("./.vs"));
        CleanDirectories(GetDirectories("./**/obj"));
        CleanDirectories(GetDirectories("./**/bin"));
    });

// Restores NuGet packages.
Task("RestorePackages")
    .Does(() =>
    {
        if (noRestore)
        {
            Warning("Skipping restore step.");
            return;
        }

        NuGetRestore(solution);
    });

// Copies resources from a folder within the Assets folder to rebrand the application.
Task("Marketing")
    .Does(() =>
    {
        if (string.IsNullOrWhiteSpace(marketing))
        {
            Information("Marketing disabled. Skipping.");
            return;
        }

        if (GitHasUncommitedChanges(root) && !force)
        {
            if (flavor == Flavor.CI)
            {
                Warning("Ignoring uncommitted changes for CI build.");
            }
            else
            {
                Information("Locally modified files:");
            
                foreach (var item in RepoFileStatuses($"{root}"))
                {
                    if (item.State != FileStatus.Ignored)
                    {
                        Information($"    {item.FilePath}:  {item.State}");
                    }
                }

                throw new Exception("Marketing targets can only be changed on a clean git repo.");
            }
        }

        // allow fully-qualified paths
        var sourcePath = marketing.Contains("Assets")
            ? new DirectoryPath(marketing) 
            : new DirectoryPath($"./Assets/{marketing}");

        if (!DirectoryExists(sourcePath))
        {
            throw new Exception($"Unable to find source path \"{sourcePath}\"");
        }

        CopyDirectory(sourcePath, root);
        Information($"Applied marketing {marketing} from {sourcePath} to {root}");

        foreach (var item in RepoFileStatuses($"{root}"))
        {
            if (item.State == FileStatus.NewInWorkdir 
                && !item.FilePath.StartsWith(folder.Assets)
                && !item.FilePath.EndsWith(".resx"))
            {
                throw new Exception($"Unexpected new file in work dir after applying marketing: {item.FilePath}");
            }
        }
    });


// Runs machine translation of the strings files for the desired localizations
Task("MachineTranslation")
    .IsDependentOn("RestorePackages")
    .Does(() =>
    {
        // Make sure we are requesting localizations
        if (!localizations.Any() || (localizations.Count() == 1 && string.IsNullOrWhiteSpace(localizations[0])))
        {
            // If there are no localizations we want to default to our English-only version.
            Information("Localization disabled. Skipping.");
            return;
        }

        // Load in the proper strings files
        // For now we'll translate any Strings.resx or StringsSpecific.resx.
        var stringsFiles = System.IO.Directory.GetFiles(folder.Data, "Strings.resx");
        var stringsSpecificFiles = System.IO.Directory.GetFiles(folder.Data, "StringsSpecific.resx");
        var resxFiles = stringsFiles.Concat(stringsSpecificFiles);
        
        Information($"Found {resxFiles.Count()} Strings files");

        var allFiles = new Dictionary<string, List<DictionaryEntry>>();

        foreach (var eachFile in resxFiles)
        {
            Information($"Reading {eachFile}");
            
            var entries = new List<DictionaryEntry>();

            var path = $"./{eachFile}";
            var resxText = System.IO.File.ReadAllText(path);
            var readableText = PrepareResxStringForTranslation(resxText);
            System.IO.File.WriteAllText(path, readableText);

            using (var resxReader = new ResXResourceReader(path))
            {
                foreach (DictionaryEntry entry in resxReader) 
                {
                    entries.Add(entry);
                }
            }

            allFiles.Add(eachFile, entries);

            // After we've created it we need to re-prepare it for the app.
            var readingText = System.IO.File.ReadAllText(path);
            var appReadyText = PrepareResxStringForApp(readingText);
            System.IO.File.WriteAllText(path, appReadyText);
        }

        // Loop through each language desired and get the translations
        foreach (var eachLocalization in localizations)
        {
            var targetLanguage = (string)eachLocalization;

            // Skip over english as that is the base language.
            if (targetLanguage == "en")
            {
                continue;
            }

            // A quick fix to better support our nuget: https://github.com/vivet/GoogleApi/blob/0554046cbd6f4c9b0b6e72a6a41bf5dcd35aca18/GoogleApi/Entities/Common/Enums/Extensions/StringExtension.cs
            if (eachLocalization == "es-MX")
            {
                targetLanguage = "es-419";
            }

            // Get the localization for this language
            // Loop through each of the files.
            foreach (var eachFile in allFiles)
            {
                // Create new set
                var thisTranslationSet = new List<DictionaryEntry>();
                var entries = eachFile.Value;
                var allKeys = entries.Select(arg => (string)arg.Key);
                var allStrings = entries.Select(arg => (string)arg.Value);

                var translations = TranslateMultipleBlocks(allStrings, targetLanguage);

                for (int i = 0; i < translations.Count(); i++)
                {
                    thisTranslationSet.Add(new DictionaryEntry(allKeys.ElementAt(i), System.Net.WebUtility.HtmlDecode(translations.ElementAt(i).TranslatedText)));
                }

                // Save the file
                var translatedFilePath = $"./{eachFile.Key.Replace(".resx", $".{eachLocalization}.resx")}";

                using (var resxWriter = new ResXResourceWriter(translatedFilePath))
                {
                    foreach (var entry in thisTranslationSet) 
                    {
                        resxWriter.AddResource((string)entry.Key, (string)entry.Value);
                    }

                    resxWriter.Generate();

                    // After we've created it we need to re-prepare it for the app.
                    var readingText = System.IO.File.ReadAllText(translatedFilePath);
                    var appReadyText = PrepareResxStringForApp(readingText);
                    System.IO.File.WriteAllText(translatedFilePath, appReadyText);
                }
            }
        }

        Information("Machine translations complete.");
    });

Task("Localization")
    .Does(() =>
    {
        if (!localizations.Any() || (localizations.Count() == 1 && string.IsNullOrWhiteSpace(localizations[0])))
        {
            // If there are no localizations we want to default to our English-only version.
            Information("Localization disabled. Skipping.");
            return;
        }

        // Set the proper data in the plist
        var info = File($"./{folder.Apple}/Info.plist");
        dynamic infoData = DeserializePlist(info);
        var languages = localizations;
        infoData["CFBundleLocalizations"] = languages.Append("en").ToArray();
        SerializePlist(info, infoData);

        Information("Localizations complete.");
    });

// Retrieves version information using GitVersion, and applies that information to the AssemblyInfo file, plists, and/or Android manifest.
Task("GitVersion")
    .Does(() =>
    {
        GitVersion gitVersion;

        try 
        {
            gitVersion = GitVersion(new GitVersionSettings
            {
                NoFetch = true,
            });
        }
        catch
        {
            Warning("Failed to assign GitVersion. Version data will not be included in assembly.");
            return;
        }
        
        assemblyVersion = gitVersion.AssemblySemVer;
        packageVersion = gitVersion.NuGetVersion;
        appVersion = gitVersion.MajorMinorPatch;
        fullSemVersion = gitVersion.FullSemVer;
        infoVersion = gitVersion.InformationalVersion;
        branchName = gitVersion.BranchName;
        gitSha = gitVersion.Sha;
        injectedEnvironmentVariables[VersionKey] = fullSemVersion;

        Information($"AssemblySemVer: {assemblyVersion}");
        Information($"NuGetVersion: {packageVersion}");
        Information($"AppVersion: {appVersion}");
        Information($"InformationalVersion: {infoVersion}");
        Information($"FullSemVer: {fullSemVersion}");
        Information($"Branch: {branchName}");

        var visible = isReleaseBuild 
            ? new string[] {}
            : new [] { folder.Test };

        CreateAssemblyInfo(netAssemblyInfoLocation, new AssemblyInfoSettings
        {
            Version = assemblyVersion,
            FileVersion = gitVersion.AssemblySemFileVer,
            InformationalVersion = gitVersion.InformationalVersion,
            ComVisible = false,
            InternalsVisibleTo = visible,
            Company = companyName,
            Configuration = $"{configuration}",
            Title = assemblyName,
            Product = assemblyName,
            Copyright = copyright,
            Description = projectName,
            CustomAttributes = new []
            {
                new AssemblyInfoCustomAttribute
                {
                    NameSpace = "System.Resources",
                    Name = "NeutralResourcesLanguage",
                    Value = "en",
                },
            },
        });
    });

Task("WriteManifestInfo")
    .IsDependentOn("GitVersion")
    .Does(() =>
    {
        if (isApplePlatform)
        {
            var info = File($"./{folder.Apple}/Info.plist");
            dynamic infoData = DeserializePlist(info);
            infoData["CFBundleShortVersionString"] = uploadToStore ? appVersion : fullSemVersion;
            infoData["CFBundleVersion"] = $"{buildNumber}";
            infoData["CFBundleIdentifier"] = packageIdentifier;
            infoData["CFBundleName"] = appName;
            infoData["CFBundleDisplayName"] = appName;
            SerializePlist(info, infoData);
            
            var entitled = File($"./{folder.Apple}/Entitlements.plist");
            dynamic entitledData = DeserializePlist(entitled);
            entitledData["keychain-access-groups"][0] = $"$(AppIdentifierPrefix){packageIdentifier}";
            entitledData["aps-environment"] = $"{apsEnvironment}".ToLower();
            SerializePlist(entitled, entitledData);

            var settings = File($"./{folder.Apple}/Settings.bundle/Root.plist");
            dynamic settingsData = DeserializePlist(settings);
            dynamic prefData = settingsData["PreferenceSpecifiers"];
            dynamic versionData = prefData[0];
            versionData["DefaultValue"] = fullSemVersion;
            dynamic buildData = prefData[1];
            buildData["DefaultValue"] = $"{buildNumber}";
            SerializePlist(settings, settingsData);
        }
        else
        {
            var manifest = File($"./{folder.Android}/Properties/AndroidManifest.xml");
            var settings = new XmlPokeSettings
            {
                Namespaces = new Dictionary<string, string>
                {
                    { "android", "http://schemas.android.com/apk/res/android" }
                },
            };

            XmlPoke(manifest, "manifest/@android:versionCode", $"{buildNumber}", settings);
            XmlPoke(manifest, "manifest/@android:versionName", uploadToStore ? appVersion : fullSemVersion, settings);
            XmlPoke(manifest, "manifest/application/@android:label", appName, settings);
            XmlPoke(manifest, "manifest/@package", packageIdentifier);
        }
    });

Task("WriteGitHash")
    .IsDependentOn("GitVersion")
    .Does(() =>
    {
        if (writeGitHash || task == "WriteGitHash")
        {
            injectedEnvironmentVariables[GitHashKey] = gitSha;
            Information($"Will write git hash: {gitSha}");
        }
        else
        {
            Information($"Nothing to do.");
        }
    });

// If the AppCenter IDs are not set, this retrieves them from the AppCenter API.
// This assumes that the AppCenter URL slug is in standard project-flavor-platform format.
// If the URL slugs are not standardized, you will have to pass in the value using the appCenterId flag.
Task("AppCenterId")
    .Does(() =>
    {
        if (skipAppCenterId)
        {
            Information("Skipping");
            return;
        }

        if (appCenterId != defaultGuid)
        {
            Information("AppCenter ID already set, skipping");
            return;
        }

        string GetAppSecret(string pre, string app, string token)
        {
            Information($"App: {pre}/{app}");

            var appDetails = AppCenterAppsShowWithResult(new AppCenterAppsShowSettings
            {
                App = $"{pre}/{app}",
                Token = token,
            });

            foreach (var detail in appDetails)
            {
                if (detail.StartsWith("App Secret:"))
                {
                    return detail.Substring("App Secret:".Length).Trim();
                }
            }

            throw new Exception($"No app ID found for {pre}/{app}");
        }

        if (string.IsNullOrWhiteSpace(appCenterApp))
        {
            appCenterApp = $"{projectName}-{flavor}-{(isApplePlatform ? "iOS" : "Android")}";
            Information($"Setting app center app ID to {appCenterApp}");
            try 
            {
                appCenterId = GetAppSecret(appCenterPrefix, appCenterApp, appCenterToken);
            }
            catch
            {
                throw new Exception("AppCenter ID cannot be set, use --skipAppCenterId to bypass.");
            }
             
            Information($"AppCenter ID is {appCenterId}");
        }
        else
        {
            Information($"App: {appCenterPrefix}/{appCenterApp}");

            var appDetails = AppCenterAppsShowWithResult(new AppCenterAppsShowSettings
            {
                App = $"{appCenterPrefix}/{appCenterApp}",
                Token = appCenterToken,
            });

            foreach (var detail in appDetails)
            {
                if (detail.StartsWith("App Secret:"))
                {
                    appCenterId = detail.Substring("App Secret:".Length).Trim();
                }
            }

            if (appCenterId == defaultGuid)
            {
                Warning("AppCenter ID has not been set, and will use a default value.");
            }
        }
    });

// Transforms the T4 template for constants into a C# file with values injected such as API tokens and build configuration details.
Task("InjectParameters")
    .IsDependentOn("AppCenterId")
    .Does(() =>
    {
        if (changeNoConstants)
        {
            Information("Skipping");
            return;
        }

        var inputFile = File($"./{folder.Data}/Constants.generator.tt");
        var outputFile = File($"./{folder.Data}/Constants.generated.cs");
        var properties = new Dictionary<string, string>
        {
            { nameof(configuration), $"{configuration}" },
            { nameof(defaultServer), $"{defaultServer}" },
            { nameof(appCenterId), $"{appCenterId}" },
            { nameof(packageIdentifier), $"{packageIdentifier}" },
            { nameof(flavor), $"{flavor}" },
            { nameof(fullSemVersion), fullSemVersion },
            { nameof(appName), appName },
            { nameof(clientId), clientId },
            { nameof(clientSecret), clientSecret },
            { nameof(supportPath), supportPath },
            { nameof(legalInfoPath), legalInfoPath },
            { nameof(userAgent), userAgent },
            { nameof(debugDefaultServer), debugDefaultServer },
            { nameof(oauthRedirectUri), oauthRedirectUri },
            { nameof(allowedHost), allowedHost },
            { nameof(defaultReachabilityUri), defaultReachabilityUri },
            { nameof(unityMessageLabel), unityMessageLabel },
        };

        foreach (BuildFlags flag in Enum.GetValues(typeof(BuildFlags)))
        {
            var flagName = $"{flag}";
            var propertyName = $"{Char.ToLowerInvariant(flagName[0])}{flagName.Substring(1)}";

            if (propertyName == "none")
            {
                continue;
            }

            properties[propertyName] = $"{buildFlags.HasFlag(flag)}".ToLower();
        }

        TransformTemplate(inputFile, new TextTransformSettings
        {
            Properties = properties,
            OutputFile = outputFile,
            ToolPath = MakeAbsolute(new FilePath("./t4")),
        });

        Information($"Injected {properties.Count()} properties from {inputFile} to {outputFile}.");
    });

Task("Badge")
    .IsDependentOn("Marketing")
    .Does(() =>
    {
        if (!badgeIcons)
        {
            Information("Icon badging is disabled. Skipping.");
            return;
        }

        Color shieldColor;

        switch (flavor)
        {
            case Flavor.Canary:
                shieldColor = Color.FromArgb(244, 121, 33);
                break;
            case Flavor.CI:
                shieldColor = Color.FromArgb(63, 128, 195);
                break;
            default:
                shieldColor = Color.FromArgb(142, 142, 155);
                break;
        }

        Badge(new BadgeSettings
        {
            NoBadge = true,
            Shield = new ShieldSettings
            {
                Label = $"{flavor}",
                Message = $"{buildNumber}",
                Color = shieldColor,
            },
            ShieldGravity = Cake.Badge.Gravity.South,
            Glob = isApplePlatform 
                ? $"/{folder.Apple}/Assets.xcassets/AppIcon.appiconset/*.png"
                : $"/{folder.Android}/Resources/mipmap-*/*.png",
            Verbose = true,
        });

        // badging doesn't support vector drawables yet, here's the next best thing
        if (platform == Platform.Android && !string.IsNullOrWhiteSpace(marketing))
        {
            var launcherBackground = File($"./{folder.Android}/Resources/values/ic_launcher_background.xml");
            string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
            XmlPoke(launcherBackground, "resources/color[@name = 'ic_launcher_background']", ColorToHex(shieldColor));
            ReplaceRegexInFiles($"./{folder.Android}/Resources/**/ic_launcher_foreground.xml", "android:fillColor=\"(#[A-Fa-f0-9]+)\"", $"android:fillColor=\"#000000\"");
        }

        if (inAppBadges)
        {
            Badge(new BadgeSettings
            {
                NoBadge = true,
                Shield = new ShieldSettings
                {
                    Label = $"{flavor}",
                    Message = $"{buildNumber}",
                    Color = shieldColor,
                },
                ShieldGravity = Cake.Badge.Gravity.SouthEast,
                ShieldNoResize = true,
                Glob = isApplePlatform 
                    ? $"/{folder.Apple}/Resources/icon_no_shadow*.png"
                    : $"/{folder.Android}/Resources/drawable-*/icon_no_shadow*.webp",
                Verbose = true,
            });
        }
    });

// Loads the correct google services file, based on the package identifier.
Task("Firebase")
    .Does(() => 
    {
        if (skipFirebase)
        {
            Information("Skipping.");
            return;
        }

        var ext = isApplePlatform ? "plist" : "json";
        var source = $"Firebase/{packageIdentifier}.google-services.{ext}";

        if (!FileExists(source))
        {
            throw new Exception($"Failed to find Google Services file matching {packageIdentifier}.");
        }

        var contents = FileReadText(source);

        if (isApplePlatform)
        {
            dynamic plist = DeserializePlist(File(source));
            var bundleId = $"{plist["BUNDLE_ID"]}";
            
            if (bundleId != packageIdentifier)
            {
                throw new Exception($"Incorrect bundle identifier in Firebase configuration file: {bundleId}");
            }
            else
            {
                Information($"Found matching bundle ID {bundleId}");
            }
        }
        else
        {
            var json = ParseJson(contents);
            var packageName = json["client"][0]["client_info"]["android_client_info"]["package_name"].ToString();

            if (packageName != packageIdentifier)
            {
                throw new Exception($"Incorrect package name in Firebase configuration file: {packageName}");
            }
            else
            {
                Information($"Found matching package name {packageName}");
            }
        }

        var suffix = isApplePlatform ? "iOS" : "Android";
        var dest = $"PERLS.{suffix}/google-services.{ext}";
        FileWriteText(dest, contents);

        Information($"Wrote Google Services configuration from {source} to {dest}");
    });

Task("VerifyStyles")
    .Does(() =>
    {
        HashSet<string> ParseDocument(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);

            if (xml.ChildNodes.Count < 2)
            {
                throw new Exception("Not enough child nodes in Styles XAML.");
            }

            var firstNode = xml.ChildNodes[1];
            var styles = new HashSet<string>();

            foreach (XmlNode child in firstNode)
            {
                if (child.Name == "Style")
                {
                    var attr = child.Attributes["x:Key"];

                    if (attr?.Value is string key)
                    {
                        if (styles.Contains(key))
                        {
                            throw new Exception($"Duplicate key in {path} : {key}");
                        }

                        styles.Add(key);
                    }
                }
            }

            return styles;
        }

        var styles = ParseDocument("./PERLS/Resources/Styles.xaml");
        Information($"Found {styles.Count()} styles in base.");

        foreach (var dirs in GetDirectories("./Assets/**/Resources"))
        {
            var file = File($"{dirs}/Styles.xaml");

            if (FileExists(file))
            {
                var thisStyles = ParseDocument($"{file}");

                if (thisStyles.SetEquals(styles))
                {
                    Information($"Found file with matching styles at {MakeRelative(file)}");
                }
                else
                {
                    var missingStyles = styles.Except(thisStyles);
                    var extraStyles = thisStyles.Except(styles);
                    var message = string.Empty;

                    if (missingStyles.Any())
                    {
                        message += $"File at {MakeRelative(file)} is missing styles: {string.Join(", ", missingStyles)}. ";
                    }
                    
                    if (extraStyles.Any())
                    {
                        message += $"File at {MakeRelative(file)} has extra styles: {string.Join(", ", extraStyles)}.";
                    }

                    throw new Exception(message);
                }
            }
        }
    });

// Any special handling for build flags should happen here.
Task("UpdateBuildFlags")
    .Does(async () =>
    {
        var info = File($"./{folder.Apple}/Info.plist");
        dynamic infoData = DeserializePlist(info);
        var list = new List<string>(infoData["UIBackgroundModes"]);
        var isInList = list.Contains("audio");
        var isFlagOn = buildFlags.HasFlag(BuildFlags.Podcasts);

        if (isInList && !isFlagOn)
        {
            Information("Removing audio background mode.");
            list.Remove("audio");
        }
        else if (!isInList && isFlagOn)
        {
            Information("Inserting audio background mode.");
            list.Add("audio");
        }
        else
        {
            Information($"Audio background mode is already {(isInList ? "enabled" : "disabled")}");
        }
        
        infoData["UIBackgroundModes"] = list.ToArray();
        SerializePlist(info, infoData);

        var nativeReferencePath = $"{folder.Unity}/NativeReferences";

        if (!buildFlags.HasFlag(BuildFlags.EnableUnityFramework))
        {
            Information($"Removing Unity framework.");
            CleanDirectory(nativeReferencePath);
            return;
        }

        async Task BulkDownload(IEnumerable<string> localFiles, IEnumerable<string> remoteFiles)
        {
            foreach (var (local, remote) in localFiles.Zip(remoteFiles))
            {
                Information($"Downloading {remote} to {local}");

                await S3Download(local, remote, new DownloadSettings
                {
                    AccessKey = awsAccessKey,
                    SecretKey = awsSecretKey,
                    Region = RegionEndpoint.USEast1,
                    BucketName = awsBucketName
                });
            }
        }

        void VerifyKeys()
        {
            if (string.IsNullOrWhiteSpace(awsAccessKey))
            {
                throw new Exception("AWS access key is required to download assets.");
            }

            if (string.IsNullOrWhiteSpace(awsSecretKey))
            {
                throw new Exception("AWS secret key is required to download assets.");
            }

            if (string.IsNullOrWhiteSpace(awsBucketName))
            {
                throw new Exception("AWS bucket name is required to download assets.");
            }
        }

        if (!awsFrameworkFilenames.Any())
        {
            throw new Exception("AWS framework file names are required to download native frameworks.");
        }

        var localTempPaths = awsFrameworkFilenames.Select(filename => $"./{folder.Temp}/{filename}").ToList();
        var frameworkPaths = awsFrameworkFilenames.Select(filename => $"{nativeReferencePath}/{System.IO.Path.GetFileName(filename).TrimEnd(new [] { '.', 'z', 'i', 'p'})}").ToList();

        if (DirectoriesExist(frameworkPaths))
        {
            Information("Native frameworks already downloaded.");
        }
        else
        {
            if (!FilesExist(localTempPaths))
            {
                VerifyKeys();
                Information("Downloading native frameworks...");
                EnsureDirectoryExists(folder.Temp);
                await BulkDownload(localTempPaths, awsFrameworkFilenames);
            }

            foreach (var path in localTempPaths)
            {
                Unzip(path, nativeReferencePath);
            }

            Information($"Unzipped framework(s) to {nativeReferencePath}");
        }

        if (!awsAssetFilenames.Any())
        {
            throw new Exception("AWS asset file names are required to download assets.");
        }

        // this path is recognized and required by the vuforia library
        // in the future, a more general approach is needed to support other native libraries
        var assetPath = $"{folder.Unity}/Vuforia";
        var assetPaths = awsAssetFilenames.Select(filename => $"{assetPath}/{System.IO.Path.GetFileName(filename)}").ToList();

        if (FilesExist(assetPaths))
        {
            Information("Assets already downloaded.");
        }
        else
        {
            VerifyKeys();
            Information("Downloading assets...");
            EnsureDirectoryExists(assetPath);
            await BulkDownload(assetPaths, awsAssetFilenames);
        }

        // update bundled resources in unity library project file
        XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
        var document = XDocument.Load(project.Unity);

        // filter to item groups
        var itemGroups = from itemGroup in document.Descendants(ns + "ItemGroup") select itemGroup;

        foreach (var element in itemGroups)
        {
            // find the LinkFiles item group
            foreach (var attribute in element.Attributes())
            {
                if (attribute.Name == "Label" && attribute.Value == "LinkFiles")
                {
                    // remove any previous bundled resources
                    element.Descendants(ns + "BundleResource").Remove();

                    // add the bundled resources passed in via argument
                    var tree = awsAssetFilenames
                        .Select(filename => $"Vuforia/{System.IO.Path.GetFileName(filename)}")
                        .Select(path => new XElement(ns + "BundleResource", new XAttribute("Include", path)));

                    element.Add(tree);
                }
            }
        }

        document.Save(project.Unity);

        Information("Bundled resources updated.");
    });

// Builds each project using MSBuild or dotnet, depending on the project type.
// This mostly doesn't create applications, just DLLs. Useful to verify that the build succeeds.
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("Marketing")
    .IsDependentOn("Localization")
    .IsDependentOn("GitVersion")
    .IsDependentOn("InjectParameters")
    .IsDependentOn("UpdateBuildFlags")
    .Does(() =>
    {
        if (isApplePlatform)
        {
            MSBuild(project.Apple, new MSBuildSettings
            {
                Configuration = $"{configuration}",
            });
        }
        else
        {
            MSBuild(project.Android, new MSBuildSettings
            {
                Configuration = $"{configuration}",
            });
        }
    });

// Run the test project and collect coverage.
Task("Test")
    .Does(() =>
    {
        if (skipTests)
        {
            Information("Skipping.");
            return;
        }

        DotNetCoreBuild(project.Test, new DotNetCoreBuildSettings
        {
            Configuration = $"{configuration}",
            NoIncremental = true,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
        });

        var testDllPath = MakeAbsolute(File($"./{folder.Test}/bin/{configuration}/netcoreapp3.1/PERLS.Tests.dll"));

        XUnit2($"{testDllPath}", new XUnit2Settings 
        {
            Parallelism = ParallelismOption.All,
            NUnitReport = true,
            NoAppDomain = true,
            OutputDirectory = "./build",
            WorkingDirectory = MakeAbsolute(Directory(folder.Test))
        });
    });

// Bundle the application into an IPA (when the platform flag is set to iPhone, the default) or APK (for the Android platform).
Task("Bundle")
    .IsDependentOn("Clean")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("Marketing")
    .IsDependentOn("Localization")
    .IsDependentOn("WriteManifestInfo")
    .IsDependentOn("WriteGitHash")
    .IsDependentOn("InjectParameters")
    .IsDependentOn("Badge")
    .IsDependentOn("Firebase")
    .IsDependentOn("UpdateBuildFlags")
    .IsDependentOn("Test")
    .IsDependentOn("VerifyStyles")
    .Does(() =>
    {
        CreateDirectory(buildPath);
        
        if (isApplePlatform)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new Exception("Unable to determine assembly name.");
            }

            MSBuild(project.Apple, config =>
            {
                config.Configuration = $"{configuration}";
                config.Targets.Add("Build");
                config.Properties.Add("Platform", new[] { $"{platform}" });
                config.Properties.Add("BuildIpa", new[] { $"{true}" });
                config.Properties.Add("IpaPackageDir", new[] { $"bin/{platform}/{configuration}" });

                if (!string.IsNullOrWhiteSpace(codesignKey))
                {
                    config.Properties.Add("CodesignKey", new[] { codesignKey });
                }
                
                if (!string.IsNullOrWhiteSpace(codesignProvision))
                {
                    config.Properties.Add("CodesignProvision", new[] { codesignProvision });
                }
            });

            bundlePath = File($"{folder.Apple}/bin/{platform}/{configuration}/{assemblyName}.ipa");

            var symbolsPath = new DirectoryPath($"{folder.Apple}/bin/{platform}/{configuration}/{assemblyName}.app.dSYM");
            symbolsArchivePath = buildPath.CombineWithFilePath(new FilePath($"{packageIdentifier}-{fullSemVersion}-{buildNumber}.dSYM.zip"));
            Zip(symbolsPath, symbolsArchivePath);
            Information($"Zipped symbols to {symbolsArchivePath}");
        }
        else
        {
            MSBuild(project.Android, config =>
            {
                config.Configuration = $"{configuration}";
                config.Targets.Add("SignAndroidPackage");
                
                var allLocalizations = localizations;

                if (localizations.Count() == 1 && string.IsNullOrWhiteSpace(localizations[0]))
                {
                    allLocalizations = new string[] {"en"};
                }
                else
                {
                    allLocalizations = allLocalizations.Append("en").ToArray();
                }

                // resx files are to be formatted either {two-letter-lang-code}.resx or {two-letter-lang-code}-{regioncode}.mx.
                // Android adds the "-r" before the region code.
                var joinedLocalizations = string.Join(",", allLocalizations).Replace("-", "-r");

                var args = $"-c {joinedLocalizations}";
                
                config.Properties.Add("AndroidAapt2LinkExtraArgs", new[] { args });
                config.Properties.Add("AndroidResgenExtraArgs", new[] { args });
                config.Properties.Add("AndroidUseSharedRuntime", new[] { $"{androidUseSharedRuntime}" });
                config.Properties.Add("EmbedAssembliesIntoApk", new[] { $"{embedAssembliesIntoApk}" });
                config.Properties.Add("AndroidPackageFormat", new[] { $"{androidPackageFormat}".ToLower() });

                if (androidLinkTool != AndroidLinkTool.None)
                {
                    config.Properties.Add("AndroidLinkTool", new[] { $"{androidLinkTool}".ToLower() });
                }

                if (!string.IsNullOrWhiteSpace(androidSigningKeyStore))
                {
                    config.Properties.Add("AndroidKeyStore", new[] { $"{true}" });
                    config.Properties.Add("AndroidSigningKeyStore", new [] { $"{androidSigningKeyStore}" });
                    config.Properties.Add("AndroidSigningKeyAlias", new [] { $"{androidSigningKeyAlias}" });
                    config.Properties.Add("AndroidSigningStorePass", new [] { $"{androidSigningStorePass}" });
                    config.Properties.Add("AndroidSigningKeyPass", new [] { $"{androidSigningKeyPass}" });
                }
            });

            bundlePath = File($"{folder.Android}/bin/{configuration}/{packageIdentifier}-Signed.{extension}");
        }

        var newPath = buildPath.CombineWithFilePath(new FilePath($"{packageIdentifier}-{fullSemVersion}-{buildNumber}.{extension}"));
        CopyFile(bundlePath, newPath);
        bundlePath = newPath;

        Information($"Bundled application to {bundlePath}");
        var bundleHash = CalculateFileHash(bundlePath).ToHex();
        var bundleSize = $"{FileSize(bundlePath)}";
        Information($"  Bundle hash: {bundleHash}");
        Information($"  Bundle size: {bundleSize} bytes");
        injectedEnvironmentVariables[BundleHashKey] = bundleHash;
        injectedEnvironmentVariables[BundleSizeKey] = bundleSize;
    });

// Install the application to the connected device.
Task("Install")
    .IsDependentOn("Bundle")
    .Does(() =>
    {
        switch (platform)
        {
            case Platform.iPhone:
                StartProcess("ideviceinstaller", $"-i {bundlePath}");
                break;
            case Platform.iPhoneSimulator:
                var simulators = ListAppleSimulators().Where(sim => sim.Runtime.Contains("iOS")).OrderBy(sim => sim.Name);
                var booted = simulators.FirstOrDefault(sim => sim.State == "Booted");

                if (booted is AppleSimulator sim)
                {
                    InstalliOSApplication(sim.UDID, $"{bundlePath}");
                }
                else
                {
                    throw new Exception("No simulator found.");
                }
                break;
            case Platform.Android:
                AdbInstall(bundlePath);
                break;
        }
    });

// Run the application on a simulator. This is still a work in progress.
Task("Run")
    .IsDependentOn("Install")
    .Does(() =>
    {
        if (isApplePlatform)
        {
            if (platform != Platform.iPhoneSimulator)
            {
                throw new Exception("Run is only supported on the iPhone simulator currently.");
            }

            var simulators = ListAppleSimulators().Where(sim => sim.Runtime.Contains("iOS")).OrderBy(sim => sim.Name);
            var booted = simulators.FirstOrDefault(sim => sim.State == "Booted");

            if (booted is AppleSimulator sim)
            {
                LaunchiOSApplication(sim.UDID, packageIdentifier);
            }
            else
            {
                throw new Exception("Please launch an iOS simulator first.");
            }
        }
        else
        {
            // this always throws a Java exception
            var emulators = AndroidAvdListDevices(new AndroidAvdManagerToolSettings
            {
                SdkRoot = androidSdkRoot,
            });

            // this doesn't specify an emulator
            AdbInstall(bundlePath, new AdbToolSettings
            {
            });

            AmStartActivity($"{packageIdentifier}/.SplashActivity", new AmStartOptions
            {
                EnableDebugging = false,
                ForceStopTarget = true,
            });
        }
    });

// Generates a changelog for this build.
Task("Changelog")
    .IsDependentOn("GitVersion")
    .Does(() =>
    {
        if (startChangelogFromEnvInject)
        {
            if (!FileExists(envInjectFile))
            {
                Warning("Could not envInjectFile for starting changelog");
            }
            else
            {
                var changelogDictionary = new Dictionary<string, string>(ParseEnvInject(envInjectFile));

                if (changelogDictionary.ContainsKey(GitHashKey))
                {
                    changelogStart = changelogDictionary[GitHashKey];
                    Information($"Read previous git hash from env inject: {changelogStart}");
                }
                else 
                {
                    Warning("Could not find GitHashKey in envInjectFile");
                }

            }
        }

        if (flavor == Flavor.CI && string.IsNullOrWhiteSpace(changelogStart))
        {
            changelogStart = "origin/master";
            Information($"Starting changelog from master");
        }

        if (flavor == Flavor.CI && string.IsNullOrWhiteSpace(changelogEnd))
        {
            changelogEnd = branchName;
            Information($"Ending changelog with branch name {changelogEnd}");
        }

        if (flavor == Flavor.Canary && string.IsNullOrWhiteSpace(changelogStart))
        {
            changelogStart = $"{GitLog($"{root}", 2).Last().Sha}";
            Information($"Starting changelog from previous commit {changelogStart}");
        }

        if (!string.IsNullOrWhiteSpace(changelogStart) && string.IsNullOrWhiteSpace(changelogEnd))
        {
            changelogEnd = gitSha;
            Information($"Assuming changelog end position of current commit {changelogEnd}");
        }

        if (string.IsNullOrWhiteSpace(changelogStart) || string.IsNullOrWhiteSpace(changelogEnd))
        {
            Information("Missing changelog start or end position. Skipping.");
            return;
        }

        // Call git-clog to determine the changes
        var changes = GitClogWithResult(new GitClogSettings
        {
            Verbose = true,
            Output = Format.Changes,
            Range = $"{changelogStart}..{changelogEnd}",
        }).ToList();

        if (!changes.Any())
        {
            Information("No changes found, skipping.");
            return;
        }

        // Determine URLs for related tickets
        var urls = GitClogWithResult(new GitClogSettings
        {
            Verbose = true,
            Output = Format.Urls,
            Range = $"{changelogStart}..{changelogEnd}",
        }).Select(line => $"* {line}");

        if (!string.IsNullOrWhiteSpace(changelogFile))
        {
            // Write changelog to file (default is "changelog.txt")
            FileWriteLines(changelogFile, changes.Select(line => $"* {line}").ToArray());

            if (urls.Any())
            {
                // Append ticket URLs to changelog
                FileAppendLines(changelogFile, new [] { "### Related tickets" });
                FileAppendLines(changelogFile, urls.ToArray());
            }
        }

        changelog = string.Join($"\n", changes.Select(line => $"• {line}"));

        // Append condensed changelog to environment variable injection file
        injectedEnvironmentVariables["CHANGELOG"] = changelog;
    });


// Cleanup previous builds in AppCenter. Usually only used for CI builds.
Task("DeployCleanup")
    .IsDependentOn("AppCenterId")
    .IsDependentOn("GitVersion")
    .Does(() =>
    {
        if (flavor != Flavor.CI && !force)
        {
            Information("Not a CI build. Skipping deploy cleanup.");
            return;
        }

        if (string.IsNullOrWhiteSpace(branchName))
        {
            throw new Exception("Branch name could not be determined.");
        }

        if (string.IsNullOrWhiteSpace(appCenterApp))
        {
            throw new Exception("App center app ID must be provided.");
        }

        Information($"Looking for release notes with \"{branchName}\" prefix");
        Information($"App {appCenterPrefix}/{appCenterApp}");

        var releaseArray = AppCenterDistributeReleasesListWithResult(new AppCenterDistributeReleasesListSettings
        {
            App = $"{appCenterPrefix}/{appCenterApp}",
            Output = "json",
            Token = appCenterToken,
        }).ToArray();

        var releases = JArray.Parse(releaseArray[0]).Select(p => (string)p["id"]);

        Information($"Found {releases.Count()} previous release IDs");

        Parallel.ForEach(releases, releaseId =>
        {
            var previousBuild = AppCenterDistributeReleasesShowWithResult(new AppCenterDistributeReleasesShowSettings
            {
                App = $"{appCenterPrefix}/{appCenterApp}",
                Output = "json",
                Token = appCenterToken,
                ReleaseId = releaseId,
            }).First();

            if (previousBuild == null)
            {
                Information($"No release for ID {releaseId}");
                return;
            }

            var parsed = JObject.Parse(previousBuild);

            if (parsed == null)
            {
                Information($"Failed to parse: \"{previousBuild}\"");
                return;
            }

            var thisReleaseNotes = (string)parsed["releaseNotes"];

            if (thisReleaseNotes == null)
            {
                Information("No release notes for app.");
                return;
            }

            var id = (string)parsed["id"];

            if (id == null)
            {
                Information("No release ID for app.");
                return;
            }

            var uploadDate = (DateTimeOffset)parsed["uploadedAt"];

            if (id == null)
            {
                Information($"No upload date for release {id}.");
                return;
            }

            var difference = DateTimeOffset.Now - uploadDate;
            
            if (thisReleaseNotes.StartsWith(branchName) || difference.Days > 14)
            {
                AppCenterDistributeReleasesDelete(new AppCenterDistributeReleasesDeleteSettings
                {
                    App = $"{appCenterPrefix}/{appCenterApp}",
                    Output = "json",
                    Token = appCenterToken,
                    ReleaseId = id,
                    Quiet = true,
                });

                Information($"Deleted {difference.Days} day old release {id} for {branchName}");
                return;
            }
        });
    });

// Deploy the application to AppCenter.
Task("Deploy")
    .IsDependentOn("Bundle")
    .IsDependentOn("AppCenterId")
    .IsDependentOn("DeployCleanup")
    .IsDependentOn("Changelog")
    .Does(() =>
    {
        if (platform == Platform.iPhoneSimulator)
        {
            throw new Exception("You probably don't want to upload a simulator build to AppCenter.");
        }

        var fullReleaseNotes = new List<string>();

        // if release notes are provided, use those
        // otherwise, use the auto-generated changelog
        if (!string.IsNullOrWhiteSpace(releaseNotes))
        {
            var regex = new Regex(@"\\r|\r|\n|\\n", RegexOptions.IgnoreCase);
            var splitNotes = regex.Split(releaseNotes);
            var filterNotes = splitNotes.Where(str => !string.IsNullOrWhiteSpace(str)).Select(str => $"• {str.Trim()}");
            Information($"Found {filterNotes.Count()} provided release notes");
            fullReleaseNotes.AddRange(filterNotes);
        }
        else if (!string.IsNullOrWhiteSpace(changelog))
        {
            fullReleaseNotes.Add(changelog);
        }

        // CI build release notes are prefixed with the branch name for pruning purposes
        if (flavor == Flavor.CI)
        {
            fullReleaseNotes.Insert(0, branchName);
        }

        // everything except releases gets the full informational version appended
        if (flavor != Flavor.Release)
        {
            fullReleaseNotes.Add(infoVersion);
        }

        // smush together all release notes
        // if we don't escape quotes, Cake.AppCenter gets angry
        var flatReleaseNotes = string.Join("\n", fullReleaseNotes).Replace("\"", "\\\"");
        
        Information($"Bundle Path: {bundlePath?.ToString()}");
        Information($"Group Path: {appCenterGroupName}");

        // We have to trim release notes to 500 characters for Google Play
        if (platform == Platform.Android && flatReleaseNotes.Length > 499)
        {
            flatReleaseNotes = flatReleaseNotes.Substring(0, 498);
        }

        Information("Release notes:");
        Information(flatReleaseNotes);

        IEnumerable<string> result;

        if (uploadToStore)
        {
            if (flavor != Flavor.Release)
            {
                throw new Exception("Refusing to upload a non-release flavor to store.");
            }

            if (configuration != Configuration.Release)
            {
                throw new Exception("Refusing to upload a debug build to store.");
            }

            result = AppCenterDistributeStoresPublishWithResult(new AppCenterDistributeStoresPublishSettings
            {
                App = $"{appCenterPrefix}/{appCenterApp}",
                File = bundlePath.ToString(),
                Store = appCenterGroupName,
                ReleaseNotes = flatReleaseNotes,
                Output = "json",
                Token = appCenterToken,
            });
        }
        else
        {
            result = AppCenterDistributeReleaseWithResult(new AppCenterDistributeReleaseSettings
            {
                App = $"{appCenterPrefix}/{appCenterApp}",
                File = bundlePath.ToString(),
                Group = appCenterGroupName,
                ReleaseNotes = string.Join(appCenterNewline, flatReleaseNotes.Split("\n")),
                Output = "json",
                Token = appCenterToken,
            });
        }

        foreach (var line in result)
        {
            var id = (string)JObject.Parse(line)["id"];
            var url = new Uri($"https://install.appcenter.ms/orgs/{appCenterPrefix}/apps/{appCenterApp}/releases/{id}");
            var shortName = projectName.ToLower().Replace('-', '_');
            var key = isApplePlatform 
                ? $"{shortName}_ios_link"
                : $"{shortName}_android_link";
            Information($"Download URL is: {url}");

            injectedEnvironmentVariables[key] = $"{url}";
            
            break;
        }

        if (isApplePlatform)
        {
            if (symbolsArchivePath == null)
            {
                throw new Exception("No symbols archive path is defined.");
            }

            Information($"Uploading iOS symbols from {symbolsArchivePath}");

            AppCenterCrashesUploadSymbols(new AppCenterCrashesUploadSymbolsSettings
            {
                App = $"{appCenterPrefix}/{appCenterApp}",
                Output = "json",
                Token = appCenterToken,
                Symbol = $"{symbolsArchivePath}",
            });

            Information("Symbol upload complete");
        }
    });

// Clean up after the build is complete, and write properties file.
Teardown(context =>
{
    if (gitReset)
    {
        Warning("Performing a git reset");
        StartProcess("git", "checkout -- .");
        StartProcess("git", "clean -xdfe build");
    }

    if (FileExists(envInjectFile))
    {
        Information("Reading environment injection file");

        var pairs = ParseEnvInject(envInjectFile);

        foreach (var (key, value) in pairs)
        {
            if (!injectedEnvironmentVariables.ContainsKey(key))
            {
                injectedEnvironmentVariables[key] = value;
                Information($"Read existing key-value pair {key}={value}");
            }
            else
            {
                var escapedValue = UnescapeNewLines(value);

                if (injectedEnvironmentVariables[key] == escapedValue)
                {
                    Information($"Found matching key-value pair {key}={escapedValue}");
                }
                else
                {
                    Warning($"Found conflicting key-value pairs for key {key}");
                    Warning($"  Value in file: {escapedValue}");
                    Warning($"  Value from build: {injectedEnvironmentVariables[key]}");
                }
            }
        }
    }

    var lineArray = injectedEnvironmentVariables.Select(pair => $"{pair.Key}={EscapeNewLines(pair.Value)}").ToArray();

    Information("Writing environment injection file");

    foreach (var line in lineArray)
    {
        Information($"  {line}");
    }

    EnsureDirectoryExists(MakeAbsolute(File(envInjectFile).Path.GetDirectory()));
    FileWriteLines(envInjectFile, lineArray);
});

// Run
RunTarget(task);
