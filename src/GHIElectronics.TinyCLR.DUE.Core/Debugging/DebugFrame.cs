// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Debugging.DebugFrame
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

namespace GHIElectronics.TinyCLR.DUE.Debugging
{
  public class DebugFrame
  {
    private readonly SymbolTable symbols;

    public DebugFrame Parent { get; private set; }

    public ushort Index { get; private set; }

    internal DebugFrame(SymbolTable symbols, ushort index, DebugFrame parent = null)
    {
      this.symbols = symbols;
      this.Parent = parent;
      this.Index = index;
    }

    public Symbol[] Symbols => this.symbols.AllSymbols;
  }
}
