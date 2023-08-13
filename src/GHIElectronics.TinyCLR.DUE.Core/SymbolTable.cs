// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.SymbolTable
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using GHIElectronics.TinyCLR.DUE.Collections;
using System.Collections;

namespace GHIElectronics.TinyCLR.DUE
{
  public class SymbolTable : SortedStringMap
  {
    private ushort index;
    private SymbolTable outer;

    public SymbolTable Parent { get; private set; }

    public SymbolTable(SymbolTable parent = null, bool nested = false)
    {
      this.Parent = parent;
      if (!nested)
        return;
      this.outer = this.Parent;
      while (this.outer != null && this.outer.outer != null)
        this.outer = this.outer.Parent;
    }

    public Symbol[] AllSymbols
    {
      get
      {
        Symbol[] symbolArray = new Symbol[this.Count];
        int num = 0;
        foreach (SortedStringMapEntry entry in this.Entries)
          symbolArray[num++] = (Symbol) entry.Value;
        return symbolArray;
      }
    }

    public Symbol[] BuiltIns
    {
      get
      {
        ArrayList arrayList = new ArrayList();
        foreach (SortedStringMapEntry entry in this.Entries)
        {
          Symbol symbol = (Symbol) entry.Value;
          if (symbol.SymbolKind == SymbolKind.BuiltIn)
            arrayList.Add((object) symbol);
        }
        return (Symbol[]) arrayList.ToArray(typeof (Symbol));
      }
    }

    public Symbol[] Functions
    {
      get
      {
        ArrayList arrayList = new ArrayList();
        foreach (SortedStringMapEntry entry in this.Entries)
        {
          Symbol symbol = (Symbol) entry.Value;
          if (symbol.SymbolKind == SymbolKind.Function)
            arrayList.Add((object) symbol);
        }
        return (Symbol[]) arrayList.ToArray(typeof (Symbol));
      }
    }

    public Symbol[] U8ArrayLiterals
    {
      get
      {
        ArrayList arrayList = new ArrayList();
        foreach (SortedStringMapEntry entry in this.Entries)
        {
          Symbol symbol = (Symbol) entry.Value;
          if (symbol.SymbolKind == SymbolKind.U8Array)
            arrayList.Add((object) symbol);
        }
        return (Symbol[]) arrayList.ToArray(typeof (Symbol));
      }
    }

    public Symbol[] StringLiterals
    {
      get
      {
        ArrayList arrayList = new ArrayList();
        foreach (SortedStringMapEntry entry in this.Entries)
        {
          Symbol symbol = (Symbol) entry.Value;
          if (symbol.SymbolKind == SymbolKind.String)
            arrayList.Add((object) symbol);
        }
        return (Symbol[]) arrayList.ToArray(typeof (Symbol));
      }
    }

    public Symbol Declare(string name, SymbolKind kind)
    {
      if (this.outer != null)
      {
        this.index = this.outer.index;
        ++this.outer.index;
      }
      Symbol symbol = new Symbol(name, this.index++, this.Parent == null ? SymbolScope.Global : SymbolScope.Local, kind);
      this.Set(name, (object) symbol);
      return symbol;
    }

    public Symbol Declare(
      string name,
      SymbolScope symbolScope,
      SymbolKind kind,
      ushort index = 65535)
    {
      bool flag = index == ushort.MaxValue && this.outer != null && kind != SymbolKind.String && kind != SymbolKind.U8Array && kind != SymbolKind.Function;
      SymbolTable symbolTable = this;
      if (!flag && symbolScope == SymbolScope.Global && symbolTable.Parent != null)
      {
        while (symbolTable.Parent != null)
          symbolTable = symbolTable.Parent;
      }
      if (symbolTable.Contains(name))
        Errors.Raise("Identifier already declared '{0}'", ushort.MaxValue, (ushort) 4095, (object) name);
      if (flag)
      {
        this.index = this.outer.index;
        ++this.outer.index;
      }
      Symbol symbol = new Symbol(name, index == ushort.MaxValue ? symbolTable.index++ : index, symbolScope, kind);
      symbolTable.Set(name, (object) symbol);
      return symbol;
    }

    public Symbol Resolve(string name)
    {
      for (SymbolTable symbolTable = this; symbolTable != null; symbolTable = symbolTable.Parent)
      {
        if (symbolTable[name] is Symbol symbol1)
          return symbol1;
      }
      return (Symbol) null;
    }

    public Symbol Resolve(string name, SymbolScope scope)
    {
      if (scope != SymbolScope.Local)
        return this.Resolve(name);
      return this[name] is Symbol symbol ? symbol : (Symbol) null;
    }
  }
}
