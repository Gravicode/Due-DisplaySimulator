// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Debugging.DebugFunction
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

namespace GHIElectronics.TinyCLR.DUE.Debugging
{
  public class DebugFunction
  {
    private uint range;

    public string Name { get; private set; }

    public int StartIP
    {
      get => (int) (this.range >> 16) & (int) ushort.MaxValue;
      set => this.range = (uint) ((int) this.range & (int) ushort.MaxValue | value << 16);
    }

    public int EndIP
    {
      get => (int) this.range & (int) ushort.MaxValue;
      set => this.range = (uint) ((int) this.range & -65536 | value);
    }

    public DebugFunction Prev { get; private set; }

    public DebugFunction(string name, int startIP, int endIP, DebugFunction prev)
    {
      this.Name = name;
      this.StartIP = startIP;
      this.EndIP = endIP;
      this.Prev = prev;
    }
  }
}
