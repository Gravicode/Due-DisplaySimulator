// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Collections.IntArrayList
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;
using System.Collections;
using System.Text;

namespace GHIElectronics.TinyCLR.DUE.Collections
{
  public class IntArrayList : IList, ICollection, IEnumerable, ICloneable
  {
    private const int INITIAL_CAPACITY = 4;
    private int count;
    private int[] data;
    private object syncRoot;

    public int[] Buffer => this.data;

    public int Count => this.count;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public object SyncRoot
    {
      get
      {
        if (this.syncRoot == null)
          this.syncRoot = new object();
        return this.syncRoot;
      }
    }

    public bool IsSynchronized => false;

    object IList.this[int index]
    {
      get => (object) this.data[index];
      set => this.data[index] = (int) value;
    }

    public int this[int index]
    {
      get => this.data[index];
      set => this.data[index] = value;
    }

    public IntArrayList(int capacity = 4) => this.data = new int[capacity];

    public IntArrayList(IntArrayList other)
      : this(other.count)
    {
      this.Add(other);
    }

    public IntArrayList(int[] arr)
      : this(arr.Length)
    {
      this.Add(arr);
    }

    public void Clip(int count) => this.count = count >= 0 && count <= this.count ? count : throw new ArgumentOutOfRangeException();

    public int Add(int c)
    {
      if (this.count == this.data.Length)
        this.GrowBy(1);
      this.data[this.count++] = c;
      return this.count - 1;
    }

    public int Add(IntArrayList l)
    {
      this.GrowBy(l.count);
      Array.Copy((Array) l.data, 0, (Array) this.data, this.count, l.count);
      this.count += l.count;
      return this.count - 1;
    }

    public int Add(int[] arr)
    {
      this.GrowBy(arr.Length);
      Array.Copy((Array) arr, 0, (Array) this.data, this.count, arr.Length);
      this.count += arr.Length;
      return this.count - arr.Length;
    }

    public void Insert(int index, int c)
    {
      if (index < 0 || index > this.count)
        throw new ArgumentOutOfRangeException();
      if (index == this.count)
      {
        this.Add(c);
      }
      else
      {
        if (this.count == this.data.Length)
          this.GrowBy(1);
        Array.Copy((Array) this.data, index, (Array) this.data, index + 1, this.count - index);
        this.data[index] = c;
        ++this.count;
      }
    }

    public void RemoveAt(int index)
    {
      if (index < 0 || index >= this.count)
        throw new ArgumentOutOfRangeException();
      if (index < this.count - 1)
        Array.Copy((Array) this.data, index + 1, (Array) this.data, index, this.count - index - 1);
      if (this.count <= 0)
        return;
      --this.count;
    }

    public bool Contains(int value) => this.IndexOf(value) >= 0;

    public int IndexOf(int value)
    {
      for (int index = 0; index < this.data.Length; ++index)
      {
        if (this.data[index] == value)
          return index;
      }
      return -1;
    }

    public void Remove(int value)
    {
      int index = this.IndexOf(value);
      if (index < 0)
        return;
      this.RemoveAt(index);
    }

    public int Add(object value) => this.Add((int) value);

    public bool Contains(object value) => this.Contains((int) value);

    public void Clear() => this.count = 0;

    public int IndexOf(object value) => this.IndexOf((int) value);

    public void Insert(int index, object value) => this.Insert(index, (int) value);

    public void Remove(object value) => this.Remove((int) value);

    public void CopyTo(Array array, int index) => Array.Copy((Array) this.data, index, array, 0, this.count - index);

    public void TrimExcess()
    {
      int[] numArray = new int[this.Count];
      Array.Copy((Array) this.data, 0, (Array) numArray, 0, this.count);
      this.data = numArray;
    }

    public IEnumerator GetEnumerator() => (IEnumerator) new IntArrayList.Enumerator(this);

    public object Clone() => (object) new IntArrayList(this);

    public int[] ToArray()
    {
      int[] numArray = new int[this.Count];
      Array.Copy((Array) this.data, 0, (Array) numArray, 0, this.count);
      return numArray;
    }

    public override string ToString()
    {
      if (this.count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(this.count);
      for (int index = 0; index < this.count; ++index)
        stringBuilder.Append(this.data[index]);
      return stringBuilder.ToString();
    }

    private void GrowBy(int min)
    {
      if (this.count + min < this.data.Length)
        return;
      int[] numArray = new int[Math.Max(this.count + min, this.count * 3 / 2)];
      Array.Copy((Array) this.data, (Array) numArray, this.data.Length);
      this.data = numArray;
    }

    private class Enumerator : IEnumerator
    {
      private static object sentinal = new object();
      private IntArrayList list;
      private int index;
      private object currentElement;

      public Enumerator(IntArrayList list)
      {
        this.list = list;
        this.index = -1;
        this.currentElement = IntArrayList.Enumerator.sentinal;
      }

      public object Current => this.currentElement != IntArrayList.Enumerator.sentinal ? this.currentElement : throw new InvalidOperationException();

      public bool MoveNext()
      {
        if (this.index >= this.list.Count)
        {
          this.currentElement = IntArrayList.Enumerator.sentinal;
          return false;
        }
        ++this.index;
        this.currentElement = (object) this.list[this.index];
        return true;
      }

      public void Reset()
      {
        this.index = -1;
        this.currentElement = IntArrayList.Enumerator.sentinal;
      }
    }
  }
}
