// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.DueException
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;

namespace GHIElectronics.TinyCLR.DUE
{
  public class DueException : Exception
  {
    public ushort SrcLine { get; private set; }

    public ushort SrcCol { get; private set; }

    public DueException(string message, ushort srcLine, ushort srcCol, Exception innerException = null)
      : base(message, innerException)
    {
      this.SrcLine = srcLine;
      this.SrcCol = srcCol;
    }

    public DueException(string message, Exception innerException = null)
      : base(message, innerException)
    {
      this.SrcLine = ushort.MaxValue;
      this.SrcCol = ushort.MaxValue;
    }
  }
}
