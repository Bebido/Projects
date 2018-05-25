; Program gwiazdki.asm
; Wywietlanie znaków * w takt przerwa zegarowych
; Uruchomienie w trybie rzeczywistym procesora x86
; lub na maszynie wirtualnej
; zakoczenie programu po naciniciu klawisza 'x'
; asemblacja (MASM 4.0): masm gwiazdki.asm,,,;
; konsolidacja (LINK 3.60): link gwiazdki.obj;

.386

rozkazy SEGMENT use16
		ASSUME CS:rozkazy


;============================================================
; procedura obs³ugi przerwania zegarowego

	obsluga_zegara PROC

; przechowanie uywanych rejestrów
	push ax
	push bx
	push es

; wpisanie adresu pamici ekranu do rejestru ES - pamiec
; ekranu dla trybu tekstowego zaczyna si od adresu B8000H,
; jednak do rejestru ES wpisujemy wartosc B800H,
; bo w trakcie obliczenia adresu procesor kadorazowo mnozy
; zawarto rejestru ES przez 16
	mov ax, 0B800h ;adres pamici ekranu
	mov es, ax

; zmienna 'licznik' zawiera adres biezacy w pamieci ekranu
	mov bx, cs:licznik

; przes³anie do pamici ekranu kodu ASCII wywietlanego znaku
; i kodu koloru: bia³y na czarnym tle (do nastpnego bajtu)
	mov byte PTR es:[bx], '*' ; kod ASCII
	mov byte PTR es:[bx+1], 00000110B ; kolor

; zwikszenie o 2 adresu biecego w pamici ekranu
	add bx,2

; sprawdzenie czy adres biecy osign³ koniec pamici ekranu
	cmp bx,4000
	jb wysw_dalej ; skok gdy nie koniec ekranu

; wyzerowanie adresu biecego, gdy ca³y ekran zapisany
	mov bx, 0

;zapisanie adresu biecego do zmiennej 'licznik'

wysw_dalej:
	mov cs:licznik,bx


; odtworzenie rejestrów
	pop es
	pop bx
	pop ax

; skok do oryginalnej procedury obs³ugi przerwania zegarowego
	jmp dword PTR cs:wektor8

; dane programu ze wzgldu na specyfik obs³ugi przerwa
; umieszczone s w segmencie kodu
	licznik dw 320 ; wywietlanie poczwszy od 2. wiersza
	wektor8 dd ?

obsluga_zegara ENDP

;============================================================
; program g³ówny - instalacja i deinstalacja procedury
; obs³ugi przerwa
; ustalenie strony nr 0 dla trybu tekstowego
zacznij:
	mov al, 0
	mov ah, 5
	int 10

	mov ax, 0
	mov ds,ax ; zerowanie rejestru DS

; odczytanie zawartoci wektora nr 8 i zapisanie go
; w zmiennej 'wektor8' (wektor nr 8 zajmuje w pamici 4 bajty
; poczwszy od adresu fizycznego 8 * 4 = 32)
	mov eax,ds:[32] ; adres fizyczny 0*16 + 32 = 32
	mov cs:wektor8, eax

; wpisanie do wektora nr 8 adresu procedury 'obsluga_zegara'
	mov ax, SEG obsluga_zegara ; cz segmentowa adresu
	mov bx, OFFSET obsluga_zegara ; offset adresu

	cli ; zablokowanie przerwan

; zapisanie adresu procedury do wektora nr 8
	mov ds:[32], bx ; OFFSET
	mov ds:[34], ax ; cz. segmentowa
	sti ;odblokowanie przerwa

; oczekiwanie na nacinicie klawisza 'x'

aktywne_oczekiwanie:

	mov ah,1
	int 16H

; funkcja INT 16H (AH=1) BIOSu ustawia ZF=1 jeli
; nacisneto jaki klawisz
	jz aktywne_oczekiwanie

; odczytanie kodu ASCII nacinitego klawisza (INT 16H, AH=0)
; do rejestru AL
	mov ah, 0
	int 16H
	cmp al, 'x' ; porównanie z kodem litery 'x'

	jne aktywne_oczekiwanie ; skok, gdy inny znak

; deinstalacja procedury obs³ugi przerwania zegarowego
; odtworzenie oryginalnej zawartoci wektora nr 8
	mov eax, cs:wektor8
	cli
	mov ds:[32], eax	; przes³anie wartoci oryginalnej
						; do wektora 8 w tablicy wektorów
						; przerwan
	sti

; zakoczenie programu
	mov al, 0
	mov ah, 4CH
	int 21H
rozkazy ENDS

nasz_stos SEGMENT stack
	db 128 dup (?)
nasz_stos ENDS

END zacznij