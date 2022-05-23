using System;
using System.IO;
using System.Text;

namespace APS_2022_1.Net.IO
{
    public class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opCode)
        { 
            _ms.WriteByte(opCode);
        }

        public void WriteMessage(string msg)
        { 
            var msLength = msg.Length;
            _ms.Write(BitConverter.GetBytes(msLength));
            _ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}
