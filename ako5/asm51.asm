; Program przyk³adowy ilustrujcy operacje AVX 2.0 procesora
; Poniszy podprogram jest przystosowany do wywo³ywania
; z poziomu jzyka C++ (program arytmc_AVX.cpp) w trybie 64
; bitowym

public FMA

	; podprogram oblicza matX=scalarA*matX + matY, tzw. AXPY
	; ostatni paramter count informuje o dlugosci wektora
	; (float * matX, float * matY, float scalarA, int count);
	; rcx				 rdx			xmm2		r9
	; matA
	; call -> rbp+8
	; rbp -> rbp
.code

FMA:
	;prolog i zapamitanie rejestrów
	push rbp
	mov rbp,rsp
	push rbx
	push rsi
	push rdi

	mov rsi,rcx ; utwórz kopi adresu macierzy A
	mov rdi,rdx ; utwórz kopi adresu macierzy B

	; wyznaczenie liczby powtórze ecx<- count/32
	; d³ugo wektora musi by wielokrotnoci liczby 32
	mov rdx,0
	mov rbx,32
	mov rax,r9
	div rbx
	xchg rdx,rax
	cmp rax,0
	jne koniec
	mov rcx,rdx ; w rcx ilosc wykonan
	; w³aciwa ptla obliczeniowa
	again:
	; xmm2 do pamici (czyli mnonik scalarA)
	vmovups XMMWORD PTR dana32, xmm2
	; przeniesienie wartosci scalarA do wszystkich 8 czci ymm2
	vbroadcastss ymm2,dana32
	; w rejestrze ymm2 jest 8 razy scalarA
	; za³adowanie 8 kolejnych elementów macierzy matA do ymm0
	vmovaps ymm0,YMMWORD PTR [rsi]
	; za³adowanie 8 kolejnych elementów macierz matB do ymm1
	vmovaps ymm1, YMMWORD PTR [rdi]
	; rozkaz mnoenia typu FMA ymm0 <- ymm0 * ymm2 + ymm1
	VFMADD132PS ymm0,ymm1,ymm2 ; ymmA <- ymmA * ymmC + ymmB
	; czyli wykonano fa[k] = a * fa[k] + fb[k];
	; zapis wyniku do macierzy matA
	vmovaps YMMWORD PTR [rsi],ymm0
	;aktualizacja wskazników
	add rsi,8*4
	add rdi,8*4
	loop again
	koniec:
	pop rdi
	pop rsi
	pop rbx
	pop rbp
	ret

.data

	dana32 dd 4 dup (?) ; miejsce na parametr scalarA

END