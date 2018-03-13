using System.Threading;
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

            Thread.Sleep(100 * numberOfCycles);
        }

        public void IncrementClock(decimal numberOfCycles)
        {
            Timer += numberOfCycles;

            Thread.Sleep(100 * (int)numberOfCycles);
        }
    }
}
