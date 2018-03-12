namespace Z80.Z80
{
    public class Register
    {
        public ushort Value { get; set; }
        public byte High 
        {
            get
            {
                return (byte)Value;
            }
            set
            {
                var low = Low;

                Value = (ushort)((low << 8) + value);
            }
        }
        public byte Low 
        { 
            get 
            {
                return (byte)(Value >> 8);
            }
            set
            {
                var high = High;

                Value = (ushort)((value << 8) + high);
            }
        }

        public Register(ushort initialValue)
        {
            Value = initialValue;    
        }
    }
}
