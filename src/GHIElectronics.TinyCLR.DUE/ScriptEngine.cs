// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.ScriptEngine
// Assembly: GHIElectronics.TinyCLR.DUE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEED8E87-DFD5-41C9-AD3A-F430E438C7AE
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.dll

using GHIElectronics.TinyCLR.DUE.Compiler;
using GHIElectronics.TinyCLR.DUE.Debugging;
using GHIElectronics.TinyCLR.DUE.IO;
using System;

namespace GHIElectronics.TinyCLR.DUE
{
  public class ScriptEngine : IDebuggee
  {
    private readonly NativeSymbolTable library = new NativeSymbolTable();
    private readonly VM vm;
    private DebugTable debugTable;
    private int currentFrameIndex;
    private IPersister persister;
    private ServerDebugCallback debugEventCallback;

    public ushort SrcLine { get; private set; }

    public ushort SrcCol { get; private set; }

    public ushort SrcLength { get; private set; }

    public bool HitBreakpoint { get; private set; }

    public ScriptEngine(IPersister persister, params Type[] types)
    {
      this.persister = persister;
      this.library.LoadFrom(types);
      this.vm = new VM(this.library.Values);
      this.vm.SetConsole((IConsole) new DefaultConsole());
    }

    public ScriptEngine(params Type[] types)
      : this((IPersister) null, types)
    {
    }

    public void SetConsole(IConsole console) => this.vm.SetConsole(console);

    public event ServerDebugCallback DebugEvent
    {
      add
      {
        this.vm.DebugEvent += new DebugEventHandler(this.Vm_DebugEvent);
        this.debugEventCallback += value;
      }
      remove
      {
        this.vm.DebugEvent -= new DebugEventHandler(this.Vm_DebugEvent);
        this.debugEventCallback -= value;
      }
    }

    private void Vm_DebugEvent(object sender, DebugEventArgs e)
    {
      if (this.debugEventCallback == null)
        return;
      this.HitBreakpoint = e.Breakpoint;
      this.SrcLine = e.SrcLine;
      this.SrcCol = e.SrcCol;
      this.SrcLength = e.SrcLength;
      this.currentFrameIndex = (int) e.Frame;
      e.Action = this.debugEventCallback((IDebuggee) this);
    }

    public void Run(string code, DebugLevel debugLevel = DebugLevel.None)
    {
      using (CodeGenerator codeGenerator = new CodeGenerator(new Scanner((TextReaderEx) new StringReaderEx(code)), this.library, debugLevel))
        this.Run(codeGenerator.Compile());
    }

    public void Run(IPersister persister)
    {
      CompileResult result = persister.Load();
      if (result == null)
        return;
      this.Run(result);
    }

    public CompileResult Compile(TextReaderEx source, DebugLevel debugLevel)
    {
      using (CodeGenerator codeGenerator = new CodeGenerator(new Scanner(source), this.library, debugLevel))
      {
        CompileResult result = codeGenerator.Compile();
        if (this.persister != null)
          this.persister.Save(result);
        return result;
      }
    }

    public void Run(CompileResult result)
    {
      this.debugTable = result.DebugTable;
      this.vm.Run(result);
    }

    public bool ToggleBreakpoint(ushort line) => this.vm.ToggleBreakpoint(line);

    public Variable[] GetLocals() => this.GetVariables(this.currentFrameIndex);

    public Variable[] GetAllInScope() => this.GetVariables(this.currentFrameIndex);

    public string[] GetCallStack()
    {
      int[] callStack = this.vm.GetCallStack();
      string[] strArray = new string[callStack.Length];
      int num = 0;
      foreach (int ip in callStack)
        strArray[num++] = this.debugTable.ResolveFunction(ip);
      return strArray;
    }

    public void Stop() => this.vm.Stop();

    public void Reset() => this.vm.Reset();

    private static bool Contains(Variable[] variables, int count, string key)
    {
      for (int index = 0; index < count; ++index)
      {
        if (variables[index].Name == key)
          return true;
      }
      return false;
    }

    private Variable[] GetVariables(int frameIndex, bool localsOnly = false)
    {
      DebugFrame parent = this.debugTable[frameIndex];
      Variable[] variables = new Variable[10];
      int length = 0;
      while (parent != null && length < variables.Length)
      {
        foreach (Symbol symbol in parent.Symbols)
        {
          if ((symbol.SymbolKind == SymbolKind.Variable || symbol.SymbolKind == SymbolKind.Const) && !ScriptEngine.Contains(variables, length, symbol.Name) && (symbol.SymbolKind == SymbolKind.Variable || symbol.SymbolKind == SymbolKind.Const))
          {
            switch (symbol.SymbolScope)
            {
              case SymbolScope.Global:
                variables[length++] = new Variable(symbol.Name, this.vm.GetGlobal((int) symbol.Index));
                break;
              case SymbolScope.Local:
                variables[length++] = new Variable(symbol.Name, this.vm.GetLocal((int) symbol.Index));
                break;
              case SymbolScope.Argument:
                variables[length++] = new Variable(symbol.Name, this.vm.GetArg((int) symbol.Index));
                break;
            }
            if (length >= variables.Length)
              break;
          }
        }
        parent = parent.Parent;
        if (localsOnly && parent != null && parent.Parent == null)
          break;
      }
      Variable[] variableArray = new Variable[length];
      Array.Copy((Array) variables, (Array) variableArray, length);
      return variableArray;
    }
  }
}
