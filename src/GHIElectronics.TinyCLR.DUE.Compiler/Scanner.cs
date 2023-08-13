// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Compiler.Scanner
// Assembly: GHIElectronics.TinyCLR.DUE.Compiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4604F13D-6F7E-4BF6-9153-3AEAE79C81C3
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Compiler.dll

using GHIElectronics.TinyCLR.DUE.IO;
using System.Collections;

namespace GHIElectronics.TinyCLR.DUE.Compiler
{
  public class Scanner : ISourceLocation
  {
    public const char EOF = '\0';
    private readonly TextReaderEx rdr;
    private int currPos;
    private int readPos;
    private int lineStart;
    private Token curTok;
    private bool indentedBlocks;
    private int indentDepth;
    private int indentBlockStart;
    private readonly Stack indentStack = new Stack();

    public char CurrChar { get; private set; }

    public char PeekChar { get; private set; }

    public ushort Line { get; private set; }

    public ushort Col => (ushort) (this.currPos - this.lineStart + 1);

    public int Pos => this.currPos;

    public Scanner(TextReaderEx rdr)
    {
      this.currPos = -1;
      this.lineStart = 0;
      this.readPos = -1;
      this.rdr = rdr;
      this.Line = (ushort) 1;
      this.CurrChar = char.MinValue;
      this.PeekChar = this.ScanChar();
      int num = (int) this.NextChar();
      this.curTok = new Token().Sof(this);
    }

    public char NextChar()
    {
      this.CurrChar = this.PeekChar;
      this.PeekChar = this.ScanChar();
      return this.CurrChar;
    }

    private char ScanChar()
    {
      if (this.rdr.Peek() < 0)
      {
        this.currPos = this.readPos;
        return char.MinValue;
      }
      this.currPos = this.readPos++;
      return (char) this.rdr.Read();
    }

    public Token GetToken()
    {
      if (this.curTok.IsEof)
        return this.curTok;
      this.curTok = this.ScanToken();
      return this.curTok;
    }

    private Token ScanToken()
    {
      while (this.CurrChar != char.MinValue)
      {
        switch (this.CurrChar)
        {
          case '\n':
            int num1 = (int) this.NextChar();
            if (this.CurrChar == '\r')
            {
              int num2 = (int) this.NextChar();
            }
            this.lineStart = this.currPos;
            ++this.Line;
            this.indentDepth = 0;
            continue;
          case '\r':
            int num3 = (int) this.NextChar();
            if (this.CurrChar == '\n')
            {
              int num4 = (int) this.NextChar();
            }
            this.lineStart = this.currPos;
            ++this.Line;
            this.indentDepth = 0;
            continue;
          case '!':
          case '%':
          case '&':
          case '(':
          case ')':
          case '*':
          case '+':
          case ',':
          case '-':
          case ';':
          case '<':
          case '=':
          case '>':
          case '[':
          case ']':
          case '^':
          case '{':
          case '|':
          case '}':
          case '~':
            return this.CheckOutent() ?? new Token().ScanSymbol(this);
          case '"':
          case '\'':
            return new Token().ScanString(this);
          case '#':
            this.SkipLine();
            continue;
          case '/':
            if (this.PeekChar != '/')
              return new Token().ScanSymbol(this);
            this.SkipLine();
            continue;
          case ':':
            this.indentedBlocks = true;
            this.indentBlockStart = this.indentDepth;
            return new Token().ScanSymbol(this);
          default:
            if (this.CurrChar.IsWhiteSpace())
            {
              bool flag = this.Col == (ushort) 1;
              while (this.CurrChar.IsWhiteSpace())
              {
                int num5 = (int) this.NextChar();
                if (flag)
                  ++this.indentDepth;
              }
              continue;
            }
            if (this.CurrChar.IsDigit())
              return new Token().ScanNumber(this);
            if (this.CurrChar.IsLetter())
              return this.CheckOutent() ?? new Token().ScanWord(this);
            Errors.Raise("Unexpected '{0}'", (ISourceLocation) this, (object) this.CurrChar);
            continue;
        }
      }
      if (this.indentStack.Count <= 0)
        return new Token().Eof(this);
      this.indentStack.Pop();
      return new Token().Outdent(this);
    }

    private Token CheckOutent()
    {
      if (this.indentedBlocks && this.indentDepth > this.indentBlockStart)
      {
        this.indentBlockStart = -1;
        if (this.indentDepth > this.LastIndent())
        {
          this.indentStack.Push((object) this.indentDepth);
          return new Token().Indent(this);
        }
        if (this.indentDepth < this.LastIndent())
        {
          this.indentStack.Pop();
          if (this.indentStack.Count == 0)
            this.indentedBlocks = false;
          return new Token().Outdent(this);
        }
      }
      return (Token) null;
    }

    private int LastIndent() => this.indentStack.Count == 0 ? 0 : (int) this.indentStack.Peek();

    private void SkipLine()
    {
      while (this.PeekChar != '\n' && this.PeekChar != '\r' && this.PeekChar != char.MinValue)
      {
        int num1 = (int) this.NextChar();
      }
      int num2 = (int) this.NextChar();
    }
  }
}
