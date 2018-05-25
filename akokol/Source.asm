.686
.model flat

extern _ExitProcess@4 : PROC
public _main
.data

liczba dd 11223344h

.code

_main:

mov ax, 10

start:
	mov ecx, 3
	sub ax, 10
	loop start


push 0
call _ExitProcess@4

END