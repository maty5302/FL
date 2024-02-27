namespace Cv2;

public enum TokenType
{
    Identifier,
    Number,
    Operator,
    Delimiter,
    Keyword,
    EOF
}

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }    
    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
    
    public override string ToString()
    {
        if (Type == TokenType.Identifier)
            return $"ID: {Value}";
        else if(Type == TokenType.Number)
            return $"NUM: {Value}";
        else if(Type == TokenType.Operator)
            return $"OP: {Value}";
        else if (Type == TokenType.Delimiter)
        {
            if(Value=="(")
                return "LPAR: (";
            else if(Value==")")
                return "RPAR: )";
            else if(Value==";")
                return "SEMICOLON: ;";
            else
                return $"DEL: {Value}";
        }
        else if(Type == TokenType.Keyword)
            return $"{Value.ToUpper()}";
        else
            return $"{Type}: {Value}";
    }
}