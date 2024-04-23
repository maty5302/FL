# FL

FL is a project that aims to create a simple programming language and its interpreter. The project is written in C# and it uses ANTLR to generate a parser for a language. 
After the parser checks for syntax errors, then it goes and checks type checking. The language then generates instructions that are then processed through a virtual machine to get you an output.
The language can do basic arithmetic operations, conditional statements, and loops.

# Assignment of project
<details>
<summary>Detailed assignment there</summary>
  
## Basic Description
The project will be composed of the following steps:
1. **Using ANTLR**, implement a parser for the language specified below. If there is at least one syntax error, report this error (or errors) and stop the computations.
2. If there are no syntactic errors, perform the type checking. If there are some type of errors, report all these errors and stop the computation.
3. If there are no type errors, generate the appropriate target code. It will be a text file composed of stack-based instructions that are defined below.
4. Implement an interpreter, that gets a text file with these instructions and evaluates them.


## Language Specification
### Program's Formatting
The program consists of a sequence of commands. Commands are written with free formatting. Comments, spaces, tabs, and line breaks serve only as delimiters and do not affect the meaning of the program. **Comments** are bounded by two slashes and the end of the line. Keywords are reserved. Identifiers and keywords are case sensitive.

### Literals
There are the following literals:
- integers - `int` - sequence of digits.
- floating point numbers - `float` - sequence of digits containing a `'.'` character.
- booleans - `bool` - values: `true` and `false`.
- strings - `string`  - text given in quotation marks: `"text"`. Escape sequences are optional in our strings.

### Variables
Variable identifiers are composed of letters and digits, and it must start with a letter. Each variable must be declared before it is used. Repeated declaration of a variable with the same name is an error. Variables must have one of the following types: `int`, `float`, `bool` or `string`. After the variables are declared, they have initial values: `0`, `0.0`, `""` respectively `false`.

### Statements
The following statements are defined:
- `;` - empty command.
- `type variable, variable, ... ;` - declaration of variables, all these variables have the same type `type`. It can be one of: `int`, `float`, `bool`, `String`
- `expression ;` - it evaluates given expression, the resulting value of the expression is ignored. Note, there can be some side effects like an assignment to a variable.
- `read variable, variable, ... ;` - it reads values from standard input and then these values are assigned to corresponding variables. Each of these input values is on a separate line and it is verified, that have an appropriate type.
- `write expression, expression, ... ;` - it writes values of expressions to standard output. The `"\n"` character is written after the value of the last expression.
- `{ statement statement ... }` - block of statements.
- `if (condition) statement [else statement]` - conditional statement - condition is an expression with a type: `bool`. The else part of the statement is optional.
- `while (condition) statement` - a cycle - condition must be a `bool` expression. This cycle repeats the given statement while the condition holds (it is `true`).

### Expression
Lists in expression trees are literals or variables. Types of operands must preserve the type of the operator. If necessary, `int` values are **automatically** cast to `float`. In other words, the type of `5 + 5.5` is `float`, and the number `5` which type `int` is automatically converted to `float`. There is **no** conversion from `float` to `int`!

The following table defines operators in our expressions. Operator Signature is defined using letters: 'I, R, B, S' which corresponds to types: `int`, `float`, `bool`, `string`.

| Description                 | Operator     | Operator's Signature                |
|-----------------------------|--------------|-------------------------------------|
| unary minus                 | `-`          | `I → I ∨ F → F`                     |
| binary arithmetic operators | `+, -, *, /` | `I × I → I ∨ F × F → F`             |
| modulo                      | `%`          | `I × I → I`                         |
| concatenation of strings    | `.`          | `S × S → S`                         |
| relational operators        | `< >`        | `x × x → B, where x ∈ {I, F}`       |
| comparison                  | `== !=`      | `x × x → B, where x ∈ {I, F, S}`    |
| logic and, or               | `&& \|\|`    | `B × B → B`                         |
| logic not                   | `!`          | `B → B`                             |
| assignment                  | `=`          | `x × x → x, where x ∈ {I, F, S, B}` |

In the assignment, the left operand is strictly a variable and the right operand is an expression. The type of the variable is the type of the left operand. A side effect is storing the value on the right side into the variable. The automatic conversion cannot change the type of the variable, i.e., it is impossible to store `float` value in `int` variable.

We can **use parentheses** in expressions.

All operators (except `=`) have left associativity (`=` have right associativity), and their priority is (from lowest to highest):
1. `=`
2. `||`
3. `&&`
4. `== !=`
5. `< >`
6. `+ - .`
7. `* / %`
8. `!`
9. `unary -`

## Our (Stack-based) Instructions Set
All instructions are stack-based. The main memory is a stack and while evaluating the instructions, the input data are taken from the stack and the results are put also in the stack.

| Instruction | Description                                                                                                                                                           |
|-------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `add`       | binary `+`                                                                                                                                                            |
| `sub`       | binary `-`                                                                                                                                                            |
| `mul`       | binary `*`                                                                                                                                                            |
| `div`       | binary `/`                                                                                                                                                            |
| `mod`       | binary `%`                                                                                                                                                            |
| `uminus`    | unary `-`                                                                                                                                                             |
| `concat`    | binary `.` - a concatenation of strings                                                                                                                                 |
| `and`       | binary `&&`                                                                                                                                                           |
| `or`        | binary `\|\|`                                                                                                                                                         |
| `gt`        | binary `>`                                                                                                                                                            |
| `lt`        | binary `<`                                                                                                                                                            |
| `eq`        | binary `==` - compares two values                                                                                                                                     |
| `not`       | unary `!` - negating boolean value                                                                                                                                    |
| `itof`      | Instruction takes int value from the stack, converts it to float, and returns it to stack.                                                                             |
| `push T x`  | Instruction pushes the value `x` of type `T`. Where `T` represents `I - int`, `F - float`, `S - string`, `B - bool`. Example: push I 10, push B true, push S "A B C " |
| `pop`       | Instruction takes one value from the stack and discards it.                                                                                                           |
| `load id`   | Instruction loads value of variable `id` on the stack.                                                                                                                    |
| `save id`   | Instruction takes value from the top of the stack and stores it into the variable with the name `id`                                                                      |
| `label n`   | Instruction marks the spot in source code with the unique number `n`                                                                                                      |
| `jmp n`     | Instruction jumps to the label defined by unique number `n`                                                                                                           |
| `fjmp n`    | Instruction takes a boolean value from the stack and if it is `false`, it will perform a jump to a label with the unique number `n`                                         |
| `print n`   | Instruction takes `n` values from the stack and prints them on standard output                                                                                            |
| `read T`    | Instruction reads the value of type `T` (`I - int`, `F - float`, `S - string`, `B - bool`) from standard input and stores it on the stack                                 |

</details>

# Implementation

1. **Lexer and parser** - The lexer and parser are made with ANTLR after you define your grammar, which is done in the [FL.g4](Project/FL.g4) file. Using it, appropriate classes are generated into the project and then used in [Program.cs](Project/Program.cs) for checking the syntax errors. It's trying to parse input which you can specify in [Program.cs](Project/Program.cs).
2. **Type checking** - Type checking is done in the [EvalVisitor](Project/EvalVisitor.cs) class, using the parse tree generated by the parser and checking if the types of operands in the expressions are correct. It also checks for any bad assignment to a variable. [SymbolTable](Project/SymbolTable.cs) is used for storing information about variables and checking if they are already declared or not declared at all.
3. **Making instructions** - Making instructions is done in the [EvalCompute.cs](Project/EvalCompute.cs) class. It takes a parse tree, which was type-checked, and generates stack-based instructions that are executed by an virtual machine. 
4. **Virtual machine** - The [Virtual machine](Project/VirtualMachine.cs) reads generated instructions and executes them using a stack and dictionary which acts as a memory.

# Example
| Code written | Instructions | Output in terminal |
|---|---|---|
| float a;<br>a=1+2*3.3;<br>write a;<br><br>if(4>5)<br>{<br>    write "4>5";<br>}<br>else<br>{<br>    write "4<5";<br>}<br><br><br>int b;<br>while(b<5)<br>{<br>    write "b=", b;<br>    b=b+1;<br>} | push F 0.0<br>save a<br>push I 1<br>itof<br>push I 2<br>itof<br>push F 3.3<br>mul F<br>add I<br>save a<br>load a<br>pop<br>load a<br>print 1<br>push I 4<br>push I 5<br>gt<br>fjmp 8<br>push S "4>5"<br>print 1<br>jmp 9<br>label 8<br>push S "4<5"<br>print 1<br>label 9<br>push I 0<br>save b<br>label 10<br>load b<br>push I 5<br>lt<br>fjmp 11<br>push S "b="<br>load b<br>print 2<br>load b<br>push I 1<br>add <br>save b<br>load b<br>pop<br>jmp 10<br>label 11 | [Info] \| Parsing: Input_files/input.txt<br>[Info] \| No syntax and type-check errors found.<br>[Info] \| Generating instructions...<br>[Info] \| Instructions generated.<br>[Info] \| Running virtual machine...<br>7.6<br>4<5<br>b=  0<br>b=  1<br>b=  2<br>b=  3<br>b=  4<br>[Info] \| Virtual machine finished. Exiting... |
