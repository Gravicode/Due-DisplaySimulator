// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Symbol
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

namespace GHIElectronics.TinyCLR.DUE
{
  public class Symbol
  {
    private uint meta;

    public string Name { get; private set; }

    public ushort Index
    {
      get => (ushort) (this.meta & (uint) ushort.MaxValue);
      internal set => this.meta = (uint) ((int) this.meta & -65536 | (int) value & (int) ushort.MaxValue);
    }

    public SymbolScope SymbolScope
    {
      get => (SymbolScope) ((this.meta & 15728640U) >> 20);
      private set => this.meta = (uint) ((int) this.meta & -15728641 | (int) (value & (SymbolScope) 15) << 20);
    }

    public SymbolKind SymbolKind
    {
      get => (SymbolKind) ((this.meta & 983040U) >> 16);
      private set => this.meta = (uint) ((int) this.meta & -983041 | (int) (value & (SymbolKind) 15) << 16);
    }

    public Symbol(string name, ushort index, SymbolScope symbolScope, SymbolKind kind)
    {
      this.Name = name;
      this.Index = index;
      this.SymbolScope = symbolScope;
      this.SymbolKind = kind;
    }
  }
}
