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
	int wykladnik_modulo = wykladnik % 5;
	bool petla = false;
	for (int i = 0; i < wykladnik_modulo; ++i)
	{
		petla = true;
		wynik = wynik * podstawa;
		if (wynik > 1000)
		{
			wynik = wynik % 1000;
		}

	}
	if (petla == false)
	{
		wynik = podstawa;
	}
	wynik = wynik % 10;
	return wynik;
};