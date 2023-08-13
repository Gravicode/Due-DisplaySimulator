// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Errors
// Assembly: GHIElectronics.TinyCLR.DUE.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 391D9FA1-A140-46BC-909C-8E012264B67C
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Core.dll

namespace GHIElectronics.TinyCLR.DUE
{
  public class Errors
  {
    public const string Unexpected = "Unexpected '{0}'";
    public const string NegateNonNumeric = "You cannot negate a non-numeric value";
    public const string AssignToConstant = "Attempt to modify constant '{0}'";
    public const string InvalidConstDeclaration = "Constants must be declared before any other statements";
    public const string IdentifierNotDeclared = "Identifier not declared '{0}'";
    public const string IdentifierAlreadyDeclared = "Identifier already declared '{0}'";
    public const string FunctionNotDeclared = "Function not declared '{0}'";
    public const string NotAFunction = "Not a function '{0}'";
    public const string ArgumentCountMismatch = "Argument count mismatch, Expected '{0}' got '{1}'";
    public const string ArgumentTypeMismatch = "Argument type mismatch for argument '{0}' of '{1}'";
    public const string TypeMismatch = "Expected '{0}' type";
    public const string InvalidBuiltInReturnType = "Invalid return type for builtin method '{0}'";
    public const string InvalidBuiltInParameterType = "Invalid parameter type for builtin method '{0}' argument '{1}'";
    public const string Expected = "Expected {0}";
    public const string OnlyIndexArraysOrStrings = "Only index arrays or strings";
    public const string IndexOutOfBounds = "Index out of bounds";
    public const string InvalidOperator = "Invalid operator";
    public const string IndexMustBeNumeric = "Index must be numeric";
    public const string NoResult = "No result returned";
    public const string CallbackNotAFunction = "Callback is not a function";
    public const string SyntaxError = "Syntax error";
    public const string InvalidBinaryNumber = "Invalid binary number";
    public const string InvalidHexNumber = "Invalid hex number";
    public const string InvalidNumber = "Invalid number";
    public const string BreakOutsideLoop = "Break outside loop";
    public const string ContinueOutsideLoop = "Continue outside loop";
    public const string TooManyArguments = "Too many arguments";
    public const string InvalidType = "Invalid type '{0}'";
    public const string StackError = "Stack error";

    public static object Raise(string message, ISourceLocation location, params object[] args) => Errors.Raise(message, location.Line, location.Col, args);

    public static object Raise(string message, ushort line, ushort col, params object[] args) => throw new DueException(string.Format("Error at {0}:{1} {2}", new object[3]
    {
      (object) line,
      (object) col,
      (object) string.Format(message, args)
    }), line, col);
  }
}
