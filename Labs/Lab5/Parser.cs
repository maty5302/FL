using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Lab5
{
    public class Parser
    {
        private Scanner scanner;
        private Token token;
        public bool Error { get; private set; }
        public List<int> Rules { get; set; } = new List<int>();
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            this.token = scanner.NextToken();
        }

        private void Expect(TokenType expectedType)
        {
            if (token.Type != expectedType)
                //throw new Exception($"Expected {expectedType}, got {token.Type}");
                Error = true;
            token = scanner.NextToken();
        }
        
        public int E()
        {
            Rules.Add(1);
            var left = T();
            return E1(left);
        }

        private int T()
        {
            Rules.Add(5);
            var left = F();
            return T1(left);
        }

        private int F()
        {
            int result;
            if (token.Type == TokenType.LeftPAR)
            {
                Rules.Add(9);
                Expect(TokenType.LeftPAR);
                result = E();
                Expect(TokenType.RightPAR);
                
            }
            else if (token.Type == TokenType.NUMBER)
            {
                Rules.Add(10);
                result = int.Parse(token.Value);
                Expect(TokenType.NUMBER);
                
            }
            else
            {
                Error = true;
                result = -1;
            }
            return result;
        }
            
        private int T1(int left)
        {
            if (token.Type == TokenType.MUL)
            {
                Rules.Add(6);
                Expect(TokenType.MUL);
                var right = F();
                return T1(left * right);
            }
            else if (token.Type == TokenType.DIV)
            {
                Rules.Add(7);
                Expect(TokenType.DIV);
                return T1(left/ F());
            }
            else if (token.Type == TokenType.EOL || token.Type == TokenType.RightPAR || token.Type == TokenType.PLUS || token.Type == TokenType.MINUS)
            {
                Rules.Add(8);
                return left;
            }
            else
            {
                Error = true;
                return -1;
            }
        }
        
        private int E1(int left)
        {
            if (token.Type == TokenType.PLUS)
            {
                Rules.Add(2);
                Expect(TokenType.PLUS);
                return E1(left + T());
            }
            else if (token.Type == TokenType.MINUS)
            {
                Rules.Add(3);
                Expect(TokenType.MINUS);
                return E1(left-T());
            }
            else if (token.Type == TokenType.EOL || token.Type == TokenType.RightPAR)
            {
                Rules.Add(4);
                return left;
            }
            else
            {
                //throw new Exception($"Expected PLUS or MINUS, got {token.Type}");
                Error = true;
                return -1;
            }
        }
    }
}
