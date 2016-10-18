; wczytywanie i wyswietlanie tekstu wielkimi literami
; (inne znaki sie nie zmieniaja)

.686
.model flat

extern _ExitProcess@4 : PROC
extern _MessageBoxA@16 : PROC
extern _MessageBoxW@16 : PROC
extern __write : PROC ; (dwa znaki podkreslenia)
extern __read : PROC ; (dwa znaki podkreslenia)

public _main

.data

tytul			db 'MessageBoxA', 0
tytul_w			db 'M', 0, 'B', 0, 'W', 0, 0, 0
tekst_pocz		db 'Prosze napisac jakis tekst '
				db 'i nacisnac Enter', 10
koniec_t		db ?
magazyn			db 80 dup (?)
magazynW		db 160 dup (?)
nowa_linia		db 10
liczba_znakow	dd ?

.code

_main:

	; wyswietlenie tekstu informacyjnego

	mov ecx, (OFFSET koniec_t) - (OFFSET tekst_pocz) ; liczba znaków tekstu
	push ecx
	push OFFSET tekst_pocz ; adres tekstu
	push 1 ; nr urzadzenia (tu: ekran - nr 1)
	call __write ; wyswietlenie tekstu poczatkowego

	add esp, 12 ; usuniecie parametrów ze stosu

	; czytanie wiersza z klawiatury
	push 80 ; maksymalna liczba znaków
	push OFFSET magazyn
	push 0 ; nr urzadzenia (tu: klawiatura - nr 0)
	call __read ; czytanie znaków z klawiatury
	add esp, 12 ; usuniecie parametrów ze stosu

	; kody ASCII napisanego tekstu zosta³y wprowadzone
	; do obszaru 'magazyn'
	; funkcja read wpisuje do rejestru EAX liczbe
	; wprowadzonych znaków

	mov liczba_znakow, eax
	push edx
	; rejestr ECX pe³ni role licznika obiegów petli
	mov ecx, eax
	mov ebx, 0 ; indeks poczatkowy
	mov eax, 0 ; 
ptl:
	mov dl, magazyn[ebx] ; pobranie kolejnego znaku
	cmp dl, 0A5h; ¹
	je dalej_a

	cmp dl, 86h ; æ
	je dalej_c

	cmp dl, 0A9h; ê
	je dalej_e

	cmp dl, 88h ; '³'
	je dalej_l

	cmp dl, 0E4h ; ñ
	je dalej_n

	cmp dl, 0A2h; ó
	je dalej_o

	cmp dl, 98h; œ
	je dalej_s

	cmp dl, 0ABh ; Ÿ
	je dalej_z

	cmp dl, 0BEh; ¿
	je dalej_z2

	cmp dl, 'a'
	jb dalej ; skok, gdy znak nie wymaga zamiany
	cmp dl, 'z'
	ja dalej ; skok, gdy znak nie wymaga zamiany

	sub dl, 20H ; zamiana na wielkie litery
	mov magazynW[eax], dl ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], byte PTR 0 ; odeslanie znaku do pamieci

	jmp dalej

dalej_c:
	mov dl, 0C6h
	mov magazynW[eax], 06h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_l:
	mov dl, 0A3h
	mov magazynW[eax], 41h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_o:
	mov dl, 0D3h
	mov magazynW[eax], 0D3h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 00h ; odeslanie znaku do pamieci
	jmp dalej

dalej_z:
	mov dl, 8Fh
	mov magazynW[eax], 79h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_a:
	mov dl, 0A5h;
	mov magazynW[eax], 04h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_e:
	mov dl, 0CAh;
	mov magazynW[eax], 18h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_n:
	mov dl, 0D1h;
	mov magazynW[eax], 43h ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_s:
	mov dl, 8Ch;
	mov magazynW[eax], 5Ah ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej_z2:
	mov dl, 0AFh;
	mov magazynW[eax], 7Bh ; odeslanie znaku do pamieci
	mov magazynW[eax + 1], 01h ; odeslanie znaku do pamieci
	jmp dalej

dalej: 
	mov magazyn[ebx], dl ; odeslanie znaku do pamieci
	inc ebx
	mov eax, ebx
	add eax, eax

	dec ecx
	cmp ecx, 0
	jnz ptl ; sterowanie petla

	; wyswietlenie przekszta³conego tekstu
	push 0
	push OFFSET tytul
	push OFFSET magazyn
	push 0
	call _MessageBoxA@16 ; wyswietlenie przekszta³conego tekstu

	push 0
	push OFFSET tytul_w
	push OFFSET magazynW
	push 0
	call _MessageBoxW@16 ; wyswietlenie przekszta³conego tekstu

	pop edx

	push 0
	call _ExitProcess@4 ; zakonczenie programu
END