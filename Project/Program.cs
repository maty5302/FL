using System.Globalization;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
namespace Project;

class Program
{
    public static string DeleteEmptyLines(string input)
    {
        string pattern = @"^\s*$[\r\n]*";
    
        string result = Regex.Replace(input, pattern, string.Empty, RegexOptions.Multiline);
    
        return result;
    }
    private static void ParseFile(string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[Info] ");
        Console.ResetColor();
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
            // Console.ForegroundColor = ConsoleColor.Yellow;
            // Console.Write("[Debug] ");
            // Console.ResetColor();
            // Console.WriteLine(tree.ToStringTree(parser));
            new EvalVisitor().Visit(tree);

           if(Errors.NumberOfErrors==0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Info] ");
                Console.ResetColor();
                Console.WriteLine("No syntax and type-check errors found.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Info] ");
                Console.ResetColor();
                Console.WriteLine("Generating instructions...");
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[Debug] ");
                var res = new EvalCompute().Visit(tree);
                var resParsed = DeleteEmptyLines(res);
                StreamWriter outputFile = new StreamWriter("output.txt");
                outputFile.Write(resParsed);
                outputFile.Close();
                Console.WriteLine(resParsed);
                Console.ResetColor();
                
            }
            Errors.PrintAndClearErrors();
            
            Console.WriteLine("----------------------------------------");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[Error] ");
            Console.ResetColor();
            Console.WriteLine("Syntax errors found.");
            Console.WriteLine("----------------------------------------");
        }
    }
    static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var filePlc1 = "Input_files/PLC_t1.in";
        var filePlc2 = "Input_files/PLC_t2.in";
        var filePlc3 = "Input_files/PLC_t3.in";
        var fileName = "Input_files/input.txt";
        
        ParseFile(filePlc1);
        ParseFile(filePlc2);
        ParseFile(filePlc3);
        //ParseFile(fileName);
        
    }
}