//FCTRL3 - Dwie cyfry silni

#include <iostream>
using std::cin;
using std::cout;

int silnia(int);

int main()
{
	int testy, liczba, dziesiatki, jednosci;
	cin >> testy;
	while (testy)
	{
		cin >> liczba;
		liczba = silnia(liczba);
		jednosci = liczba % 10;
		dziesiatki = (liczba / 10) % 10;
		cout << dziesiatki << " " << jednosci << "\n";
		testy--;
	}
}

int silnia(int n)
{
	if (n > 9)
		return 0;
	int wynik = 1;
	for (int i = 1; i <= n; ++i)
		wynik = wynik * i;

	return wynik;
};