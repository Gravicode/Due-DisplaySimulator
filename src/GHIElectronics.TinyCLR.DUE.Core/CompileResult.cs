// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.CompileResult
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using GHIElectronics.TinyCLR.DUE.Debugging;

namespace GHIElectronics.TinyCLR.DUE
{
  public class CompileResult
  {
    public DebugTable DebugTable { get; private set; }

    internal int DataOffset { get; private set; }

    public byte[] Binary { get; private set; }

    public int GlobalCount { get; private set; }

    public CompileResult(DebugTable debugTable, int globalCount, int dataOffset, byte[] binary)
    {
      this.DebugTable = debugTable;
      this.GlobalCount = globalCount;
      this.DataOffset = dataOffset;
      this.Binary = binary;
    }

    public CompileResult(int globalCount, byte[] binary)
    {
      this.GlobalCount = globalCount;
      this.Binary = binary;
    }
  }
}
