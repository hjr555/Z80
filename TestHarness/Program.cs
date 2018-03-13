using System;
using System.IO;
using Z80.Z80.Ports;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var clock = new ClockAdapter.Clock();
            var memory = new MemoryAdapter.Memory(32768);

            RomLoader(memory);

            var cpu = new Z80.CPU(clock, memory);

            while(true)
            {
                cpu.ExecuteNextInstruction();

                Console.Write(clock.GetElapsedCycles());
            }
        }

        public static void RomLoader(IMemory memoryAdapter)
        {
            byte[] rom = new byte[4096];

            using (var file = File.Open("zx80.rom", FileMode.Open))
            {
                file.Read(rom, 0, 4096);
            }

            for(int pointer = 0; pointer < rom.Length; pointer++)
            {
                memoryAdapter.SetByte((ushort)pointer, rom[pointer]);
            }
        }
    }
}
