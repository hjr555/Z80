using System.ComponentModel;
using Z80.Z80.Interfaces;

namespace Z80.Z80
{
    public class Register16 : IRegister
    {
        public Register8 High { get; set; }
        public Register8 Low { get; set; }

        public Register16(ushort initialValue)
        {
            Value = initialValue;    
        }

        public static Register16 operator ++(Register16 register)
        {
            register.Increment();

            return register;
        }

        public void Increment(byte count = 1)
        {
            throw new System.NotImplementedException();
        }

        public void Decrement(byte count = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
