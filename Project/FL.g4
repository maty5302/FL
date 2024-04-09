grammar FL;

/** The start rule; begin parsing here. */
prog: statement*EOF;


expr:'-' expr #unaryMinus
    |'!' expr #not
    | expr op=('*'|'/'|'%') expr #mulDivMod  
    | expr op=('+'|'-'|'.') expr #addSub
    | expr op=('<'|'>') expr #ltGt
    | expr op=('=='|'!=') expr #eqNe
    | expr '&&' expr #and
    | expr '||' expr #or
    | expr '?' expr ':' expr  #ternary
    | <assoc=right> ID '=' expr #assign
    | ID   #id
    | INT  #int
    | FLOAT #float   
    | BOOLEAN #bool
    | STRING #string
    | '(' expr ')' #parens
    ;


type : 'int' | 'string' | 'float' | 'bool' ;

statement
    : type ID (',' ID)* ';' #declaration
    | expr ';' #printExpr
    | 'read' ID (',' ID)* ';' #read
    | 'write' expr (',' expr)* ';' #write
    | '{' statement+ '}' #block
    | 'if' '(' expr ')' statement ('else' statement)? #if
    | 'do' statement 'while' '(' expr ')' ';' #doWhile
    | 'while' '(' expr ')' statement #while
    | ';' #empty
    ;


AND: '&&';
OR: '||';
CONCAT: '.';
EQ: '==';
NE: '!=';
LT: '<' ;
GT: '>' ;
ADD: '+' ;
SUB: '-' ;
MUL: '*' ;
DIV: '/' ;
MOD: '%' ;
INT : [1-9][0-9]* | [0] ;          // match integers
FLOAT : [0-9]+[.][0-9]* ;
BOOLEAN : 'true' | 'false' ;
STRING : '"' SChar* '"' ;
ID : [a-zA-Z] [a-zA-Z0-9]*;
WS : [ \t\r\n]+ -> skip ;   // toss out whitespace

fragment SChar
    : ~["\\\r\n]
    | '\\' ['"?abfnrtv\\]
    | '\\\n'   // Added line
    | '\\\r\n' // Added line
    ;

LineComment
    : '//' ~[\r\n]* -> channel(HIDDEN)
    ;