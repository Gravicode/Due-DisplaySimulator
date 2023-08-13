// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.NativeSymbolTable
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;
using System.Collections;
using System.Reflection;

namespace GHIElectronics.TinyCLR.DUE
{
  public class NativeSymbolTable : SymbolTable
  {
    private object[] values;

    public object[] Values => this.values;

    public void LoadFrom(Type[] types)
    {
      ArrayList libraryValues = new ArrayList();
      foreach (Type type in types)
        this.LoadFrom(type, libraryValues);
      this.Trim();
      this.values = libraryValues.ToArray();
    }

    private void LoadFrom(Type type, ArrayList libraryValues)
    {
      foreach (MethodInfo method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
      {
        this.Declare(method.Name.ToLower(), SymbolScope.Global, SymbolKind.BuiltIn, (ushort) libraryValues.Count);
        libraryValues.Add((object) new NativeMethod(method));
      }
      foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
      {
        this.Declare(field.Name.ToLower(), SymbolScope.Global, SymbolKind.BuiltIn, (ushort) libraryValues.Count);
        libraryValues.Add(field.GetValue((object) null));
      }
    }

    public NativeSymbolTable()
      : base()
    {
    }
  }
}
