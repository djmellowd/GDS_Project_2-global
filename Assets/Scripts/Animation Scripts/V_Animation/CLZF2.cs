﻿using System;

namespace V_AnimationSystem {

    public static class CLZF2 {
        private static readonly uint HLOG = 14;
        private static readonly uint HSIZE = (1 << 14);
        private static readonly uint MAX_LIT = (1 << 5);
        private static readonly uint MAX_OFF = (1 << 13);
        private static readonly uint MAX_REF = ((1 << 8) + (1 << 3));

        private static readonly long[] HashTable = new long[HSIZE];

        public static byte[] Compress(byte[] inputBytes) {
            int outputByteCountGuess = inputBytes.Length * 2;
            byte[] tempBuffer = new byte[outputByteCountGuess];
            int byteCount = lzf_compress(inputBytes, ref tempBuffer);

            while (byteCount == 0) {
                outputByteCountGuess *= 2;
                tempBuffer = new byte[outputByteCountGuess];
                byteCount = lzf_compress(inputBytes, ref tempBuffer);
            }

            byte[] outputBytes = new byte[byteCount];
            Buffer.BlockCopy(tempBuffer, 0, outputBytes, 0, byteCount);
            return outputBytes;
        }

        public static byte[] Decompress(byte[] inputBytes) {
            int outputByteCountGuess = inputBytes.Length * 2;
            byte[] tempBuffer = new byte[outputByteCountGuess];
            int byteCount = lzf_decompress(inputBytes, ref tempBuffer);

            while (byteCount == 0) {
                outputByteCountGuess *= 2;
                tempBuffer = new byte[outputByteCountGuess];
                byteCount = lzf_decompress(inputBytes, ref tempBuffer);
            }

            byte[] outputBytes = new byte[byteCount];
            Buffer.BlockCopy(tempBuffer, 0, outputBytes, 0, byteCount);
            return outputBytes;
        }

        public static int lzf_compress(byte[] input, ref byte[] output) {
            int inputLength = input.Length;
            int outputLength = output.Length;

            Array.Clear(HashTable, 0, (int)HSIZE);

            long hslot;
            uint iidx = 0;
            uint oidx = 0;
            long reference;

            uint hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);
            long off;
            int lit = 0;

            for (; ; )
            {
                if (iidx < inputLength - 2) {
                    hval = (hval << 8) | input[iidx + 2];
                    hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
                    reference = HashTable[hslot];
                    HashTable[hslot] = (long)iidx;


                    if ((off = iidx - reference - 1) < MAX_OFF
                        && iidx + 4 < inputLength
                        && reference > 0
                        && input[reference + 0] == input[iidx + 0]
                        && input[reference + 1] == input[iidx + 1]
                        && input[reference + 2] == input[iidx + 2]
                        ) {
                        uint len = 2;
                        uint maxlen = (uint)inputLength - iidx - len;
                        maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;

                        if (oidx + lit + 1 + 3 >= outputLength)
                            return 0;

                        do
                            len++;
                        while (len < maxlen && input[reference + len] == input[iidx + len]);

                        if (lit != 0) {
                            output[oidx++] = (byte)(lit - 1);
                            lit = -lit;
                            do
                                output[oidx++] = input[iidx + lit];
                            while ((++lit) != 0);
                        }

                        len -= 2;
                        iidx++;

                        if (len < 7) {
                            output[oidx++] = (byte)((off >> 8) + (len << 5));
                        } else {
                            output[oidx++] = (byte)((off >> 8) + (7 << 5));
                            output[oidx++] = (byte)(len - 7);
                        }

                        output[oidx++] = (byte)off;

                        iidx += len - 1;
                        hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;
                        continue;
                    }
                } else if (iidx == inputLength)
                    break;

                lit++;
                iidx++;

                if (lit == MAX_LIT) {
                    if (oidx + 1 + MAX_LIT >= outputLength)
                        return 0;

                    output[oidx++] = (byte)(MAX_LIT - 1);
                    lit = -lit;
                    do
                        output[oidx++] = input[iidx + lit];
                    while ((++lit) != 0);
                }
            }

            if (lit != 0) {
                if (oidx + lit + 1 >= outputLength)
                    return 0;

                output[oidx++] = (byte)(lit - 1);
                lit = -lit;
                do
                    output[oidx++] = input[iidx + lit];
                while ((++lit) != 0);
            }

            return (int)oidx;
        }


        public static int lzf_decompress(byte[] input, ref byte[] output) {
            int inputLength = input.Length;
            int outputLength = output.Length;

            uint iidx = 0;
            uint oidx = 0;

            do {
                uint ctrl = input[iidx++];

                if (ctrl < (1 << 5))
                {
                    ctrl++;

                    if (oidx + ctrl > outputLength) {
                        return 0;
                    }

                    do
                        output[oidx++] = input[iidx++];
                    while ((--ctrl) != 0);
                } else
                  {
                    uint len = ctrl >> 5;

                    int reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);

                    if (len == 7)
                        len += input[iidx++];

                    reference -= input[iidx++];

                    if (oidx + len + 2 > outputLength) {
                        return 0;
                    }

                    if (reference < 0) {
                        return 0;
                    }

                    output[oidx++] = output[reference++];
                    output[oidx++] = output[reference++];

                    do
                        output[oidx++] = output[reference++];
                    while ((--len) != 0);
                }
            }
            while (iidx < inputLength);

            return (int)oidx;
        }

    }

}