// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.VM
// Assembly: GHIElectronics.TinyCLR.DUE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEED8E87-DFD5-41C9-AD3A-F430E438C7AE
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.dll

using GHIElectronics.TinyCLR.DUE.Debugging;
using System;
using System.Collections;
using System.Text;

namespace GHIElectronics.TinyCLR.DUE
{
  internal class VM
  {
    internal bool raiseDebugEvents;
    private IConsole console;
    private object[] library;
    private ushort lastLine;
    private ushort lastCol = 1;
    private bool running;
    private readonly Hashtable breakpoints = new Hashtable();
    private int stepOverLine = -1;
    private int stepOverBp = -1;
    private const int BP_ARG0_OFFSET = -3;
    private const int BP_LOC0_OFFSET = 1;
    private object[] globals;
    private readonly VM.VmStack stack = new VM.VmStack();
    private byte[] code;
    private int ip;
    private static readonly VM.ExecOp[] execTable;
    private static DebugEventArgs dbgArgs = new DebugEventArgs();
    private static object[][] argBuffer = new object[11][]
    {
      new object[0],
      new object[1],
      new object[2],
      new object[3],
      new object[4],
      new object[5],
      new object[6],
      new object[7],
      new object[8],
      new object[9],
      new object[10]
    };
    private static Type arrayListType = typeof (ArrayList);
    private static Type doubleType = typeof (double);
    private static Type floatType = typeof (float);
    private static Type intType = typeof (int);
    private static Type byteType = typeof (byte);
    private static Type stringType = typeof (string);
    private static Type objectType = typeof (object);

    public event DebugEventHandler DebugEvent;

    internal VM(object[] library) => this.library = library;

    internal void Reset()
    {
      this.breakpoints.Clear();
      this.stepOverLine = -1;
      this.stepOverBp = -1;
      this.ip = 0;
      this.lastLine = (ushort) 0;
      this.lastCol = (ushort) 1;
      this.stack.Clear();
      this.code = (byte[]) null;
      if (this.globals != null)
      {
        for (int index = 0; index < this.globals.Length; ++index)
        {
          object global = this.globals[index];
          if (global != null && !global.GetType().IsArray && global is IDisposable disposable4)
            disposable4.Dispose();
          this.globals[index] = (object) null;
        }
        this.globals = (object[]) null;
      }
      if (this.console == null)
        this.console = (IConsole) new DefaultConsole();
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();
    }

    internal void Run(CompileResult compilerResult)
    {
      this.raiseDebugEvents = this.DebugEvent != null;
      this.code = compilerResult.Binary;
      this.globals = new object[compilerResult.GlobalCount];
      try
      {
        this.running = true;
        VM.ExecOp[] execTable = VM.execTable;
        byte[] code = this.code;
        while (this.running)
          execTable[(int) code[this.ip++]](this);
      }
      catch (DueException ex)
      {
        throw;
      }
      catch (IndexOutOfRangeException ex)
      {
        if (this.stack.sp >= 0 && this.stack.sp < 64)
          return;
        Errors.Raise("Stack error", this.lastLine, this.lastCol);
      }
      catch (Exception ex)
      {
        Errors.Raise(ex.Message, this.lastLine, this.lastCol);
      }
      finally
      {
        this.running = false;
        this.Reset();
      }
    }

    internal void SetConsole(IConsole console) => this.console = console;

    internal void Stop() => this.running = false;

    internal bool ToggleBreakpoint(ushort line)
    {
      if (this.breakpoints.Contains((object) line))
      {
        this.breakpoints.Remove((object) line);
        return false;
      }
      this.breakpoints.Add((object) line, (object) true);
      return true;
    }

    internal object GetGlobal(int index) => this.globals[index];

    internal object GetLocal(int index) => this.stack.Peek(this.stack.bp, 1 + index);

    internal object GetArg(int index) => this.stack.Peek(this.stack.bp, -3 - index);

    internal int[] GetCallStack() => this.stack.GetCallStack(this.ip);

    internal void OnDebugEvent(DebugEventArgs e)
    {
      DebugEventHandler debugEvent = this.DebugEvent;
      if (debugEvent == null)
        return;
      debugEvent((object) this, e);
    }

    static VM()
    {
      VM.execTable = new VM.ExecOp[96];
      VM.execTable[0] = new VM.ExecOp(VM.Exec_NOP);
      VM.execTable[1] = new VM.ExecOp(VM.Exec_HALT);
      VM.execTable[2] = new VM.ExecOp(VM.Exec_DBG);
      VM.execTable[3] = new VM.ExecOp(VM.Exec_DUP);
      VM.execTable[4] = new VM.ExecOp(VM.Exec_DROP);
      VM.execTable[5] = new VM.ExecOp(VM.Exec_NEG);
      VM.execTable[6] = new VM.ExecOp(VM.Exec_LD_IM_0);
      VM.execTable[7] = new VM.ExecOp(VM.Exec_LD_IM_1);
      VM.execTable[8] = new VM.ExecOp(VM.Exec_LD_IM_I4);
      VM.execTable[9] = new VM.ExecOp(VM.Exec_LD_IM_PTR);
      VM.execTable[10] = new VM.ExecOp(VM.Exec_LD_IM_STR);
      VM.execTable[11] = new VM.ExecOp(VM.Exec_LD_IM_F4_0);
      VM.execTable[12] = new VM.ExecOp(VM.Exec_LD_IM_F4_1);
      VM.execTable[13] = new VM.ExecOp(VM.Exec_LD_IM_F4);
      VM.execTable[14] = new VM.ExecOp(VM.Exec_ALLOC_BYTES);
      VM.execTable[15] = new VM.ExecOp(VM.Exec_ALLOC_ARRAY);
      VM.execTable[16] = new VM.ExecOp(VM.Exec_LD_ARR_ELEM);
      VM.execTable[17] = new VM.ExecOp(VM.Exec_ST_ARR_ELEM);
      VM.execTable[18] = new VM.ExecOp(VM.Exec_LD_ARR_LEN);
      VM.execTable[19] = new VM.ExecOp(VM.Exec_LD_ARG_0);
      VM.execTable[20] = new VM.ExecOp(VM.Exec_LD_ARG_1);
      VM.execTable[21] = new VM.ExecOp(VM.Exec_LD_ARG_2);
      VM.execTable[22] = new VM.ExecOp(VM.Exec_LD_ARG_3);
      VM.execTable[23] = new VM.ExecOp(VM.Exec_LD_ARG_4);
      VM.execTable[24] = new VM.ExecOp(VM.Exec_LD_ARG_5);
      VM.execTable[25] = new VM.ExecOp(VM.Exec_LD_ARG_6);
      VM.execTable[26] = new VM.ExecOp(VM.Exec_LD_ARG_7);
      VM.execTable[27] = new VM.ExecOp(VM.Exec_LD_ARG_8);
      VM.execTable[28] = new VM.ExecOp(VM.Exec_LD_ARG_9);
      VM.execTable[29] = new VM.ExecOp(VM.Exec_LD_ARG);
      VM.execTable[30] = new VM.ExecOp(VM.Exec_ST_ARG_0);
      VM.execTable[31] = new VM.ExecOp(VM.Exec_ST_ARG_1);
      VM.execTable[32] = new VM.ExecOp(VM.Exec_ST_ARG_2);
      VM.execTable[33] = new VM.ExecOp(VM.Exec_ST_ARG_3);
      VM.execTable[34] = new VM.ExecOp(VM.Exec_ST_ARG_4);
      VM.execTable[35] = new VM.ExecOp(VM.Exec_ST_ARG_5);
      VM.execTable[36] = new VM.ExecOp(VM.Exec_ST_ARG_6);
      VM.execTable[37] = new VM.ExecOp(VM.Exec_ST_ARG_7);
      VM.execTable[38] = new VM.ExecOp(VM.Exec_ST_ARG_8);
      VM.execTable[39] = new VM.ExecOp(VM.Exec_ST_ARG_9);
      VM.execTable[40] = new VM.ExecOp(VM.Exec_ST_ARG);
      VM.execTable[41] = new VM.ExecOp(VM.Exec_LD_LOC_0);
      VM.execTable[42] = new VM.ExecOp(VM.Exec_LD_LOC_1);
      VM.execTable[43] = new VM.ExecOp(VM.Exec_LD_LOC_2);
      VM.execTable[44] = new VM.ExecOp(VM.Exec_LD_LOC_3);
      VM.execTable[45] = new VM.ExecOp(VM.Exec_LD_LOC_4);
      VM.execTable[46] = new VM.ExecOp(VM.Exec_LD_LOC_5);
      VM.execTable[47] = new VM.ExecOp(VM.Exec_LD_LOC_6);
      VM.execTable[48] = new VM.ExecOp(VM.Exec_LD_LOC_7);
      VM.execTable[49] = new VM.ExecOp(VM.Exec_LD_LOC_8);
      VM.execTable[50] = new VM.ExecOp(VM.Exec_LD_LOC_9);
      VM.execTable[51] = new VM.ExecOp(VM.Exec_LD_LOC);
      VM.execTable[52] = new VM.ExecOp(VM.Exec_LD_GBL);
      VM.execTable[53] = new VM.ExecOp(VM.Exec_LD_BLT_CONST);
      VM.execTable[65] = new VM.ExecOp(VM.Exec_ST_GBL);
      VM.execTable[54] = new VM.ExecOp(VM.Exec_ST_LOC_0);
      VM.execTable[55] = new VM.ExecOp(VM.Exec_ST_LOC_1);
      VM.execTable[56] = new VM.ExecOp(VM.Exec_ST_LOC_2);
      VM.execTable[57] = new VM.ExecOp(VM.Exec_ST_LOC_3);
      VM.execTable[58] = new VM.ExecOp(VM.Exec_ST_LOC_4);
      VM.execTable[59] = new VM.ExecOp(VM.Exec_ST_LOC_5);
      VM.execTable[60] = new VM.ExecOp(VM.Exec_ST_LOC_6);
      VM.execTable[61] = new VM.ExecOp(VM.Exec_ST_LOC_7);
      VM.execTable[62] = new VM.ExecOp(VM.Exec_ST_LOC_8);
      VM.execTable[63] = new VM.ExecOp(VM.Exec_ST_LOC_9);
      VM.execTable[64] = new VM.ExecOp(VM.Exec_ST_LOC);
      VM.execTable[66] = new VM.ExecOp(VM.Exec_ADD);
      VM.execTable[67] = new VM.ExecOp(VM.Exec_SUB);
      VM.execTable[68] = new VM.ExecOp(VM.Exec_MUL);
      VM.execTable[69] = new VM.ExecOp(VM.Exec_DIV);
      VM.execTable[70] = new VM.ExecOp(VM.Exec_MOD);
      VM.execTable[71] = new VM.ExecOp(VM.Exec_AND);
      VM.execTable[72] = new VM.ExecOp(VM.Exec_OR);
      VM.execTable[73] = new VM.ExecOp(VM.Exec_NOT);
      VM.execTable[74] = new VM.ExecOp(VM.Exec_BIT_NOT);
      VM.execTable[75] = new VM.ExecOp(VM.Exec_BIT_AND);
      VM.execTable[76] = new VM.ExecOp(VM.Exec_BIT_OR);
      VM.execTable[77] = new VM.ExecOp(VM.Exec_BIT_XOR);
      VM.execTable[78] = new VM.ExecOp(VM.Exec_SHL);
      VM.execTable[79] = new VM.ExecOp(VM.Exec_SHR);
      VM.execTable[80] = new VM.ExecOp(VM.Exec_TST_EQ);
      VM.execTable[81] = new VM.ExecOp(VM.Exec_TST_NE);
      VM.execTable[82] = new VM.ExecOp(VM.Exec_TST_LT);
      VM.execTable[83] = new VM.ExecOp(VM.Exec_TST_LE);
      VM.execTable[84] = new VM.ExecOp(VM.Exec_TST_GT);
      VM.execTable[85] = new VM.ExecOp(VM.Exec_TST_GE);
      VM.execTable[86] = new VM.ExecOp(VM.Exec_JP);
      VM.execTable[87] = new VM.ExecOp(VM.Exec_JP_Z);
      VM.execTable[88] = new VM.ExecOp(VM.Exec_JP_NZ);
      VM.execTable[89] = new VM.ExecOp(VM.Exec_CALL);
      VM.execTable[90] = new VM.ExecOp(VM.Exec_NCALL);
      VM.execTable[91] = new VM.ExecOp(VM.Exec_ALLOC_LOCALS);
      VM.execTable[92] = new VM.ExecOp(VM.Exec_RET0);
      VM.execTable[93] = new VM.ExecOp(VM.Exec_RET);
      VM.execTable[94] = new VM.ExecOp(VM.Exec_PRINT);
      VM.execTable[95] = new VM.ExecOp(VM.Exec_CLS);
    }

    private static void Exec_NOP(VM vm)
    {
    }

    private static void Exec_HALT(VM vm) => vm.Stop();

    private static void Exec_DBG(VM vm)
    {
      if (vm.raiseDebugEvents)
      {
        ushort uint16_1 = BitConverter.ToUInt16(vm.code, vm.ip);
        vm.ip += 2;
        ushort uint16_2 = BitConverter.ToUInt16(vm.code, vm.ip);
        vm.ip += 2;
        ushort uint16_3 = BitConverter.ToUInt16(vm.code, vm.ip);
        vm.ip += 2;
        ushort uint16_4 = BitConverter.ToUInt16(vm.code, vm.ip);
        vm.ip += 2;
        bool breakPoint = (int) uint16_2 != (int) vm.lastLine && vm.breakpoints.Contains((object) uint16_2);
        vm.lastLine = uint16_2;
        if (!breakPoint && vm.stepOverLine != -1 && (vm.stepOverLine == (int) uint16_2 || vm.stack.bp > vm.stepOverBp))
          return;
        VM.dbgArgs.Set(breakPoint, uint16_1, vm.stack.bp, uint16_2, uint16_3, uint16_4);
        vm.OnDebugEvent(VM.dbgArgs);
        switch (VM.dbgArgs.Action)
        {
          case DebugAction.StepOver:
            vm.stepOverLine = (int) uint16_2;
            vm.stepOverBp = vm.stack.bp;
            break;
          case DebugAction.Stop:
            vm.Stop();
            break;
          default:
            vm.stepOverLine = -1;
            vm.stepOverBp = -1;
            break;
        }
      }
      else
      {
        vm.lastLine = BitConverter.ToUInt16(vm.code, vm.ip + 2);
        vm.ip += 8;
      }
    }

    private static void Exec_DUP(VM vm) => vm.stack.Dup();

    private static void Exec_DROP(VM vm) => vm.stack.Drop();

    private static void Exec_NEG(VM vm) => vm.stack.ReplaceTop(Value.Negate(vm.stack.Peek()));

    private static void Exec_LD_IM_0(VM vm) => vm.stack.Push((object) 0);

    private static void Exec_LD_IM_1(VM vm) => vm.stack.Push((object) 1);

    private static void Exec_LD_IM_I4(VM vm)
    {
      vm.stack.Push((object) BitConverter.ToInt32(vm.code, vm.ip));
      vm.ip += 4;
    }

    private static void Exec_LD_IM_PTR(VM vm)
    {
      int int32_1 = BitConverter.ToInt32(vm.code, vm.ip);
      vm.ip += 4;
      int int32_2 = BitConverter.ToInt32(vm.code, vm.ip);
      vm.ip += 4;
      byte[] numArray = new byte[int32_2];
      Array.Copy((Array) vm.code, int32_1, (Array) numArray, 0, int32_2);
      vm.stack.Push((object) numArray);
    }

    private static void Exec_LD_IM_STR(VM vm)
    {
      int int32_1 = BitConverter.ToInt32(vm.code, vm.ip);
      vm.ip += 4;
      int int32_2 = BitConverter.ToInt32(vm.code, vm.ip);
      vm.ip += 4;
      vm.stack.Push((object) Encoding.UTF8.GetString(vm.code, int32_1, int32_2));
    }

    private static void Exec_LD_IM_F4_0(VM vm) => vm.stack.Push((object) 0.0);

    private static void Exec_LD_IM_F4_1(VM vm) => vm.stack.Push((object) 1.0);

    private static void Exec_LD_IM_F4(VM vm)
    {
      vm.stack.Push((object) (double) BitConverter.ToSingle(vm.code, vm.ip));
      vm.ip += 4;
    }

    private static void Exec_ALLOC_BYTES(VM vm)
    {
      byte[] numArray = new byte[vm.stack.PopInt()];
      vm.stack.Push((object) numArray);
    }

    private static void Exec_ALLOC_ARRAY(VM vm)
    {
      int capacity = vm.stack.PopInt();
      ArrayValue arrayValue = new ArrayValue(capacity);
      for (int index = 0; index < capacity; ++index)
        arrayValue.Add((object) "");
      vm.stack.Push((object) arrayValue);
    }

    private static void Exec_LD_ARR_ELEM(VM vm)
    {
      object obj = vm.stack.Peek(-1);
      int index = vm.stack.PeekInt();
      try
      {
        switch (obj)
        {
          case byte[] numArray2:
            vm.stack.ReplaceTop2((object) numArray2[index]);
            break;
          case ArrayList arrayList2:
            vm.stack.ReplaceTop2(arrayList2[index]);
            break;
          case string str2:
            vm.stack.ReplaceTop2((object) str2[index].ToString());
            break;
          default:
            Errors.Raise("Only index arrays or strings", vm.lastLine, vm.lastCol);
            break;
        }
      }
      catch (ArgumentOutOfRangeException ex)
      {
        Errors.Raise("Index out of bounds", vm.lastLine, vm.lastCol);
      }
    }

    private static void Exec_ST_ARR_ELEM(VM vm)
    {
      try
      {
        object obj = vm.stack.Peek(-2);
        int index = vm.stack.PeekInt(-1);
        switch (obj)
        {
          case byte[] numArray2:
            numArray2[index] = (byte) vm.stack.PeekInt();
            break;
          case ArrayList arrayList2:
            arrayList2[index] = vm.stack.Peek();
            break;
          default:
            Errors.Raise("Expected '{0}' type", vm.lastLine, vm.lastCol, (object) "array/bytearray");
            break;
        }
        vm.stack.Unroll(3);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        Errors.Raise("Index out of bounds", vm.lastLine, vm.lastCol);
      }
    }

    private static void Exec_LD_ARR_LEN(VM vm)
    {
      switch (vm.stack.Peek())
      {
        case byte[] numArray:
          vm.stack.ReplaceTop((object) numArray.Length);
          break;
        case ArrayList arrayList:
          vm.stack.ReplaceTop((object) arrayList.Count);
          break;
        case string str:
          vm.stack.ReplaceTop((object) str.Length);
          break;
      }
    }

    private static void Exec_LD_ARG_0(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -3));

    private static void Exec_LD_ARG_1(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -4));

    private static void Exec_LD_ARG_2(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -5));

    private static void Exec_LD_ARG_3(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -6));

    private static void Exec_LD_ARG_4(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -7));

    private static void Exec_LD_ARG_5(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -8));

    private static void Exec_LD_ARG_6(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -9));

    private static void Exec_LD_ARG_7(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -10));

    private static void Exec_LD_ARG_8(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -11));

    private static void Exec_LD_ARG_9(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -12));

    private static void Exec_LD_ARG(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, -3 - (int) vm.code[vm.ip++]));

    private static void Exec_ST_ARG_0(VM vm) => vm.stack.Set(vm.stack.bp, -3, vm.stack.Pop());

    private static void Exec_ST_ARG_1(VM vm) => vm.stack.Set(vm.stack.bp, -4, vm.stack.Pop());

    private static void Exec_ST_ARG_2(VM vm) => vm.stack.Set(vm.stack.bp, -5, vm.stack.Pop());

    private static void Exec_ST_ARG_3(VM vm) => vm.stack.Set(vm.stack.bp, -6, vm.stack.Pop());

    private static void Exec_ST_ARG_4(VM vm) => vm.stack.Set(vm.stack.bp, -7, vm.stack.Pop());

    private static void Exec_ST_ARG_5(VM vm) => vm.stack.Set(vm.stack.bp, -8, vm.stack.Pop());

    private static void Exec_ST_ARG_6(VM vm) => vm.stack.Set(vm.stack.bp, -9, vm.stack.Pop());

    private static void Exec_ST_ARG_7(VM vm) => vm.stack.Set(vm.stack.bp, -10, vm.stack.Pop());

    private static void Exec_ST_ARG_8(VM vm) => vm.stack.Set(vm.stack.bp, -11, vm.stack.Pop());

    private static void Exec_ST_ARG_9(VM vm) => vm.stack.Set(vm.stack.bp, -12, vm.stack.Pop());

    private static void Exec_ST_ARG(VM vm) => vm.stack.Set(vm.stack.bp, -3 - (int) vm.code[vm.ip++], vm.stack.Pop());

    private static void Exec_LD_LOC_0(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 1));

    private static void Exec_LD_LOC_1(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 2));

    private static void Exec_LD_LOC_2(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 3));

    private static void Exec_LD_LOC_3(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 4));

    private static void Exec_LD_LOC_4(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 5));

    private static void Exec_LD_LOC_5(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 6));

    private static void Exec_LD_LOC_6(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 7));

    private static void Exec_LD_LOC_7(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 8));

    private static void Exec_LD_LOC_8(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 9));

    private static void Exec_LD_LOC_9(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 10));

    private static void Exec_LD_LOC(VM vm) => vm.stack.Push(vm.stack.Peek(vm.stack.bp, 1 + (int) vm.code[vm.ip++]));

    private static void Exec_LD_GBL(VM vm)
    {
      vm.stack.Push(vm.globals[(int) BitConverter.ToUInt16(vm.code, vm.ip)]);
      vm.ip += 2;
    }

    private static void Exec_LD_BLT_CONST(VM vm) => vm.stack.Push(vm.library[(int) vm.code[vm.ip++]]);

    private static void Exec_ST_GBL(VM vm)
    {
      vm.globals[(int) BitConverter.ToInt16(vm.code, vm.ip)] = vm.stack.Pop();
      vm.ip += 2;
    }

    private static void Exec_ST_LOC_0(VM vm) => vm.stack.Set(vm.stack.bp, 1, vm.stack.Pop());

    private static void Exec_ST_LOC_1(VM vm) => vm.stack.Set(vm.stack.bp, 2, vm.stack.Pop());

    private static void Exec_ST_LOC_2(VM vm) => vm.stack.Set(vm.stack.bp, 3, vm.stack.Pop());

    private static void Exec_ST_LOC_3(VM vm) => vm.stack.Set(vm.stack.bp, 4, vm.stack.Pop());

    private static void Exec_ST_LOC_4(VM vm) => vm.stack.Set(vm.stack.bp, 5, vm.stack.Pop());

    private static void Exec_ST_LOC_5(VM vm) => vm.stack.Set(vm.stack.bp, 6, vm.stack.Pop());

    private static void Exec_ST_LOC_6(VM vm) => vm.stack.Set(vm.stack.bp, 7, vm.stack.Pop());

    private static void Exec_ST_LOC_7(VM vm) => vm.stack.Set(vm.stack.bp, 8, vm.stack.Pop());

    private static void Exec_ST_LOC_8(VM vm) => vm.stack.Set(vm.stack.bp, 9, vm.stack.Pop());

    private static void Exec_ST_LOC_9(VM vm) => vm.stack.Set(vm.stack.bp, 10, vm.stack.Pop());

    private static void Exec_ST_LOC(VM vm) => vm.stack.Set(vm.stack.bp, 1 + (int) vm.code[vm.ip++], vm.stack.Pop());

    private static void Exec_ADD(VM vm) => vm.stack.ReplaceTop2(Value.Add(vm.stack.Peek(-1), vm.stack.Peek()));

    private static void Exec_SUB(VM vm) => vm.stack.ReplaceTop2(Value.Sub(vm.stack.Peek(-1), vm.stack.Peek()));

    private static void Exec_MUL(VM vm) => vm.stack.ReplaceTop2(Value.Mul(vm.stack.Peek(-1), vm.stack.Peek()));

    private static void Exec_DIV(VM vm) => vm.stack.ReplaceTop2(Value.Div(vm.stack.Peek(-1), vm.stack.Peek()));

    private static void Exec_MOD(VM vm) => vm.stack.ReplaceTop2(Value.Mod(vm.stack.Peek(-1), vm.stack.Peek()));

    private static void Exec_AND(VM vm) => vm.stack.ReplaceTop2((object) Value.And(vm.stack.PeekInt(-1), vm.stack.PeekInt()));

    private static void Exec_OR(VM vm) => vm.stack.ReplaceTop2((object) Value.Or(vm.stack.PeekInt(-1), vm.stack.PeekInt()));

    private static void Exec_NOT(VM vm) => vm.stack.ReplaceTop((object) Value.Not(vm.stack.PeekInt()));

    private static void Exec_BIT_NOT(VM vm) => vm.stack.ReplaceTop((object) ~vm.stack.PeekInt());

    private static void Exec_BIT_AND(VM vm) => vm.stack.ReplaceTop2((object) (vm.stack.PeekInt(-1) & vm.stack.PeekInt()));

    private static void Exec_BIT_OR(VM vm) => vm.stack.ReplaceTop2((object) (vm.stack.PeekInt(-1) | vm.stack.PeekInt()));

    private static void Exec_BIT_XOR(VM vm) => vm.stack.ReplaceTop2((object) (vm.stack.PeekInt(-1) ^ vm.stack.PeekInt()));

    private static void Exec_SHL(VM vm) => vm.stack.ReplaceTop2((object) (vm.stack.PeekInt(-1) << vm.stack.PeekInt()));

    private static void Exec_SHR(VM vm) => vm.stack.ReplaceTop2((object) (vm.stack.PeekInt(-1) >> vm.stack.PeekInt()));

    private static void Exec_TST_EQ(VM vm) => vm.stack.ReplaceTop2((object) (Value.Compare(vm.stack.Peek(-1), vm.stack.Peek()) == 0 ? 1 : 0));

    private static void Exec_TST_NE(VM vm) => vm.stack.ReplaceTop2((object) (Value.Compare(vm.stack.Peek(-1), vm.stack.Peek()) != 0 ? 1 : 0));

    private static void Exec_TST_LT(VM vm) => vm.stack.ReplaceTop2((object) (Value.Compare(vm.stack.Peek(-1), vm.stack.Peek()) < 0 ? 1 : 0));

    private static void Exec_TST_LE(VM vm) => vm.stack.ReplaceTop2((object) (Value.Compare(vm.stack.Peek(-1), vm.stack.Peek()) <= 0 ? 1 : 0));

    private static void Exec_TST_GT(VM vm) => vm.stack.ReplaceTop2((object) (Value.Compare(vm.stack.Peek(-1), vm.stack.Peek()) > 0 ? 1 : 0));

    private static void Exec_TST_GE(VM vm) => vm.stack.ReplaceTop2((object) (Value.Compare(vm.stack.Peek(-1), vm.stack.Peek()) >= 0 ? 1 : 0));

    private static void Exec_JP(VM vm) => vm.ip = BitConverter.ToInt32(vm.code, vm.ip);

    private static void Exec_JP_Z(VM vm)
    {
      int int32 = BitConverter.ToInt32(vm.code, vm.ip);
      vm.ip += 4;
      if (vm.stack.PopInt() != 0)
        return;
      vm.ip = int32;
    }

    private static void Exec_JP_NZ(VM vm)
    {
      int int32 = BitConverter.ToInt32(vm.code, vm.ip);
      vm.ip += 4;
      if (vm.stack.PopInt() == 0)
        return;
      vm.ip = int32;
    }

    private static void Exec_CALL(VM vm)
    {
      VM.VmStack stack = vm.stack;
      byte num1 = vm.code[vm.ip++];
      int num2 = stack.PeekInt((int) -num1);
      stack.Push((object) num1);
      stack.Push((object) vm.ip);
      stack.Push((object) stack.bp);
      stack.bp = stack.sp;
      vm.ip = num2;
    }

    private static void Exec_NCALL(VM vm)
    {
      VM.VmStack stack = vm.stack;
      byte num1 = vm.code[vm.ip++];
      byte num2 = vm.code[vm.ip++];
      int ip = vm.ip;
      NativeMethod nativeMethod = vm.library[(int) num1] as NativeMethod;
      object[] parameters = VM.argBuffer[(int) num2];
      Type[] paramTypes = nativeMethod.paramTypes;
      for (int index1 = 1; index1 <= (int) num2; ++index1)
      {
        Type targetType = paramTypes[(int) num2 - index1];
        object obj = stack.Pop();
        int index2 = (int) num2 - index1;
        if ((object) targetType != (object) VM.objectType && (object) obj.GetType() != (object) targetType)
        {
          obj = VM.CoerceType(obj, targetType);
          if (obj == null)
            Errors.Raise("Argument type mismatch for argument '{0}' of '{1}'", vm.lastLine, vm.lastCol, (object) index2, (object) nativeMethod.method.Name);
        }
        parameters[index2] = obj;
      }
      object obj1 = nativeMethod.method.Invoke((object) null, parameters);
      stack.Push(obj1);
      vm.ip = ip;
      Array.Clear((Array) parameters, 0, (int) num2);
    }

    private static void Exec_ALLOC_LOCALS(VM vm) => vm.stack.sp = vm.stack.bp + (int) vm.code[vm.ip++];

    private static void Exec_RET0(VM vm)
    {
      vm.stack.Push((object) 0);
      VM.Exec_RET(vm);
    }

    private static void Exec_PRINT(VM vm)
    {
      object obj = vm.stack.Pop();
      vm.console?.Print((object) obj.ToString());
    }

    private static void Exec_CLS(VM vm) => vm.console?.Cls();

    private static void Exec_RET(VM vm)
    {
      VM.VmStack stack = vm.stack;
      object obj = stack.Pop();
      stack.Unroll((int) vm.code[vm.ip++]);
      int num1 = stack.PopInt();
      int num2 = stack.PopInt();
      stack.Unroll(stack.PopInt());
      stack.Pop();
      stack.bp = num1;
      stack.Push(obj);
      vm.ip = num2;
    }

    private static object CoerceType(object value, Type targetType)
    {
      switch (value)
      {
        case ArrayValue arrayValue:
          return (object) targetType == (object) VM.arrayListType ? (object) arrayValue : (object) null;
        case double num1:
          if ((object) targetType == (object) VM.doubleType)
            return (object) num1;
          if ((object) targetType == (object) VM.floatType)
            return (object) (float) num1;
          if ((object) targetType == (object) VM.intType)
            return (object) (int) num1;
          if ((object) targetType == (object) VM.byteType)
            return (object) (byte) num1;
          return (object) targetType == (object) VM.stringType ? (object) num1.ToString() : (object) null;
        case float num2:
          if ((object) targetType == (object) VM.doubleType)
            return (object) (double) num2;
          if ((object) targetType == (object) VM.floatType)
            return (object) num2;
          if ((object) targetType == (object) VM.intType)
            return (object) (int) num2;
          if ((object) targetType == (object) VM.byteType)
            return (object) (byte) num2;
          return (object) targetType == (object) VM.stringType ? (object) num2.ToString() : (object) null;
        case int num3:
          if ((object) targetType == (object) VM.doubleType)
            return (object) (double) num3;
          if ((object) targetType == (object) VM.floatType)
            return (object) (float) num3;
          if ((object) targetType == (object) VM.intType)
            return (object) num3;
          if ((object) targetType == (object) VM.byteType)
            return (object) (byte) num3;
          return (object) targetType == (object) VM.stringType ? (object) num3.ToString() : (object) null;
        case byte num4:
          if ((object) targetType == (object) VM.doubleType)
            return (object) (double) num4;
          if ((object) targetType == (object) VM.floatType)
            return (object) (float) num4;
          if ((object) targetType == (object) VM.intType)
            return (object) (int) num4;
          if ((object) targetType == (object) VM.byteType)
            return (object) num4;
          return (object) targetType == (object) VM.stringType ? (object) num4.ToString() : (object) null;
        default:
          return (object) null;
      }
    }

    private class VmStack
    {
      public const int Max = 64;
      private readonly object[] stack = new object[64];
      public int sp;
      public int bp;

      public void Clear()
      {
        for (int index = 0; index < this.stack.Length; ++index)
        {
          if (this.stack[index] is IDisposable disposable2)
            disposable2.Dispose();
          this.stack[index] = (object) null;
        }
        this.sp = 0;
        this.bp = 0;
      }

      public void Unroll(int n)
      {
        for (int index = 0; index < n; ++index)
          this.stack[--this.sp] = (object) null;
      }

      public void Push(object value) => this.stack[this.sp++] = value;

      public object Pop()
      {
        object obj = this.stack[--this.sp];
        this.stack[this.sp] = (object) null;
        return obj;
      }

      public int PopInt()
      {
        switch (this.stack[--this.sp])
        {
          case int num1:
            return num1;
          case byte num2:
            return (int) num2;
          case float num3:
            return (int) num3;
          case double num4:
            return (int) num4;
          default:
            throw new Exception("Invalid number");
        }
      }

      public object Peek() => this.stack[this.sp - 1];

      public object Peek(int offset) => this.stack[this.sp - 1 + offset];

      public object Peek(int bp, int offset) => this.stack[bp - 1 + offset];

      public int PeekInt()
      {
        switch (this.stack[this.sp - 1])
        {
          case int num1:
            return num1;
          case byte num2:
            return (int) num2;
          case float num3:
            return (int) num3;
          case double num4:
            return (int) num4;
          default:
            throw new Exception("Invalid number");
        }
      }

      public int PeekInt(int offset)
      {
        switch (this.stack[this.sp - 1 + offset])
        {
          case int num1:
            return num1;
          case byte num2:
            return (int) num2;
          case float num3:
            return (int) num3;
          case double num4:
            return (int) num4;
          default:
            throw new Exception("Invalid number");
        }
      }

      public int PeekInt(int bp, int offset)
      {
        switch (this.stack[bp - 1 + offset])
        {
          case int num1:
            return num1;
          case byte num2:
            return (int) num2;
          case float num3:
            return (int) num3;
          case double num4:
            return (int) num4;
          default:
            throw new Exception("Invalid number");
        }
      }

      public void Dup() => this.stack[this.sp++] = this.stack[this.sp - 2];

      public void Drop() => this.stack[--this.sp] = (object) null;

      public void Set(int bp, int offset, object value) => this.stack[bp - 1 + offset] = value;

      public void ReplaceTop(object value) => this.stack[this.sp - 1] = value;

      public void ReplaceTop2(object value)
      {
        this.stack[--this.sp] = (object) null;
        this.stack[this.sp - 1] = value;
      }

      public int[] GetCallStack(int ip)
      {
        ArrayList arrayList = new ArrayList()
        {
          (object) ip
        };
        for (int bp = this.bp; bp > 0; bp = this.PeekInt(bp, 0))
        {
          int num = this.PeekInt(bp, -1);
          arrayList.Add((object) num);
        }
        return (int[]) arrayList.ToArray(typeof (int));
      }
    }

    private delegate void ExecOp(VM vm);
  }
}
