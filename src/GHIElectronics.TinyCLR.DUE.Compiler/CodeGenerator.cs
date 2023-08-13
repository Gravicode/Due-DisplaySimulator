// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Compiler.CodeGenerator
// Assembly: GHIElectronics.TinyCLR.DUE.Compiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4604F13D-6F7E-4BF6-9153-3AEAE79C81C3
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Compiler.dll

using GHIElectronics.TinyCLR.DUE.Collections;
using GHIElectronics.TinyCLR.DUE.Debugging;
using System;
using System.Collections;

namespace GHIElectronics.TinyCLR.DUE.Compiler
{
  public class CodeGenerator : IDisposable
  {
    private static readonly string[] emptyStringArray = new string[0];
    private Scanner scanner;
    private Token prevTok;
    private Token currTok;
    private Token nextTok;
    private CodeBuffer code = new CodeBuffer();
    private SortedStringMap functionPatches = new SortedStringMap();
    private SortedStringMap constPatches = new SortedStringMap();
    private SortedStringMap globalConstants = new SortedStringMap();
    private int stringId;
    private SortedStringMap stringMap = new SortedStringMap();
    private NativeSymbolTable library;
    private SymbolTable currentScope;
    private Frame frame;
    private DebugTable debugTable;
    private DebugLevel debugLevel;
    private Stack loopStartStack = new Stack();
    private Stack loopExitStack = new Stack();
    private DebugFunction currentFunction;
    private int lastLine = -1;

    public CodeGenerator(Scanner scanner, NativeSymbolTable library, DebugLevel debugLevel)
    {
      this.scanner = scanner;
      this.library = library;
      this.debugLevel = debugLevel;
      this.nextTok = this.scanner.GetToken();
      this.NextToken();
    }

    public CompileResult Compile()
    {
      if (this.debugLevel != DebugLevel.None)
        this.debugTable = new DebugTable();
      this.currentScope = new SymbolTable();
      this.CompileStringConst("CR", "\n");
      if (this.debugLevel != DebugLevel.None)
        this.debugTable.Enter(this.currentScope);
      int at = this.code.Emit_JP(65278);
      int count1 = this.code.Count;
      while (this.currTok.IsNot(TokenType.Eof))
        this.CompileStatement();
      StringTable.Clear();
      this.EmitDebugInfo(this.currTok);
      this.code.Emit_HALT();
      this.code.PatchOpArgHere(at);
      this.PatchFunctions();
      this.PatchByteArrays();
      this.PatchStrings();
      this.code.Emit_JP(count1);
      int count2 = this.code.Count;
      this.GenerateConstants();
      while (this.code.Count % 4 > 0)
        this.code.Emit_NOP();
      this.code.TrimExcess();
      if (this.debugTable != null)
        this.debugTable.SetFunctionList(this.currentFunction);
      return new CompileResult(this.debugTable, this.code.GlobalCount, count2, this.code.Buffer);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.scanner = (Scanner) null;
      this.prevTok = (Token) null;
      this.currTok = (Token) null;
      this.nextTok = (Token) null;
      this.code = (CodeBuffer) null;
      this.functionPatches = (SortedStringMap) null;
      this.constPatches = (SortedStringMap) null;
      this.globalConstants = (SortedStringMap) null;
      this.stringMap = (SortedStringMap) null;
      this.library = (NativeSymbolTable) null;
      this.currentScope = (SymbolTable) null;
      this.frame = (Frame) null;
      this.debugTable = (DebugTable) null;
      this.loopStartStack = (Stack) null;
      this.loopExitStack = (Stack) null;
      GC.Collect();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~CodeGenerator() => this.Dispose(false);

    private void PatchByteArrays()
    {
      foreach (Symbol u8ArrayLiteral in this.currentScope.U8ArrayLiterals)
      {
        int num = this.code.Emit_LD_IM_PTR(65278, 65278);
        this.code.Emit_ST_GBL(u8ArrayLiteral.Index);
        this.constPatches.Set(u8ArrayLiteral.Name, (object) num);
      }
    }

    private void PatchStrings()
    {
      foreach (Symbol stringLiteral in this.currentScope.StringLiterals)
      {
        int num = this.code.Emit_LD_IM_STR(65278, 65278);
        this.code.Emit_ST_GBL(stringLiteral.Index);
        this.constPatches.Set(stringLiteral.Name, (object) num);
      }
    }

    private void PatchFunctions()
    {
      SortedStringSet sortedStringSet = new SortedStringSet();
      foreach (Symbol function in this.currentScope.Functions)
        sortedStringSet.Add(function.Name);
      foreach (SortedStringMapEntry entry in this.functionPatches.Entries)
      {
        sortedStringSet.Remove(entry.Key);
        Symbol symbol = this.currentScope.Resolve(entry.Key, SymbolScope.Global);
        if (symbol == null)
          Errors.Raise("Function not declared '{0}'", ushort.MaxValue, ushort.MaxValue, (object) entry.Key);
        this.code.Emit_LD_IM_I4((int) entry.Value);
        this.code.Emit_ST_GBL(symbol.Index);
      }
      if (sortedStringSet.Count <= 0)
        return;
      Errors.Raise("Function not declared '{0}'", (ushort) 0, (ushort) 0, (object) sortedStringSet.Entries[0]);
    }

    private void GenerateConstants()
    {
      foreach (SortedStringMapEntry entry in this.globalConstants.Entries)
      {
        int constPatch = (int) this.constPatches[entry.Key];
        switch (this.currentScope.Resolve(entry.Key).SymbolKind)
        {
          case SymbolKind.U8Array:
            byte[] globalConstant1 = (byte[]) this.globalConstants[entry.Key];
            this.code.PatchOpArg(this.code.PatchOpArgHere(constPatch), globalConstant1.Length);
            foreach (byte num in globalConstant1)
              this.code.Emit(num);
            break;
          case SymbolKind.String:
            object globalConstant2 = this.globalConstants[entry.Key];
            string globalConstant3 = (string) this.globalConstants[entry.Key];
            this.code.PatchOpArg(this.code.PatchOpArgHere(constPatch), globalConstant3.Length);
            foreach (byte num in globalConstant3.ToCharArray())
              this.code.Emit(num);
            break;
          default:
            Errors.Raise("Syntax error", (ushort) 65278, (ushort) 65278);
            break;
        }
      }
    }

    private void EmitDebugInfo(Token token)
    {
      switch (this.debugLevel)
      {
        case DebugLevel.Line:
          if (this.lastLine == (int) token.Line)
            break;
          this.lastLine = (int) token.Line;
          this.code.Emit_DBG(this.debugTable.Current.Index, token.Line, token.Col, token.Length);
          break;
        case DebugLevel.Verbose:
          this.code.Emit_DBG(this.debugTable.Current.Index, token.Line, token.Col, token.Length);
          break;
      }
    }

    private bool NextToken()
    {
      if (this.currTok != null && this.currTok.IsEof)
        return false;
      this.prevTok = this.currTok;
      this.currTok = this.nextTok;
      this.nextTok = this.scanner.GetToken();
      return !this.currTok.IsEof;
    }

    private Token Expect(TokenType tokenType, string errorMessage, params object[] args)
    {
      if (this.currTok.TokenType != tokenType)
        Errors.Raise(errorMessage, (ISourceLocation) this.prevTok, args);
      Token currTok = this.currTok;
      this.NextToken();
      return currTok;
    }

    private void CompileStatement()
    {
      switch (this.currTok.TokenType)
      {
        case TokenType.Identifier:
          this.CompileIdentifierStatement();
          break;
        case TokenType.VarDecl:
        case TokenType.ConstDecl:
          this.CompileVarOrConstDecl();
          break;
        case TokenType.FuncDecl:
          this.CompileFuncDecl();
          break;
        case TokenType.Return:
          this.CompileReturn();
          break;
        case TokenType.If:
          this.CompileIf();
          break;
        case TokenType.While:
          this.CompileWhile();
          break;
        case TokenType.Break:
          this.CompileBreak();
          break;
        case TokenType.Continue:
          this.CompileContinue();
          break;
        case TokenType.Print:
          this.CompilePrint();
          break;
        case TokenType.Cls:
          this.CompileCls();
          break;
        default:
          Errors.Raise("Unexpected '{0}'", (ISourceLocation) this.currTok, this.currTok.Value);
          break;
      }
    }

    private void CompileLoadSymbol(string name)
    {
      Symbol symbol = this.currentScope.Resolve(name);
      if (symbol.SymbolScope != SymbolScope.Global)
        Errors.Raise("Syntax error", (ushort) 0, (ushort) 0);
      this.code.Emit_LD_GBL(symbol.Index);
    }

    private void CompileStatementList(TokenType terminator, params TokenType[] terminators)
    {
      while (!this.currTok.IsEof && this.currTok.IsNot(TokenType.Error) && this.currTok.IsNot(terminator) && this.currTok.IsNotOneOf(terminators))
        this.CompileStatement();
    }

    private void CompileVarOrConstDecl()
    {
      SymbolKind kind = this.currTok.Is(TokenType.ConstDecl) ? SymbolKind.Const : SymbolKind.Variable;
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      Token token = this.Expect(TokenType.Identifier, "Expected {0}", (object) "variable name");
      string name = (string) token.Value;
      if (this.currentScope.Resolve(name, SymbolScope.Local) != null)
        Errors.Raise("Identifier already declared '{0}'", (ISourceLocation) token, (object) name);
      this.EmitDebugInfo(token);
      this.Expect(TokenType.Assign, "Expected {0}", (object) "'='");
      this.CompileExpression();
      Symbol symbol;
      if (this.frame != null)
      {
        symbol = this.currentScope.Declare(name, SymbolScope.Local, kind, (ushort) this.frame.LocalCount);
        ++this.frame.LocalCount;
      }
      else
        symbol = this.currentScope.Declare((string) token.Value, SymbolScope.Global, kind);
      this.CompileAssignment(token, symbol);
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
    }

    private void CompileAssignment(Token identifier)
    {
      this.EmitDebugInfo(identifier);
      string name = (string) identifier.Value;
      Symbol symbol = this.currentScope.Resolve(name);
      if (symbol == null)
        Errors.Raise("Identifier not declared '{0}'", (ISourceLocation) identifier, (object) name);
      if (symbol.SymbolKind == SymbolKind.Const)
        Errors.Raise("Attempt to modify constant '{0}'", (ISourceLocation) identifier, (object) name);
      if (symbol == null)
        Errors.Raise("Identifier not declared '{0}'", (ISourceLocation) identifier, (object) name);
      this.CompileExpression();
      this.CompileAssignment(identifier, symbol);
    }

    private void CompileAssignment(Token identifier, Symbol symbol)
    {
      if (symbol.SymbolKind == SymbolKind.Array || symbol.SymbolKind == SymbolKind.U8Array)
      {
        this.code.Emit_ST_ARR_ELEM();
      }
      else
      {
        switch (symbol.SymbolScope)
        {
          case SymbolScope.Global:
            this.code.Emit_ST_GBL(symbol.Index);
            break;
          case SymbolScope.Local:
            this.code.Emit_ST_LOC((byte) symbol.Index);
            break;
          case SymbolScope.Argument:
            this.code.Emit_ST_ARG((byte) symbol.Index);
            break;
          default:
            Errors.Raise("Syntax error", (ISourceLocation) identifier);
            break;
        }
      }
    }

    private void CompileFuncDecl()
    {
      this.NextToken();
      Token token = this.Expect(TokenType.Identifier, "Expected {0}", (object) "Function Name");
      string str = (string) token.Value;
      int at1 = this.code.Emit_JP(65278);
      if (this.functionPatches.Contains(str))
        Errors.Raise("Identifier already declared '{0}'", (ISourceLocation) token, (object) str);
      this.functionPatches[str] = (object) this.code.Count;
      this.Expect(TokenType.LParen, "Expected {0}", (object) "'('");
      string[] identifierList = this.ParseIdentifierList(TokenType.RParen);
      this.Expect(TokenType.RParen, "Expected {0}", (object) "')'");
      Symbol symbol = this.currentScope.Resolve(str);
      if (symbol == null)
        this.currentScope.Declare(str, SymbolScope.Global, SymbolKind.Function);
      else if (symbol.SymbolKind != SymbolKind.Function)
        Errors.Raise("Identifier already declared '{0}'", (ISourceLocation) token, (object) str);
      this.currentScope = new SymbolTable(this.currentScope);
      this.frame = new Frame()
      {
        ArgCount = (byte) identifierList.Length
      };
      for (int index = 0; index < identifierList.Length; ++index)
        this.currentScope.Declare(identifierList[index], SymbolScope.Argument, SymbolKind.Variable, (ushort) (identifierList.Length - index - 1));
      int at2 = this.code.Emit_ALLOC_LOCALS(byte.MaxValue);
      int count = this.code.Count;
      this.CompileCodeBlock();
      this.code.PatchOpArg(at2, this.frame.LocalCount);
      if (this.code.LastOpCode != OpCode.RET)
        this.code.Emit_RET0(this.frame.LocalCount);
      this.code.PatchOpArgHere(at1);
      this.frame = (Frame) null;
      this.currentScope = this.currentScope.Parent;
      this.currentFunction = new DebugFunction(str, count, this.code.Count, this.currentFunction);
    }

    private void CompileCodeBlock()
    {
      try
      {
        if (this.debugLevel != DebugLevel.None)
          this.debugTable.Enter(this.currentScope);
        TokenType tokenType = TokenType.End;
        string str = "END";
        if (this.currTok.Is(TokenType.Colon))
        {
          this.NextToken();
          this.Expect(TokenType.Indent, "Expected {0}", (object) "Indented code block");
          tokenType = TokenType.Outdent;
          str = "Outdent";
        }
        else if (this.currTok.Is(TokenType.LBrace))
        {
          this.NextToken();
          tokenType = TokenType.RBrace;
          str = "}";
        }
        this.CompileStatementList(tokenType);
        if (this.currTok.IsNot(tokenType))
          Errors.Raise("Expected {0}", (ISourceLocation) this.currTok, (object) str);
        this.NextToken();
      }
      finally
      {
        if (this.debugLevel != DebugLevel.None)
          this.debugTable.LeaveScope();
      }
    }

    private void CompileReturn()
    {
      int num = !this.nextTok.IsNot(TokenType.SemiColon) ? 0 : ((int) this.nextTok.Line == (int) this.currTok.Line ? 1 : 0);
      Token currTok = this.currTok;
      this.NextToken();
      if (num != 0)
        this.CompileExpression();
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      this.EmitDebugInfo(currTok);
      this.code.Emit_RET(this.frame.LocalCount);
    }

    private void CompileBreak()
    {
      if (this.loopExitStack.Count == 0)
        Errors.Raise("Break outside loop", (ISourceLocation) this.currTok);
      this.NextToken();
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      this.code.Emit_JP((int) this.loopExitStack.Peek());
    }

    private void CompileContinue()
    {
      if (this.loopStartStack.Count == 0)
        Errors.Raise("Continue outside loop", (ISourceLocation) this.currTok);
      this.NextToken();
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      this.code.Emit_JP((int) this.loopStartStack.Peek());
    }

    private void CompileIdentifierStatement()
    {
      Token currTok = this.currTok;
      if (this.nextTok.Is(TokenType.Assign))
      {
        this.NextToken();
        this.NextToken();
        this.CompileAssignment(currTok);
      }
      else if (this.nextTok.Is(TokenType.LBracket))
      {
        this.CompileIdentifierExpression();
        while (this.currTok.Is(TokenType.LBracket))
        {
          this.Expect(TokenType.LBracket, "Expected {0}", (object) '[');
          this.CompileExpression();
          this.Expect(TokenType.RBracket, "Expected {0}", (object) ']');
          if (this.currTok.Is(TokenType.Assign))
          {
            this.NextToken();
            this.CompileExpression();
            this.code.Emit_ST_ARR_ELEM();
            break;
          }
          this.code.Emit_LD_ARR_ELEM();
        }
      }
      else
      {
        this.CompileIdentifierExpression();
        this.CompileAccessor();
        this.code.Emit_DROP();
      }
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
    }

    private void CompileFunctionCall()
    {
      int num = this.CompileExpressionList();
      if (num > 10)
        Errors.Raise("Too many arguments", (ISourceLocation) this.currTok);
      this.code.Emit_CALL((byte) num);
    }

    private void CompileNativeCall(byte index)
    {
      Token prevTok = this.prevTok;
      if (this.library.Values[(int) index] is NativeMethod nativeMethod)
      {
        int length = nativeMethod.method.GetParameters().Length;
        int num = this.CompileExpressionList();
        if (num != length)
          Errors.Raise("Argument count mismatch, Expected '{0}' got '{1}'", (ISourceLocation) prevTok, (object) length, (object) num);
        this.code.Emit_NCALL(index, (byte) num);
      }
      else
        Errors.Raise("Function not declared '{0}'", (ISourceLocation) prevTok, prevTok.Value);
    }

    private int CompileExpressionList(TokenType terminator = TokenType.RParen)
    {
      this.NextToken();
      int num = 0;
      while (this.currTok.IsNot(terminator))
      {
        this.CompileExpression();
        ++num;
        if (this.currTok.Is(TokenType.Comma))
          this.NextToken();
      }
      this.Expect(terminator, "Syntax error");
      return num;
    }

    private void CompileIf()
    {
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      IntArrayList intArrayList = new IntArrayList()
      {
        this.CompileIfCondition()
      };
      if (this.currTok.Is(TokenType.End))
      {
        this.NextToken();
      }
      else
      {
        while (this.currTok.Is(TokenType.ElseIf))
        {
          this.NextToken();
          this.EmitDebugInfo(this.currTok);
          intArrayList.Add(this.CompileIfCondition());
        }
        if (this.currTok.Is(TokenType.End))
          this.NextToken();
        else if (this.currTok.Is(TokenType.Else))
        {
          this.NextToken();
          this.EmitDebugInfo(this.currTok);
          this.currentScope = new SymbolTable(this.currentScope, true);
          this.CompileCodeBlock();
          this.currentScope = this.currentScope.Parent;
        }
      }
      foreach (int at in intArrayList)
      {
        if (at >= 0)
          this.code.PatchOpArgHere(at);
      }
    }

    private int CompileIfCondition()
    {
      this.CompileExpression();
      int at = this.code.Emit_JP_Z(65278);
      this.currentScope = new SymbolTable(this.currentScope, true);
      this.CompileIfCodeBlock();
      this.currentScope = this.currentScope.Parent;
      int num = -1;
      if (this.code.LastOpCode != OpCode.RET && this.code.LastOpCode != OpCode.RET0)
        num = this.code.Emit_JP(65278);
      this.code.PatchOpArgHere(at);
      return num;
    }

    private void CompileIfCodeBlock()
    {
      try
      {
        if (this.debugLevel != DebugLevel.None)
          this.debugTable.Enter(this.currentScope);
        TokenType terminator = TokenType.End;
        if (this.currTok.Is(TokenType.Colon))
        {
          this.NextToken();
          this.Expect(TokenType.Indent, "Expected {0}", (object) "Indented code block");
          terminator = TokenType.Outdent;
        }
        else if (this.currTok.Is(TokenType.LBrace))
        {
          this.NextToken();
          terminator = TokenType.RBrace;
        }
        if (terminator == TokenType.End)
        {
          this.CompileStatementList(terminator, TokenType.Else, TokenType.ElseIf);
        }
        else
        {
          this.CompileStatementList(terminator);
          this.NextToken();
        }
      }
      finally
      {
        if (this.debugLevel != DebugLevel.None)
          this.debugTable.LeaveScope();
      }
    }

    private void CompileWhile()
    {
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      int count = this.code.Count;
      this.loopStartStack.Push((object) count);
      this.CompileExpression();
      int at1 = this.code.Emit_JP_NZ(65278);
      int at2 = this.code.Emit_JP(65278);
      this.loopExitStack.Push((object) at2);
      this.code.PatchOpArgHere(at1);
      this.currentScope = new SymbolTable(this.currentScope, true);
      this.CompileCodeBlock();
      this.currentScope = this.currentScope.Parent;
      this.code.Emit_JP(count);
      this.code.PatchOpArgHere(at2);
      this.loopExitStack.Pop();
      this.loopStartStack.Pop();
    }

    private void CompilePrint()
    {
      Token currTok = this.currTok;
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      if ((int) this.currTok.Line == (int) currTok.Line)
        this.CompileExpression();
      else
        this.CompileLoadSymbol("CR");
      this.code.Emit_PRINT();
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
    }

    private void CompileCls()
    {
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      this.code.Emit_CLS();
    }

    private int CompileShortCircuitOn(bool state)
    {
      this.code.Emit_DUP();
      return state ? this.code.Emit_JP_NZ(65278) : this.code.Emit_JP_Z(65278);
    }

    private void CompileExpression()
    {
      this.CompileAndExpression();
      IntArrayList intArrayList = new IntArrayList();
      while (this.currTok.Is(TokenType.Or))
      {
        intArrayList.Add(this.CompileShortCircuitOn(true));
        this.NextToken();
        this.CompileAndExpression();
        this.code.Emit_OR();
      }
      foreach (int at in intArrayList)
        this.code.PatchOpArgHere(at);
    }

    private void CompileAndExpression()
    {
      this.CompileBitOrExpression();
      IntArrayList intArrayList = new IntArrayList();
      while (this.currTok.Is(TokenType.And))
      {
        intArrayList.Add(this.CompileShortCircuitOn(false));
        this.NextToken();
        this.CompileBitOrExpression();
        this.code.Emit_AND();
      }
      foreach (int at in intArrayList)
        this.code.PatchOpArgHere(at);
    }

    private void CompileBitOrExpression()
    {
      this.CompileBitAndExpression();
      while (this.currTok.Is(TokenType.BitOr) || this.currTok.Is(TokenType.BitXor))
      {
        TokenType tokenType = this.currTok.TokenType;
        this.NextToken();
        this.CompileBitAndExpression();
        switch (tokenType)
        {
          case TokenType.BitOr:
            this.code.Emit_BIT_OR();
            continue;
          case TokenType.BitXor:
            this.code.Emit_BIT_XOR();
            continue;
          default:
            continue;
        }
      }
    }

    private void CompileBitAndExpression()
    {
      this.CompileRelExpression();
      while (this.currTok.Is(TokenType.BitAnd))
      {
        this.NextToken();
        this.CompileRelExpression();
        this.code.Emit_BIT_AND();
      }
    }

    private void CompileRelExpression()
    {
      this.CompileShiftExpression();
      while (this.currTok.Is(TokenType.Lt) || this.currTok.Is(TokenType.Leq) || this.currTok.Is(TokenType.Gt) || this.currTok.Is(TokenType.Geq) || this.currTok.Is(TokenType.Eq) || this.currTok.Is(TokenType.Neq))
      {
        TokenType tokenType = this.currTok.TokenType;
        this.NextToken();
        this.CompileShiftExpression();
        switch (tokenType)
        {
          case TokenType.Eq:
            this.code.Emit_TST_EQ();
            continue;
          case TokenType.Neq:
            this.code.Emit_TST_NE();
            continue;
          case TokenType.Lt:
            this.code.Emit_TST_LT();
            continue;
          case TokenType.Leq:
            this.code.Emit_TST_LE();
            continue;
          case TokenType.Gt:
            this.code.Emit_TST_GT();
            continue;
          case TokenType.Geq:
            this.code.Emit_TST_GE();
            continue;
          default:
            continue;
        }
      }
    }

    private void CompileShiftExpression()
    {
      this.CompileAddExpression();
      while (this.currTok.Is(TokenType.BitOr) || this.currTok.Is(TokenType.BitXor))
      {
        TokenType tokenType = this.currTok.TokenType;
        this.NextToken();
        this.CompileAddExpression();
        switch (tokenType)
        {
          case TokenType.Shl:
            this.code.Emit_SHL();
            continue;
          case TokenType.Shr:
            this.code.Emit_SHR();
            continue;
          default:
            continue;
        }
      }
    }

    private void CompileAddExpression()
    {
      this.CompileMultExpression();
      while (this.currTok.Is(TokenType.Plus) || this.currTok.Is(TokenType.Minus))
      {
        TokenType tokenType = this.currTok.TokenType;
        this.NextToken();
        this.CompileMultExpression();
        switch (tokenType)
        {
          case TokenType.Plus:
            this.code.Emit_ADD();
            continue;
          case TokenType.Minus:
            this.code.Emit_SUB();
            continue;
          default:
            continue;
        }
      }
    }

    private void CompileMultExpression()
    {
      this.CompileFactor();
      while (this.currTok.Is(TokenType.Times) || this.currTok.Is(TokenType.Divide) || this.currTok.Is(TokenType.Mod))
      {
        TokenType tokenType = this.currTok.TokenType;
        this.NextToken();
        this.CompileFactor();
        switch (tokenType)
        {
          case TokenType.Times:
            this.code.Emit_MUL();
            continue;
          case TokenType.Divide:
            this.code.Emit_DIV();
            continue;
          case TokenType.Mod:
            this.code.Emit_MOD();
            continue;
          default:
            continue;
        }
      }
    }

    private void CompileFactor()
    {
      bool flag = false;
      while (this.currTok.Is(TokenType.Minus) || this.currTok.Is(TokenType.Plus))
      {
        if (this.currTok.Is(TokenType.Minus))
          flag = !flag;
        this.NextToken();
      }
      if (flag)
      {
        this.CompileFactor();
        this.code.Emit_NEG();
      }
      else
      {
        while (this.currTok.Is(TokenType.Not))
        {
          flag = !flag;
          this.NextToken();
        }
        if (flag)
        {
          this.CompileFactor();
          this.code.Emit_NOT();
        }
        else
        {
          while (this.currTok.Is(TokenType.BitNot))
          {
            flag = !flag;
            this.NextToken();
          }
          if (flag)
          {
            this.CompileFactor();
            this.code.Emit_BIT_NOT();
          }
          else
          {
            switch (this.currTok.TokenType)
            {
              case TokenType.Int:
                this.EmitDebugInfo(this.currTok);
                this.code.Emit_LD_IM_I4((int) this.currTok.Value);
                this.NextToken();
                break;
              case TokenType.Float:
                this.EmitDebugInfo(this.currTok);
                this.code.Emit_LD_IM_F4((float) (double) this.currTok.Value);
                this.NextToken();
                break;
              case TokenType.String:
                this.CompileStringLiteral();
                this.CompileAccessor();
                break;
              case TokenType.ByteArray:
                this.CompileByteArrayLiteral();
                this.CompileAccessor();
                break;
              case TokenType.Identifier:
                this.CompileIdentifierExpression();
                this.CompileAccessor();
                break;
              case TokenType.LParen:
                this.NextToken();
                this.CompileExpression();
                this.Expect(TokenType.RParen, "Expected {0}", (object) "')'");
                break;
              case TokenType.LBracket:
                this.CompileArrayLiteral();
                this.CompileAccessor();
                break;
              case TokenType.Array:
                this.CompileArray();
                break;
              case TokenType.Len:
                this.CompileLen();
                break;
              default:
                Errors.Raise("Syntax error", (ISourceLocation) this.currTok);
                break;
            }
          }
        }
      }
    }

    private void CompileIdentifierExpression()
    {
      this.EmitDebugInfo(this.currTok);
      Token currTok = this.currTok;
      string name = (string) currTok.Value;
      this.NextToken();
      Symbol symbol = this.currentScope.Resolve(name) ?? this.library.Resolve(name);
      if (symbol == null)
      {
        if (this.currTok.Is(TokenType.LParen))
          symbol = this.currentScope.Declare(name, SymbolScope.Global, SymbolKind.Function);
        else
          Errors.Raise("Identifier not declared '{0}'", (ISourceLocation) currTok, (object) name);
      }
      switch (symbol.SymbolScope)
      {
        case SymbolScope.Global:
          if (symbol.SymbolKind == SymbolKind.BuiltIn)
          {
            if (this.library.Values[(int) symbol.Index] is NativeMethod)
            {
              this.CompileNativeCall((byte) symbol.Index);
              break;
            }
            this.code.Emit_LD_BLT_CONST((byte) symbol.Index);
            break;
          }
          this.code.Emit_LD_GBL(symbol.Index);
          break;
        case SymbolScope.Local:
          this.code.Emit_LD_LOC((byte) symbol.Index);
          break;
        case SymbolScope.Argument:
          this.code.Emit_LD_ARG((byte) symbol.Index);
          break;
        default:
          Errors.Raise("Syntax error", (ISourceLocation) currTok);
          break;
      }
    }

    private void CompileAccessor()
    {
      bool flag = true;
      do
      {
        switch (this.currTok.TokenType)
        {
          case TokenType.LParen:
            this.CompileFunctionCall();
            break;
          case TokenType.LBracket:
            this.CompileIndexAccessor();
            break;
          default:
            flag = false;
            break;
        }
      }
      while (flag);
    }

    private void CompileIndexAccessor()
    {
      while (this.currTok.TokenType == TokenType.LBracket)
      {
        this.NextToken();
        this.CompileExpression();
        this.Expect(TokenType.RBracket, "Expected {0}", (object) ']');
        this.code.Emit_LD_ARR_ELEM();
      }
    }

    private void CompileByteArrayLiteral()
    {
      Token currTok = this.currTok;
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      this.Expect(TokenType.LParen, "Expected {0}", (object) '(');
      if (this.currTok.Is(TokenType.LBracket))
      {
        this.EmitDebugInfo(this.currTok);
        this.NextToken();
        ByteArrayList byteArrayList = new ByteArrayList();
        while (this.currTok.IsNot(TokenType.RBracket))
        {
          Token token = this.Expect(TokenType.Int, "Expected '{0}' type", (object) "byte");
          if (token.Value is int num4 && (num4 < 0 || num4 > (int) byte.MaxValue))
            Errors.Raise("Expected '{0}' type", (ISourceLocation) token, (object) "byte");
          byteArrayList.Add((byte) (int) token.Value);
          if (this.currTok.Is(TokenType.Comma))
            this.NextToken();
        }
        if (this.currTok.IsNot(TokenType.RBracket))
          Errors.Raise("Expected {0}", (ISourceLocation) this.currTok, (object) ']');
        this.NextToken();
        Symbol symbol = this.currentScope.Declare("u8[" + byteArrayList.Count.ToString() + "]@" + currTok.Line.ToString() + ":" + currTok.Col.ToString(), SymbolScope.Global, SymbolKind.U8Array);
        byte[] numArray = new byte[byteArrayList.Count];
        byteArrayList.CopyTo((Array) numArray, 0);
        this.globalConstants.Set(symbol.Name, (object) numArray);
        this.code.Emit_LD_GBL(symbol.Index);
      }
      else
      {
        this.CompileExpression();
        this.code.Emit_ALLOC_BYTES();
      }
      this.Expect(TokenType.RParen, "Expected {0}", (object) ')');
    }

    private void CompileArrayLiteral()
    {
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      this.code.Emit_LD_IM_I4(0);
      this.code.Emit_ALLOC_ARRAY();
      while (this.currTok.IsNot(TokenType.RBracket))
      {
        this.CompileExpression();
        this.code.Emit_ADD();
        if (this.currTok.Is(TokenType.Comma))
          this.NextToken();
      }
      if (this.currTok.IsNot(TokenType.RBracket))
        Errors.Raise("Expected {0}", (ISourceLocation) this.currTok, (object) ']');
      this.NextToken();
    }

    private void CompileStringConst(string name, string str) => this.globalConstants.Set(this.currentScope.Declare(name, SymbolScope.Global, SymbolKind.String).Name, (object) str);

    private void CompileStringLiteral()
    {
      this.EmitDebugInfo(this.currTok);
      Token currTok = this.currTok;
      this.NextToken();
      string key = (string) currTok.Value;
      Symbol symbol;
      if (this.stringMap.Contains(key))
      {
        symbol = (Symbol) this.stringMap[key];
      }
      else
      {
        symbol = this.currentScope.Declare("str@" + this.stringId.ToString(), SymbolScope.Global, SymbolKind.String);
        ++this.stringId;
        this.stringMap.Set(key, (object) symbol);
        this.globalConstants.Set(symbol.Name, (object) key);
      }
      this.code.Emit_LD_GBL(symbol.Index);
    }

    private void CompileArray()
    {
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      this.Expect(TokenType.LParen, "Expected {0}", (object) '(');
      this.CompileExpression();
      this.Expect(TokenType.RParen, "Expected {0}", (object) ')');
      this.code.Emit_ALLOC_ARRAY();
    }

    private void CompileLen()
    {
      this.EmitDebugInfo(this.currTok);
      this.NextToken();
      this.Expect(TokenType.LParen, "Expected {0}", (object) '(');
      this.CompileExpression();
      this.Expect(TokenType.RParen, "Expected {0}", (object) ')');
      this.code.Emit_LD_ARR_LEN();
    }

    private string[] ParseIdentifierList(TokenType terminator)
    {
      if (this.currTok.IsEof || !this.currTok.IsNot(TokenType.Error) || !this.currTok.IsNot(terminator))
        return CodeGenerator.emptyStringArray;
      ArrayList arrayList = new ArrayList();
      Token token1 = this.Expect(TokenType.Identifier, "Expected {0}", (object) "identifier");
      arrayList.Add((object) (string) token1.Value);
      while (this.currTok.Is(TokenType.Comma))
      {
        this.NextToken();
        Token token2 = this.Expect(TokenType.Identifier, "Expected {0}", (object) "identifier");
        arrayList.Add((object) (string) token2.Value);
      }
      return (string[]) arrayList.ToArray(typeof (string));
    }
  }
}
