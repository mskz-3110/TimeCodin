using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace TimeCodin {
  public class TimeCodinTestEditor {
    private readonly string[] LtcFilePaths = new string[]{
      "./Assets/TimeCodin/Tests/Editor/LTC_09595900_2mins_24fps_48000x16.wav",
      "./Assets/TimeCodin/Tests/Editor/LTC_09595900_2mins_30fps_44100x16.wav",
      "./Assets/TimeCodin/Tests/Editor/LTC_09595900_2mins_30fps_48000x8.wav",
      "./Assets/TimeCodin/Tests/Editor/LTC_09595900_2mins_30fps_48000x16.wav",
      "./Assets/TimeCodin/Tests/Editor/LTC_09595900_2mins_30fps_48000x24.wav"
    };

    [Test]
    public void BitArrayTest(){
      var bitArray = new BitArray();
      Assert.That(bitArray.Length == 0);
      Assert.That(bitArray.ToString() == "");
      bitArray.Add(true);
      Assert.That(bitArray.Length == 1);
      Assert.That(bitArray.ToString() == "1");
      bitArray.Add(false);
      Assert.That(bitArray.Length == 2);
      Assert.That(bitArray.ToString() == "10");
      Assert.That(bitArray[0]);
      Assert.That(!bitArray[1]);
      Assert.That(bitArray.Equals(new bool[]{true, false}));
      bitArray.Clear();
      Assert.That(bitArray.Length == 0);
      Assert.That(bitArray.ToString() == "");

      Assert.That(bitArray.FindLastIndex(Ltc.Frame.SyncWordBitArray) == -1);
      foreach (var bit in Ltc.Frame.SyncWordBitArray){
        bitArray.Add(bit);
      }
      Assert.That(bitArray.ToString() == "0011111111111101");
      Assert.That(bitArray.Length == Ltc.Frame.SyncWordBitArray.Length);
      Assert.That(bitArray.Equals(Ltc.Frame.SyncWordBitArray));
      Assert.That(bitArray.FindLastIndex(Ltc.Frame.SyncWordBitArray) == 0);
      Assert.That(bitArray.FindLastIndex(new bool[]{false}) == 14);
      Assert.That(bitArray.FindLastIndex(new bool[]{true}) == 15);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, false}) == 0);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, true}) == 12);

      bitArray.Clear();
      bitArray.Add(false);
      bitArray.Add(true);
      Assert.That(bitArray.FindLastIndex(new bool[]{false}) == 0);
      Assert.That(bitArray.FindLastIndex(new bool[]{true}) == 1);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, true}) == 0);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, false}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, true}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, false}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, true, true}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, true, true}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, false, true}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, true, false}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, false, false}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{true, false, false}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, true, false}) == -1);
      Assert.That(bitArray.FindLastIndex(new bool[]{false, false, true}) == -1);
    }

    [Test]
    public void LtcTest(){
      var waveDataReader = new WaveDataReader();
      foreach (var ltcFilePath in LtcFilePaths){
        waveDataReader.ReadEach(ltcFilePath, (reader, cts, size) => {
          var format = waveDataReader.Format;
          Debug.Log($"{Path.GetFileName(ltcFilePath)} Format({format})");
          var ltcAnalyzer = new LtcAnalyzer();
          var minTimeCode = new TimeCode(TimeCode.MaxValue);
          var maxTimeCode = new TimeCode();
          var onRead = Wave.GetReadEvent(format.BitsPerSample);
          var waves = new float[format.SamplePerSec / 100 * format.Channels];
          var count = size / waves.Length / (format.BitsPerSample / 8);
          for (var i = 0; i < count; ++i){
            Wave.Read(reader, waves, onRead);
            ltcAnalyzer.Analyze(waves, format.Channels, (in TimeCode timeCode) => {
              minTimeCode.Value = Math.Min(minTimeCode.Value, timeCode.Value);
              maxTimeCode.Value = Math.Max(maxTimeCode.Value, timeCode.Value);
            });
          }
          var bmc = ltcAnalyzer.Bmc;
          var ltc = ltcAnalyzer.Ltc;
          var framePerSec = ltc.FramePerSec;
          var diffTimeCode = TimeCode.ToFloat(maxTimeCode, framePerSec) - TimeCode.ToFloat(minTimeCode, framePerSec);
          Debug.Log($"Bmc(OnCount={bmc.OnCount} OffCount={bmc.OffCount}) Ltc(Min={minTimeCode} Max={maxTimeCode} Diff={diffTimeCode:F3} FramePerSec={framePerSec})");
          Assert.That((0 < bmc.OnCount) && (bmc.OnCount < bmc.MinOffCount));
          Assert.That((0 < bmc.OffCount) && (bmc.MinOffCount <= bmc.OffCount));
          Assert.That(120f <= diffTimeCode);
        });
      }
    }
  }
}