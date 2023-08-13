// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.DUE.Compiler.CodeBuffer
// Assembly: GHIElectronics.TinyCLR.DUE.Compiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4604F13D-6F7E-4BF6-9153-3AEAE79C81C3
// Assembly location: D:\experiment\BMC.AirQuality\src\NF.AirQuality\bin\Debug\GHIElectronics.TinyCLR.DUE.Compiler.dll

using GHIElectronics.TinyCLR.DUE.Collections;
using System;

namespace GHIElectronics.TinyCLR.DUE.Compiler
{
  internal class CodeBuffer : ByteArrayList
  {
    private int globalCount;
    private OpCode lastOpCode;

    public CodeBuffer()
      : base(1024)
    {
    }

    public int GlobalCount => this.globalCount;

    public int Emit_NOP() => this.Emit(OpCode.NOP);

    public int Emit_HALT() => this.Emit(OpCode.HALT);

    public int Emit_DBG(ushort frame, ushort line, ushort col, ushort len)
    {
      int num = this.Emit(OpCode.DBG);
      this.Emit(frame);
      this.Emit(line);
      this.Emit(col);
      this.Emit(len);
      return num;
    }

    public int Emit_DUP() => this.Emit(OpCode.DUP);

    public int Emit_DROP() => this.Emit(OpCode.DROP);

    public int Emit_NEG() => this.Emit(OpCode.NEG);

    public int Emit_LD_IM_I4(int value)
    {
      if (value == 0)
        return this.Emit(OpCode.LD_IM_0);
      if (value == 1)
        return this.Emit(OpCode.LD_IM_1);
      int num = this.Emit(OpCode.LD_IM_I4);
      this.Emit(value);
      return num;
    }

    public int Emit_LD_IM_F4(float value)
    {
      if ((double) value == 0.0)
        return this.Emit(OpCode.LD_IM_F4_0);
      if ((double) value == 1.0)
        return this.Emit(OpCode.LD_IM_F4_1);
      int num = this.Emit(OpCode.LD_IM_F4);
      this.Emit(value);
      return num;
    }

    public int Emit_LD_IM_PTR(int address, int len)
    {
      int num = this.Emit(OpCode.LD_IM_PTR);
      this.Emit(address);
      this.Emit(len);
      return num;
    }

    public int Emit_LD_IM_STR(int address, int len)
    {
      int num = this.Emit(OpCode.LD_IM_STR);
      this.Emit(address);
      this.Emit(len);
      return num;
    }

    public int Emit_ALLOC_BYTES() => this.Emit(OpCode.ALLOC_BYTES);

    public int Emit_ALLOC_ARRAY() => this.Emit(OpCode.ALLOC_ARRAY);

    public int Emit_LD_ARR_ELEM() => this.Emit(OpCode.LD_ARR_ELEM);

    public int Emit_ST_ARR_ELEM() => this.Emit(OpCode.ST_ARR_ELEM);

    public int Emit_LD_ARR_LEN() => this.Emit(OpCode.LD_ARR_LEN);

    public int Emit_LD_ARG(byte index)
    {
      if (index < (byte) 10)
        return this.Emit((OpCode) (19U + (uint) index));
      int num = this.Emit(OpCode.LD_ARG);
      this.Emit(index);
      return num;
    }

    public int Emit_ST_ARG(byte index)
    {
      if (index < (byte) 10)
        return this.Emit((OpCode) (30U + (uint) index));
      int num = this.Emit(OpCode.ST_ARG);
      this.Emit(index);
      return num;
    }

    public int Emit_LD_LOC(byte index)
    {
      if (index < (byte) 10)
        return this.Emit((OpCode) (41U + (uint) index));
      int num = this.Emit(OpCode.LD_LOC);
      this.Emit(index);
      return num;
    }

    public int Emit_LD_GBL(ushort index)
    {
      int num = this.Emit(OpCode.LD_GBL);
      this.Emit(index);
      return num;
    }

    public int Emit_LD_BLT_CONST(byte index)
    {
      int num = this.Emit(OpCode.LD_BLT_CONST);
      this.Emit(index);
      return num;
    }

    public int Emit_ST_LOC(byte index)
    {
      if (index < (byte) 10)
        return this.Emit((OpCode) (54U + (uint) index));
      int num = this.Emit(OpCode.ST_LOC);
      this.Emit(index);
      return num;
    }

    public int Emit_ST_GBL(ushort index)
    {
      int num = this.Emit(OpCode.ST_GBL);
      this.Emit(index);
      ++this.globalCount;
      return num;
    }

    public int Emit_ADD() => this.Emit(OpCode.ADD);

    public int Emit_SUB() => this.Emit(OpCode.SUB);

    public int Emit_MUL() => this.Emit(OpCode.MUL);

    public int Emit_DIV() => this.Emit(OpCode.DIV);

    public int Emit_MOD() => this.Emit(OpCode.MOD);

    public int Emit_AND() => this.Emit(OpCode.AND);

    public int Emit_OR() => this.Emit(OpCode.OR);

    public int Emit_NOT() => this.Emit(OpCode.NOT);

    public int Emit_BIT_NOT() => this.Emit(OpCode.BIT_NOT);

    public int Emit_BIT_AND() => this.Emit(OpCode.BIT_AND);

    public int Emit_BIT_OR() => this.Emit(OpCode.BIT_OR);

    public int Emit_BIT_XOR() => this.Emit(OpCode.BIT_XOR);

    public int Emit_SHL() => this.Emit(OpCode.SHL);

    public int Emit_SHR() => this.Emit(OpCode.SHR);

    public int Emit_TST_EQ() => this.Emit(OpCode.TST_EQ);

    public int Emit_TST_NE() => this.Emit(OpCode.TST_NE);

    public int Emit_TST_LT() => this.Emit(OpCode.TST_LT);

    public int Emit_TST_LE() => this.Emit(OpCode.TST_LE);

    public int Emit_TST_GT() => this.Emit(OpCode.TST_GT);

    public int Emit_TST_GE() => this.Emit(OpCode.TST_GE);

    public int Emit_JP(int address)
    {
      int num = this.Emit(OpCode.JP);
      this.Emit(address);
      return num;
    }

    public int Emit_JP_Z(int address)
    {
      int num = this.Emit(OpCode.JP_Z);
      this.Emit(address);
      return num;
    }

    public int Emit_JP_NZ(int address)
    {
      int num = this.Emit(OpCode.JP_NZ);
      this.Emit(address);
      return num;
    }

    public int Emit_CALL(byte argCount)
    {
      int num = this.Emit(OpCode.CALL);
      this.Emit(argCount);
      return num;
    }

    public int Emit_NCALL(byte index, byte argCount)
    {
      int num = this.Emit(OpCode.NCALL);
      this.Emit(index);
      this.Emit(argCount);
      return num;
    }

    public int Emit_ALLOC_LOCALS(byte localCount)
    {
      int num = this.Emit(OpCode.ALLOC_LOCALS);
      this.Emit(localCount);
      return num;
    }

    public int Emit_RET0(byte localCount)
    {
      int num = this.Emit(OpCode.RET0);
      this.Emit(localCount);
      return num;
    }

    public int Emit_RET(byte localCount)
    {
      int num = this.Emit(OpCode.RET);
      this.Emit(localCount);
      return num;
    }

    public int Emit_PRINT() => this.Emit(OpCode.PRINT);

    public int Emit_CLS() => this.Emit(OpCode.CLS);

    public int PatchOpArgHere(int at) => this.PatchOpArg(at, this.Count);

    public int PatchOpArg(int at, byte value)
    {
      this[at + 1] = value;
      return at + 1;
    }

    public int PatchOpArg(int at, int value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      this[at + 1] = bytes[0];
      this[at + 2] = bytes[1];
      this[at + 3] = bytes[2];
      this[at + 4] = bytes[3];
      return at + 4;
    }

    public OpCode LastOpCode => this.lastOpCode;

    private int Emit(OpCode instruction)
    {
      int num = this.Emit((byte) instruction);
      this.lastOpCode = instruction;
      return num;
    }

    public int Emit(byte value) => this.Add(value);

    private int Emit(ushort value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      int num = this.Add(bytes[0]);
      this.Add(bytes[1]);
      return num;
    }

    private int Emit(int value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      int num = this.Add(bytes[0]);
      this.Add(bytes[1]);
      this.Add(bytes[2]);
      this.Add(bytes[3]);
      return num;
    }

    private int Emit(float value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      int num = this.Add(bytes[0]);
      this.Add(bytes[1]);
      this.Add(bytes[2]);
      this.Add(bytes[3]);
      return num;
    }
  }
}
