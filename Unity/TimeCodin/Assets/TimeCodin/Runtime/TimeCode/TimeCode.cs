using System.Runtime.InteropServices;

namespace TimeCodin {
  [StructLayout(LayoutKind.Explicit, Pack = 1)]
  public struct TimeCode {
    public const int MinValue = 0x00000000;

    public const int MaxValue = 0x18000000;

    static public float ToFloat(byte hour, byte min, byte sec, byte frame = 0, byte framePerSec = 0){
      return (hour * 3600 + min * 60 + sec) + ((framePerSec == 0) ? 0 : (float)frame / framePerSec);
    }

    static public float ToFloat(TimeCode timeCode, byte framePerSec = 0){
      return ToFloat(timeCode.Hour, timeCode.Min, timeCode.Sec, timeCode.Frame, framePerSec);
    }

    [FieldOffset(0)] public int Value;
    [FieldOffset(0)] public byte Frame;
    [FieldOffset(1)] public byte Sec;
    [FieldOffset(2)] public byte Min;
    [FieldOffset(3)] public byte Hour;

    public TimeCode(int value) : this(){
      Value = value;
    }

    public TimeCode(byte hour, byte min, byte sec, byte frame) : this(){
      Hour = hour;
      Min = min;
      Sec = sec;
      Frame = frame;
    }

    public override string ToString(){
      return $"{Hour:00}:{Min:00}:{Sec:00}:{Frame:00}";
    }
  }
}