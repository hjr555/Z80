using ClockAdapter;
using MemoryAdapter;
using System;
using System.IO;
using Z80;
using Z80.Z80.Ports;

namespace TestHarness
{
    class Program
    {
        public static Clock Clock { get; set; }
        public static Memory Memory { get; set; }
        public static CPU CPU { get; set; }

        static void Main(string[] args)
        {
            Console.WindowHeight = 25;
            Console.WindowWidth = 120;
            Console.BufferHeight = 25;
            Console.BufferWidth = 120;

            Console.Clear();
            Console.CursorVisible = false;

            Clock = new Clock();
            Memory = new Memory(32768);
            RomLoader(Memory);

            CPU = new Z80.CPU(Clock, Memory);

            while(true)
            {
                UpdateConsoleDisplay();

                CPU.ExecuteNextInstruction();
            }
        }

        private static void UpdateConsoleDisplay()
        {
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("Registers");
            Console.WriteLine();

            var originalColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("<---- 16 Bit ---->\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<---------- 8 Bit ---------->");
            Console.ForegroundColor = originalColour;

            Console.WriteLine($"AF: {Registers.AF.ToHex()} ({Registers.AF.ToString()})\tA: {Registers.A.ToHex()} ({Registers.A.ToString()})\tF: {Registers.F.ToHex()} ({ Registers.F.ToString()})");
            Console.WriteLine($"BC: {Registers.BC.ToHex()} ({Registers.BC.ToString()})\tB: {Registers.B.ToHex()} ({Registers.B.ToString()})\tC: {Registers.C.ToHex()} ({ Registers.C.ToString()})");
            Console.WriteLine($"DE: {Registers.DE.ToHex()} ({Registers.DE.ToString()})\tD: {Registers.D.ToHex()} ({Registers.D.ToString()})\tE: {Registers.E.ToHex()} ({ Registers.E.ToString()})");
            Console.WriteLine($"HL: {Registers.HL.ToHex()} ({Registers.HL.ToString()})\tH: {Registers.H.ToHex()} ({Registers.H.ToString()})\tL: {Registers.L.ToHex()} ({ Registers.L.ToString()})");

            Console.WriteLine();
            
            Console.WriteLine($"Flags");
            Console.WriteLine($"Sign: {Registers.Sign}\tZero:   {Registers.Zero}\tF5:       {Registers.F5}\tHalf:  {Registers.HalfCarry}\nF3:   {Registers.F3}\tParity: {Registers.Parity}\tNegative: {Registers.Subtract}\tCarry: {Registers.Carry}");
            Console.WriteLine();
            //last instruction next instruction I R, IFF1 IFF2 IM
            
            Console.WriteLine(
                $"IX: 0x{ Registers.IX.Value.ToString("X") }\t" +
                $"IY: 0x{ Registers.IY.Value.ToString("X") }");
            Console.WriteLine(
                $"SP: 0x{ Registers.SP.Value.ToString("X") }\t" +
                $"PC: 0x{ Registers.PC.Value.ToString("X") }");

            Console.WriteLine();
            Console.WriteLine($"Clock Cycles: { Clock.GetElapsedCycles() }");
            Console.WriteLine($"Instruction Count: {CPU.InstructionCounter}");
            Console.WriteLine();
            Console.WriteLine($"Current Instruction: 0x{ CPU.CurrentInstruction.ToString("X") }");
            Console.WriteLine($"Next Instruction:    0x{ CPU.NextInstruction.ToString("X") }");
            Console.WriteLine();

            Console.WriteLine("Enter\t-> Execute next instruction");
            Console.WriteLine("X\t-> Reboot");

            bool validKeyPressed = false;
            while(!validKeyPressed)
            {
                var keypress = Console.ReadKey(true);

                switch(keypress.Key)
                {
                    case ConsoleKey.Enter:
                        validKeyPressed = true;
                        break;
                    case ConsoleKey.X:
                        // Reboot();
                        //validKeyPressed = true;
                        break;
                    
                }
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
