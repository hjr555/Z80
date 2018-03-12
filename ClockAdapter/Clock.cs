using Z80.Z80.Ports;

namespace ClockAdapter
{
    public class Clock : IClock
    {
        private ulong Timer { get; set; }

        public ulong GetElapsedCycles()
        {
            return Timer;
        }

        public void IncrementClock(int numberOfCycles)
        {
            Timer += (ulong)numberOfCycles;
        }
    }
}
