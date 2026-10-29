namespace Guidance.Core
{
    /// <summary>
    /// Read-only view of game state that conditions evaluate against.
    /// Implement it directly over your game, or fill a <see cref="GuidanceContext"/>.
    /// </summary>
    public interface IGuidanceContext
    {
        bool TryGet<T>(string key, out T value);
    }
}
