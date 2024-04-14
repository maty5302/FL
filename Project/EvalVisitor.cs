namespace Project;

public class EvalVisitor : FLBaseVisitor<MyType>
{
    private SymbolTable symbolTable = new SymbolTable();
    
    public override MyType VisitProg(FLParser.ProgContext context)
    {
        foreach (var statement in context.statement())
        {
            Visit(statement);
        }
        return MyType.ERROR;
    }

    public override MyType VisitDeclaration(FLParser.DeclarationContext context)
    {
        var type = Visit(context.type());
        foreach (var id in context.ID())
        {
            symbolTable.Add(id.Symbol, type);
        }
        return MyType.ERROR;
    }
    
    public override MyType VisitType(FLParser.TypeContext context)
    {
        if (context.GetText() == "int")
        {
            return MyType.INT;
        }
        else if (context.GetText() == "float")
        {
            return MyType.FLOAT;
        }
        else if (context.GetText() == "bool")
        {
            return MyType.BOOL;
        }
        else if (context.GetText() == "string")
        {
            return MyType.STRING;
        }
        return MyType.ERROR;
    }
    
    public override MyType VisitFloat(FLParser.FloatContext context)
    {
        return MyType.FLOAT;
    }
    
    public override MyType VisitInt(FLParser.IntContext context)
    {
        return MyType.INT;
    }
    
    public override MyType VisitString(FLParser.StringContext context)
    {
        return MyType.STRING;
    }
    
    public override MyType VisitBool(FLParser.BoolContext context)
    {
        return MyType.BOOL;
    }
    
    public override MyType VisitId(FLParser.IdContext context)
    {
        return symbolTable[context.ID().Symbol];
    }
    
    public override MyType VisitParens(FLParser.ParensContext context)
    {
        return Visit(context.expr());
    }
    
    public override MyType VisitNot(FLParser.NotContext context)
    {
        var value = Visit(context.expr());
        if (value == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        if (value != MyType.BOOL)
        {
            Errors.ReportError(context.Start, "Operand must be a boolean");
            return MyType.ERROR;
        }
        return MyType.BOOL;
    }

    public override MyType VisitUnaryMinus(FLParser.UnaryMinusContext context)
    {
        var value = Visit(context.expr());
        if (value == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        if (value == MyType.INT)
        {
            return MyType.INT;
        }
        else if (value==MyType.FLOAT)
        {
            return MyType.FLOAT;
        }
        else
        {
            Errors.ReportError(context.Start, "Operand must be a number");
            return MyType.ERROR;
        }
    }

    public override MyType VisitMulDivMod(FLParser.MulDivModContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left == MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        
        if (left == MyType.INT && right == MyType.INT)
        {
            if (context.op.Type == FLParser.MUL)
            {
                return MyType.INT;
            }
            else if (context.op.Type == FLParser.DIV)
            {
                return MyType.INT;
            }
            else if (context.op.Type == FLParser.MOD)
            {
                return MyType.INT;
            }
        }
        else //todo fix
        {
            if (context.op.Type == FLParser.MUL)
            {
                return MyType.FLOAT;
            }
            else if (context.op.Type == FLParser.DIV)
            {
                return MyType.FLOAT;
            }
            else if (context.op.Type == FLParser.MOD)
            {
                Errors.ReportError(context.op, "Modulus operator cannot be used with float");
                return MyType.ERROR;
            }
        }
        return MyType.ERROR;
    }
    
    public override MyType VisitAddSub(FLParser.AddSubContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left == MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        
        if (left == MyType.INT && right == MyType.INT)
        {
            if (context.op.Type == FLParser.ADD || context.op.Type == FLParser.SUB)
            {
                return MyType.INT;
            }
            else
            {
                Errors.ReportError(context.op, "You cannot concatenate numbers");
                return MyType.ERROR;
            }
        }
        else if((left == MyType.FLOAT && right==MyType.FLOAT)||(left == MyType.FLOAT && right==MyType.INT)||(left == MyType.INT && right==MyType.FLOAT))
        {
            if (context.op.Type == FLParser.ADD || context.op.Type == FLParser.SUB)
            {
                return MyType.FLOAT;
            }
            else
            {
                Errors.ReportError(context.op, "You cannot concatenate numbers");
                return MyType.ERROR;
            }
        }
        else if (left == MyType.STRING && right == MyType.STRING)
        {
            if (context.op.Type == FLParser.CONCAT)
            {
                return (MyType.STRING);
            }
            else
            {
                Errors.ReportError(context.op, "You cannot subtract and and strings");
                return MyType.ERROR;
            }
        }
        else
        {
            Errors.ReportError(context.op,"You cannot add, subtract or concatenate bool");
            return MyType.ERROR;
        }
        return MyType.ERROR;
    }
    
    public override MyType VisitLtGt(FLParser.LtGtContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left == MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        
        if ((left == MyType.INT && right == MyType.INT) || (left == MyType.FLOAT && right == MyType.FLOAT))
        {
            return MyType.BOOL;
        }
        else if((left==MyType.FLOAT && right==MyType.INT)||(left==MyType.INT && right==MyType.FLOAT))
        {
            return MyType.BOOL;
        }
        else
        {
            Errors.ReportError(context.op, "Both operands must be numbers");
            return MyType.ERROR;
        }
        
    }
    
    public override MyType VisitEqNe(FLParser.EqNeContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left == MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }

        if (left == right)
        {
            return MyType.BOOL;
        }
        else
        {
            Errors.ReportError(context.op, "Both operands must be of the same type");
            return MyType.ERROR;
        }
    }
    
    public override MyType VisitAnd(FLParser.AndContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left == MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        
        if (left == MyType.BOOL && right == MyType.BOOL)
        {
            return MyType.BOOL;
        }
        
        
        return MyType.ERROR;
    }
    
    public override MyType VisitOr(FLParser.OrContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (left == MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        
        if (left == MyType.BOOL && right == MyType.BOOL)
        {
            return MyType.BOOL;
        }
        else
        {
            Errors.ReportError(context.Start, "Both operands must be boolean");
            return MyType.ERROR;
        }
    }
    
    public override MyType VisitAssign(FLParser.AssignContext context)
    {
        var right = Visit(context.expr());
        var left = symbolTable[context.ID().Symbol];
        
        if( left ==MyType.ERROR || right == MyType.ERROR)
        {
            return MyType.ERROR;
        }
        if(left == MyType.INT && right == MyType.FLOAT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type int, but trying to assign float");
            return MyType.ERROR;
        }
        else if(left==MyType.INT && right == MyType.BOOL)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type int, but trying to assign bool");
            return MyType.ERROR;
        }
        else if(left==MyType.INT && right == MyType.STRING)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type int, but trying to assign string");
            return MyType.ERROR;
        }
        else if(left==MyType.FLOAT && right == MyType.INT)
        {
            return MyType.FLOAT;
        }
        else if(left==MyType.FLOAT && right == MyType.BOOL)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type float, but trying to assign bool");
            return MyType.ERROR;
        }
        else if(left==MyType.FLOAT && right == MyType.STRING)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type float, but trying to assign string");
            return MyType.ERROR;
        }
        else if(left==MyType.BOOL && right == MyType.INT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type bool, but trying to assign int");
            return MyType.ERROR;
        }
        else if(left==MyType.BOOL && right == MyType.FLOAT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type bool, but trying to assign float");
            return MyType.ERROR;
        }
        else if(left==MyType.BOOL && right == MyType.STRING)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type bool, but trying to assign string");
            return MyType.ERROR;
        }
        else if(left==MyType.STRING && right == MyType.INT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type string, but trying to assign int");
            return MyType.ERROR;
        }
        else if(left==MyType.STRING && right == MyType.FLOAT)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type string, but trying to assign float");
            return MyType.ERROR;
        }
        else if(left==MyType.STRING && right == MyType.BOOL)
        {
            Errors.ReportError(context.ID().Symbol,$"Variable {context.ID().Symbol.Text} is of type string, but trying to assign bool");
            return MyType.ERROR;
        }
        else
        {
            symbolTable[context.ID().Symbol] = right;
            return right;
        }
        
    }

    public override MyType VisitIf(FLParser.IfContext context)
    {
        var condition = Visit(context.expr());
        if (condition != MyType.BOOL)
        {
            Errors.ReportError(context.expr().Start, "Condition must be a boolean");
            return MyType.ERROR;
        }
        else
        {
            return MyType.BOOL;
        }
    }
    
    public override MyType VisitWhile(FLParser.WhileContext context)
    {
        var condition = Visit(context.expr());
        if (condition != MyType.BOOL)
        {
            Errors.ReportError(context.expr().Start, "Condition must be a boolean");
            return MyType.ERROR;
        }
        else
        {
            return Visit(context.statement());
        }
    }
    
    public override MyType VisitTernary(FLParser.TernaryContext context)
    {
        var condition = Visit(context.expr(0));
        
        if (condition != MyType.BOOL)
        {
            Errors.ReportError(context.expr(0).Start, "Condition must be a boolean");
            return MyType.ERROR;
        }
        else
        {
            var k= Visit(context.expr(1));
            var l = Visit(context.expr(2));
           
            
            if(k==MyType.FLOAT || l==MyType.FLOAT)
            {
                return MyType.FLOAT;
            }
            else if(k != l)
            {
                Errors.ReportError(context.expr(2).Start, "Both expressions must be of the same type");
                return MyType.ERROR;
            }
            else
            {
                return k;
            }

        }
    }
}