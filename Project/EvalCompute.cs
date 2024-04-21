using System.ComponentModel.Design;
using System.Text;
using Antlr4.Runtime.Misc;

namespace Project;

public class EvalCompute : FLBaseVisitor<string>
{
    private SymbolTable symbolTable = new SymbolTable();
    private static int counter = 0;
    public override string VisitProg([NotNull] FLParser.ProgContext context)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var statement in context.statement())
        {
            var code = Visit(statement);
            sb.AppendLine(code);
        }
        return sb.ToString();
    }
    
    public override string VisitDeclaration([NotNull] FLParser.DeclarationContext context)
    {
        var type = context.type().GetText();
        var mytype = type switch
        {
            "int" => "I",
            "float" => "F",
            "string" => "S",
            "bool" => "B",
            _ => throw new System.Exception("Unknown type")
        };
        var mType = type switch
        {
            "int" => MyType.INT,
            "float" => MyType.FLOAT,
            "string" => MyType.STRING,
            "bool" => MyType.BOOL,
            _ => throw new System.Exception("Unknown type")
        };
        var defaultVal = mytype switch
        {
            "I" => "0",
            "F" => "0.0",
            "S" => "\"\"",
            "B" => "false",
            _ => throw new System.Exception("Unknown type")
        };
        StringBuilder sb = new StringBuilder();
        
        foreach (var id in context.ID())
        {
            symbolTable.Add(id.Symbol, mType);
            sb.AppendLine($"push {mytype} {defaultVal}");
            sb.AppendLine($"save {id.GetText()}");
        }

        return sb.ToString();

    }
    
    public override string VisitWrite([NotNull] FLParser.WriteContext context)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var id in context.expr())
        {
            sb.AppendLine(Visit(id));
        }

        int j = context.expr().Length;
        sb.AppendLine($"print {j}");
        return sb.ToString();
    }
    
    public override string VisitString([NotNull] FLParser.StringContext context)
    {
        return $"push S {context.STRING().GetText()}";
    }
    
    public override string VisitInt([NotNull] FLParser.IntContext context)
    {
        return $"push I {context.INT().GetText()}";
    }
    
    public override string VisitFloat([NotNull] FLParser.FloatContext context)
    {
        return $"push F {context.FLOAT().GetText()}";
    }

    public override string VisitBool([NotNull] FLParser.BoolContext context)
    {
        return $"push B {context.BOOLEAN().GetText()}";
    }
    
    public override string VisitAssign([NotNull] FLParser.AssignContext context)
    {
        StringBuilder sb = new StringBuilder();
        var type = symbolTable[context.ID().Symbol];
        sb.AppendLine(Visit(context.expr()));
        if (type == MyType.FLOAT && context.expr() is FLParser.IntContext)
            sb.AppendLine("itof");
        sb.AppendLine($"save {context.ID().GetText()}");
        sb.Append($"load {context.ID().GetText()}");
        return sb.ToString();
    }
    

    public override string VisitPrintExpr([NotNull] FLParser.PrintExprContext context)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Visit(context.expr()));
        sb.AppendLine("pop");
        return sb.ToString();
    }
    
    public override string VisitId([NotNull] FLParser.IdContext context)
    {
        return $"load {context.ID().GetText()}";
    }

    public override string VisitUnaryMinus([NotNull] FLParser.UnaryMinusContext context)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Visit(context.expr()));
        sb.Append("uminus");
        return sb.ToString();
    }

    public override string VisitAddSub([NotNull] FLParser.AddSubContext context)
    {
        StringBuilder sb = new StringBuilder();
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        var datatype = "";
        sb.AppendLine(left);
        if((left.Contains("push I") && right.Contains("push F"))){
            sb.AppendLine("itof");
            datatype = "F";
        }
        sb.AppendLine(right);
        if (left.Contains("push F") && right.Contains("push I"))
        {
            sb.AppendLine("itof");
            datatype = "F";
        }
        if(right.Contains("F") && left.Contains("F"))
            datatype = "F";
        else if(right.Contains("I") && left.Contains("I"))
            datatype = "I";
        else if(right.Contains("S") && left.Contains("S"))
            datatype = "S";

        sb.Append(context.op.Text == "+" ? "add " + datatype : context.op.Text == "-" ? "sub " + datatype: "concat " + datatype);
        
        return sb.ToString();
    }
    
    public override string VisitMulDivMod([NotNull] FLParser.MulDivModContext context)
    {
        StringBuilder sb = new StringBuilder();
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        var datatype = "";
        sb.AppendLine(left);
        if ((left.Contains("push I") && right.Contains("push F")))
        {
            sb.AppendLine("itof");
            datatype = "F";
        }

        sb.AppendLine(right);
        if (left.Contains("push F") && right.Contains("push I"))
        {
            sb.AppendLine("itof");
            datatype = "F";
        }
        if(right.Contains("F") && left.Contains("F"))
            datatype = "F";
        else if(right.Contains("I") && left.Contains("I"))
            datatype = "I";
        else if(right.Contains("S") && left.Contains("S"))
            datatype = "S";
        sb.Append(context.op.Text == "*" ? "mul " + datatype : context.op.Text == "/" ? "div " + datatype : "mod " + datatype);
        return sb.ToString();
    }
    
    public override string VisitParens([NotNull] FLParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    public override string VisitRead([NotNull] FLParser.ReadContext context)
    {
       
        StringBuilder sb = new StringBuilder();
        foreach (var id in context.ID()){
            var type = symbolTable[id.Symbol];
            var mytype = type switch
            {
                MyType.INT => "I",
                MyType.FLOAT => "F",
                MyType.STRING => "S",
                MyType.BOOL => "B",
                _ => throw new System.Exception("Unknown type")
            };
            sb.AppendLine($"read {mytype}");
            sb.AppendLine($"save {id.GetText()}");
        }
        return sb.ToString();
    }

    public override string VisitLtGt([NotNull] FLParser.LtGtContext context)
    {
        StringBuilder sb = new StringBuilder();
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        sb.AppendLine(left);
        if((left.Contains("push I") && right.Contains("push F")))
            sb.AppendLine("itof");
        sb.AppendLine(right);
        if (left.Contains("push F") && right.Contains("push I"))
            sb.AppendLine("itof");
        var op = context.op.Text == "<" ? "lt" : "gt";
        sb.Append(op);
        return sb.ToString();

    }

    public override string VisitEqNe([NotNull] FLParser.EqNeContext context)
    {
        StringBuilder sb = new StringBuilder();
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        sb.AppendLine(left);
        if((left.Contains("push I") && right.Contains("push F")))
            sb.AppendLine("itof");
        sb.AppendLine(right);
        if (left.Contains("push F") && right.Contains("push I"))
            sb.AppendLine("itof");
        if (context.op.Text == "!=")
        {
            sb.AppendLine("eq");
            sb.Append("not");
        }
        else
            sb.Append("eq");
        return sb.ToString();
        
    }

    public override string VisitNot([NotNull] FLParser.NotContext context)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Visit(context.expr()));
        sb.Append("not");
        return sb.ToString();
    }
    
    public override string VisitAnd([NotNull] FLParser.AndContext context)
    {
        StringBuilder sb = new StringBuilder();
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        sb.AppendLine(left);
        sb.AppendLine(right);
        sb.Append("and");
        return sb.ToString();
    }

    public override string VisitOr([NotNull] FLParser.OrContext context)
    {
        StringBuilder sb = new StringBuilder();
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        sb.AppendLine(left);
        sb.AppendLine(right);
        sb.Append("or");
        return sb.ToString();
    }

    public override string VisitBlock([NotNull] FLParser.BlockContext context)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var statement in context.statement())
        {
            sb.Append(Visit(statement));
        }
        return sb.ToString();
    } 
    
    public override string VisitIf([NotNull] FLParser.IfContext context)
    {
        StringBuilder sb = new StringBuilder();
        var condition = Visit(context.expr());
        sb.AppendLine(condition);
        sb.AppendLine($"fjmp {counter}");
        var then = Visit(context.statement(0));
        sb.AppendLine(then);
        sb.AppendLine($"jmp {counter + 1}");
        sb.AppendLine($"label {counter}");
        if (context.statement().Length == 2)
        {
            var elseStmt = Visit(context.statement(1));
            sb.AppendLine(elseStmt);
        }
        sb.Append($"label {counter+1}");
        counter+=2;
        return sb.ToString();
        
    }
    
    public override string VisitWhile([NotNull] FLParser.WhileContext context)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"label {counter}");
        var condition = Visit(context.expr());
        sb.AppendLine(condition);
        sb.AppendLine($"fjmp {counter + 1}");
        var block = Visit(context.statement());
        sb.AppendLine(block);
        sb.AppendLine($"jmp {counter}");
        sb.Append($"label {counter + 1}");
        counter+=2;
        return sb.ToString();
    }
}