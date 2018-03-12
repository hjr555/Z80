namespace Z80
{
    public class Register16Bit
    {
        private byte leftValue;
        private byte rightValue;

        public Register16Bit()
        {
            leftValue = default(byte);
            rightValue = default(byte);
        }

        /// <summary>
        /// Get the left byte of a 16 bit register
        /// </summary>
        public byte GetLeftValue() => leftValue;

        /// <summary>
        /// Get the right byte of a 16 bit register
        /// </summary>
        public byte GetRightValue() => rightValue;

        /// <summary>
        /// Sets the left byte of a 16 bit register
        /// </summary>
        public void SetLeftValue(byte value)
        {
            leftValue = value;
        }

        /// <summary>
        /// Sets the right byte of a 16 bit register
        /// </summary>
        public void SetRightValue(byte value)
        {
            rightValue = value;
        }

        /// <summary>
        /// Gets the 16 bit register value
        /// </summary>
        public ushort GetValue() => (ushort)((leftValue << 8) | rightValue);

        /// <summary>
        /// Sets the 16 bit register
        /// </summary>
        public void SetValue(ushort value)
        {
            leftValue = (byte)(value >> 8);
            rightValue = (byte)value;
        }
    }
}
