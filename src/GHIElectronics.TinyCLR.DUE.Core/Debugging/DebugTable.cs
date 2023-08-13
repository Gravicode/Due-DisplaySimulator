// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Debugging.DebugTable
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System.Collections;

namespace GHIElectronics.TinyCLR.DUE.Debugging
{
  public class DebugTable
  {
    private readonly ArrayList entries = new ArrayList();
    private DebugFunction functionList;

    public DebugFrame Current { get; private set; }

    public void Enter(SymbolTable symbolTable)
    {
      DebugFrame debugFrame = new DebugFrame(symbolTable, (ushort) this.entries.Count, this.Current);
      this.entries.Add((object) debugFrame);
      this.Current = debugFrame;
    }

    public void LeaveScope()
    {
      if (this.Current == null)
        return;
      if (this.Current.Parent != null && this.Current.Symbols.Length == 0)
        this.entries[this.entries.Count - 1] = (object) this.Current.Parent;
      this.Current = this.Current.Parent;
    }

    public DebugFrame this[int i] => (DebugFrame) this.entries[i];

    public void SetFunctionList(DebugFunction functionList) => this.functionList = functionList;

    public string ResolveFunction(int ip)
    {
      for (DebugFunction debugFunction = this.functionList; debugFunction != null; debugFunction = debugFunction.Prev)
      {
        if (ip >= debugFunction.StartIP && ip < debugFunction.EndIP)
          return debugFunction.Name;
      }
      return string.Empty;
    }
  }
}
