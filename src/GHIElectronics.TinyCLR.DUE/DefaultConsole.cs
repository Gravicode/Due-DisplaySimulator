// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.DefaultConsole
// Assembly: GHIElectronics.TinyCLR.DUE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEED8E87-DFD5-41C9-AD3A-F430E438C7AE
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.dll

using System.Diagnostics;

namespace GHIElectronics.TinyCLR.DUE
{
  public class DefaultConsole : IConsole
  {
    public void Cls()
    {
    }

    public void Print(object text) => Trace.WriteLine(text?.ToString());
  }
}
