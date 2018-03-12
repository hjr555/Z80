using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80.Z80.Exceptions;
using Z80.Z80.Ports;

namespace MemoryAdapter
{
    public class Memory : IMemory
    {
        private byte[] memory;

        public Memory(int memorySize)
        {
            memory = new byte[memorySize];
        }

        public byte GetByte(ushort address)
        {
            return memory[address];
        }

        public ushort GetWord(ushort address)
        {
            ushort low = memory[address++];
            ushort high = memory[address];

            return (ushort)((ushort)(high << 8) + low);
        }

        public void SetByte(ushort address, byte data)
        {
            if(address > (ushort) (memory.Count() - 1))
            {
                throw new ReferenceOutsideAddressSpaceException();
            }

            memory[address] = data;
        }

        public void SetWord(ushort address, ushort data)
        {
            if (address > (ushort)(memory.Count() - 1))
            {
                throw new ReferenceOutsideAddressSpaceException();
            }

            var low = (byte)data;
            var high = (byte)(data >> 8);

            memory[address++] = high;
            memory[address] = low;
        }
    }
}
