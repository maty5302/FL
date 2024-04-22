using System.Text;

namespace Project;

public class VirtualMachine
{
    private Stack<object> stack;
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
            else if(instruction[0].StartsWith("add"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if(a is int && b is int)
                    stack.Push((int)a + (int)b);
                else if(a is float && b is float)
                    stack.Push((float)a + (float)b);
            }
            else if(instruction[0].StartsWith("sub"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if(a is int && b is int)
                    stack.Push((int)a - (int)b);
                else if(a is float && b is float)
                    stack.Push((float)a - (float)b);
            }
            else if(instruction[0].StartsWith("mul"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if(a is int && b is int)
                    stack.Push((int)a * (int)b);
                else if(a is float && b is float)
                    stack.Push((float)a * (float)b);
            }
            else if(instruction[0].StartsWith("div"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if(a is int && b is int)
                    stack.Push((int)a / (int)b);
                else if(a is float && b is float)
                    stack.Push((float)a / (float)b);
            }
            else if(instruction[0].StartsWith("and"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if(a is bool && b is bool)
                    stack.Push((bool)a && (bool)b);
            }
            else if(instruction[0].StartsWith("or"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if(a is bool && b is bool)
                    stack.Push((bool)a || (bool)b);
            }
            else if(instruction[0].StartsWith("itof"))
            {
                // int to float
                var a = stack.Pop();
                if(a is int)
                    stack.Push((float)(int)a);
            }
            else if (instruction[0].StartsWith("concat"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if (a is string && b is string)
                    stack.Push((string)a + (string)b);
            }
            else if (instruction[0].StartsWith("mod"))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                if (a is int && b is int)
                    stack.Push((int)a % (int)b);
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
                var input = Console.ReadLine();
                if (instruction[1] == "I")
                    stack.Push(int.Parse(input));
                else if (instruction[1] == "F")
                    stack.Push(float.Parse(input));
                else if (instruction[1] == "S")
                    stack.Push(input);
                else
                    stack.Push(bool.Parse(input));
            }
            else if (instruction[0].StartsWith("pop"))
            {
                stack.Pop();
            }
            else if (instruction[0].StartsWith("eq"))
            {
                var right = stack.Pop();
                var left = stack.Pop();
                if (left is int && right is int)
                    stack.Push((int)left == (int)right);
                else if (left is float && right is float)
                    stack.Push((float)left == (float)right);
                else if (left is bool && right is bool)
                    stack.Push((bool)left == (bool)right);
                else if (left is string && right is string)
                    stack.Push((string)left == (string)right);
            }
            else if (instruction[0].StartsWith("gt"))
            {
                var right = stack.Pop();
                var left = stack.Pop();
                if (left is int && right is int)
                    stack.Push((int)left > (int)right);
                else if (left is float && right is float)
                    stack.Push((float)left > (float)right);
            }
            else if(instruction[0].StartsWith("lt"))
            {
                var right = stack.Pop();
                var left = stack.Pop();
                if (left is int && right is int)
                    stack.Push((int)left < (int)right);
                else if (left is float && right is float)
                    stack.Push((float)left < (float)right);
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
                throw new Exception("Invalid instruction");
            }
        }
    }
    
    
}