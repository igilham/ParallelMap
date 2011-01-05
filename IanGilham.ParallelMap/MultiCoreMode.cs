
namespace IanGilham.ParallelUtils
{
    /// <summary>
    /// This enum contains the three possible values for
    /// the Mode property, which is used to configure how
    /// threads will or will not be used by the Map functions.
    /// </summary>
    public enum MultiCoreMode
    {
        AlwaysUseThreads,
        NeverUseThreads,
        UseThreadsIfMultipleProcessorsPresent
    }
}
