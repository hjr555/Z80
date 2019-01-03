using System;

namespace Z80.Enumerations
{
    [Flags]
    public enum Flags
    {
        Carry = 1,
        Negative = 2,
        Parity = 4,
        F3 = 8,
        HalfCarry = 16,
        F5 = 32,
        Zero = 64,
        Sign = 128
    }
}
