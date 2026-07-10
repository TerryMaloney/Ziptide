namespace Ziptide.Core
{
    /// <summary>Neutral static channel (the PvpNoise idiom) between the flight lane and consumers
    /// that must not reference the Ship assembly. ShipFlightRuntime raises TargetDisabled on every
    /// FLIGHT_DISABLE; the Tidefront space-defense mission counts them. Producers set, consumers
    /// poll/subscribe — no hard assembly coupling.</summary>
    public static class FlightSignals
    {
        public static System.Action<string> TargetDisabled;
    }
}
