namespace Z80.Z80.Ports
{
    public interface IMemory
    {
        byte GetByte(ushort address);
        ushort GetWord(ushort address);
        void SetByte(ushort address, byte data);
        void SetWord(ushort address, ushort data);
    }
}
