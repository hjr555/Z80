namespace Z80.Z80.Interfaces
{
    public interface IRegister
    {
        void Increment(byte count = 1);
        void Decrement(byte count = 1);
    }
}
