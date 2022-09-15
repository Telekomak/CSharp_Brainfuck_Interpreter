using System.Text;

class Program
{
    private static byte[] _array = new byte[UInt16.MaxValue + 1];
    private static UInt16 _arrayPointer = 0;
    private static long _instructionPointer = 0;
    private static char[] _program = new char[0];
    private static bool _numericOutput = false;
    private static bool _isLoopOpen = false;

    static void Main(string[] args)
    {
        if (!VerifyArgs(args))
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            return;
        }

        _program = ReadFile(args[0]);
        
        if (!Execute(out string error)) PrintError(error);
        else Console.WriteLine("\n\nBF: Program exited successfully");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }

    private static bool VerifyArgs(string[] args)
    {
        if (args.Length == 0)
        {
            PrintError("BF ERROR: Too few arguments!");
            return false;
        }
        else if (args.Length > 1)
        {
            PrintError("BF ERROR: Too many arguments!");
            return false;
        }
        else if (!File.Exists(args[0]))
        {
            PrintError("BF ERROR: File does not exist!");
            return false;
        }

        return true;
    }

    private static char[] ReadFile(string filename)
    {
        Console.WriteLine($"Reading file: {filename}");
        using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            return reader.ReadToEnd().ToCharArray();
        }
    }

    private static bool Execute(out string error)
    {
        error = $"BF ERROR [{_instructionPointer}]: Unknown symbol {_program[_instructionPointer]}!";
    
        while (_instructionPointer < _program.Length)
        {
            switch (_program[_instructionPointer])
            {
                case '>':
                    _arrayPointer++;
                    break;
                case '<':
                    _arrayPointer--;
                    break;
                case '+':
                    _array[_arrayPointer]++;
                    break;
                case '-':
                    _array[_arrayPointer]--;
                    break;
                case '.':
                    if (_numericOutput)
                    {
                        Console.Write(_array[_arrayPointer]);
                        break;
                    }
                    Console.Write((char)(_array[_arrayPointer]));
                    break;
                case ',':
                    _array[_arrayPointer] = (byte)Console.Read();
                    break;
                case '[':
                    if (!GoToLoopEnd() || _isLoopOpen)
                    {
                        error = $"BF ERROR [{_instructionPointer}]: Expected \"]\" after \"[\"!";
                        return false;
                    }

                    _isLoopOpen = true;
                    break;
                case ']':
                    if (!GoToLoopStart() || !_isLoopOpen)
                    {
                        error = $"BF ERROR [{_instructionPointer}]: Expected \"[\" before \"]\"!";
                        return false;
                    }

                    _isLoopOpen = false;
                    break;
                case '\n':
                    break;
                case '\r':
                    break;
                case ' ':
                    break;
                case 'n':
                    _numericOutput = true;
                    break;
                case 'c':
                    _numericOutput = false;
                    break;
                default:
                    return false;
            }

            _instructionPointer++;
        }

        error = "";
        return true;
    }

    private static bool GoToLoopStart()
    {
        if (_array[_arrayPointer] != 0)
        {
            for (long i = _instructionPointer - 1; i >= 0; i--)
            {
                if (_program[i] == '[')
                {
                    _instructionPointer += i - _instructionPointer;
                    return true;
                }
                if (_program[i] == ']') return false;
            }

            return false;
        }

        return true;
    }

    private static bool GoToLoopEnd()
    {
        if (_array[_arrayPointer] == 0)
        {
            for (long i = _instructionPointer + 1; i < _program.Length; i++)
            {
                if (_program[i] == ']')
                {
                    _instructionPointer = i + 1;
                    return true;
                }
                if (_program[i] == '[') return false;
            }

            return false;
        }

        return true;
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ResetColor();
    }
}