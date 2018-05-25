.686
.model flat

extern __write : PROC
extern _ExitProcess@4 : PROC
extern __read : PROC

public _main

.data
znaki db 12 dup (?)
obszar db 12 dup (?) 
dziesiec dd 10   ; mno¿nik 
dekoder db '0123456789ABCDE'

.code

wyswietl_EAX   PROC 
               pusha 
			    
     mov       esi, 10        ; indeks w tablicy 'znaki' 
     mov       ebx, 10        ; dzielnik równy 10 
konwersja: 
     mov       edx, 0   ; zerowanie starszej czêœci dzielnej 
     div       ebx      ; dzielenie przez 10, reszta w EDX, 
						; iloraz w EAX 
     add       dl, 30H  ; zamiana reszty z dzielenia na kod 
						; ASCII 
     mov       znaki [esi], dl; zapisanie cyfry w kodzie ASCII 
     dec       esi            ; zmniejszenie indeksu 
     cmp       eax, 0         ; sprawdzenie czy iloraz = 0 
     jne       konwersja      ; skok, gdy iloraz niezerowy 
						  ; wype³nienie pozosta³ych bajtów spacjami i wpisanie 
						  ; znaków nowego wiersza 
wypeln: 
     or        esi, esi 
     jz        wyswietl       ; skok, gdy ESI = 0 
     mov       byte PTR znaki [esi], 20H ; kod spacji 
     dec       esi            ; zmniejszenie indeksu 
     jmp       wypeln

wyswietl: 
     mov       byte PTR znaki [0], 0AH ; kod nowego wiersza 
     mov       byte PTR znaki [11], 0AH ; kod nowego wiersza
; wyœwietlenie cyfr na ekranie 
     push      dword PTR 12   ; liczba wyœwietlanych znaków 
     push      dword PTR OFFSET znaki   ; adres wyœw. obszaru 
     push      dword PTR 1; numer urz¹dzenia (ekran ma numer 1) 
     call      __write        ; wyœwietlenie liczby na ekranie 
     add       esp, 12        ; usuniêcie parametrów ze stosu 
			    
     popa
	 ret 
wyswietl_EAX   ENDP 

;///////////////////////////////////////////

wczytaj_do_EAX	PROC
	push ebx
	push ecx

; wczytywanie liczby dziesiêtnej z klawiatury – po 
; wprowadzeniu cyfr nale¿y nacisn¹æ klawisz Enter 

; liczba po konwersji na postaæ binarn¹ zostaje wpisana 
; do rejestru EAX 

; deklaracja tablicy do przechowywania wprowadzanych cyfr 
; (w obszarze danych) 

     push      dword PTR 12		; max iloœæ znaków wczytywanej liczby 
     push      dword PTR OFFSET obszar  ; adres obszaru pamiêci 
     push      dword PTR 0	; numer urz¹dzenia (0 dla klawiatury) 
     call      __read    ; odczytywanie znaków z klawiatury 
                    ; (dwa znaki podkreœlenia przed read) 
     add       esp, 12   ; usuniêcie parametrów ze stosu 

; bie¿¹ca wartoœæ przekszta³canej liczby przechowywana jest 
; w rejestrze EAX; przyjmujemy 0 jako wartoœæ pocz¹tkow¹

     mov eax, 0          
     mov ebx, OFFSET obszar  ; adres obszaru ze znakami

pobieraj_znaki: 
     mov cl, [ebx]	; pobranie kolejnej cyfry w kodzie 
					; ASCII 
     inc ebx		; zwiêkszenie indeksu 
     cmp       cl,10     ; sprawdzenie czy naciœniêto Enter 
     je        byl_enter ; skok, gdy naciœniêto Enter 
     sub       cl, 30H   ; zamiana kodu ASCII na wartoœæ cyfry 
     movzx     ecx, cl   ; przechowanie wartoœci cyfry w 
						 ; rejestrze ECX 

     ; mno¿enie wczeœniej obliczonej wartoœci razy 10 
     mul       dword PTR dziesiec             
     add       eax, ecx  ; dodanie ostatnio odczytanej cyfry 
     jmp       pobieraj_znaki ; skok na pocz¹tek pêtli 
byl_enter: 
; wartoœæ binarna wprowadzonej liczby znajduje siê teraz w rejestrze EAX 

	pop ecx
	pop ebx
	ret
wczytaj_do_EAX ENDP

;///////////////////////////////////////////

wyswietl_EAX_hex    PROC 
; wyœwietlanie zawartoœci rejestru EAX 
; w postaci liczby szesnastkowej 

     pusha          ; przechowanie rejestrów 

; rezerwacja 12 bajtów na stosie (poprzez zmniejszenie 
; rejestru ESP) przeznaczonych na tymczasowe przechowanie 
; cyfr szesnastkowych wyœwietlanej liczby 

     sub       esp, 12 
     mov       edi, esp  ; adres zarezerwowanego obszaru 
                         ; pamiêci 
; przygotowanie konwersji            
     mov       ecx, 8    ; liczba obiegów pêtli konwersji 
     mov       esi, 1    ; indeks pocz¹tkowy u¿ywany przy 
                         ; zapisie cyfr 

; pêtla konwersji 
ptl3hex:    
; przesuniêcie cykliczne (obrót) rejestru EAX o 4 bity w lewo 
; w szczególnoœci, w pierwszym obiegu pêtli bity nr 31 - 28 
; rejestru EAX zostan¹ przesuniête na pozycje 3 - 0 
     rol       eax, 4               
; wyodrêbnienie 4 najm³odszych bitów i odczytanie z tablicy
; 'dekoder' odpowiadaj¹cej im cyfry w zapisie szesnastkowym 
     mov       ebx, eax  ; kopiowanie EAX do EBX 
     and       ebx, 0000000FH ; zerowanie bitów 31 - 4  rej.EBX 
     mov       dl, dekoder[ebx] ; pobranie cyfry z tablicy  

; przes³anie cyfry do obszaru roboczego 
     mov       [edi][esi], dl  
     inc       esi       ;inkrementacja modyfikatora 
     loop      ptl3hex   ; sterowanie pêtl¹

; wpisanie znaku nowego wiersza przed i po cyfrach 
     mov       byte PTR [edi][0], 10 
     mov       byte PTR [edi][9], 10 


	 mov esi, 1;
ptl_zera_na_spacje:
	mov bl, byte PTR [edi][esi]
	cmp bl, '0';
	jne koniec
	mov bl, 20h
	mov [edi][esi], bl
	inc esi
	jmp ptl_zera_na_spacje

koniec:
; wyœwietlenie przygotowanych cyfr 
     push      10   ; 8 cyfr + 2 znaki nowego wiersza 
     push      edi  ; adres obszaru roboczego 
     push      1    ; nr urz¹dzenia (tu: ekran) 
     call      __write   ; wyœwietlenie 
; usuniêcie ze stosu 24 bajtów, w tym 12 bajtów zapisanych 
; przez 3 rozkazy push przed rozkazem call 
; i 12 bajtów zarezerwowanych na pocz¹tku podprogramu 

     add       esp, 24              
     popa      ; odtworzenie rejestrów 
     ret       ; powrót z podprogramu 
wyswietl_EAX_hex    ENDP 

;///////////////////////////////////////////

wczytaj_do_EAX_hex  PROC 
; wczytywanie liczby szesnastkowej z klawiatury – liczba po 
; konwersji na postaæ binarn¹ zostaje wpisana do rejestru EAX 
; po wprowadzeniu ostatniej cyfry nale¿y nacisn¹æ klawisz 
; Enter 
     push      ebx 
     push      ecx 
     push      edx 
     push      esi 
     push      edi 
     push      ebp 
; rezerwacja 12 bajtów na stosie przeznaczonych na tymczasowe 
; przechowanie cyfr szesnastkowych wyœwietlanej liczby 
     sub       esp, 12   ; rezerwacja poprzez zmniejszenie ESP
     mov       esi, esp ; adres zarezerwowanego obszaru pamiêci 
     push      dword PTR 10 ; max iloœæ znaków wczytyw. liczby 
     push      esi       ; adres obszaru pamiêci 
     push      dword PTR 0; numer urz¹dzenia (0 dla klawiatury) 
     call      __read    ; odczytywanie znaków z klawiatury 
                         ; (dwa znaki podkreœlenia przed read) 
     add       esp, 12   ; usuniêcie parametrów ze stosu 
     mov       eax, 0    ; dotychczas uzyskany wynik 

pocz_konw: 
     mov       dl, [esi] ; pobranie kolejnego bajtu 
     inc       esi       ; inkrementacja indeksu 
     cmp       dl, 10    ; sprawdzenie czy naciœniêto Enter 
     je        gotowe    ; skok do koñca podprogramu 
; sprawdzenie czy wprowadzony znak jest cyfr¹ 0, 1, 2 , ..., 9 
     cmp       dl, '0' 
     jb        pocz_konw ; inny znak jest ignorowany 
     cmp       dl, '9' 
     ja        sprawdzaj_dalej 
     sub       dl, '0'   ; zamiana kodu ASCII na wartoœæ cyfry 
dopisz: 
     shl       eax, 4 ; przesuniêcie logiczne w lewo o 4 bity 
     or        al, dl ; dopisanie utworzonego kodu 4-bitowego
                      ; na 4 ostatnie bity rejestru EAX 
     jmp       pocz_konw ; skok na pocz¹tek pêtli konwersji 
; sprawdzenie czy wprowadzony znak jest cyfr¹ A, B, ..., F 
sprawdzaj_dalej: 
     cmp       dl, 'A' 
     jb        pocz_konw      ; inny znak jest ignorowany 
     cmp       dl, 'F' 
     ja        sprawdzaj_dalej2 
     sub       dl, 'A' - 10   ; wyznaczenie kodu binarnego 
     jmp       dopisz 

; sprawdzenie czy wprowadzony znak jest cyfr¹ a, b, ..., f 
sprawdzaj_dalej2: 
     cmp       dl, 'a' 
     jb        pocz_konw   ; inny znak jest ignorowany 
     cmp       dl, 'f' 
     ja        pocz_konw   ; inny znak jest ignorowany 
     sub       dl, 'a' - 10 
     jmp       dopisz 
gotowe: 
; zwolnienie zarezerwowanego obszaru pamiêci 
     add       esp, 12 
     pop       ebp 
     pop       edi 
     pop       esi 
     pop       edx 
     pop       ecx 
     pop       ebx 
     ret 
wczytaj_do_EAX_hex  ENDP 

;///////////////////////////////////////////

_main:

push eax
pop eax
	
	call wczytaj_do_EAX_hex
	call wyswietl_EAX

	push 0
	call _ExitProcess@4

END