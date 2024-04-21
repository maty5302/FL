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
        foreach (var instruction in this.codeInstructions)
        {
            if (instruction[0].StartsWith("push"))
            {
                if (instruction[1] == "I")
                    stack.Push(int.Parse(instruction[2]));
                else if (instruction[1] == "F")
                    stack.Push(float.Parse(instruction[2]));
                else if (instruction[1] == "S")
                {
                    string final = "";
                    for (int i = 2; i < instruction.Length; i++){
                        final += instruction[i].Replace("\"","") + " ";
                    }
                    stack.Push(final.Trim());
                }
                else
                    stack.Push(bool.Parse(instruction[2]));
            }
            else if (instruction[0].StartsWith("print"))
            {
                string[] words = new string[int.Parse(instruction[1])];
                for(int i=0; i<int.Parse(instruction[1]); i++)
                {
                    var value = stack.Pop();
                    if(value is int)
                        words[i] = (int)value + " ";
                    else if(value is float)
                        words[i]= ((float)value).ToString() + " ";
                    else if(value is bool)
                        words[i]= (bool)value + " ";
                    else 
                        words[i]= (string)value + " ";
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
            else
            {
                throw new Exception("Invalid instruction");
            }
        }
    }
    
    
}