// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Collections.SortedStringMap
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;

namespace GHIElectronics.TinyCLR.DUE.Collections
{
  public class SortedStringMap
  {
    private static readonly SortedStringMapEntry[] empty = new SortedStringMapEntry[0];
    private SortedStringMapEntry[] entries;
    private int count;

    public int Count => this.count;

    public SortedStringMapEntry[] Entries
    {
      get
      {
        if (this.entries == null)
          return SortedStringMap.empty;
        if (this.count < this.entries.Length)
          this.Trim();
        return this.entries;
      }
    }

    public void Trim()
    {
      if (this.entries == null || this.entries.Length <= this.count)
        return;
      SortedStringMapEntry[] sortedStringMapEntryArray = new SortedStringMapEntry[this.count];
      Array.Copy((Array) this.entries, 0, (Array) sortedStringMapEntryArray, 0, this.count);
      this.entries = sortedStringMapEntryArray;
    }

    public void Clear()
    {
      Array.Clear((Array) this.entries, 0, this.count);
      this.count = 0;
      this.entries = (SortedStringMapEntry[]) null;
    }

    public object Get(string key)
    {
      int index = this.Search(key);
      return index >= 0 ? this.entries[index].Value : (object) null;
    }

    public void Set(string key, object value)
    {
      if (this.entries == null)
      {
        this.InsertAt(0, key, value);
      }
      else
      {
        int index = this.Search(key);
        if (index >= 0)
          this.entries[index].Value = value;
        else
          this.InsertAt(~index, key, value);
      }
    }

    public bool Contains(string key) => this.Search(key) >= 0;

    private int Search(string key) => this.Search(key, 0, this.count);

    private int Search(string key, int index, int length)
    {
      int num1 = index;
      int num2 = index + length - 1;
      while (num1 <= num2)
      {
        int index1 = num1 + num2 >> 1;
        int num3 = string.Compare(this.entries[index1].Key, key);
        if (num3 == 0)
          return index1;
        if (num3 < 0)
          num1 = index1 + 1;
        else
          num2 = index1 - 1;
      }
      return ~num1;
    }

    private void InsertAt(int index, string key, object value)
    {
      this.Grow(1);
      if (index < this.count)
        Array.Copy((Array) this.entries, index, (Array) this.entries, index + 1, this.count - index);
      this.entries[index] = new SortedStringMapEntry(key, value);
      ++this.count;
    }

    public object this[string key]
    {
      get => this.Get(key);
      set => this.Set(key, value);
    }

    public string[] Keys
    {
      get
      {
        string[] strArray = new string[this.count];
        for (int index = 0; index < this.count; ++index)
          strArray[index] = this.entries[index].Key;
        return strArray;
      }
    }

    private void Grow(int required)
    {
      if (this.entries == null)
      {
        this.entries = new SortedStringMapEntry[Math.Max(4, required)];
      }
      else
      {
        int val1 = this.count + required;
        if (val1 < this.entries.Length)
          return;
        SortedStringMapEntry[] sortedStringMapEntryArray = new SortedStringMapEntry[Math.Max(val1, this.count * 3 / 2)];
        Array.Copy((Array) this.entries, (Array) sortedStringMapEntryArray, this.count);
        this.entries = sortedStringMapEntryArray;
      }
    }
  }
}
