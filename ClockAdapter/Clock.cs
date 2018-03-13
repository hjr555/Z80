using Z80.Z80.Ports;

namespace ClockAdapter
{
    public class Clock : IClock
    {
        private decimal Timer { get; set; }

        public decimal GetElapsedCycles()
        {
            return Timer;
        }

        public void IncrementClock(int numberOfCycles)
        {
            Timer += (decimal)numberOfCycles;
        }

        public void IncrementClock(decimal numberOfCycles)
        {
            Timer += numberOfCycles;
        }
    }
}
