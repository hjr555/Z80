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

        public void Decrement(byte count = 1)
        {
            Value -= count;
        }

        public void Increment(byte count = 1)
        {
            Value += count;
        }
    }
}
