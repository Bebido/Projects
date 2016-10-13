#include <iostream>
using std::cin;
using std::cout;

int potega_ostatnia_cyfra(int, int);

int main()
{
	int testy, podstawa, wykladnik, wynik;
	cin >> testy;
	while (testy)
	{
		cin >> podstawa;
		cin >> wykladnik;
		wynik = potega_ostatnia_cyfra(podstawa, wykladnik);
		cout << wynik << "\n";
		testy--;
	}
	return 0;
}

int potega_ostatnia_cyfra(int podstawa, int wykladnik)
{
	int wynik = 1;
	for (int i = 0; i < wykladnik; i++)
	{
		wynik = wynik * podstawa;
		if (wynik > 1000)
			wynik = wynik % 1000;

	}
	wynik = wynik % 10;
	return wynik;
};