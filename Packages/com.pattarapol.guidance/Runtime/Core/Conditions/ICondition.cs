namespace Guidance.Core
{
    public interface ICondition
    {
        bool Evaluate(IGuidanceContext context);
    }
}
