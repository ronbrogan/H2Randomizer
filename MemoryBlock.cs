using System;

namespace H2Randomizer
{
    public class MemoryBlock
    {
        public const nint Alignment = 64;
        public nint Address { get; private set; }
        public int Size { get; private set; }

        private nint freeSpot;

        public MemoryBlock(nint address, int size)
        {
            this.Address = address;
            this.Size = size;
            this.freeSpot = address;
        }

        public void Allocate(nint bytes, out nint address, nint alignment = Alignment)
        {
            address = Align(this.freeSpot, alignment);

            this.freeSpot = address + bytes;

            if (this.freeSpot > this.Address + this.Size)
                throw new OutOfMemoryException("Block allocation ran out of memory");
        }

        private static nint Align(nint address, nint alignment = Alignment)
        {
            return ((address + alignment - 1) / alignment) * alignment;
        }
    }
}
