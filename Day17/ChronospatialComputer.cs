using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day17;
public struct Instruction
{
    public byte Opcode { get; set; }
    public byte Operand { get; set; }
}

public class ChronospatialComputer
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int IP { get; set; }

    public List<byte> Instructions { get; set; } = [];
    public List<byte> Output { get; set; } = [];

    public ChronospatialComputer(int a, int b, int c, List<byte> instructions)
    {
        A = a;
        B = b;
        C = c;
        Instructions = instructions;
    }

    public void Run()
    {
        while (TryGetNextInstruction(out var instruction))
        {
            ExecuteInstruction(instruction);
        }
    }

    private void ExecuteInstruction(Instruction instruction)
    {
        var incrementBy = 2;

        switch (instruction.Opcode)
        {
            case 0:
                ExecuteADV(instruction.Operand);
                break;

            case 1:
                ExecuteBXL(instruction.Operand);
                break;

            case 2:
                ExecuteBST(instruction.Operand);
                break;

            case 3:
                if (ExecuteJNZ(instruction.Operand))
                    incrementBy = 0;
                break;

            case 4:
                ExecuteBXC();
                break;

            case 5:
                ExecuteOUT(instruction.Operand);
                break;

            case 6:
                ExecuteBDV(instruction.Operand);
                break;

            case 7:
                ExecuteCDV(instruction.Operand);
                break;
        }

        IP += incrementBy;
    }

    private void ExecuteADV(byte operand)
        => A = (int)(A / Math.Pow(2, GetOperandValue(operand)));
    private void ExecuteBDV(byte operand)
        => B = (int)(A / Math.Pow(2, GetOperandValue(operand)));
    private void ExecuteCDV(byte operand)
        => C = (int)(A / Math.Pow(2, GetOperandValue(operand)));
    private void ExecuteBXC()
        => B = B ^ C;
    private void ExecuteBXL(byte operand)
        => B = B ^ operand;
    private void ExecuteBST(byte operand)
        => B = GetOperandValue(operand) % 8;
    private bool ExecuteJNZ(byte operand)
    {
        if (A == 0)
            return false;

        IP = operand;
        return true;
    }

    private void ExecuteOUT(byte operand)
    {
        Output.Add((byte)(GetOperandValue(operand) % 8));
    }

    private int GetOperandValue(byte operand)
    {
        return operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => A,
            5 => B,
            6 => C,
            _ => throw new ArgumentException($"Invalid operand: '{operand}'", nameof(operand))
        };
    }

    private bool TryGetNextInstruction(out Instruction instruction)
    {
        if (IP > Instructions.Count - 2)
        {
            instruction = default;
            return false;
        }

        var opcode = Instructions[IP];
        var operand = Instructions[IP + 1];

        instruction = new Instruction
        {
            Opcode = opcode,
            Operand = operand
        };

        return true;
    }
}
