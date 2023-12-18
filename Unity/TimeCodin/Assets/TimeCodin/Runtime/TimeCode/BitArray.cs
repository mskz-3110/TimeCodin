using System;
using System.Text;

namespace TimeCodin {
  public class BitArray {
    static public bool Equals(bool[] bitArray1, int index1, bool[] bitArray2, int index2, int length){
      for (int i = 0; i < length; ++i){
        if (bitArray1[index1 + i] != bitArray2[index2 + i]) return false;
      }
      return true;
    }

    private bool[] m_BitArray = new bool[0];
    public bool this[int index] => m_BitArray[index];

    private int m_Length;
    public int Length => m_Length;

    private StringBuilder m_StringBuilder = new StringBuilder();

    private const int MinCapacity = 4;

    public BitArray() : this(MinCapacity){}

    public BitArray(int capacity){
      m_BitArray = new bool[Math.Max(MinCapacity, capacity)];
    }

    public void Clear(){
      m_Length = 0;
    }

    public void Add(bool bit){
      if (m_BitArray.Length <= m_Length){
        Array.Resize(ref m_BitArray, m_BitArray.Length * 2);
      }
      m_BitArray[m_Length++] = bit;
    }

    public void Remove(int length){
      if (m_Length < length) length = m_Length;
      m_Length -= length;
      if (0 < m_Length){
        Buffer.BlockCopy(m_BitArray, length, m_BitArray, 0, m_Length);
      }
    }

    public bool Equals(bool[] bitArray){
      return (bitArray.Length == m_Length) ? Equals(m_BitArray, 0, bitArray, 0, Length) : false;
    }

    public int FindLastIndex(bool[] bitArray){
      int length = bitArray.Length;
      for (int i = m_Length - length; 0 <= i; --i){
        if (Equals(m_BitArray, i, bitArray, 0, length)) return i;
      }
      return -1;
    }

    public int ToInt(int index, int count){
      int value = 0;
      for (int i = 0; i < count; ++i){
        if (m_BitArray[index + i]) value |= 1 << i;
      }
      return value;
    }

    public override string ToString(){
      m_StringBuilder.Clear();
      for (int i = 0; i < m_Length; ++i){
        m_StringBuilder.Append(m_BitArray[i] ? "1" : "0");
      }
      return m_StringBuilder.ToString();
    }
  }
}