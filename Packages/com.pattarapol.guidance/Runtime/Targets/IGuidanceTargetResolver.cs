namespace Guidance
{
    public interface IGuidanceTargetResolver
    {
        bool TryResolve(string targetId, out IGuidanceTarget target);
    }
}
