﻿using System;
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
            Registers.PC++;

            switch (instruction)
            {
                case 0x00: // NOP
                    Clock.IncrementClock(4);
                    break;

                case 0x01: LD_RR_nn(Registers.BC);
                    break;

                case 0x02: // LD (bc), a
                    RAM.SetByte(Registers.BC.Value, Registers.A.Value);
                    Clock.IncrementClock(7);
                    break;

                case 0x03: Increment(Registers.BC);
                    break;

                case 0x04: Increment(Registers.B);
                    break;

                case 0x05: Decrement(Registers.B);
                    break;

                case 0x06: LD_R_n(Registers.B);
                    break;

                case 0x07: RLCA();
                    break;

                case 0x08:
                    Registers.EX_AF();
                    Clock.IncrementClock(4);
                    break;

                case 0x09: Add_HL_RR(Registers.BC);
                    break;

                case 0x0A: LD_R_rr(Registers.A, Registers.BC);
                    break;

                case 0x0B: Decrement(Registers.BC);
                    break;

                case 0x0C: Increment(Registers.C);
                    break;

                case 0x0D: Decrement(Registers.C);
                    break;

                case 0x0E: LD_R_n(Registers.C);
                    break;

                case 0x0F: RRCA();
                    break;

                /*--------------------------------------------*/

                case 0x10: DNJZ();
                    break;

                case 0x11: LD_RR_nn(Registers.DE);
                    break;

                case 0x12: LD_rr_R(Registers.DE);
                    break;

                case 0x13: Increment(Registers.DE);
                    break;

                case 0x14: Increment(Registers.D);
                    break;

                case 0x15: Decrement(Registers.D);
                    break;

                case 0x16: LD_R_n(Registers.D);
                    break;
                
                case 0x17: RLA();
                    break;

                case 0x18: JR_n(); 
                    break;

                case 0x19: break;
                case 0x1A: break;
                case 0x1B: break;
                case 0x1C: break;
                case 0x1D: break;
                case 0x1E: break;
                case 0x1F: break;

                /*--------------------------------------------*/

                case 0x20: break;

                case 0x21: // LD HL, nn      10
                    LD_RR_nn(Registers.HL);
                    break;

                case 0x22: break;
                case 0x23: break;
                case 0x24: break;
                case 0x25: break;

                case 0x26: // LD H, n
                    LD_R_n(Registers.H);
                    break;
                
                case 0x27: break;
                case 0x28: break;
                case 0x29: break;
                case 0x2A: break;
                case 0x2B: break;
                case 0x2C: break;
                case 0x2D: break;
                case 0x2E: break;
                case 0x2F: break;

                /*--------------------------------------------*/

                case 0X30: break;

                case 0x31:// LD SP, nn      10
                    LD_RR_nn(Registers.SP);
                    break;
                
                case 0x32: break;
                case 0x33: break;
                case 0x34: break;
                case 0x35: break;
                case 0x36: break;
                case 0x37: break;
                case 0x38: break;
                case 0x39: break;
                case 0x3A: break;
                case 0x3B: break;
                case 0x3C: break;
                case 0x3D: break;
                case 0x3E: break;
                case 0x3F: break;

                /*--------------------------------------------*/

                default:
                    throw new NotImplementedException();
            }
        }

        // INC BC
        // INC DE
        // INC HL
        // INC SP
        private void Increment(Register16 register)
        {
            Clock.IncrementClock(6);

            register++;
        }

        // INC B
        // INC D
        // INC H
        // INC C
        // INC E
        // INC L
        // INC A
        private void Increment(Register8 register)
        {
            Clock.IncrementClock(4);

            register++;
        }


        // DEC BC
        // DEC DE
        // DEC HL
        // DEC SP
        private void Decrement(Register16 register)
        {
            Clock.IncrementClock(6);

            register--;
        }

        private void Decrement(Register8 register)
        {
            Clock.IncrementClock(4);

            register--;
        }


        // 8 bit load group
        // LD r, r'
        private void LD_R_R()
        {
            

            Clock.IncrementClock(1);
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
        /// Load value R into address pointed to by rr
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

        private void Add_HL_RR(Register16 register)
        {
            register.Add(register);

            Clock.IncrementClock(11);
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

        private void RLA()
        {
            bool carryBit = (byte)(Registers.A.Value & (byte)128) == 128;

            var result = (byte)(Registers.A.Value << 1);

            if(Registers.Carry)
            {
                result = (byte)(result | 1);
            }

            Registers.Carry = carryBit;
            Registers.A.Value = result;

            Clock.IncrementClock(4);
        }

        private void JR_n()
        {
            Registers.PC.Add(GetByte());

            Clock.IncrementClock(12);
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
