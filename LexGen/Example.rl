ident='[A-Z_a-z][0-9A-Z_a-z]*'
int='0|-?[1-9][0-9]*'
float='(0|-?[1-9][0-9]*)(\.[0-9]+([Ee]-?[1-9][0-9]*)?)?'
space='[\r\n\t\v\f ]'
op='[\-\+\\\*%=]'
lineComment= '\/\/[^\n]*'
blockComment<blockEnd="*/">= "/*"