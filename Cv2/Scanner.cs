namespace Cv2;

public class Scanner
{
    private readonly string _input;
    private int _position;
    private List<string> operators = new List<string> { "+", "-", "*", "/"};
    private List<string> keywords = new List<string> { "div", "mod" };
    private List<string>  delimiters = new List<string> { "(", ")", ";" };
    
    public Scanner(string input)
    {
        _input = input;
        _position = 0;
    }

    public Token NextToken()
    {
        if(_position>= _input.Length)
            return new Token(TokenType.EOF, "");
        
        if (_input[_position] == '/' && _input[_position + 1] == '/')
        {
            while (_input[_position] != '\n')
                _position++;
            
        }
        else if (operators.Contains(_input[_position].ToString()))
            return new Token(TokenType.Operator, _input[_position++].ToString());
        else if (delimiters.Contains(_input[_position].ToString()))
            return new Token(TokenType.Delimiter, _input[_position++].ToString());
        else if (char.IsDigit(_input[_position]))
        {
            string temp = "";
            while (char.IsDigit(_input[_position]))
            {
                temp += _input[_position];
                _position++;
            }

            return new Token(TokenType.Number, temp);
        }
        else if (char.IsLetter(_input[_position]))
        {
            string temp = "";
            while (char.IsLetter(_input[_position]))
            {
                temp += _input[_position];
                _position++;
                if (_position < _input.Length)
                    continue;
                else
                {
                    break;
                }
            }

            if (keywords.Contains(temp))
                return new Token(TokenType.Keyword, temp);
            else
                return new Token(TokenType.Identifier, temp);
        }
        _position++;
        return NextToken();
    }

}