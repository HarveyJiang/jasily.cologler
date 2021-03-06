﻿namespace System.IO
{
    public struct JasilyBuffer
    {
        public JasilyBuffer(byte[] buffer, int offset, int count)
        {
            this.Buffer = buffer;
            this.Offset = offset;
            this.Count = count;
        }

        public byte[] Buffer { get; }

        public int Offset { get; }

        public int Count { get; }

        public JasilyBuffer GetValidBuffer()
        {
            if (this.Offset == 0 && this.Count == this.Buffer.Length)
                return this.Clone();

            var buffer = new byte[this.Count];
            Array.Copy(this.Buffer, this.Offset, buffer, 0, this.Count);
            return new JasilyBuffer(buffer, 0, this.Count);
        }

        public JasilyBuffer Clone() => new JasilyBuffer(this.Buffer, 0, this.Count);
    }
}
