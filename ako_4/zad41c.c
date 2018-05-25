#include <stdio.h>
int * kopia_tablicy(int tabl[], unsigned int n);


int main()
{
	int tab[6] = {5, 1, 0, 4, 12, 0};
	int rozmiar = 6;
	int *adres;
	adres = kopia_tablicy(tab, rozmiar);
	
	return 0;
}