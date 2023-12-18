using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace TimeCodin {
  public class WaveDataReader {
    public delegate void ReadEvent(BinaryReader reader, CancellationTokenSource cts, uint size);

    private WaveChunkReader m_WaveChunkReader = new WaveChunkReader();

    private Wave.Format m_Format = new Wave.Format();
    public Wave.Format Format => m_Format;

    public void ReadEach(string filePath, ReadEvent onRead){
      m_WaveChunkReader.ReadEach(filePath, (reader, cts, id, size) => {
        switch (id){
          case "RIFF":{
            if (WaveChunkReader.GetString(reader.ReadBytes(4)) != "WAVE") cts.Cancel();
          }break;

          case "fmt ":{
            m_Format.Read(reader);
            reader.BaseStream.Position += size - Marshal.SizeOf<Wave.Format>();
          }break;

          case "data":{
            onRead(reader, cts, size);
          }break;

          default:{
            reader.BaseStream.Position += size;
          }break;
        }
      });
    }
  }
}