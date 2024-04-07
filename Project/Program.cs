using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
namespace Project;

class Program
{
    static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var fileName = "input.txt";
        Console.WriteLine("Parsing: " + fileName);
        var inputFile = new StreamReader(fileName);
        AntlrInputStream input = new AntlrInputStream(inputFile);
        FLLexer lexer = new FLLexer(input);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        FLParser parser = new FLParser(tokens);
        parser.AddErrorListener(new VerboseListener());

        IParseTree tree = parser.prog();
        
        if(parser.NumberOfSyntaxErrors == 0)
        {
            Console.WriteLine(tree.ToStringTree(parser));
            new EvalVisitor().Visit(tree);
            Errors.PrintAndClearErrors();
        }
        else
        {
            Console.WriteLine("Syntax errors found.");
        }
    }
}