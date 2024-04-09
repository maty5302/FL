using Antlr4.Runtime;

namespace Project
{
    public class Errors
    {
        private static readonly List<string> ErrorsData = new List<string>();
        static public void ReportError(IToken token, string message)
        {
            ErrorsData.Add($"Error at line: {token.Line}:{token.Column} - {message}");
        }
        public static int NumberOfErrors {  get { return ErrorsData.Count; } }
        public static void PrintAndClearErrors()
        {
            foreach (var error in ErrorsData)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[Error] ");
                Console.ResetColor();
                Console.WriteLine(error);
            }
            ErrorsData.Clear(); 
        }
    }
}
