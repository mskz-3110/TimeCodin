using System;

namespace TimeCodin {
  public class Ltc {
    public class Frame {
      static public readonly int BitLength = 80;

      static public readonly bool[] SyncWordBitArray = new bool[]{
        false, false, true, true, true, true, true, true,
        true, true, true, true, true, true, false, true
      };

      static public byte GetHour(BitArray bitArray, int headIndex){
        return (byte)(bitArray.ToInt(headIndex + 48, 4) + bitArray.ToInt(headIndex + 56, 2) * 10);
      }

      static public byte GetMin(BitArray bitArray, int headIndex){
        return (byte)(bitArray.ToInt(headIndex + 32, 4) + bitArray.ToInt(headIndex + 40, 3) * 10);
      }

      static public byte GetSec(BitArray bitArray, int headIndex){
        return (byte)(bitArray.ToInt(headIndex + 16, 4) + bitArray.ToInt(headIndex + 24, 3) * 10);
      }

      static public byte GetFrame(BitArray bitArray, int headIndex){
        return (byte)(bitArray.ToInt(headIndex, 4) + bitArray.ToInt(headIndex + 8, 2) * 10);
      }
    }

    public delegate void AnalyzeEvent(in TimeCode timeCode);

    private TimeCode m_TimeCode = new TimeCode();

    private byte m_MaxFrame;
    public byte FramePerSec => (byte)(m_MaxFrame + 1);

    public void Analyze(BitArray bitArray, AnalyzeEvent onAnalyze){
      if (bitArray.Length < Frame.BitLength) return;

      int syncWordIndex = bitArray.FindLastIndex(Frame.SyncWordBitArray);
      if (syncWordIndex < 0){
        bitArray.Remove(bitArray.Length - Frame.BitLength + 1);
        return;
      }

      int headIndex = syncWordIndex - (Frame.BitLength - Frame.SyncWordBitArray.Length);
      if (0 <= headIndex){
        m_TimeCode.Hour = Frame.GetHour(bitArray, headIndex);
        m_TimeCode.Min = Frame.GetMin(bitArray, headIndex);
        m_TimeCode.Sec = Frame.GetSec(bitArray, headIndex);
        m_TimeCode.Frame = Frame.GetFrame(bitArray, headIndex);
        m_MaxFrame = Math.Max(m_MaxFrame, m_TimeCode.Frame);
        onAnalyze(in m_TimeCode);
      }
      bitArray.Remove(syncWordIndex + Frame.SyncWordBitArray.Length);
    }
  }
}