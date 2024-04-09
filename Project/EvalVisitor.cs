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
        return (MyType.ERROR, 0);
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
    
    public override (MyType type, object value) VisitNot(FLParser.NotContext context)
    {
        var value = Visit(context.expr());
        if (value.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        if (value.type != MyType.BOOL)
        {
            Errors.ReportError(context.Start, "Operand must be a boolean");
            return (MyType.ERROR, -1);
        }
        return (MyType.BOOL, !(bool)value.value);
    }

    public override (MyType type, object value) VisitUnaryMinus(FLParser.UnaryMinusContext context)
    {
        var value = Visit(context.expr());
        if (value.type == MyType.ERROR)
        {
            return (MyType.ERROR, -1);
        }
        if (value.type == MyType.INT)
        {
            return (MyType.INT, -(int)value.value);
        }
        else
        {
            return (MyType.FLOAT, -ToFloat(value.value));
        }
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
                Errors.ReportError(context.op, "Modulus operator cannot be used with float");
                return (MyType.ERROR, -1);
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
        else if(left.type == MyType.FLOAT && right.type==MyType.FLOAT)
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
        else if (left.type == MyType.STRING && right.type == MyType.STRING)
        {
            if (context.op.Type == FLParser.CONCAT)
            {
                return (MyType.STRING, (string)left.value + (string)right.value);
            }
            else
            {
                Errors.ReportError(context.op, "You can concatenate only strings");
                return (MyType.ERROR, -1);
            }
        }
        else
        {
            Errors.ReportError(context.op,"You can add and subtract only numbers");
            return (MyType.ERROR, -1);
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
        else if(left.type==MyType.FLOAT && right.type==MyType.FLOAT) //how about string compare? or bool?
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
        else if(left.type==MyType.STRING && right.type==MyType.STRING)
        {
            if (context.op.Type == FLParser.EQ)
            {
                return (MyType.BOOL, (string)left.value == (string)right.value);
            }
            else if (context.op.Type == FLParser.NE)
            {
                return (MyType.BOOL, (string)left.value != (string)right.value);
            }
        }
        else if(left.type==MyType.BOOL && right.type==MyType.BOOL)
        {
            if (context.op.Type == FLParser.EQ)
            {
                return (MyType.BOOL, (bool)left.value == (bool)right.value);
            }
            else if (context.op.Type == FLParser.NE)
            {
                return (MyType.BOOL, (bool)left.value != (bool)right.value);
            }
        }
        else
        {
            Errors.ReportError(context.op, "Both operands must be of the same type");
            return (MyType.ERROR, -1);
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
        else
        {
            Errors.ReportError(context.Start, "Both operands must be boolean");
            return (MyType.ERROR, -1);
        }
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
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type int, but trying to assign float");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.INT && right.type == MyType.BOOL)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type int, but trying to assign bool");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.INT && right.type == MyType.STRING)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type int, but trying to assign string");
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
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type float, but trying to assign bool");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.FLOAT && right.type == MyType.STRING)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type float, but trying to assign string");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.BOOL && right.type == MyType.INT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type bool, but trying to assign int");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.BOOL && right.type == MyType.FLOAT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type bool, but trying to assign float");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.BOOL && right.type == MyType.STRING)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type bool, but trying to assign string");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.STRING && right.type == MyType.INT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type string, but trying to assign int");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.STRING && right.type == MyType.FLOAT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type string, but trying to assign float");
            return (MyType.ERROR, -1);
        }
        else if(left.Type==MyType.STRING && right.type == MyType.BOOL)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type string, but trying to assign bool");
            return (MyType.ERROR, -1);
        }
        else
        {
            symbolTable[context.ID().Symbol] = right;
            return right;
        }
        
    }

    public override (MyType type, object value) VisitIf(FLParser.IfContext context)
    {
        var condition = Visit(context.expr());
        if (condition.type != MyType.BOOL)
        {
            Errors.ReportError(context.expr().Start, "Condition must be a boolean");
            return (MyType.ERROR, -1);
        }
        else
        {
            if ((bool)condition.value)
            {
                Visit(context.statement(0));
            }
            else if (context.statement().Length > 1)
            {
                Visit(context.statement(1));
            }
            
            return (MyType.ERROR,0);
        }
    }
    
    public override (MyType type, object value) VisitWhile(FLParser.WhileContext context)
    {
        var condition = Visit(context.expr());
        if (condition.type != MyType.BOOL)
        {
            Errors.ReportError(context.expr().Start, "Condition must be a boolean");
            return (MyType.ERROR, -1);
        }
        else
        {
            Visit(context.statement());
            return (MyType.ERROR, 0);
        }
    }
    
    public override (MyType type, object value) VisitTernary(FLParser.TernaryContext context)
    {
        var condition = Visit(context.expr(0));
        
        if (condition.type != MyType.BOOL)
        {
            Errors.ReportError(context.expr(0).Start, "Condition must be a boolean");
            return (MyType.ERROR, -1);
        }
        else
        {
            var k= Visit(context.expr(1));
            var l = Visit(context.expr(2));
            if(k.type==MyType.FLOAT && l.type==MyType.FLOAT)
            {
                return (MyType.FLOAT, 0.0);
            }
            else if(k.type==MyType.INT && l.type==MyType.INT)
            {
                return (MyType.INT,0);
            }
            else if(k.type==MyType.INT && l.type==MyType.FLOAT)
            {
                return (MyType.FLOAT, 0.0);
            }
            else if(k.type==MyType.FLOAT && l.type==MyType.INT)
            {
                return (MyType.FLOAT, 0.0);
            }
            else if (k.type == MyType.BOOL && l.type == MyType.BOOL)
            {
                return (MyType.BOOL, false);
            }
            else if (k.type == MyType.STRING && l.type == MyType.STRING)
            {
                return (MyType.STRING, "");
            }
            else
            {
                if (k.type != l.type)
                {
                    Errors.ReportError(context.expr(2).Start, "Both expressions must be of the same type");
                    return (MyType.ERROR, -1);
                }
                return (MyType.ERROR, -1);
            }
        }
    }
}