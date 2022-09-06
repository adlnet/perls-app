# PERLS Mobile App

## Application requirements

* iOS 11
* Android 6 or later

## Development environment

* MacOS 10.13 or higher
* Mac: Visual Studio for Mac 8.4.x or higher
* Xamarin.iOS 12 (requires Xcode 10.3)
* Xamarin.Android 9 (requires Android API 28)
* [GitVersion](http://gitversion.readthedocs.io) (highly recommended for release builds)

## Building

### 1. Download dependencies

NuGet packages can be restored with the following command:

    nuget restore

When building via Cake, NuGet restoration will be done automatically as part of the build process.

### 2. Build binaries

Note that when building, some warnings about conflicts between different assembly versions, Android resource compatibility, and InnerClass attributes for anonymous inner classes are to be expected.

On Windows, the `msbuild` command may give warnings about System types being defined in an assembly that is not referenced. Windows users will need to ensure that their system is configured for the build pipeline.

When building for the first time using [./build.sh](./build.sh) `--skipAppCenterId` will likely need to be added to skip setting the AppCenter ID in the built application. AppCenter IDs are only required for analytics and crash reporting and the app will function properly without it.

Invoking `build.sh` by default will build, but not bundle the application. To create a platform-specific bundle, you can use the following build parameters. The default platform is `iPhone`. To build for iOS, you will need to provide a provisioning profile via the `codesignKey` and `codesignProvision` build parameters. The built application will be found within the `build` folder.

    ./build.sh \
        --task=Bundle \
        --platform=Android \
        --skipAppCenterId \
        --skipFirebase \
        --skipTests

Note that skipping Firebase will cause the application to crash on launch, as the Firebase initializer requires valid values or else throws an exception. The expected location for a Firebase configuration file (JSON for Android, PLIST for iOS) is `Firebase/{packageIdentifier}.google-services.{ext}` and this file will be included in the build based on the expected package identifier. The package identifier can be set via a build parameter passed to the build script, i.e. `--packageIdentifier=com.mycompany.myapp`.

#### Optional build properties

See the "Parameters and arguments" section of [build.cake](./build.cake).

#### Build for iOS

Builds for this application are performed using [Cake](https://cakebuild.net/). It is highly recommended that you invoke the Cake bootstrap script with `./build.sh`. On Windows, you may need to use a PowerShell Cake bootstrap script, which is not provided in this repo. Use of the bootstrap script ensures that Cake is run in the `dotnet` runtime and uses the pinned version of Cake specified in `tools/packages.config`.

Make sure that the provisioning profile is up to date for iOS builds. If not, the application will not be built. 

    ./build.sh --configuration=Release --platform=iPhone --task=Bundle

The IPA and dSYM will be located in `./build` unless specified otherwise by the build arguments. Building for iOS on Windows requires a connected macOS device to build; see more information [here](https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/windows/connecting-to-mac/).

#### Build for Android

Android can build and deploy debug builds without any kind of profile or codesigning, but release builds will likely require passing values for `androidSigningKeyStore`, `androidSigningKeyAlias`, `androidSigningStorePass`, and `androidSigningKeyPass`.

    ./build.sh --configuration=Release --platform=Android --task=Bundle

The APK or AAB will be located in `./build` unless specified otherwise by the build arguments.

