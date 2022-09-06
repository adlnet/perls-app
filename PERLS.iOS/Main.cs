using Foundation;
using UIKit;

// Frustratingly, the Xamarin linker thinks we have no use of the `code` property on NSErrorException
// (because we reference it dynamically). We need to explicitly instruct the linker to not "optimize"
// NSErrorException so we can continue to distinguish offline exceptions from other scenarios.
[assembly: Preserve(typeof(NSErrorException), AllMembers = true, Conditional = false)]

namespace PERLS.iOS
{
    /// <summary>
    /// Base Application class.
    /// </summary>
    public class Application
    {
        static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
