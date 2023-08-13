// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.IO.StringReaderEx
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;

namespace GHIElectronics.TinyCLR.DUE.IO
{
  public class StringReaderEx : TextReaderEx
  {
    private string s;
    private int pos;
    private int len;

    public StringReaderEx(string s)
    {
      this.s = s ?? throw new ArgumentNullException();
      this.len = s.Length;
    }

    protected override void Dispose(bool disposing)
    {
      this.s = (string) null;
      this.pos = 0;
      this.len = 0;
      base.Dispose(disposing);
    }

    public override int Peek()
    {
      if (this.s == null)
        throw new ObjectDisposedException(nameof(StringReaderEx));
      return this.pos == this.len ? -1 : (int) this.s[this.pos];
    }

    public override int Read()
    {
      if (this.s == null)
        throw new ObjectDisposedException(nameof(StringReaderEx));
      return this.pos == this.len ? -1 : (int) this.s[this.pos++];
    }

    public override string ReadToEnd()
    {
      if (this.s == null)
        throw new ObjectDisposedException(nameof(StringReaderEx));
      string str = this.pos != 0 ? this.s.Substring(this.pos, this.len - this.pos) : this.s;
      this.pos = this.len;
      return str;
    }
  }
}
