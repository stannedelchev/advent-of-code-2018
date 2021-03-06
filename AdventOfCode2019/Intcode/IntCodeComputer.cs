﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode2019.Intcode.OpCodes;

namespace AdventOfCode2019.Intcode
{
    internal class IntCodeComputer
    {
        private readonly Dictionary<long, long> memory = new Dictionary<long, long>();

        private readonly LinkedList<long> outputs;

        public IntCodeComputer()
        {
            this.InstructionPointer = 0;
            this.RelativeBase = 0;

            this.Input = new Queue<long>();

            this.outputs = new LinkedList<long>();
            this.Output += NoOp;

            this.State = IntCodeComputerState.InitialState;
        }

        public Queue<long> Input { get; }

        public event EventHandler<long> Output;

        public long InstructionPointer { get; internal set; }

        public long RelativeBase { get; internal set; }

        public IEnumerable<long> Outputs => this.outputs;

        public IntCodeComputerState State { get; private set; }

        public IEnumerable<KeyValuePair<long, long>> Memory => this.memory;

        public long this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                this.memory.TryGetValue(index, out var result);
                return result;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            set => this.memory[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Initialize(in long[] program)
        {
            this.memory.Clear();
            for (var i = 0; i < program.Length; i++)
            {
                this.memory[i] = program[i];
            }

            this.State = IntCodeComputerState.Initialized;
            this.InstructionPointer = 0;
            this.Input.Clear();
            this.outputs.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ExecuteProgram()
        {
            this.State = IntCodeComputerState.Running;

            while (true)
            {
                var opCode = this.Decode();
                var newState = opCode.Execute(this);

                if (newState != IntCodeComputerState.Outputting)
                {
                    this.State = newState;
                }

                if (this.State == IntCodeComputerState.Halted ||
                    this.State == IntCodeComputerState.WaitingForInput)
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal void AppendOutput(in long output)
        {
            this.outputs.AddLast(output);
            this.Output(this, output);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private IOpCode Decode()
        {
            var instruction = memory[InstructionPointer];
            var type = instruction % 100;
            instruction /= 100;
            var arg1Mode = (ArgumentMode)(instruction % 10);
            instruction /= 10;
            var arg2Mode = (ArgumentMode)(instruction % 10);
            instruction /= 10;
            var arg3Mode = (ArgumentMode)(instruction % 10);
            instruction /= 10;

            IOpCode result = null;
            switch (type)
            {
                case 1:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        var arg2Index = GetMemoryIndex(arg2Mode, 1);
                        var arg3Index = GetMemoryIndex(arg3Mode, 2);
                        result = OpCodeCache.OpCodeSum.From(arg1Index, arg2Index, arg3Index);
                    }
                    break;
                case 2:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        var arg2Index = GetMemoryIndex(arg2Mode, 1);
                        var arg3Index = GetMemoryIndex(arg3Mode, 2);
                        result = OpCodeCache.OpCodeMultiply.From(arg1Index, arg2Index, arg3Index);
                    }
                    break;
                case 3:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        result = OpCodeCache.OpCodeInput.From(arg1Index);
                    }
                    break;
                case 4:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        result = OpCodeCache.OpCodeOutput.From(arg1Index);
                    }
                    break;
                case 5:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        var arg2Index = GetMemoryIndex(arg2Mode, 1);
                        result = OpCodeCache.OpCodeJumpIfTrue.From(arg1Index, arg2Index);
                    }
                    break;
                case 6:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        var arg2Index = GetMemoryIndex(arg2Mode, 1);
                        result = OpCodeCache.OpCodeJumpIfFalse.From(arg1Index, arg2Index);
                    }
                    break;
                case 7:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        var arg2Index = GetMemoryIndex(arg2Mode, 1);
                        var arg3Index = GetMemoryIndex(arg3Mode, 2);
                        result = OpCodeCache.OpCodeLessThan.From(arg1Index, arg2Index, arg3Index);
                    }
                    break;
                case 8:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        var arg2Index = GetMemoryIndex(arg2Mode, 1);
                        var arg3Index = GetMemoryIndex(arg3Mode, 2);
                        result = OpCodeCache.OpCodeEquals.From(arg1Index, arg2Index, arg3Index);
                    }
                    break;
                case 9:
                    {
                        var arg1Index = GetMemoryIndex(arg1Mode, 0);
                        result = OpCodeCache.OpCodeAdjustRelativeBase.From(arg1Index);
                    }
                    break;
                case 99:
                    return OpCodeCache.OpCodeHalt;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private long GetMemoryIndex(in ArgumentMode mode, in long argumentPosition)
        {
            return mode switch
            {
                ArgumentMode.Positional => memory[InstructionPointer + argumentPosition + 1],
                ArgumentMode.Immediate => (InstructionPointer + argumentPosition + 1),
                ArgumentMode.Relative => (memory[InstructionPointer + argumentPosition + 1] + RelativeBase),
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void NoOp(object _, long __)
        {

        }
    }
}
