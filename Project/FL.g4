grammar FL;

/** The start rule; begin parsing here. */
prog: statement*EOF;


expr:'-' expr
    |'!' expr
    | expr ('*'|'/'|'%') expr  
    | expr ('+'|'-'|'.') expr  
    | expr ('<'|'>') expr
    | expr ('=='|'!=') expr
    | expr '&&' expr
    | expr '||' expr
    | ID '=' expr
    | ID
    | INT  
    | FLOAT    
    | BOOLEAN
    | STRING
    | '(' expr ')' 
    ;


INT : [1-9][0-9]* | [0] ;          // match integers
FLOAT : [0-9]+[.][0-9]* ;
BOOLEAN : 'true' | 'false' ;
STRING : '"' SChar* '"' ;
ID : [a-zA-Z] [a-zA-Z0-9]*;
WS : [ \t\r\n]+ -> skip ;   // toss out whitespace

type : 'int' | 'string' | 'float' | 'bool' ;

statement
    : type ID (',' ID)* ';'
    | expr ';'
    | 'read' ID (',' ID)* ';'
    | 'write' expr (',' expr)* ';'
    | '{' statement+ '}'
    | 'if' '(' expr ')' statement ('else' statement)?
    | 'while' '(' expr ')' statement
    | ';'
    ;

fragment SChar
    : ~["\\\r\n]
    | '\\' ['"?abfnrtv\\]
    | '\\\n'   // Added line
    | '\\\r\n' // Added line
    ;

LineComment
    : '//' ~[\r\n]* -> channel(HIDDEN)
    ;