; program przyk³adowy (wersja 32-bitowa) 
.686 
.model flat 
extern   _ExitProcess@4  : PROC 
extern   __write         : PROC   ; (dwa znaki podkreœlenia) 

public  _main 
.data 
tekst     db   10, 'Nazywam si', 0A9H, ' Mariusz' , 10 
          db   'M', 0A2h, 'j pierwszy 32-bitowy program ' 
          db   'asemblerowy dzia', 88h, 'a ju', 0BEh, ' poprawnie!', 10 

.code 
_main: 
     mov       ecx, 86   ; liczba znaków wyœwietlanego tekstu 
						; wywo³anie funkcji ”write” z biblioteki jêzyka C 
     push      ecx      ; liczba znaków wyœwietlanego tekstu 
     push      dword PTR OFFSET tekst   ; po³o¿enie obszaru 
                                        ; ze znakami 
     push      dword PTR 1   ; uchwyt urz¹dzenia wyjœciowego 
     call      __write        ; wyœwietlenie znaków  
                              ; (dwa znaki podkreœlenia _ ) 
     add       esp, 12        ; usuniêcie parametrów ze stosu 
								; zakoñczenie wykonywania programu 
     push      dword PTR 0     ; kod powrotu programu  
     call      _ExitProcess@4   
END 