using Z80.Enumerations;
using Z80.Z80;

namespace Z80
{
    public static class Registers
    {
        private static Flags flags => (Flags)F;

        // Shadow registers
        private static Register AFshadow;
        private static Register BCshadow;
        private static Register DEshadow;
        private static Register HLshadow;

        public static Register AF;
        public static Register BC;
        public static Register DE;
        public static Register HL;

        public static byte A
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
        public static byte F
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
        public static byte B
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
        public static byte C
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
        public static byte D
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
        public static byte E
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
        public static byte H
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
        public static byte L
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
        public static byte I;

        /// <summary>
        /// Memory Refresh
        /// </summary>
        public static byte R;

        /// <summary>
        /// Stack Pointer
        /// </summary>
        public static Register SP;

        /// <summary>
        /// Program Counter.
        /// </summary>
        public static Register PC;

        /// <summary>
        /// Index Register
        /// </summary>
        public static Register IX;

        /// <summary>
        /// Index Register
        /// </summary>
        public static Register IY;

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
                    F = (byte)(F & (byte)Flags.Carry);
                }
                else
                {
                    F = (byte)(F & (255 - (byte)Flags.Carry));
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
                    F = (byte)(F & (byte)Flags.Zero);
                }
                else
                {
                    F = (byte)(F & (255 - (byte)Flags.Zero));
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
                    F = (byte)(F & (byte)Flags.Sign);
                }
                else
                {
                    F = (byte)(F & (255 - (byte)Flags.Sign));
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
                    F = (byte)(F & (byte)Flags.Parity);
                }
                else
                {
                    F = (byte)(F & (255 - (byte)Flags.Parity));
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
                    F = (byte)(F & (byte)Flags.Negative);
                }
                else
                {
                    F = (byte)(F & (255 - (byte)Flags.Negative));
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
                    F = (byte)(F & (byte)Flags.HalfCarry);
                }
                else
                {
                    F = (byte)(F & (255 - (byte)Flags.HalfCarry));
                }
            }
        }

        public static void Initialize()
        {
            AF = new Register(0xFFFF);
            BC = new Register(0xFFFF);
            DE = new Register(0xFFFF);
            HL = new Register(0xFFFF);
            AFshadow = new Register(0xFFFF);
            BCshadow = new Register(0xFFFF);
            DEshadow = new Register(0xFFFF);
            HLshadow = new Register(0xFFFF);

            I = 0xFF;
            R = 0xFF;
            SP = new Register(0xFFFF);
            PC = new Register(0x0000);
            IX = new Register(0xFFFF);
            IY = new Register(0xFFFF);
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

        static Registers()
        {
            Initialize();
        }
    }
}
