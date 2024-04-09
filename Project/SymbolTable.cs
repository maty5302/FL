using Antlr4.Runtime;

namespace Project;

public class SymbolTable
{

    Dictionary<string, (MyType type,object Value)> memory = new Dictionary<string, (MyType type, object Value)>();

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
                memory.Add(name, (MyType.INT, 0));
            }
            else if(type==MyType.FLOAT)
            {
                memory.Add(name, (MyType.FLOAT, 0.0));
            }
            else if(type==MyType.BOOL)
            {
                memory.Add(name, (MyType.BOOL, false));
            }
            else if(type==MyType.STRING)
            {
                memory.Add(name, (MyType.STRING, ""));
            }
        }
    }

    public (MyType Type, object Value) this[IToken variable]
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
                return (MyType.ERROR, -1);
            }
        }
        set
        {
            var name = variable.Text.Trim();
            memory[name] = value;
        }
    }
}