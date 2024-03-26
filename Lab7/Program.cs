// See https://aka.ms/new-console-template for more information

using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Lab7;

Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
var fileName = "input.txt";
Console.WriteLine("Parsing: " + fileName);
var inputFile = new StreamReader(fileName);
AntlrInputStream input = new AntlrInputStream(inputFile);
PLC_Lab7_exprLexer lexer = new PLC_Lab7_exprLexer(input);
CommonTokenStream tokens = new CommonTokenStream(lexer);
PLC_Lab7_exprParser parser = new PLC_Lab7_exprParser(tokens);
parser.AddErrorListener(new VerboseListener());

IParseTree tree = parser.prog();

if(parser.NumberOfSyntaxErrors == 0)
{
    Console.WriteLine(tree.ToStringTree(parser));
}