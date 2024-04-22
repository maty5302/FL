// See https://aka.ms/new-console-template for more information
StreamReader sr = new StreamReader("input.txt");
int numberOfLines = int.Parse(sr.ReadLine());
string[] lines = new string[numberOfLines];
for (int i = 0; i < numberOfLines; i++)
{
    lines[i] = sr.ReadLine();
}


for (int a = 0; a<lines.Length; a++)
{
    var input = lines[a];
    input = input.Replace(" ","");
    bool previousOperand = false;
    int counter = 0;

    for (int i = 0; i < input.Length; i++)
    {
        if (input[i] == '(')
        {
            counter++;
        }
        else if (input[i] == ')')
        {
            //Console.WriteLine(input);
            if (counter > 0)
            {
                bool first = true;
                string temp = "";
                int c = 0;
                for (int j = 0; j < input.Length; j++)
                {
                    if (counter == c)
                    {
                        if(input[j] == ')')
                        {
                            counter--;
                            System.Data.DataTable table = new System.Data.DataTable();
                            string result = table.Compute(temp, "").ToString();
                            temp = "(" + temp + ")";
                            input = input.Replace(temp,result);
                            i = 0;
                            counter = 0;
                            break; 
                        }
                        else
                        {
                            if((input[j]=='+' || input[j]=='-' || input[j]=='*' || input[j]=='/'))
                            {
                                if(previousOperand && first)
                                {
                                    Console.WriteLine("Invalid");
                                    return;
                                }
                                else
                                {
                                    previousOperand = true;
                                }
                            }
                            else
                            {
                                previousOperand = false;
                            }
                            first = false;
                            temp += input[j];
                        }
                    }
                    if(input[j] == '(')
                    {
                        c++;
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid");
                return;
            }
        }
        else if(input[i] == '+' || input[i] == '-' || input[i] == '*' || input[i] == '/')
        {
            if(previousOperand)
            {
                Console.WriteLine("Invalid");
                return;
            }
            else
            {
                previousOperand = true;
            }
        }
        else
        {
            previousOperand = false;
        }
    }

    if(counter == 0)
    {
        System.Data.DataTable table = new System.Data.DataTable();
        string result = table.Compute(input, "").ToString();
        input = result;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid");
        return;
    }
    Console.WriteLine(input);
}

