using Z80.Z80.Interfaces;

namespace Z80.Z80
{
    public class Register16 : IRegister
    {
        public Register8 High { get; set; }
        public Register8 Low { get; set; }

        public ushort Value
        {
            get
            {
                return (ushort)((Low.Value << 8) + High.Value);
            }
            set
            {
                High.Value = (byte)value;
                Low.Value = (byte)(value >> 8);
            }
        }

        public Register16(ushort initialValue)
        {
            High = new Register8(0xFF);
            Low = new Register8(0xFF);

            Value = initialValue;    
        }

        public void Add(Register16 register)
        {
            Value = (ushort)(Value + register.Value);
        }

        public void Add(byte value)
        {
            Value = (ushort)(Value + value);
        }

        public void Sub(byte value)
        {
            Value = (ushort)(Value - value);
        }
        
        public static Register16 operator ++(Register16 register)
        {
            register.Add(1);

            return register;
        }

        public static Register16 operator --(Register16 register)
        {
            register.Sub(1);

            return register;
        }

        public override string ToString()
        {
            return Value.ToString("X");
        }
    }
}
