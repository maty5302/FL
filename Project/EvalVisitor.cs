namespace Project;

public class EvalVisitor : FLBaseVisitor<(MyType type, object value)>
{
    SymbolTable symbolTable = new SymbolTable();
    
    private float ToFloat(object value)
    {
        if (value is int x) return (float)x;
        return (float)value;
    }
    
    public override (MyType type, object value) VisitProg(FLParser.ProgContext context)
    {
        foreach (var statement in context.statement())
        {
            Visit(statement);
        }
        return (MyType.ERROR, -1);
    }
    
    public override (MyType type, object value) VisitDeclaration(FLParser.DeclarationContext context)
    {
        var type = Visit(context.type());
        foreach (var id in context.ID())
        {
            symbolTable.Add(id.Symbol, type.type);
        }
        return (MyType.ERROR, -1);
    }
    
    public override (MyType type, object value) VisitType(FLParser.TypeContext context)
    {
        if (context.GetText() == "int")
        {
            return (MyType.INT, 0);
        }
        else if (context.GetText() == "float")
        {
            return (MyType.FLOAT, 0.0);
        }
        else if (context.GetText() == "bool")
        {
            return (MyType.BOOL, false);
        }
        else if (context.GetText() == "string")
        {
            return (MyType.STRING, "");
        }
        return (MyType.ERROR, -1);
    }
    
    public override (MyType type, object value) VisitFloat(FLParser.FloatContext context)
    {
        return (MyType.FLOAT, float.Parse(context.GetText()));
    }
    
    public override (MyType type, object value) VisitInt(FLParser.IntContext context)
    {
        return (MyType.INT, int.Parse(context.GetText()));
    }
    
    public override (MyType type, object value) VisitString(FLParser.StringContext context)
    {
        return (MyType.STRING, context.GetText());
    }
    
    public override (MyType type, object value) VisitBool(FLParser.BoolContext context)
    {
        return (MyType.BOOL, bool.Parse(context.GetText()));
    }
    
    public override (MyType type, object value) VisitId(FLParser.IdContext context)
    {
        return symbolTable[context.ID().Symbol];
    }
    
    public override (MyType type, object value) VisitParens(FLParser.ParensContext context)
    {
        return Visit(context.expr());
    }
    
    public override (MyType type, object value) VisitMulDivMod(FLParser.MulDivModContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left.type == MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        
        if (left.type == MyType.INT && right.type == MyType.INT)
        {
            if (context.op.Type == FLParser.MUL)
            {
                return (MyType.INT, (int)left.value * (int)right.value);
            }
            else if (context.op.Type == FLParser.DIV)
            {
                return (MyType.INT, (int)left.value / (int)right.value);
            }
            else if (context.op.Type == FLParser.MOD)
            {
                return (MyType.INT, (int)left.value % (int)right.value);
            }
        }
        else
        {
            if (context.op.Type == FLParser.MUL)
            {
                return (MyType.FLOAT, ToFloat(left.value) * ToFloat(right.value));
            }
            else if (context.op.Type == FLParser.DIV)
            {
                return (MyType.FLOAT, ToFloat(left.value) / ToFloat(right.value));
            }
            else if (context.op.Type == FLParser.MOD)
            {
                return (MyType.FLOAT, ToFloat(left.value) % ToFloat(right.value));
            }
        }
        return (MyType.ERROR, -2);
    }
    
    public override (MyType type, object value) VisitAddSub(FLParser.AddSubContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left.type == MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        
        if (left.type == MyType.INT && right.type == MyType.INT)
        {
            if (context.op.Type == FLParser.ADD)
            {
                return (MyType.INT, (int)left.value + (int)right.value);
            }
            else if (context.op.Type == FLParser.SUB)
            {
                return (MyType.INT, (int)left.value - (int)right.value);
            }
        }
        else
        {
            if (context.op.Type == FLParser.ADD)
            {
                return (MyType.FLOAT, ToFloat(left.value) + ToFloat(right.value));
            }
            else if (context.op.Type == FLParser.SUB)
            {
                return (MyType.FLOAT, ToFloat(left.value) - ToFloat(right.value));
            }
        }
        return (MyType.ERROR, -2);
    }
    
    public override (MyType type, object value) VisitLtGt(FLParser.LtGtContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left.type == MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        
        if (left.type == MyType.INT && right.type == MyType.INT)
        {
            if (context.op.Type == FLParser.LT)
            {
                return (MyType.BOOL, (int)left.value < (int)right.value);
            }
            else if (context.op.Type == FLParser.GT)
            {
                return (MyType.BOOL, (int)left.value > (int)right.value);
            }
        }
        else
        {
            if (context.op.Type == FLParser.LT)
            {
                return (MyType.BOOL, ToFloat(left.value) < ToFloat(right.value));
            }
            else if (context.op.Type == FLParser.GT)
            {
                return (MyType.BOOL, ToFloat(left.value) > ToFloat(right.value));
            }
        }
        return (MyType.ERROR, -2);
    }
    
    public override (MyType type, object value) VisitEqNe(FLParser.EqNeContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left.type == MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        
        if (left.type == MyType.INT && right.type == MyType.INT)
        {
            if (context.op.Type == FLParser.EQ)
            {
                return (MyType.BOOL, (int)left.value == (int)right.value);
            }
            else if (context.op.Type == FLParser.NE)
            {
                return (MyType.BOOL, (int)left.value != (int)right.value);
            }
        }
        else //how about string compare? or bool?
        {
            if (context.op.Type == FLParser.EQ)
            {
                return (MyType.BOOL, ToFloat(left.value) == ToFloat(right.value));
            }
            else if (context.op.Type == FLParser.NE)
            {
                return (MyType.BOOL, ToFloat(left.value) != ToFloat(right.value));
            }
        }
        return (MyType.ERROR, -2);
    }
    
    public override (MyType type, object value) VisitAnd(FLParser.AndContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left.type == MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        
        if (left.type == MyType.BOOL && right.type == MyType.BOOL)
        {
            return (MyType.BOOL, (bool)left.value && (bool)right.value);
        }
        
        
        return (MyType.ERROR, -2);
    }
    
    public override (MyType type, object value) VisitOr(FLParser.OrContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left.type == MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        
        if (left.type == MyType.BOOL && right.type == MyType.BOOL)
        {
            return (MyType.BOOL, (bool)left.value || (bool)right.value);
        }
        
        
        return (MyType.ERROR, -2);
    }
    
    public override (MyType type, object value) VisitAssign(FLParser.AssignContext context)
    {
        var right = Visit(context.expr());
        var left = symbolTable[context.ID().Symbol];
        if( left.Type ==MyType.ERROR || right.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        if(left.Type == MyType.INT && right.type == MyType.FLOAT)
        {
            // Error: Cannot convert float to int
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.INT && right.type == MyType.BOOL)
        {
            // Error: Cannot convert bool to int
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.INT && right.type == MyType.STRING)
        {
            // Error: Cannot convert string to int
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.FLOAT && right.type == MyType.INT)
        {
            var val = (MyType.FLOAT, ToFloat(right.value));
            symbolTable[context.ID().Symbol] = val;
            return val; 
        }
        else if(left.Type==MyType.FLOAT && right.type == MyType.BOOL)
        {
            // Error: Cannot convert bool to float
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.FLOAT && right.type == MyType.STRING)
        {
            // Error: Cannot convert string to float
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.BOOL && right.type == MyType.INT)
        {
            // Error: Cannot convert int to bool
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.BOOL && right.type == MyType.FLOAT)
        {
            // Error: Cannot convert float to bool
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.BOOL && right.type == MyType.STRING)
        {
            // Error: Cannot convert string to bool
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.STRING && right.type == MyType.INT)
        {
            // Error: Cannot convert int to string
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.STRING && right.type == MyType.FLOAT)
        {
            // Error: Cannot convert float to string
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.STRING && right.type == MyType.BOOL)
        {
            // Error: Cannot convert bool to string
            return (MyType.ERROR, -1);
        }
        else
        {
            symbolTable[context.ID().Symbol] = right;
            return right;
        }
        
    }

    public override (MyType type, object value) VisitRead(FLParser.ReadContext context)
    {
           
        return (MyType.ERROR, -2);
    }
}