// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Value
// Assembly: GHIElectronics.TinyCLR.DUE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEED8E87-DFD5-41C9-AD3A-F430E438C7AE
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.dll

using System;
using System.Collections;

namespace GHIElectronics.TinyCLR.DUE
{
  internal static class Value
  {
    public static object Negate(object value)
    {
      double num1;
      switch (value)
      {
        case int num2:
          num1 = (double) -num2;
          break;
        case byte num3:
          num1 = (double) -num3;
          break;
        case float num4:
          num1 = -(double) num4;
          break;
        case double num5:
          num1 = -num5;
          break;
        default:
          throw new Exception("You cannot negate a non-numeric value");
      }
      return (object) num1;
    }

    public static int Compare(object lhs, object rhs)
    {
      if ((object) lhs.GetType() != (object) rhs.GetType())
        Value.PromoteTypes(ref lhs, ref rhs);
      switch (lhs)
      {
        case float num5:
          float num1 = num5 - (float) rhs;
          if ((double) num1 == 0.0)
            return 0;
          return (double) num1 <= 0.0 ? -1 : 1;
        case double num6:
          double num2 = num6 - (double) rhs;
          if (num2 == 0.0)
            return 0;
          return num2 <= 0.0 ? -1 : 1;
        case int num7:
          int num3 = num7 - (int) rhs;
          if (num3 == 0)
            return 0;
          return num3 <= 0 ? -1 : 1;
        case byte num8:
          int num4 = (int) num8 - (int) (byte) rhs;
          if (num4 == 0)
            return 0;
          return num4 <= 0 ? -1 : 1;
        case string _:
          return string.Compare((string) lhs, (string) rhs);
        default:
          Value.RaiseInvalidTypeException(lhs);
          return 0;
      }
    }

    public static object Add(object lhs, object rhs)
    {
      if (!(lhs is byte[]) && lhs is ArrayList arrayList)
      {
        arrayList.Add(rhs);
        return (object) arrayList;
      }
      if (lhs is byte[] numArray1 && rhs is byte[] numArray2)
      {
        byte[] numArray = new byte[numArray1.Length + numArray2.Length];
        Array.Copy((Array) numArray1, (Array) numArray, numArray1.Length);
        Array.Copy((Array) numArray2, 0, (Array) numArray, numArray1.Length, numArray2.Length);
        return (object) numArray;
      }
      if ((object) lhs.GetType() != (object) rhs.GetType())
        Value.PromoteTypes(ref lhs, ref rhs);
      switch (lhs)
      {
        case int num1:
          return (object) (num1 + (int) rhs);
        case float num2:
          return (object) (float) ((double) num2 + (double) (float) rhs);
        case double num3:
          return (object) (num3 + (double) rhs);
        case byte num4:
          return (object) ((int) num4 + (int) (byte) rhs);
        case string _:
          return (object) ((string) lhs + (string) rhs);
        default:
          Value.RaiseInvalidTypeException(lhs);
          return (object) null;
      }
    }

    public static object Sub(object lhs, object rhs)
    {
      if ((object) lhs.GetType() != (object) rhs.GetType())
        Value.PromoteTypes(ref lhs, ref rhs);
      switch (lhs)
      {
        case int num1:
          return (object) (num1 - (int) rhs);
        case float num2:
          return (object) (float) ((double) num2 - (double) (float) rhs);
        case double num3:
          return (object) (num3 - (double) rhs);
        case byte num4:
          return (object) ((int) num4 - (int) (byte) rhs);
        default:
          Value.RaiseInvalidTypeException(lhs);
          return (object) null;
      }
    }

    public static object Mul(object lhs, object rhs)
    {
      if ((object) lhs.GetType() != (object) rhs.GetType())
        Value.PromoteTypes(ref lhs, ref rhs);
      switch (lhs)
      {
        case int num1:
          return (object) (num1 * (int) rhs);
        case float num2:
          return (object) (float) ((double) num2 * (double) (float) rhs);
        case double num3:
          return (object) (num3 * (double) rhs);
        case byte num4:
          return (object) ((int) num4 * (int) (byte) rhs);
        default:
          Value.RaiseInvalidTypeException(lhs);
          return (object) null;
      }
    }

    public static object Div(object lhs, object rhs)
    {
      if ((object) lhs.GetType() != (object) rhs.GetType())
        Value.PromoteTypes(ref lhs, ref rhs);
      switch (lhs)
      {
        case int num1:
          return (object) ((double) num1 / (double) (int) rhs);
        case float num2:
          return (object) ((double) num2 / (double) (float) rhs);
        case double num3:
          return (object) (num3 / (double) rhs);
        case byte num4:
          return (object) ((double) num4 / (double) (byte) rhs);
        default:
          Value.RaiseInvalidTypeException(lhs);
          return (object) null;
      }
    }

    public static object Mod(object lhs, object rhs)
    {
      if ((object) lhs.GetType() != (object) rhs.GetType())
        Value.PromoteTypes(ref lhs, ref rhs);
      switch (lhs)
      {
        case int num1:
          return (object) (num1 % (int) rhs);
        case float num2:
          return (object) (float) ((double) num2 % (double) (float) rhs);
        case double num3:
          return (object) (num3 % (double) rhs);
        case byte num4:
          return (object) ((int) num4 % (int) (byte) rhs);
        default:
          Value.RaiseInvalidTypeException(lhs);
          return (object) null;
      }
    }

    public static int And(int lhs, int rhs) => lhs == 0 || rhs == 0 ? 0 : 1;

    public static int Or(int lhs, int rhs) => lhs == 0 && rhs == 0 ? 0 : 1;

    public static int Not(int lhs) => lhs == 0 ? 1 : 0;

    private static void PromoteTypes(ref object lhs, ref object rhs)
    {
      if (lhs is string || rhs is string)
      {
        lhs = (object) lhs.ToString();
        rhs = (object) rhs.ToString();
      }
      else if (lhs is double || rhs is double || lhs is float || rhs is float)
      {
        Value.ChangeTypeToDouble(ref lhs);
        Value.ChangeTypeToDouble(ref rhs);
      }
      else if (lhs is int || rhs is int || lhs is byte || rhs is byte)
      {
        Value.ChangeTypeToInt32(ref lhs);
        Value.ChangeTypeToInt32(ref rhs);
      }
      else
        Value.RaiseInvalidTypeException(lhs);
    }

    private static void ChangeTypeToDouble(ref object value)
    {
      if (value is double)
        return;
      if (value is float num4)
        value = (object) (double) num4;
      else if (value is int num5)
        value = (object) (double) num5;
      else if (value is byte num6)
        value = (object) (double) num6;
      else
        Value.RaiseInvalidTypeException(value);
    }

    private static void ChangeTypeToInt32(ref object value)
    {
      if (value is int)
        return;
      if (value is byte num)
        value = (object) (int) num;
      else
        Value.RaiseInvalidTypeException(value);
    }

    private static void RaiseInvalidTypeException(object o) => throw new Exception(string.Format("Invalid type '{0}'", new object[1]
    {
      (object) o?.GetType().ToString()
    }));
  }
}
