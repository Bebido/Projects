.686
.model flat

public _kopia_tablicy
extern _malloc : PROC

.code

_kopia_tablicy PROC

	push ebp;
	mov ebp, esp;
	push edi

	mov edi, 81
	push edi
	add esp, 4
	fild dword ptr [esp - 4]

	mul ecx

	pop edi
	pop ebp
	ret

_kopia_tablicy ENDP
END