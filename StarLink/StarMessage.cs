using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarLink
{
    public class StarMessage
    {
        public static int BufferSize = 1024;

        List<byte> dataStream;

        int readPosition;
        public int ReadPosition
        {
            get
            {
                return readPosition;
            }
            set
            {
                readPosition = value;
                if (readPosition < 0)
                    readPosition = 0;
                if (readPosition > dataStream.Count)
                    ReadPosition = dataStream.Count;
            }
        }

        public bool Empty
        {
            get {
                return (dataStream.Count <= 0);
            }
        }

        public StarMessage()
        {
            dataStream = new List<byte>();
        }

        public StarMessage(byte[] buffer)
        {
            dataStream = new List<byte>();
            dataStream.AddRange(buffer);
            ReadPosition = 0;
        }

        public int ReadInt()
        {
            var value = BitConverter.ToInt32(ToBytes(), ReadPosition);
            ReadPosition += sizeof(int);
            return value;
        }

        public long ReadLong()
        {
            var value = BitConverter.ToInt64(ToBytes(), ReadPosition);
            ReadPosition += sizeof(long);
            return value;
        }

        public float ReadFloat()
        {
            var value = BitConverter.ToSingle(ToBytes(), ReadPosition);
            ReadPosition += sizeof(float);
            return value;
        }

        public bool ReadBool()
        {
            var value = BitConverter.ToBoolean(ToBytes(), ReadPosition);
            ReadPosition += sizeof(bool);
            return value;
        }

        public double ReadDouble()
        {
            var value = BitConverter.ToDouble(ToBytes(), ReadPosition);
            ReadPosition += sizeof(double);
            return value;
        }

        public string ReadString()
        {
            int length = ReadInt();
            if (length > 0)
            {
                var value = Encoding.UTF8.GetString(ToBytes(), ReadPosition, length);
                ReadPosition += length;
                return value;
            }
            return string.Empty;
        }

        public void Write(int value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(char value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            dataStream.AddRange(BitConverter.GetBytes(value.Length));
            dataStream.AddRange(ASCIIEncoding.UTF8.GetBytes(value));
        }

        public override string ToString()
        {
            return ASCIIEncoding.UTF8.GetString(ToBytes());
        }

        public string ToString(int size)
        {
            return ASCIIEncoding.UTF8.GetString(ToBytes(), 0, size);
        }

        public byte[] ToBytes()
        {
            return dataStream.ToArray();
        }

    }
}
