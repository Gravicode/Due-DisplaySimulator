// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Compiler.CharExtensions
// Assembly: GHIElectronics.TinyCLR.DUE.Compiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4604F13D-6F7E-4BF6-9153-3AEAE79C81C3
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Compiler.dll

namespace GHIElectronics.TinyCLR.DUE.Compiler
{
  internal static class CharExtensions
  {
    public static char ToLower(this char ch) => ch < 'A' && ch > 'Z' ? ch : (char) ((uint) ch | 32U);

    public static bool IsDigit(this char ch) => ch >= '0' && ch <= '9';

    public static bool IsLetter(this char ch)
    {
      if (ch >= 'a' && ch <= 'z')
        return true;
      return ch >= 'A' && ch <= 'Z';
    }

    public static bool IsLetterOrDigit(this char ch) => ch.IsDigit() || ch.IsLetter();

    public static bool IsIdentifierChar(this char ch) => ch.IsDigit() || ch.IsLetter() || ch == '_';

    public static bool IsPrintable(this char ch) => ch >= ' ';

    public static bool IsWhiteSpace(this char ch) => ch == ' ' || ch == '\t';
  }
}
