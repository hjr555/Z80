using System;
using System.Threading.Tasks;
using Z80.Z80.Ports;

namespace Z80
{
    public class CPU
    {
        private IClock Clock { get; set; }
        private IMemory RAM { get; }
        private Task Process { get; set; }
        private bool Break { get; set; }

        public CPU(int ramSize, IClock clock, IMemory ram)
        {
            RAM = ram;
            Clock = clock;
        }

        public CPU(int ramSize, byte[] rom, IClock clock, IMemory ram)
        {
            RAM = ram;
            Clock = clock;
        }

        public void Poke(byte data, ushort address)
        {
            RAM.SetByte(address, data);
        }

        public byte Peek(ushort address)
        {
            return RAM.GetByte(address);
        }

        public void Execute()
        {
            this.Break = false;

            Process = new Task(() =>
            {
                while (true)
                {
                    FetchDecodeExecute();

                    if(this.Break)
                    {
                        break;
                    }
                }
            });

            Process.Start();
        }

        public void ExecuteNextInstruction()
        {
            FetchDecodeExecute();
        }

        public void Stop()
        {
            this.Break = true;
        }

        /* Z80 instructions are represented in memory as byte sequences of the form (items in brackets are optional):
         *
         * [prefix byte,]  opcode  [,displacement byte]  [,immediate data]
         * - OR -
         * two prefix bytes,  displacement byte,  opcode
         *
         * NOTATION:
         * d: 8-bit relative address
         * n: 8-bit constant
         * nn: 16-bit constant/address
         * 
         * Low endian - Low bytes come first
         */
        private void FetchDecodeExecute()
        {
            var instruction = RAM.GetByte(Registers.PC++);

            switch (instruction)
            {
                case 0x00: // NOP
                    
                    Clock.IncrementClock(4);
                    break;

                case 0x01: // LD BC, nn      10
                    // 3 byte instruction

                    Registers.BC = GetWord();

                    Clock.IncrementClock(10);
                    break;

                case 0x02: // LD (bc), a
                    RAM.SetByte(Registers.BC, Registers.A);
                    Clock.IncrementClock(7);
                    break;

                case 0x03: // INC BC
                    Registers.BC = Increment(Registers.BC);
                    break;

                case 0x04: // INC B
                    Registers.B = Increment(Registers.B);
                    break;

                case 0x05: // DEC B
                    Registers.B = Decrement(Registers.B);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private ushort Increment(ushort register)
        {
            Clock.IncrementClock(6);

            register++;

            return register;
        }

        private byte Increment(byte register)
        {
            Clock.IncrementClock(4);

            if (register == byte.MaxValue)
            {
                Registers.Parity = true;
            }

            register++;

            return register;
        }

        private byte Decrement(byte register)
        {
            Clock.IncrementClock(4);

            if(register == byte.MinValue)
            {
                Registers.Parity = true;
            }

            register--;

            return register;
        }

        private ushort GetWord()
        {
            ushort returnValue;

            returnValue = RAM.GetWord(Registers.PC);

            Registers.PC += 2;

            return returnValue;
        }

        private byte GetByte()
        {
            byte returnValue;

            returnValue = RAM.GetByte(Registers.PC++);

            return returnValue;
        }
    }
}
