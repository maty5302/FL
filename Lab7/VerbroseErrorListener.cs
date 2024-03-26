using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Lab7;

public class VerboseListener : BaseErrorListener
{
    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {

        IList<string> stack = ((Parser)recognizer).GetRuleInvocationStack();
        stack.Reverse();

        Console.Error.WriteLine("rule stack: " + String.Join(", ", stack));
        Console.Error.WriteLine("line " + line + ":" + charPositionInLine + " at " + offendingSymbol + ": " + msg);
    }
}