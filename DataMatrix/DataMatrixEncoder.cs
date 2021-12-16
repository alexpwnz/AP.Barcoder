﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AP.Barcoder.DataMatrix
{
    public static class DataMatrixEncoder
    {
        public static IBarcode Encode(string content)
        {
            byte[] data = EncodeText(content);

            CodeSize size = CodeSizes.All.FirstOrDefault(x => x.DataCodewords >= data.Length)
                            ?? throw new InvalidOperationException("Too much data to encode");

            data = AddPadding(data, size.DataCodewords);
            data = ErrorCorrection.CalculateEcc(data, size);
            DataMatrixCode code = Render(data, size);
            code.Content = content;
            return code;
        }

        private static DataMatrixCode Render(byte[] data, CodeSize size)
        {
            CodeLayout codeLayout = new CodeLayout(size);
            codeLayout.SetValues(data);
            return codeLayout.Merge();
        }

        internal static byte[] EncodeText(string content)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < content.Length;)
            {
                char c = content[i];
                i++;

                if (c >= '0' && c <= '9' && i < content.Length && content[i] >= '0' && content[i] <= '9')
                {
                    // Two numbers...
                    char c2 = content[i];
                    i++;
                    result.Add((byte) ((c - '0') * 10 + (c2 - '0') + 130));
                }
                else if (c > 127)
                {
                    // Not correct... needs to be redone later...
                    result.Add(235);
                    result.Add((byte) (c - 127));
                }
                else
                {
                    result.Add((byte) (c + 1));
                }
            }

            return result.ToArray();
        }

        internal static byte[] AddPadding(byte[] data, int toCount)
        {
            List<byte> result = new List<byte>(data);
            if (result.Count < toCount)
                result.Add(129);

            while (result.Count < toCount)
            {
                int r = 149 * (result.Count + 1) % 253 + 1;
                result.Add((byte) ((129 + r) % 254));
            }

            return result.ToArray();
        }
    }
}