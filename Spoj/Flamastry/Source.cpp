#include <iostream> //do zrobienia prog
#include <stdio.h>

void licz_znaki(char *tab, int rozmiar);

int main()
{
	int testy;
	std::cin >> testy;
	char znak, poprz_znak = 'a', napis[200] = "0";
	int rozmiar;
	znak = getchar(); // zebranie pierwszego enteru
	
	while (testy)
	{
		for (int i = 0; i < 200; ++i)
		{
			znak = getchar();
			napis[i] = znak;
			if (znak == '\n')
			{
				rozmiar = i;
				break;
			}
		}
		licz_znaki(napis, rozmiar);
		/*for (int i = 0; i < rozmiar; ++i)
		{
			std::cout << napis[i];
		}
		*/
		std::cout << std::endl;
		--testy;
	}

}

void licz_znaki(char *tab, int rozmiar)
{

}