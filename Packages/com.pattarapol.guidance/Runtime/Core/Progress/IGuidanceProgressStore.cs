namespace Guidance.Core
{
    /// <summary>Remembers which show-once rules have already been shown.</summary>
    public interface IGuidanceProgressStore
    {
        bool HasSeen(string ruleId);
        void MarkSeen(string ruleId);
        void ResetAll();
    }
}
