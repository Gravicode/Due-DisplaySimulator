// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Collections.SortedStringMapEntry
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

namespace GHIElectronics.TinyCLR.DUE.Collections
{
  public class SortedStringMapEntry
  {
    public string Key { get; private set; }

    public object Value { get; internal set; }

    public SortedStringMapEntry(string key, object value)
    {
      this.Key = key;
      this.Value = value;
    }
  }
}
