namespace Z80.Z80.Ports
{
    public interface IClock
    {
        void IncrementClock(int numberOfCycles);
        void IncrementClock(decimal numberOfCycles);
        decimal GetElapsedCycles();
    }
}
