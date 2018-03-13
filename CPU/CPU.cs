using System;
using System.Threading.Tasks;
using Z80.Z80;
using Z80.Z80.Exceptions;
using Z80.Z80.Ports;

namespace Z80
{
    public class CPU
    {
        private IClock Clock { get; set; }
        private IMemory RAM { get; }
        private Task Process { get; set; }
        private bool Break { get; set; }
        private byte InterruptMode { get; set; }

        public CPU(IClock clock, IMemory ram)
        {
            RAM = ram;
            Clock = clock;
        }

        public CPU(byte[] rom, IClock clock, IMemory ram)
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
            Registers.PC++;

            switch (instruction)
            {
                case 0x00: Clock.IncrementClock(4); break;
                case 0x01: LD_RR_nn(Registers.BC); break;

                case 0x02: 
                    RAM.SetByte(Registers.BC.Value, Registers.A.Value);
                    Clock.IncrementClock(7);
                    break;

                case 0x03: Increment(Registers.BC); break;
                case 0x04: Increment(Registers.B); break;
                case 0x05: Decrement(Registers.B); break;
                case 0x06: LD_R_n(Registers.B); break;
                case 0x07: RLCA(); break;

                case 0x08:
                    Registers.EX_AF();
                    Clock.IncrementClock(4);
                    break;

                case 0x09: Add_HL_RR(Registers.BC); break;
                case 0x0A: LD_R_rr(Registers.A, Registers.BC); break;
                case 0x0B: Decrement(Registers.BC); break;
                case 0x0C: Increment(Registers.C); break;
                case 0x0D: Decrement(Registers.C); break;
                case 0x0E: LD_R_n(Registers.C); break;
                case 0x0F: RRCA(); break;

                /*--------------------------------------------*/

                case 0x10: DNJZ(); break;
                case 0x11: LD_RR_nn(Registers.DE); break;
                case 0x12: LD_rr_R(Registers.DE); break;
                case 0x13: Increment(Registers.DE); break;
                case 0x14: Increment(Registers.D); break;
                case 0x15: Decrement(Registers.D); break;
                case 0x16: LD_R_n(Registers.D); break;
                case 0x17: RLA(); break;
                case 0x18: JR_n(); break;
                case 0x19: Add_HL_RR(Registers.DE); break;
                case 0x1A: LD_R_rr(Registers.A, Registers.DE); break;
                case 0x1B: Decrement(Registers.DE); break;
                case 0x1C: Increment(Registers.E); break;
                case 0x1D: Decrement(Registers.E); break;
                case 0x1E: LD_R_n(Registers.E); break;
                case 0x1F: RRA(); break;
                
                /*--------------------------------------------*/
                
                case 0x20:
                    throw new NotImplementedException();
                    Clock.IncrementClock(12m / 7m);
                    break;

                case 0x21: LD_RR_nn(Registers.HL); break;

                case 0x22: break;
                
                case 0x23: Increment(Registers.HL); break;
                case 0x24: Increment(Registers.H); break;
                case 0x25: Decrement(Registers.H); break;
                case 0x26: LD_R_n(Registers.H); break;
                
                case 0x27: break;
                case 0x28: break;
                case 0x29: break;
                case 0x2A: break;
                case 0x2B: Decrement(Registers.HL); break;
                case 0x2C: break;
                case 0x2D: break;
                case 0x2E: break;
                case 0x2F: break;
                
                /*--------------------------------------------*/
                
                case 0X30: break;

                case 0x31: LD_RR_nn(Registers.SP); break;
                
                case 0x32: break;
                case 0x33: break;
                case 0x34: break;
                case 0x35: break;
                case 0x36:
                    RAM.SetByte(Registers.HL.Value, GetByte());
                    Clock.IncrementClock(10);
                break;
                case 0x37: break;
                case 0x38: break;
                case 0x39: break;
                case 0x3A: break;
                case 0x3B: break;
                case 0x3C: break;
                case 0x3D: break;
                case 0x3E: LD_R_n(Registers.A); break;
                case 0x3F: break;
                /*--------------------------------------------*/
                case 0x40: LD_R_R(Registers.B, Registers.B); break;
                case 0x41: LD_R_R(Registers.B, Registers.C); break;
                case 0x42: LD_R_R(Registers.B, Registers.D); break;
                case 0x43: LD_R_R(Registers.B, Registers.E); break;
                case 0x44: LD_R_R(Registers.B, Registers.H); break;
                case 0x45: LD_R_R(Registers.B, Registers.L); break;
                case 0x46: LD_R_n(Registers.B, RAM.GetByte(Registers.HL.Value)); break;
                case 0x47: LD_R_R(Registers.C, Registers.A); break;
                case 0x48: LD_R_R(Registers.C, Registers.B); break;
                case 0x49: LD_R_R(Registers.C, Registers.C); break;
                case 0x4A: LD_R_R(Registers.C, Registers.D); break;
                case 0x4B: LD_R_R(Registers.C, Registers.E); break;
                case 0x4C: LD_R_R(Registers.C, Registers.H); break;
                case 0x4D: LD_R_R(Registers.C, Registers.L); break;
                case 0x4E: LD_R_n(Registers.C, RAM.GetByte(Registers.HL.Value)); break;
                case 0x4F: LD_R_R(Registers.C, Registers.A); break;

                /*--------------------------------------------*/
                case 0x50: LD_R_R(Registers.D, Registers.B); break;
                case 0x51: LD_R_R(Registers.D, Registers.C); break;
                case 0x52: LD_R_R(Registers.D, Registers.D); break;
                case 0x53: LD_R_R(Registers.D, Registers.E); break;
                case 0x54: LD_R_R(Registers.D, Registers.H); break;
                case 0x55: LD_R_R(Registers.D, Registers.L); break;
                case 0x56: LD_R_n(Registers.D, RAM.GetByte(Registers.HL.Value)); break;
                case 0x57: LD_R_R(Registers.D, Registers.A); break;
                case 0x58: LD_R_R(Registers.E, Registers.B); break;
                case 0x59: LD_R_R(Registers.E, Registers.C); break;
                case 0x5A: LD_R_R(Registers.E, Registers.D); break;
                case 0x5B: LD_R_R(Registers.E, Registers.E); break;
                case 0x5C: LD_R_R(Registers.E, Registers.H); break;
                case 0x5D: LD_R_R(Registers.E, Registers.L); break;
                case 0x5E: LD_R_n(Registers.E, RAM.GetByte(Registers.HL.Value)); break;
                case 0x5F: LD_R_R(Registers.E, Registers.A); break;
                /*--------------------------------------------*/
                case 0x60: LD_R_R(Registers.H, Registers.B); break;
                case 0x61: LD_R_R(Registers.H, Registers.C); break;
                case 0x62: LD_R_R(Registers.H, Registers.D); break;
                case 0x63: LD_R_R(Registers.H, Registers.E); break;
                case 0x64: LD_R_R(Registers.H, Registers.H); break;
                case 0x65: LD_R_R(Registers.H, Registers.L); break;
                case 0x66: LD_R_n(Registers.H, RAM.GetByte(Registers.HL.Value)); break;
                case 0x67: LD_R_R(Registers.H, Registers.A); break;
                case 0x68: LD_R_R(Registers.L, Registers.B); break;
                case 0x69: LD_R_R(Registers.L, Registers.C); break;
                case 0x6A: LD_R_R(Registers.L, Registers.D); break;
                case 0x6B: LD_R_R(Registers.L, Registers.E); break;
                case 0x6C: LD_R_R(Registers.L, Registers.H); break;
                case 0x6D: LD_R_R(Registers.L, Registers.L); break;
                case 0x6E: LD_R_n(Registers.L, RAM.GetByte(Registers.HL.Value)); break;
                case 0x6F: LD_R_R(Registers.L, Registers.A); break;
                /*--------------------------------------------*/
                case 0x70:
                    RAM.SetByte(Registers.HL.Value, Registers.B.Value);
                    Clock.IncrementClock(7);
                    break;
                case 0x71:
                    RAM.SetByte(Registers.HL.Value, Registers.C.Value);
                    Clock.IncrementClock(7);
                    break;
                case 0x72:
                    RAM.SetByte(Registers.HL.Value, Registers.D.Value);
                    Clock.IncrementClock(7);
                    break;
                case 0x73:
                    RAM.SetByte(Registers.HL.Value, Registers.E.Value);
                    Clock.IncrementClock(7);
                    break;
                case 0x74:
                    RAM.SetByte(Registers.HL.Value, Registers.H.Value);
                    Clock.IncrementClock(7);
                    break;
                case 0x75:
                    RAM.SetByte(Registers.HL.Value, Registers.L.Value);
                    Clock.IncrementClock(7);
                    break;

                case 0x76:
                    throw new CpuHaltInstruction();

                case 0x77:
                    RAM.SetByte(Registers.HL.Value, Registers.A.Value);
                    Clock.IncrementClock(7);
                    break;

                case 0x78: LD_R_R(Registers.A, Registers.B); break;
                case 0x79: LD_R_R(Registers.A, Registers.C); break;
                case 0x7A: LD_R_R(Registers.A, Registers.D); break;
                case 0x7B: LD_R_R(Registers.A, Registers.E); break;
                case 0x7C: LD_R_R(Registers.A, Registers.H); break;
                case 0x7D: LD_R_R(Registers.A, Registers.L); break;
                case 0x7E: LD_R_n(Registers.A, RAM.GetByte(Registers.HL.Value)); break;
                case 0x7F: LD_R_R(Registers.A, Registers.A); break;

                /*--------------------------------------------*/
                case 0x80: Add_R_R(Registers.A, Registers.B); break;
                case 0x81: Add_R_R(Registers.A, Registers.C); break;
                case 0x82: Add_R_R(Registers.A, Registers.D); break;
                case 0x83: Add_R_R(Registers.A, Registers.E); break;
                case 0x84: Add_R_R(Registers.A, Registers.H); break;
                case 0x85: Add_R_R(Registers.A, Registers.L); break;
                case 0x86: Add_R_n(Registers.A, RAM.GetByte(Registers.HL.Value)); break;
                case 0x87: Add_R_R(Registers.A, Registers.A); break;
                case 0x88: throw new NotImplementedException();
                case 0x89: throw new NotImplementedException();
                case 0x8A: throw new NotImplementedException();
                case 0x8B: throw new NotImplementedException();
                case 0x8C: throw new NotImplementedException();
                case 0x8D: throw new NotImplementedException();
                case 0x8E: throw new NotImplementedException();
                case 0x8F: throw new NotImplementedException();

                /*--------------------------------------------*/
                case 0x90: Sub_R_R(Registers.A, Registers.B); break;
                case 0x91: Sub_R_R(Registers.A, Registers.C); break;
                case 0x92: Sub_R_R(Registers.A, Registers.D); break;
                case 0x93: Sub_R_R(Registers.A, Registers.E); break;
                case 0x94: Sub_R_R(Registers.A, Registers.H); break;
                case 0x95: Sub_R_R(Registers.A, Registers.L); break;
                case 0x96:
                    Registers.A.Sub(RAM.GetByte(Registers.HL.Value));
                    Clock.IncrementClock(7);
                break;
                case 0x97: Sub_R_R(Registers.A, Registers.A); break;
                case 0x98: break;
                case 0x99: break;
                case 0x9A: break;
                case 0x9B: break;
                case 0x9C: break;
                case 0x9D: break;
                case 0x9E: break;
                case 0x9F: break;

                /*--------------------------------------------*/
                case 0xA0: break;
                /*--------------------------------------------*/
                case 0xB0: break;
                case 0xBC:
                    var tmp = Registers.A.Value - Registers.H.Value;
                    break;
                /*--------------------------------------------*/
                case 0xC0: break;

                case 0xC1: Pop(Registers.BC); break;

                case 0xC2: break;
                case 0xC3: // JP NZ
                    var dest = GetWord();
                    if (Registers.Zero)
                    {
                        Registers.PC.Value = dest;
                    }
                    break;
                case 0xC4: break;

                case 0xC5: Push(Registers.BC); break;

                case 0xC6: break;
                case 0xC7: break;
                case 0xC8: break;
                case 0xC9: break;
                case 0xCA: break;

                case 0xCB: // BIT instructions
                    throw new NotImplementedException();

                case 0xCC: break;
                case 0xCD: break;
                case 0xCE: break;
                case 0xCF: break;
                /*--------------------------------------------*/
                case 0xD0: break;

                case 0xD1: Pop(Registers.DE); break;
                case 0xD5: Push(Registers.DE); break;

                /*--------------------------------------------*/
                case 0xE0: break;

                case 0xE1: Pop(Registers.HL); break;
                case 0xE5: Push(Registers.HL); break;

                /*--------------------------------------------*/

                case 0xF1: Pop(Registers.AF); break;
                case 0xF5: Push(Registers.AF); break;

                /*--------------------------------------------*/

                case 0xDD: // IX instructions
                    throw new NotImplementedException();

                case 0xED: // EXTD instruction set
                    throw new NotImplementedException();

                case 0xFD: // IY instruction set
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException();
            }
        }

        private void Increment(Register16 register)
        {
            register++;
         
            Clock.IncrementClock(6);
        }

        private void Increment(Register8 register)
        {
            register++;
            
            Clock.IncrementClock(4);
        }

        private void Decrement(Register16 register)
        {
            register--;

            Clock.IncrementClock(6);
        }

        private void Decrement(Register8 register)
        {
            register--;
        
            Clock.IncrementClock(4);
        }


        // 8 bit load group
        // LD r, r'
        private void LD_R_R(Register8 dest, Register8 source)
        {
            dest.Value = source.Value;

            Clock.IncrementClock(4);
        }
        private void LD_R_n(Register8 dest, byte source)
        {
            dest.Value = source;

            Clock.IncrementClock(7);
        }


        // LD B, n
        // LD D, n
        // LD H, n
        // LD C, n
        // LD E, n
        // LD l, n
        /// <summary>
        /// Load the next byte in memory to the 8 bit register
        /// </summary>
        /// <param name="register"></param>
        private void LD_R_n(Register8 register)
        {
            register.Value = GetByte();

            Clock.IncrementClock(7);
        }

        // LD BC, nn
        // LD DE, nn
        // LD HL, nn
        // LD SP, nn
        private void LD_RR_nn(Register16 register)
        {
            register.Value = GetWord();

            Clock.IncrementClock(10);
        }

        /// <summary>
        /// Load register R with the value pointed to by (rr)
        /// </summary>
        /// <param name="address"></param>
        private void LD_R_rr(Register8 destination, Register16 address)
        {
            destination.Value = RAM.GetByte(address.Value);
            Clock.IncrementClock(7);
        }

        /// <summary>
        /// Load value R into address pointed to by (rr)
        /// </summary>
        /// <param name="address"></param>
        private void LD_rr_R(Register16 address)
        {
            RAM.SetByte(address.Value, Registers.A.Value);

            Clock.IncrementClock(7);
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

        private void RLCA()
        {
            bool carryBit = (byte)(Registers.A.Value & (byte)128) == 128;

            var result = (byte)(Registers.A.Value << 1);

            if(carryBit)
            {
                result = (byte)(result | 1);
            }

            Registers.Carry = carryBit;
            Registers.A.Value = result;

            Clock.IncrementClock(4);
        }

        private void RRCA()
        {
            bool carryBit = (byte)(Registers.A.Value & (byte)1) == 1;

            var result = (byte)(Registers.A.Value >> 1);

            if (carryBit)
            {
                result = (byte)(result | 128);
            }

            Registers.Carry = carryBit;
            Registers.A.Value = result;

            Clock.IncrementClock(4);
        }

        private void RLA()
        {
            bool carryBit = (byte)(Registers.A.Value & (byte)128) == 128;

            var result = (byte)(Registers.A.Value << 1);

            result = (byte)(result | (Registers.Carry ? 1 : 0));

            Registers.Carry = carryBit;
            Registers.A.Value = result;

            Clock.IncrementClock(4);
        }

        private void RRA()
        {
            bool carryBit = (byte)(Registers.A.Value & 1) == 1;

            var result = (byte)(Registers.A.Value >> 1);

            result = (byte)(result | (Registers.Carry ? 1 : 0));

            Registers.Carry = carryBit;
            Registers.A.Value = result;

            Clock.IncrementClock(4);
        }

        private void Add_HL_RR(Register16 register)
        {
            register.Add(register);

            Clock.IncrementClock(11);
        }

        private void Add_R_R(Register8 dest, Register8 source)
        {
            dest.Add(source.Value);

            Clock.IncrementClock(4);
        }

        private void Add_R_n(Register8 dest, byte value)
        {
            dest.Add(value);

            Clock.IncrementClock(7);
        }

        private void Sub_R_R(Register8 dest, Register8 source)
        {
            dest.Sub(source.Value);

            Clock.IncrementClock(4);
        }

        private void DNJZ() 
        {
            Registers.B--;

            if (Registers.B.Value != 0)
            {
                Registers.PC.Add(GetByte());
            }

            Clock.IncrementClock(13m / 8m);
        }

        private void JR_n()
        {
            Registers.PC.Add(GetByte());

            Clock.IncrementClock(12);
        }

        /// <summary>
        /// OR's a register with A
        /// </summary>
        /// <param name="register"></param>
        private void Or_R(Register8 register)
        {
            // Registers.A.Value | register.Value;

            Clock.IncrementClock(4);
        }


        private void Push(Register16 register)
        {
            Registers.SP--;
            RAM.SetByte(Registers.SP.Value, register.Low.Value);

            Registers.SP--;
            RAM.SetByte(Registers.SP.Value, register.High.Value);

            Clock.IncrementClock(11);
        }

        private void Pop(Register16 register)
        {
            register.High.Value = RAM.GetByte(Registers.SP.Value++);
            register.Low.Value = RAM.GetByte(Registers.SP.Value++);

            Clock.IncrementClock(10);
        }

        private ushort GetWord()
        {
            ushort returnValue;

            returnValue = RAM.GetWord(Registers.PC.Value);

            Registers.PC.Add(2);

            return returnValue;
        }

        private byte GetByte()
        {
            byte returnValue;

            returnValue = RAM.GetByte(Registers.PC.Value);

            Registers.PC++;

            return returnValue;
        }
    }
}
