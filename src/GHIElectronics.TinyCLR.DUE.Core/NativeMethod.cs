// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.NativeMethod
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

using System;
using System.Collections;
using System.Reflection;

namespace GHIElectronics.TinyCLR.DUE
{
  public class NativeMethod
  {
    public MethodInfo method;
    public Type[] paramTypes;

    public NativeMethod(MethodInfo mi)
    {
      this.method = mi;
      ParameterInfo[] parameters = mi.GetParameters();
      this.paramTypes = new Type[parameters.Length];
      for (int index = 0; index < parameters.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters[index];
        this.paramTypes[index] = parameterInfo.ParameterType;
        if (!this.IsValidType(parameterInfo.ParameterType))
          Errors.Raise("Invalid parameter type for builtin method '{0}' argument '{1}'", (ushort) 0, (ushort) 0, (object) mi.Name, (object) index);
      }
    }

    private bool IsValidType(Type type) => (object) type == (object) typeof (float) || (object) type == (object) typeof (double) || (object) type == (object) typeof (int) || (object) type == (object) typeof (string) || (object) type == (object) typeof (byte[]) || (object) type == (object) typeof (object) || (object) type == (object) typeof (ArrayList);
  }
}
