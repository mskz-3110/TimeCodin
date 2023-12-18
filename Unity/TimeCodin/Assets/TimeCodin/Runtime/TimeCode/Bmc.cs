using System;

namespace TimeCodin {
  public class Bmc {
    public delegate void AnalyzeEvent(bool bit);

    public int MinOffCount;

    private int m_BitCount;

    private int m_OnCount;
    public int OnCount => m_OnCount;

    private int m_OffCount;
    public int OffCount => m_OffCount;

    private bool m_LastBit;

    private bool m_IsHalfOn;

    public Bmc() : this(15){}

    public Bmc(int minOffCount){
      MinOffCount = minOffCount;
    }

    public void Analyze(float[] waves, int channels, AnalyzeEvent onAnalyze){
      for (int i = 0; i < waves.Length; i = i + channels){
        bool bit = 0 <= Math.Sign(waves[i]);
        if (m_LastBit == bit){
          ++m_BitCount;
          continue;
        }

        if (MinOffCount <= m_BitCount){
          m_OffCount = m_BitCount;
          onAnalyze(false);
          m_IsHalfOn = false;
        }else{
          if (m_IsHalfOn){
            m_OnCount = m_BitCount;
            onAnalyze(true);
            m_IsHalfOn = false;
          }else{
            m_IsHalfOn = true;
          }
        }
        m_BitCount = 1;
        m_LastBit = bit;
      }
    }
  }
}