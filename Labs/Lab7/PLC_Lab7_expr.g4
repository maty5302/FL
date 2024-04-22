grammar PLC_Lab7_expr;

/** The start rule; begin parsing here. */
prog: (expr ';')+;

expr: expr ('*'|'/') expr  
    | expr ('+'|'-') expr  
    | INT                  
    | OCT
    | HEXA
    | '(' expr ')' 
    ;

INT : [0-9]+ ;          // match integers
OCT : '0'[0-7]* ;
HEXA : '0x'[0-9a-fA-F]+ ;
WS : [ \t\r\n]+ -> skip ;   // toss out whitespace