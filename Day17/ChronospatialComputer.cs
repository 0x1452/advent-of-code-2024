namespace Day17;
public struct Instruction
{
    public byte Opcode { get; set; }
    public byte Operand { get; set; }
}

public class ChronospatialComputer
{
    public long A { get; set; }
    public long B { get; set; }
    public long C { get; set; }
    public int IP { get; set; }

    public List<byte> Instructions { get; set; } = [];
    public List<byte> Output { get; set; } = [];

    private readonly bool _debug = false;

    public ChronospatialComputer(long a, long b, long c, List<byte> instructions, bool debug = false)
    {
        A = a;
        B = b;
        C = c;
        Instructions = instructions;
        _debug = debug;
    }

    public void Run()
    {
        while (TryGetNextInstruction(out var instruction))
        {
            if (_debug)
            {
                PrintStatus();
                PrintInstruction(instruction);
            }
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
        => A = (long)(A / Math.Pow(2, GetOperandValue(operand)));
        //=> A = A >> GetOperandValue(operand);
    private void ExecuteBDV(byte operand)
        => B = (long)(A / Math.Pow(2, GetOperandValue(operand)));
        //=> B = A >> GetOperandValue(operand);
    private void ExecuteCDV(byte operand)
        => C = (long)(A / Math.Pow(2, GetOperandValue(operand)));
        //=> C = A >> GetOperandValue(operand);
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

    private long GetOperandValue(byte operand)
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

    private void PrintStatus()
    {
        Console.WriteLine($"A: {A,-10} ({Convert.ToString(A, 2)})");
        Console.WriteLine($"B: {B,-10} ({Convert.ToString(B, 2)})");
        Console.WriteLine($"C: {C,-10} ({Convert.ToString(C, 2)})");
        Console.WriteLine($"IP: {IP}");
    }

    private void PrintInstruction(Instruction instruction)
    {
        var comboOperandName = GetOperandName(instruction.Operand);
        var (operationName, description) = instruction.Opcode switch
        {
            0 => ("adv", $"A = A >> {comboOperandName}"),
            1 => ("bxl", $"B = B ^ {instruction.Operand}"),
            2 => ("bst", $"B = {comboOperandName} % 8"),
            3 => ("jnz", A == 0 ? "NOP" : $"IP = {instruction.Operand}"),
            4 => ("bxc", $"B = B ^ C"),
            5 => ("out", $"Output += {comboOperandName} % 8"),
            6 => ("bdv", $"B = A >> {comboOperandName}"),
            7 => ("cdv", $"C = A >> {comboOperandName}"),
            _ => throw new ArgumentException($"Invalid opcode: '{instruction.Opcode}'", nameof(instruction))
        };

        Console.WriteLine($"{operationName}   {description}");
    }

    private string GetOperandName(byte operand)
        => operand switch
        {
            0 or 1 or 2 or 3 => operand.ToString(),
            4 => "A",
            5 => "B",
            6 => "C",
            _ => throw new ArgumentException($"Invalid operand: '{operand}'", nameof(operand))
        };
}
