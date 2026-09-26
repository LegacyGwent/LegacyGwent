using System.Diagnostics;

namespace Assets.Script.DynamicCards
{
    // Opt-in measurement of CPU submission time, NOT GPU execution time.
    // Used by the repeatable benchmark; disabled during normal gameplay.
    internal static class DynamicCardRenderMetrics
    {
        internal static bool Enabled;
        internal static long Renders, Pixels, SubmissionTicks;
        internal static long Begin() { return Enabled ? Stopwatch.GetTimestamp() : 0; }
        internal static void End(long start, int width, int height)
        {
            if (!Enabled) return;
            Renders++; Pixels += (long)width * height;
            SubmissionTicks += Stopwatch.GetTimestamp() - start;
        }
        internal static void Reset() { Renders = Pixels = SubmissionTicks = 0; }
        internal static double SubmissionMilliseconds { get { return SubmissionTicks * 1000d / Stopwatch.Frequency; } }
    }
}
