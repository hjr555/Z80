using Z80.Z80.Interfaces;

namespace Z80.Z80
{
    public class Register8 : IRegister
    {
        public byte Value { get; set; }

        public Register8(byte value)
        {
            Value = value;
        }

        public void Sub(byte value)
        {
            Value -= value;
        }

        public void Add(byte value)
        {
            Value += value;
        }

        public static Register8 operator ++(Register8 register)
        {
            register.Add(1);

            return register;
        }

        public static Register8 operator --(Register8 register)
        {
            register.Sub(1);

            return register;
        }
    }
}
