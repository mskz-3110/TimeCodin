using System.IO;
using System.Runtime.InteropServices;

namespace TimeCodin {
  public class Wave {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Format {
      public short AudioFormat;
      public short Channels;
      public int SamplePerSec;
      public int AverageBytesPerSec;
      public short BlockAlign;
      public short BitsPerSample;

      public void Read(BinaryReader reader){
        AudioFormat = reader.ReadInt16();
        Channels = reader.ReadInt16();
        SamplePerSec = reader.ReadInt32();
        AverageBytesPerSec = reader.ReadInt32();
        BlockAlign = reader.ReadInt16();
        BitsPerSample = reader.ReadInt16();
      }

      public override string ToString(){
        return $"AudioFormat=0x{AudioFormat:X4} Channels={Channels} SamplePerSec={SamplePerSec} AverageBytesPerSec={AverageBytesPerSec} BlockAlign={BlockAlign} BitsPerSample={BitsPerSample}";
      }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private struct Int24 {
      [FieldOffset(0)] public int Value;
      [FieldOffset(0)] public byte Byte0;
      [FieldOffset(1)] public byte Byte1;
      [FieldOffset(2)] public byte Byte2;
      [FieldOffset(3)] public byte Byte3;

      public Int24(BinaryReader reader) : this(){
        Set(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
      }

      public Int24(int value) : this(){
        Value = value;
      }

      public Int24(byte byte0, byte byte1, byte byte2) : this(){
        Set(byte0, byte1, byte2);
      }

      public void Set(byte byte0, byte byte1, byte byte2){
        Byte0 = byte0;
        Byte1 = byte1;
        Byte2 = byte2;
        Byte3 = (0x80 <= byte2) ? (byte)0xFF : (byte)0x00;
      }
    }

    public delegate float ReadEvent(BinaryReader reader);

    static public float ReadInt8(BinaryReader reader){
      return (float)reader.ReadSByte() / 0x80;
    }

    static public float ReadInt16(BinaryReader reader){
      return (float)reader.ReadInt16() / 0x8000;
    }

    static public float ReadInt24(BinaryReader reader){
      return (float)(new Int24(reader)).Value / 0x800000;
    }

    static public float ReadFloat32(BinaryReader reader){
      return reader.ReadSingle();
    }

    static public ReadEvent GetReadEvent(int bitsPerSample){
      switch (bitsPerSample){
        case 8: return ReadInt8;
        case 16: return ReadInt16;
        case 24: return ReadInt24;
        default: return ReadFloat32;
      }
    }

    static public void Read(BinaryReader reader, float[] waves, ReadEvent onRead){
      for (int i = 0; i < waves.Length; ++i){
        waves[i] = onRead(reader);
      }
    }
  }
}