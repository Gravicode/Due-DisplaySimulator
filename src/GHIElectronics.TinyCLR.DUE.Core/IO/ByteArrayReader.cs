// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.IO.ByteArrayReader
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;

namespace GHIElectronics.TinyCLR.DUE.IO
{
  public class ByteArrayReader : TextReaderEx
  {
    private byte[] s;
    private int pos;
    private int len;

    public ByteArrayReader(byte[] s, int index, int count)
    {
      this.s = s ?? throw new ArgumentNullException();
      if (index < 0)
        throw new ArgumentOutOfRangeException();
      if (count < 0)
        throw new ArgumentOutOfRangeException();
      if (index + count > s.Length)
        throw new ArgumentOutOfRangeException();
      this.pos = index;
      this.len = count;
    }

    public ByteArrayReader(byte[] s)
      : this(s, 0, s.Length)
    {
    }

    protected override void Dispose(bool disposing)
    {
      this.s = (byte[]) null;
      this.pos = 0;
      this.len = 0;
      base.Dispose(disposing);
    }

    public override int Peek()
    {
      if (this.s == null)
        throw new ObjectDisposedException(nameof(ByteArrayReader));
      return this.pos == this.len ? -1 : (int) this.s[this.pos];
    }

    public override int Read()
    {
      if (this.s == null)
        throw new ObjectDisposedException(nameof(ByteArrayReader));
      return this.pos == this.len ? -1 : (int) this.s[this.pos++];
    }

    public override string ReadToEnd() => throw new NotImplementedException();
  }
}
