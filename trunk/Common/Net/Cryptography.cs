using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartExtreme.Net
{
    class Cryptography
    {
        public static uint HashPacket(byte[] pData, uint pKey, bool pEncrypt, uint npacketLength)
        {
            uint hash1 = (pKey ^ 0x14B307C8);
            uint hash2 = (pKey ^ 0x8CBF12AC);
            uint hash3 = (pKey ^ 0x240397C1);
            uint hash4 = (pKey ^ 0xF3BD29C0);

            uint checksum = 0;
            int packetLength = (int)npacketLength;//pData.Length;

            
            Action<int, uint> doHash = (index, hash) =>
            {
                pData[index + 0] ^= (byte)(hash & 0xFF);
                pData[index + 1] ^= (byte)((hash << 8) & 0xFF);
                pData[index + 2] ^= (byte)((hash << 16) & 0xFF);
                pData[index + 3] ^= (byte)((hash << 24) & 0xFF);
            };

            int offset = 0;
            int i = 0;
            for (i = 0; i < (packetLength >> 4); ++i)
            {
                if (pEncrypt) 
                    checksum ^= (uint)(BitConverter.ToUInt32(pData, offset + 12) ^ BitConverter.ToUInt32(pData, offset + 8) ^ BitConverter.ToUInt32(pData, offset + 4) ^ BitConverter.ToUInt32(pData, offset + 0));

                doHash(offset + 0, hash1);
                doHash(offset + 4, hash2);
                doHash(offset + 8, hash3);
                doHash(offset + 12, hash4);

                if (!pEncrypt)
                    checksum ^= (uint)(BitConverter.ToUInt32(pData, offset + 12) ^ BitConverter.ToUInt32(pData, offset + 8) ^ BitConverter.ToUInt32(pData, offset + 4) ^ BitConverter.ToUInt32(pData, offset + 0));

                offset += 16;
            }


            i *= 16;
            int sub = 0;

            ///Note: it has a problem with Checksum Calculation
            while (i < packetLength)
            {
                if (pEncrypt) checksum ^= (uint)(pData[i] << sub);
                pData[i] ^= (byte)(hash1 << (sub * 8));
                if (!pEncrypt) checksum ^= (uint)(pData[i] << sub);
                sub++;
                i++;
            }

            /*
            for (int i = offset; i < packetLength; i++)
            {
                if (pEncrypt) checksum ^= (uint)(pData[i] << sub);
                pData[i] ^= (byte)(hash1 << (sub * 8));
                if (!pEncrypt) checksum ^= (uint)(pData[i] << sub);
                sub++;
            }*/

            return checksum;
        }
    }
}
