// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.ArrayValue
// Assembly: GHIElectronics.TinyCLR.DUE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEED8E87-DFD5-41C9-AD3A-F430E438C7AE
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.dll

using System.Collections;

namespace GHIElectronics.TinyCLR.DUE
{
  public class ArrayValue : ArrayList
  {
    public static readonly ArrayValue Empty = new ArrayValue();

    public ArrayValue()
    {
    }

    public ArrayValue(ICollection c)
      : base(c)
    {
    }

    public ArrayValue(int capacity)
      : base(capacity)
    {
    }

    public override string ToString() => ArrayValue.ToString((ICollection) this);

    internal static string ToString(ICollection collection)
    {
      string str = "[";
      int num = 0;
      foreach (object obj in (IEnumerable) collection)
      {
        if (num > 0)
          str += ",";
        str += obj.ToString();
        if (++num == 10)
        {
          if (num < collection.Count)
          {
            str += "...";
            break;
          }
          break;
        }
      }
      return str + "]";
    }
  }
}
