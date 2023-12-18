using System.IO;
using System.Text;
using System.Threading;

namespace TimeCodin {
  public class WaveChunkReader {
    static private ASCIIEncoding ASCII = new ASCIIEncoding();

    static public string GetString(byte[] bytes){
      return ASCII.GetString(bytes, 0, bytes.Length);
    }

    public delegate void ReadEvent(BinaryReader reader, CancellationTokenSource cts, string id, uint size);

    private bool IsCanceled(CancellationTokenSource cts){
      return cts?.IsCancellationRequested ?? false;
    }

    private bool IsReadable(long position, long length, CancellationTokenSource cts){
      return IsCanceled(cts) ? false : position < length;
    }

    public void ReadEach(string filePath, ReadEvent onRead){
      byte[] idBytes = new byte[4];
      CancellationTokenSource cts = new CancellationTokenSource();
      using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath))){
        while (IsReadable(reader.BaseStream.Position, reader.BaseStream.Length, cts)){
          reader.Read(idBytes, 0, idBytes.Length);
          onRead(reader, cts, GetString(idBytes), reader.ReadUInt32());
        }
      }
    }
  }
}