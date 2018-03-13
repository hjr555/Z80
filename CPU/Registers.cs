using Z80.Enumerations;
using Z80.Z80;

namespace Z80
{
    public static class Registers
    {
        private static Flags flags => (Flags)F.Value;

        private static bool IFF1;
        private static bool IFF2;
        private static bool IM;

        // Shadow registers
        private static Register16 AFshadow;
        private static Register16 BCshadow;
        private static Register16 DEshadow;
        private static Register16 HLshadow;

        public static Register16 AF;
        public static Register16 BC;
        public static Register16 DE;
        public static Register16 HL;

        public static Register8 A
        {
            get
            {
                return AF.Low;
            }
            set
            {
                AF.Low = value;
            }
        }
        public static Register8 F
        {
            get
            {
                return AF.High;
            }
            set
            {
                AF.High = value;
            }
        }
        public static Register8 B
        {
            get
            {
                return BC.Low;
            }
            set
            {
                BC.Low = value;
            }
        }
        public static Register8 C
        {
            get
            {
                return BC.High;
            }
            set
            {
                BC.High = value;
            }
        }
        public static Register8 D
        {
            get
            {
                return DE.Low;
            }
            set
            {
                DE.Low = value;
            }
        }
        public static Register8 E
        {
            get
            {
                return DE.High;
            }
            set
            {
                DE.High = value;
            }
        }
        public static Register8 H
        {
            get
            {
                return HL.Low;
            }
            set
            {
                HL.Low = value;
            }
        }
        public static Register8 L
        {
            get
            {
                return HL.High;
            }
            set
            {
                HL.High = value;
            }
        }


        /// <summary>
        /// Interrupt Vector
        /// </summary>
        public static Register8 I;

        /// <summary>
        /// Memory Refresh
        /// </summary>
        public static Register8 R;

        /// <summary>
        /// Stack Pointer
        /// </summary>
        public static Register16 SP;

        /// <summary>
        /// Program Counter.
        /// </summary>
        public static Register16 PC;

        /// <summary>
        /// Index Register
        /// </summary>
        public static Register16 IX;

        /// <summary>
        /// Index Register
        /// </summary>
        public static Register16 IY;

        public static bool Carry
        {
            get
            {
                return flags.HasFlag(Flags.Carry);
            }
            set
            {
                if (value)
                {
                    F.Value = (byte)(F.Value & (byte)Flags.Carry);
                }
                else
                {
                    F.Value = (byte)(F.Value & (255 - (byte)Flags.Carry));
                }
            }
        }
        public static bool Zero
        {
            get
            {
                return flags.HasFlag(Flags.Zero);
            }
            set
            {
                if (value)
                {
                    F.Value = (byte)(F.Value & (byte)Flags.Zero);
                }
                else
                {
                    F.Value = (byte)(F.Value & (255 - (byte)Flags.Zero));
                }
            }
        }
        public static bool Sign
        {
            get
            {
                return flags.HasFlag(Flags.Sign);
            }
            set
            {
                if (value)
                {
                    F.Value = (byte)(F.Value & (byte)Flags.Sign);
                }
                else
                {
                    F.Value = (byte)(F.Value & (255 - (byte)Flags.Sign));
                }
            }
        }
        public static bool Parity
        {
            get
            {
                return flags.HasFlag(Flags.Parity);
            }
            set
            {
                if (value)
                {
                    F.Value = (byte)(F.Value & (byte)Flags.Parity);
                }
                else
                {
                    F.Value = (byte)(F.Value & (255 - (byte)Flags.Parity));
                }
            }
        }
        public static bool Subtract
        {
            get
            {
                return flags.HasFlag(Flags.Negative);
            }
            set
            {
                if (value)
                {
                    F.Value = (byte)(F.Value & (byte)Flags.Negative);
                }
                else
                {
                    F.Value = (byte)(F.Value & (255 - (byte)Flags.Negative));
                }
            }
        }
        public static bool HalfCarry
        {
            get
            {
                return flags.HasFlag(Flags.HalfCarry);
            }
            set
            {
                if (value)
                {
                    F.Value = (byte)(F.Value & (byte)Flags.HalfCarry);
                }
                else
                {
                    F.Value = (byte)(F.Value & (255 - (byte)Flags.HalfCarry));
                }
            }
        }

        public static void Initialize()
        {
            AF = new Register16(0xFFFF);
            BC = new Register16(0xFFFF);
            DE = new Register16(0xFFFF);
            HL = new Register16(0xFFFF);
            AFshadow = new Register16(0xFFFF);
            BCshadow = new Register16(0xFFFF);
            DEshadow = new Register16(0xFFFF);
            HLshadow = new Register16(0xFFFF);

            IX = new Register16(0xFFFF);
            IY = new Register16(0xFFFF);
            
            SP = new Register16(0xFFFF);
            PC = new Register16(0x0000);

            I = new Register8(0xFF);
            R = new Register8(0xFF);
        }

        /// <summary>
        /// Emulate the EXX function to swap between the two banks of registers.
        /// </summary>
        public static void EXX()
        {
            var tmpAF = AF;
            var tmpBC = BC;
            var tmpDE = DE;
            var tmpHL = HL;

            AF = AFshadow;
            BC = BCshadow;
            DE = DEshadow;
            HL = HLshadow;

            AFshadow = tmpAF;
            BCshadow = tmpBC;
            DEshadow = tmpDE;
            HLshadow = tmpHL;
        }

        public static void EX_AF()
        {
            var tmpAF = AF;

            AF = AFshadow;

            AFshadow = tmpAF;
        }

        static Registers()
        {
            Initialize();
        }
    }
}
