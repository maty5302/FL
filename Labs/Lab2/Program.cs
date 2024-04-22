using Cv2;

Console.WriteLine("Lexical Analysis");

var input = "    -2 + (245 div 3);  // note\n2 mod 3 * hello";
input = input.Replace(" ","");

var scanner = new Scanner(input);
var token = scanner.NextToken();
while (token.Type != TokenType.EOF)
{
    Console.WriteLine(token);
    token = scanner.NextToken();
}