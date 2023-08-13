// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Debugging.IDebuggee
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using GHIElectronics.TinyCLR.DUE.IO;

namespace GHIElectronics.TinyCLR.DUE.Debugging
{
  public interface IDebuggee
  {
    event ServerDebugCallback DebugEvent;

    ushort SrcLine { get; }

    ushort SrcCol { get; }

    ushort SrcLength { get; }

    bool ToggleBreakpoint(ushort line);

    bool HitBreakpoint { get; }

    Variable[] GetLocals();

    Variable[] GetAllInScope();

    string[] GetCallStack();

    CompileResult Compile(TextReaderEx source, DebugLevel debugLevel);

    void Run(CompileResult compileResult);

    void Stop();

    void Reset();
  }
}
