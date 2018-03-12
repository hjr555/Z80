namespace Z80.Z80.ValueObjects
{
    public class RegisterBase
    {
        public byte Value { get; set; }

        public void Increment(int count = 1)
        {
            Value += (byte)count;
        }

        public void Decrement(int count = 1)
        {
            Value -= (byte)count;
        }
    }
}
