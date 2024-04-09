using Antlr4.Runtime;

namespace Project;

public class SymbolTable
{

    Dictionary<string, MyType> memory = new Dictionary<string, MyType>();

    public void Add(IToken var, MyType type)
    {
        var name = var.Text;
        if (memory.ContainsKey(name))
        {
            Errors.ReportError(var, $"Variable {name} was already declared.");
        }
        else
        {
            if(type==MyType.INT)
            {
                memory.Add(name, (MyType.INT));
            }
            else if(type==MyType.FLOAT)
            {
                memory.Add(name, (MyType.FLOAT));
            }
            else if(type==MyType.BOOL)
            {
                memory.Add(name, (MyType.BOOL));
            }
            else if(type==MyType.STRING)
            {
                memory.Add(name, (MyType.STRING));
            }
        }
    }

    public MyType this[IToken variable]
    {
        get
        {
            var name = variable.Text;
            if (memory.ContainsKey(name))
            {
                return memory[name];
            }
            else
            {
                Errors.ReportError(variable, $"Variable {name} was NOT declared.");
                return (MyType.ERROR);
            }
        }
        set
        {
            var name = variable.Text.Trim();
            // memory[name] = value;
        }
    }
}