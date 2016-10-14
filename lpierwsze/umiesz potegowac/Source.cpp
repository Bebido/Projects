#include <iostream>

int potega_ostatnia_cyfra(int, int);

int main()
{
	int testy, podstawa, wykladnik, wynik;
	std::cin >> testy;
	while (testy)
	{
		std::cin >> podstawa;
		std::cin >> wykladnik;
		wynik = potega_ostatnia_cyfra(podstawa, wykladnik);
		std::cout << wynik << "\n";
		--testy;
	}
	return 0;
}

int potega_ostatnia_cyfra(int podstawa, int wykladnik) 
{
	int wynik = 1;
	podstawa = podstawa % 10; // wazna tylko ostatnia cyfra
	int wykladnik_modulo = (wykladnik % 4) + 4;
	for (int i = 0; i < wykladnik_modulo; ++i)
	{
		wynik = wynik * podstawa;
		if (wynik > 1000)
		{
			wynik = wynik % 1000;
		}

	}
	wynik = wynik % 10;
	return wynik;
};