namespace TimeCodin {
  public class LtcAnalyzer {
    private BitArray m_BitArray = new BitArray();
    public BitArray BitArray => m_BitArray;

    private Bmc m_Bmc = new Bmc();
    public Bmc Bmc => m_Bmc;

    private Ltc m_Ltc = new Ltc();
    public Ltc Ltc => m_Ltc;

    public void Analyze(float[] waves, int channels, Ltc.AnalyzeEvent onLtcAnalyze, Bmc.AnalyzeEvent onBmcAnalyze = null){
      m_Bmc.Analyze(waves, channels, (bit) => {
        m_BitArray.Add(bit);
        onBmcAnalyze?.Invoke(bit);
      });
      m_Ltc.Analyze(m_BitArray, onLtcAnalyze);
    }
  }
}