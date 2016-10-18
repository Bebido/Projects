; wczytywanie i wywietlanie tekstu wielkimi literami
; (inne znaki si nie zmieniaj)

.686
.model flat

extern _ExitProcess@4 : PROC
extern __write : PROC ; (dwa znaki podkrelenia)
extern __read : PROC ; (dwa znaki podkrelenia)

public _main
.data
	tekst_pocz	 db		10, 'Proszê napisac jakiœ tekst'
				 db		'i nacisn¹æ Enter', 10
	koniec_t	 db		?
	magazyn		 db	80 dup (?)
	nowa_linia	 db		10
	liczba_znakow dd ?
.code
_main:
	mov eax, 2
	mov esi,[eax]
		; wywietlenie tekstu informacyjnego
		; liczba znaków tekstu
	mov ecx, (OFFSET koniec_t) - (OFFSET tekst_pocz)
	push ecx

	push OFFSET tekst_pocz ; adres tekstu
	push 1 ; nr urzdzenia (tu: ekran - nr 1)
	call __write ; wywietlenie tekstu pocztkowego
	add esp, 12 ; usuniecie parametrów ze stosu
			; czytanie wiersza z klawiatury

	push 80 ; maksymalna liczba znaków
	push OFFSET magazyn
	push 0 ; nr urzdzenia (tu: klawiatura - nr 0)
	call __read ; czytanie znaków z klawiatury
	add esp, 12 ; usuniecie parametrów ze stosu
			; kody ASCII napisanego tekstu zosta³y wprowadzone
			; do obszaru 'magazyn'
			; funkcja read wpisuje do rejestru EAX liczbê
			; wprowadzonych znaków

	mov liczba_znakow, eax
			; rejestr ECX pe³ni rol licznika obiegów ptli
	mov ecx, eax
	mov ebx, 0 ; indeks pocztkowy

ptl: mov dl, magazyn[ebx] ; pobranie kolejnego znaku
	
	cmp dl, 0A5h; ¹
	je dalej_minus_1

	cmp dl, 86h ; æ
	je dalej_c

	cmp dl, 0A9h; ê
	je dalej_minus_1

	cmp dl, 88h ; '³'
	je dalej_l

	cmp dl, 0E4h ; ñ
	je dalej_minus_1

	cmp dl, 0A2h; ó
	je dalej_o

	cmp dl, 98h; œ
	je dalej_minus_1

	cmp dl, 0ABh ; Ÿ
	je dalej_z

	cmp dl, 0BEh; ¿
	je dalej_minus_1

	cmp dl, 'a'
	jb dalej ; skok, gdy znak nie wymaga zamiany

	cmp dl, 'z'
	ja dalej ; skok, gdy znak nie wymaga zamiany

	sub dl, 20H ; zamiana na wielkie litery
	jmp dalej

dalej_minus_1:
	dec dl ; zamiana na wielkie litery
	jmp dalej

dalej_c:
	mov dl, 8Fh
	jmp dalej

dalej_l:
	mov dl, 9Dh
	jmp dalej

dalej_o:
	mov dl, 0E0h
	jmp dalej

dalej_z:
	mov dl, 8Dh


dalej: 
	mov magazyn[ebx], dl ; odes³anie znaku do pamici
	inc ebx ; inkrementacja indeksu
	loop ptl ; sterowanie ptl
			; wywietlenie przekszta³conego tekstu
	push liczba_znakow
	push OFFSET magazyn
	push 1
	call __write ; wywietlenie przekszta³conegotekstu
	add esp, 12 ; usuniecie parametrów ze stosu
	push 0
	call _ExitProcess@4 ; zakoczenie programu
END