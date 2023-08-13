// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Debugging.Variable
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

namespace GHIElectronics.TinyCLR.DUE.Debugging
{
  public class Variable
  {
    private object value;

    public string Name { get; private set; }

    public string Value
    {
      get
      {
        if (this.value is byte[])
          return "<bytearray>";
        return this.value != null ? this.value.ToString() : "0";
      }
    }

    public Variable(string name, object value)
    {
      this.Name = name;
      this.value = value;
    }
  }
}
