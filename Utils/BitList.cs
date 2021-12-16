using System.Collections.Generic;

namespace AP.Barcoder.Utils
{
    internal struct BitList
    {
#pragma warning disable IDE0032 // Use auto property
#pragma warning restore IDE0032 // Use auto property
        private uint[] _data;

        /// <summary>
        /// Creates a <see cref="BitList" /> with the given bit capacity.
        /// All bits are initialized with false.
        /// </summary>
        /// <param name="capacity">The required capacity.</param>
        public BitList(int capacity)
        {
            Length = capacity;
            int x = 0;
            if (capacity % 32 != 0)
                x = 1;
            _data = new uint[capacity / 32 + x];
        }

        /// <summary>
        /// Returns the number of contained bits.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Appends the given bits to the end of the list.
        /// </summary>
        /// <param name="bits">The bits to append.</param>
        public void AddBit(params bool[] bits)
        {
            foreach (bool bit in bits)
            {
                int itemIndex = Length / 32;
                while (itemIndex >= (_data?.Length ?? 0))
                    Grow();
                SetBit(Length, bit);
                Length++;
            }
        }

        /// <summary>
        /// Sets the bit at the given index to the given value.
        /// </summary>
        /// <param name="index">The given index.</param>
        /// <param name="value">The given value.</param>
        public void SetBit(int index, bool value)
        {
            int itemIndex = index / 32;
            int itemBitShift = 31 - index % 32;
            if (value)
                _data[itemIndex] |= (uint) 1 << itemBitShift;
            else
                _data[itemIndex] &= ~((uint) 1 << itemBitShift);
        }

        /// <summary>
        /// Returns the bit at the given index.
        /// </summary>
        /// <param name="index">The given index.</param>
        /// <returns>The bit.</returns>
        public bool GetBit(int index)
        {
            int itemIndex = index / 32;
            int itemBitShift = 31 - index % 32;
            return ((_data[itemIndex] >> itemBitShift) & 1) == 1;
        }

        /// <summary>
        /// Appends all 8 bits of the given byte to the end of the list.
        /// MSb first.
        /// </summary>
        /// <param name="b">The byte containing the bits to add.</param>
        public void AddByte(byte b)
        {
            for (int i = 7; i >= 0; i--)
                AddBit(((b >> i) & 1) == 1);
        }

        /// <summary>
        /// Appends the last (LSB) count bits of b to the end of the list.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="count"></param>
        public void AddBits(uint b, byte count)
        {
            for (int i = count - 1; i >= 0; i--)
                AddBit(((b >> i) & 1) == 1);
        }

        /// <summary>
        /// Returns all bits of the <see cref="BitList" /> as a <see cref="byte[]" />.
        /// </summary>
        /// <returns>A <see cref="byte[]" />.</returns>
        public byte[] GetBytes()
        {
            int len = Length >> 3;
            if (Length % 8 != 0)
                len++;
            byte[] result = new byte[len];
            for (int i = 0; i < len; i++)
            {
                int shift = (3 - i % 4) * 8;
                result[i] = (byte) (_data[i / 4] >> shift);
            }

            return result;
        }

        /// <summary>
        /// Iterates through all bytes contained in the <see cref="BitList" />.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> IterateBytes()
        {
            int c = Length;
            int shift = 24;
            int i = 0;
            while (c > 0)
            {
                yield return (byte) (_data[i] >> shift);
                shift -= 8;
                if (shift < 0)
                {
                    shift = 24;
                    i++;
                }

                c -= 8;
            }
        }

        private void Grow()
        {
            int dataLength = _data?.Length ?? 0;
            int growBy = dataLength;
            if (growBy < 128)
                growBy = 128;
            else if (growBy >= 1024)
                growBy = 1024;

            uint[] nd = new uint[dataLength + growBy];
            _data?.CopyTo(nd, 0);
            _data = nd;
        }
    }
}