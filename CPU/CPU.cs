using System;
using System.Threading.Tasks;
using Z80.Z80;
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
            var instruction = RAM.GetByte(Registers.PC.Value);
            Registers.PC.Increment();

            switch (instruction)
            {
                case 0x00: // NOP
                    Clock.IncrementClock(4);
                    break;

                case 0x01: // LD BC, nn      10
                    Load_RR_nn(Registers.BC);
                    break;

                case 0x02: // LD (bc), a
                    RAM.SetByte(Registers.BC.Value, Registers.A);
                    Clock.IncrementClock(7);
                    break;

                case 0x03: // INC BC
                    Increment(Registers.BC);
                    break;

                case 0x04: // INC B
                    Registers.B = Increment(Registers.B);
                    break;

                case 0x05: // DEC B
                    Registers.B = Decrement(Registers.B);
                    break;

                case 0x06: // LD B, *
                    

                    Clock.IncrementClock(7);
                    break;

                case 0x11:// LD DE, nn      10
                    Load_RR_nn(Registers.DE);
                    break;
                case 0x21:// LD BC, nn      10
                    Load_RR_nn(Registers.HL);
                    break;
                case 0x31:// LD BC, nn      10
                    Load_RR_nn(Registers.SP);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void Increment(Register16 register)
        {
            Clock.IncrementClock(6);

            register.Increment();
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

        private void Decrement(Register16 register)
        {
            Clock.IncrementClock(6);
            
            register.Increment();
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

            returnValue = RAM.GetWord(Registers.PC.Value);

            Registers.PC.Increment(2);

            return returnValue;
        }

        private byte GetByte()
        {
            byte returnValue;

            returnValue = RAM.GetByte(Registers.PC.Value);

            Registers.PC.Increment();

            return returnValue;
        }

        // 8 bit load group
        // LD r, r'
        private void LD_R_R()
        {
            Regis

            Clock.IncrementClock(1);
        }

        private void Load_R_n()

        private void Load_RR_nn(Register16 register)
        {
            register.Value = GetWord();

            Clock.IncrementClock(10);
        }

        // LD r, n
        // LD r, (HL)
        // LD r, (IX+d)
        // LD r, (IY+d)
        // LD (HL), r
        // LD (IX+d), r
        // LD (IY+d), r
        // LD (HL), n
        // LD (IX+d), n
        // LD (IY+d), n
        // LD A, (BC)
        // LD A, (DE)
        // LD A, (nn)
        // LD (BC), A
        // LD (DE), A
        // LD I, A
        // LD R, A


    }
}
