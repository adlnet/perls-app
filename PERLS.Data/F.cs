using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PERLS
{
    /// <summary>
    /// A container class for logging.
    /// </summary>
    public static class F
    {
        /// <summary>
        /// Logging with reasonable default values.
        /// </summary>
        /// <param name="message">The message or object to log; optional.</param>
        /// <param name="caller">The compiler generated caller.</param>
        /// <param name="line">The compiler generated line.</param>
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // strip method on release builds
#endif
        public static void Log(object message = null, [CallerMemberName] string caller = null, [CallerLineNumber] uint line = 0)
        {
            if (Constants.Configuration == BuildConfiguration.Debug)
            {
                var frame = new StackTrace().GetFrame(1);
                Debug.WriteLine($"{DateTime.Now} {frame.GetMethod().DeclaringType} ({frame.GetNativeOffset()}) {caller} <{line}> {message}");
            }
        }
    }
}
