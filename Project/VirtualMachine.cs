using System.Text;

namespace Project;

public class VirtualMachine
{
    private readonly string[] knownInstructions = new string[] { "push", "print", "save", "load", "add", "sub", "mul", "div", "and", "or", "itof", "concat", "mod", "uminus", "not", "read", "pop", "eq", "gt", "lt", "jmp", "fjmp", "label" };
    
    private readonly Stack<object> stack;
    private List<string[]> codeInstructions;
    private Dictionary<string, object> memory;
    
    public VirtualMachine(string codeInstructions)
    {
        this.stack = new Stack<object>();
        this.codeInstructions = new List<string[]>();
        this.memory = new Dictionary<string, object>();
        this.codeInstructions = codeInstructions.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Split(" ")).ToList();
    }
    
    public void Run()
    {
        for (int i = 0; i < this.codeInstructions.Count; i++)
        {
            var instruction = this.codeInstructions[i];
            if (instruction[0].StartsWith("push"))
            {
                if (instruction[1] == "I")
                    stack.Push(int.Parse(instruction[2]));
                else if (instruction[1] == "F")
                    stack.Push(float.Parse(instruction[2]));
                else if (instruction[1] == "S")
                {
                    string final = "";
                    for (int j = 2; j < instruction.Length; j++){
                        final += instruction[j].Replace("\"","") + " ";
                    }
                    stack.Push(final.Trim());
                }
                else
                    stack.Push(bool.Parse(instruction[2]));
            }
            else if (instruction[0].StartsWith("print"))
            {
                string[] words = new string[int.Parse(instruction[1])];
                for(int j=0; j<int.Parse(instruction[1]); j++)
                {
                    var value = stack.Pop();
                    if(value is int)
                        words[j] = (int)value + " ";
                    else if(value is float)
                        words[j]= ((float)value).ToString() + " ";
                    else if(value is bool)
                        words[j]= (bool)value + " ";
                    else 
                        words[j]= (string)value + " ";
                }
                Array.Reverse(words);
                string final = string.Join(" ", words);
                Console.WriteLine(final.Trim());
                    
            }
            else if(instruction[0].StartsWith("save"))
            {
                var value = stack.Pop();
                memory[instruction[1]] = value;
            }
            else if(instruction[0].StartsWith("load"))
            {
                stack.Push(memory[instruction[1]]);
            }
            else if(instruction[0].StartsWith("itof"))
            {
                // int to float
                var a = stack.Pop();
                if(a is int)
                    stack.Push((float)(int)a);
            }
            else if (instruction[0].StartsWith("uminus"))
            {
                var a = stack.Pop();
                if (a is int)
                    stack.Push(-(int)a);
                else if (a is float)
                    stack.Push(-(float)a);
            }
            else if (instruction[0].StartsWith("not"))
            {
                var a = stack.Pop();
                if (a is bool)
                    stack.Push(!(bool)a);
            }
            else if (instruction[0].StartsWith("read"))
            {
                try
                {
                    var value = Console.ReadLine();
                    if (instruction[1] == "I")
                        stack.Push(int.Parse(value));
                    else if (instruction[1] == "F")
                        stack.Push(float.Parse(value));
                    else if (instruction[1] == "S")
                        stack.Push(value);
                    else
                        stack.Push(bool.Parse(value));  
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input");
                    i--;
                }
            }
            else if (instruction[0].StartsWith("pop"))
            {
                stack.Pop();
            }
            else if (instruction[0].StartsWith("jmp"))
            {
                var label = instruction[1];
                for (int j = 0; j < this.codeInstructions.Count; j++)
                {
                    if (this.codeInstructions[j][0] == "label" && this.codeInstructions[j][1] == label)
                    {
                        j--;
                        i = j;
                        break;
                    }
                }
            }
            else if (instruction[0].StartsWith("fjmp"))
            {
                var label = instruction[1];
                var condition = stack.Pop();
                if (!(bool)condition)
                {
                    for (int j = 0; j < this.codeInstructions.Count; j++)
                    {
                        if (this.codeInstructions[j][0] == "label" && this.codeInstructions[j][1] == label)
                        {
                            j--;
                            i = j;
                            break;
                        }
                    }
                }
            }
            else if (instruction[0].StartsWith("label"))
            {
                // do nothing
            }
            else
            {
                if(!knownInstructions.Contains(instruction[0]))
                    throw new Exception("Invalid instruction");
                else
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    switch (instruction[0])
                    {
                        case "add" when left is int left1 && right is int right1: stack.Push(left1+right1);
                            break; 
                        case "add" when left is float left1 && right is float right1: stack.Push(left1+right1);
                            break;
                        case "sub" when left is int left1 && right is int right1: stack.Push(left1-right1);
                            break;
                        case "sub" when left is float left1 && right is float right1: stack.Push(left1-right1);
                            break;
                        case "mul" when left is int left1 && right is int right1: stack.Push(left1*right1);
                            break;
                        case "mul" when left is float left1 && right is float right1: stack.Push(left1*right1);
                            break;
                        case "div" when left is int left1 && right is int right1: stack.Push(left1/right1);
                            break;
                        case "div" when left is float left1 && right is float right1: stack.Push(left1/right1);
                            break;
                        case "and" when left is bool left1 && right is bool right1: stack.Push(left1&&right1);
                            break;
                        case "or" when left is bool left1 && right is bool right1: stack.Push(left1||right1);
                            break;
                        case "eq" when left is int left1 && right is int right1: stack.Push(left1==right1);
                            break;
                        case "eq" when left is float left1 && right is float right1: stack.Push(left1==right1);
                            break;
                        case "eq" when left is bool left1 && right is bool right1: stack.Push(left1==right1);
                            break;
                        case "eq" when left is string left1 && right is string right1: stack.Push(left1==right1);
                            break;
                        case "gt" when left is int left1 && right is int right1: stack.Push(left1>right1);
                            break;
                        case "gt" when left is float left1 && right is float right1: stack.Push(left1>right1);
                            break;
                        case "lt" when left is int left1 && right is int right1: stack.Push(left1<right1);
                            break;
                        case "lt" when left is float left1 && right is float right1: stack.Push(left1<right1);
                            break;
                        case "mod" when left is int left1 && right is int right1: stack.Push(left1%right1);
                            break;
                        case "concat" when left is string left1 && right is string right1: stack.Push(left1+right1);
                            break;
                        default:
                            break;
                    }
                }
                
            }
        }
    }
    
    
}