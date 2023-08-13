// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Compiler.Token
// Assembly: GHIElectronics.TinyCLR.DUE.Compiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4604F13D-6F7E-4BF6-9153-3AEAE79C81C3
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Compiler.dll

using System;
using System.Text;

namespace GHIElectronics.TinyCLR.DUE.Compiler
{
  public class Token : ISourceLocation
  {
    public static readonly Token BuiltIn = new Token();
    protected static readonly StringBuilder Buffer = new StringBuilder(32);
    private ulong loc;

    public ushort Line
    {
      get => (ushort) ((this.loc & 281470681743360UL) >> 32);
      set => this.loc = (ulong) ((long) this.loc & -281470681743361L | (long) value << 32);
    }

    public ushort Col
    {
      get => (ushort) ((this.loc & 4293918720UL) >> 20);
      set => this.loc = (ulong) ((long) this.loc & -4293918721L | (long) value << 20);
    }

    public ushort Length
    {
      get => (ushort) ((this.loc & 1048320UL) >> 8);
      set => this.loc = (ulong) ((long) this.loc & -1048321L | (long) ((int) value & 4095) << 8);
    }

    public TokenType TokenType
    {
      get => (TokenType) (byte) (this.loc & (ulong) byte.MaxValue);
      set => this.loc = this.loc & 18446744073709551360UL | (ulong) (byte) value;
    }

    public object Value { get; private set; }

    public override string ToString() => string.Format("{0}:{1} <{2}>({3})", (object) this.Line, (object) this.Col, (object) this.TokenType, this.Value);

    public bool Is(TokenType tokenType) => this.TokenType == tokenType;

    public bool IsNot(TokenType tokenType) => !this.Is(tokenType);

    public bool IsOneOf(params TokenType[] tokenTypes)
    {
      foreach (TokenType tokenType in tokenTypes)
      {
        if (this.TokenType == tokenType)
          return true;
      }
      return false;
    }

    public bool IsNotOneOf(params TokenType[] tokenTypes) => !this.IsOneOf(tokenTypes);

    public bool IsEof => this.TokenType == TokenType.Eof;

    public Token ScanNumber(Scanner scanner)
    {
      this.Line = scanner.Line;
      this.Col = scanner.Col;
      int pos = scanner.Pos;
      StringBuilder stringBuilder = Token.Buffer.Clear();
      bool flag1 = false;
      bool flag2 = false;
      if (scanner.CurrChar == '0')
      {
        char lower = scanner.PeekChar.ToLower();
        flag1 = lower == 'x';
        flag2 = lower == 'b';
        if (flag1 | flag2)
        {
          int num1 = (int) scanner.NextChar();
          int num2 = (int) scanner.NextChar();
        }
      }
      for (char lower = scanner.CurrChar.ToLower(); scanner.CurrChar.IsDigit() || flag1 && lower >= 'a' && lower <= 'f'; lower = scanner.CurrChar.ToLower())
      {
        stringBuilder.Append(scanner.CurrChar);
        int num = (int) scanner.NextChar();
      }
      bool flag3 = false;
      if (scanner.CurrChar == '.')
      {
        if (flag1)
          Errors.Raise("Invalid hex number", (ISourceLocation) scanner);
        if (flag2)
          Errors.Raise("Invalid binary number", (ISourceLocation) scanner);
        flag3 = true;
        stringBuilder.Append('.');
        int num3 = (int) scanner.NextChar();
        while (scanner.CurrChar.IsDigit())
        {
          stringBuilder.Append(scanner.CurrChar);
          int num4 = (int) scanner.NextChar();
        }
      }
      this.Length = (ushort) (scanner.Pos - pos);
      if (flag3)
      {
        this.TokenType = TokenType.Float;
        try
        {
          this.Value = (object) double.Parse(stringBuilder.ToString());
        }
        catch
        {
          Errors.Raise("Invalid number", this.Line, this.Col);
        }
      }
      else
      {
        this.TokenType = TokenType.Int;
        string s = stringBuilder.ToString();
        if (flag1)
        {
          try
          {
            this.Value = (object) Convert.ToInt32(s, 16);
          }
          catch
          {
            Errors.Raise("Invalid hex number", this.Line, this.Col);
          }
        }
        else if (flag2)
        {
          int num = 0;
          for (int index = 0; index < s.Length; ++index)
          {
            if (s[index] != '1' && s[index] != '0')
              Errors.Raise("Invalid binary number", this.Line, this.Col);
            num = num * 2 + (s[index] == '1' ? 1 : 0);
          }
          this.Value = (object) num;
        }
        else
        {
          try
          {
            this.Value = (object) int.Parse(s);
          }
          catch
          {
            Errors.Raise("Invalid number", this.Line, this.Col);
          }
        }
      }
      return this;
    }

    public Token Indent(Scanner scanner)
    {
      this.Line = scanner.Line;
      this.Col = (ushort) 0;
      this.Length = scanner.Col;
      this.TokenType = TokenType.Indent;
      return this;
    }

    public Token Outdent(Scanner scanner)
    {
      this.Line = scanner.Line;
      this.Col = scanner.Col;
      this.Length = (ushort) 0;
      this.TokenType = TokenType.Outdent;
      return this;
    }

    public Token ScanSymbol(Scanner scanner)
    {
      this.Line = scanner.Line;
      this.Col = scanner.Col;
      int pos = scanner.Pos;
      char currChar = scanner.CurrChar;
      char peekChar = scanner.PeekChar;
      switch (currChar)
      {
        case '!':
          if (peekChar == '=')
          {
            this.TokenType = TokenType.Neq;
            this.Value = (object) "!=";
            int num = (int) scanner.NextChar();
            break;
          }
          this.TokenType = TokenType.Not;
          this.Value = (object) "!";
          break;
        case '%':
          this.TokenType = TokenType.Mod;
          this.Value = (object) "%";
          break;
        case '&':
          if (peekChar == '&')
          {
            this.TokenType = TokenType.And;
            this.Value = (object) "&&";
            int num = (int) scanner.NextChar();
            break;
          }
          this.TokenType = TokenType.BitAnd;
          this.Value = (object) "&";
          break;
        case '(':
          this.TokenType = TokenType.LParen;
          this.Value = (object) "(";
          break;
        case ')':
          this.TokenType = TokenType.RParen;
          this.Value = (object) ")";
          break;
        case '*':
          this.TokenType = TokenType.Times;
          this.Value = (object) "*";
          break;
        case '+':
          this.TokenType = TokenType.Plus;
          this.Value = (object) "+";
          break;
        case ',':
          this.TokenType = TokenType.Comma;
          this.Value = (object) ",";
          break;
        case '-':
          this.TokenType = TokenType.Minus;
          this.Value = (object) "-";
          break;
        case '/':
          this.TokenType = TokenType.Divide;
          this.Value = (object) "/";
          break;
        case ':':
          this.TokenType = TokenType.Colon;
          this.Value = (object) ":";
          break;
        case ';':
          this.TokenType = TokenType.SemiColon;
          this.Value = (object) ";";
          break;
        case '<':
          switch (peekChar)
          {
            case '<':
              this.TokenType = TokenType.Shl;
              this.Value = (object) "<<";
              int num1 = (int) scanner.NextChar();
              break;
            case '=':
              this.TokenType = TokenType.Leq;
              this.Value = (object) "<=";
              int num2 = (int) scanner.NextChar();
              break;
            case '>':
              this.TokenType = TokenType.Neq;
              this.Value = (object) "<>";
              int num3 = (int) scanner.NextChar();
              break;
            default:
              this.TokenType = TokenType.Lt;
              this.Value = (object) "<";
              break;
          }
          break;
        case '=':
          if (peekChar == '=')
          {
            this.TokenType = TokenType.Eq;
            this.Value = (object) "==";
            int num4 = (int) scanner.NextChar();
            break;
          }
          this.TokenType = TokenType.Assign;
          this.Value = (object) "=";
          break;
        case '>':
          switch (peekChar)
          {
            case '=':
              this.TokenType = TokenType.Geq;
              this.Value = (object) ">=";
              int num5 = (int) scanner.NextChar();
              break;
            case '>':
              this.TokenType = TokenType.Shr;
              this.Value = (object) ">>";
              int num6 = (int) scanner.NextChar();
              break;
            default:
              this.TokenType = TokenType.Gt;
              this.Value = (object) ">";
              break;
          }
          break;
        case '[':
          this.TokenType = TokenType.LBracket;
          this.Value = (object) "[";
          break;
        case ']':
          this.TokenType = TokenType.RBracket;
          this.Value = (object) "]";
          break;
        case '^':
          this.TokenType = TokenType.BitXor;
          this.Value = (object) '^';
          break;
        case '{':
          this.TokenType = TokenType.LBrace;
          this.Value = (object) "{";
          break;
        case '|':
          if (peekChar == '|')
          {
            this.TokenType = TokenType.Or;
            this.Value = (object) "||";
            int num7 = (int) scanner.NextChar();
            break;
          }
          this.TokenType = TokenType.BitOr;
          this.Value = (object) "|";
          break;
        case '}':
          this.TokenType = TokenType.RBrace;
          this.Value = (object) "}";
          break;
        case '~':
          this.TokenType = TokenType.BitNot;
          this.Value = (object) '~';
          break;
        default:
          Errors.Raise(string.Format("SymbolToken: Unexpected symbol '{0}'", new object[1]
          {
            (object) currChar
          }), this.Line, this.Col);
          break;
      }
      int num8 = (int) scanner.NextChar();
      this.Length = (ushort) (scanner.Pos - pos);
      if (this.Value is string key)
        this.Value = (object) StringTable.Intern(key);
      return this;
    }

    public Token ScanWord(Scanner scanner)
    {
      this.TokenType = TokenType.Identifier;
      this.Line = scanner.Line;
      this.Col = scanner.Col;
      int pos = scanner.Pos;
      StringBuilder stringBuilder = Token.Buffer.Clear();
      while (scanner.CurrChar.IsIdentifierChar())
      {
        stringBuilder.Append(scanner.CurrChar.ToLower());
        int num = (int) scanner.NextChar();
      }
      string key = stringBuilder.ToString();
      this.Value = (object) StringTable.Intern(key);
      switch (key)
      {
        case "and":
          this.TokenType = TokenType.And;
          break;
        case "array":
          this.TokenType = TokenType.Array;
          break;
        case "begin":
          this.TokenType = TokenType.Begin;
          break;
        case "break":
          this.TokenType = TokenType.Break;
          break;
        case "bytearray":
          this.TokenType = TokenType.ByteArray;
          break;
        case "cls":
          this.TokenType = TokenType.Cls;
          break;
        case "const":
          this.TokenType = TokenType.ConstDecl;
          break;
        case "continue":
          this.TokenType = TokenType.Continue;
          break;
        case "else":
          this.TokenType = TokenType.Else;
          break;
        case "elseif":
          this.TokenType = TokenType.ElseIf;
          break;
        case "end":
          this.TokenType = TokenType.End;
          break;
        case "func":
          this.TokenType = TokenType.FuncDecl;
          break;
        case "if":
          this.TokenType = TokenType.If;
          break;
        case "len":
          this.TokenType = TokenType.Len;
          break;
        case "or":
          this.TokenType = TokenType.Or;
          break;
        case "print":
          this.TokenType = TokenType.Print;
          break;
        case "return":
          this.TokenType = TokenType.Return;
          break;
        case "var":
          this.TokenType = TokenType.VarDecl;
          break;
        case "while":
          this.TokenType = TokenType.While;
          break;
      }
      this.Length = (ushort) (scanner.Pos - pos);
      return this;
    }

    public Token ScanString(Scanner scanner)
    {
      this.TokenType = TokenType.String;
      this.Line = scanner.Line;
      this.Col = scanner.Col;
      int pos = scanner.Pos;
      StringBuilder stringBuilder = Token.Buffer.Clear();
      char currChar = scanner.CurrChar;
      int num1 = (int) scanner.NextChar();
      while (scanner.CurrChar.IsPrintable() && ((int) scanner.CurrChar != (int) currChar || (int) scanner.PeekChar == (int) currChar))
      {
        stringBuilder.Append(scanner.CurrChar);
        int num2 = (int) scanner.NextChar();
      }
      if ((int) scanner.CurrChar != (int) currChar)
        Errors.Raise("Unexpected '{0}'", (ISourceLocation) scanner, (object) scanner.CurrChar);
      int num3 = (int) scanner.NextChar();
      this.Value = (object) StringTable.Intern(stringBuilder.ToString());
      this.Length = (ushort) (scanner.Pos - pos);
      return this;
    }

    public Token Sof(Scanner scanner)
    {
      this.Line = scanner.Line;
      this.Col = scanner.Col;
      this.TokenType = TokenType.Sof;
      this.Value = (object) "SOF";
      return this;
    }

    public Token Eof(Scanner scanner)
    {
      this.Line = scanner.Line;
      this.Col = (ushort) ((uint) scanner.Col + 1U);
      this.TokenType = TokenType.Eof;
      this.Value = (object) "EOF";
      return this;
    }
  }
}
