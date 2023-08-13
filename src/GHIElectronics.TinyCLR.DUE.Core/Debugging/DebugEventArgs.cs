// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Debugging.DebugEventArgs
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;

namespace GHIElectronics.TinyCLR.DUE.Debugging
{
  public class DebugEventArgs : EventArgs
  {
    public bool Breakpoint;
    public ushort Frame;
    public int BP;
    public ushort SrcLine;
    public ushort SrcCol;
    public ushort SrcLength;

    public DebugAction Action { get; set; }

    public DebugEventArgs(
      bool breakPoint,
      ushort frame,
      int bp,
      ushort srcLine,
      ushort srcCol,
      ushort srcLength)
    {
      this.Frame = frame;
      this.BP = bp;
      this.Breakpoint = breakPoint;
      this.SrcLine = srcLine;
      this.SrcCol = srcCol;
      this.SrcLength = srcLength;
    }

    public DebugEventArgs()
    {
    }

    public void Set(
      bool breakPoint,
      ushort frame,
      int bp,
      ushort srcLine,
      ushort srcCol,
      ushort srcLength)
    {
      this.Frame = frame;
      this.BP = bp;
      this.Breakpoint = breakPoint;
      this.SrcLine = srcLine;
      this.SrcCol = srcCol;
      this.SrcLength = srcLength;
      this.Action = DebugAction.Continue;
    }
  }
}
