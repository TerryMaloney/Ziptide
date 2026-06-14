namespace Ziptide.Core
{
    /// <summary>
    /// Implemented by entities that can be stunned/shocked (e.g. drones).
    /// </summary>
    public interface IShockable
    {
        void Shock(float seconds);
    }
}
