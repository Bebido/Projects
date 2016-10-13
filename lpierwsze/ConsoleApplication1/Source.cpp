//FCTRL3 - Dwie cyfry silni

// ^^ nie lepiej w nazwie projektu? :)

#include <iostream>
using std::cin;     // moze byc, male ryzyko ze pozniej to nie bedzie dzialac jak sie zmieni wersja c++, jednak raczej probuj sie przestawic na pisanie std:: przy kazdym uzyciu
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
		testy--;        // ta instrukcja zrobi kopie zmiennej testy, na kopii wykona operacje odejmowania, a potem kopie przypisze do oryginalu
                        // jakbys zrobil --testy to byla by tylko operacja odejmowania na oryginale
	}
}

int silnia(int n)
{
	if (n > 9)
		return 0;
	int wynik = 1;
	for (int i = 1; i <= n; ++i)    // tu za to zajebiscie ++i
		wynik = wynik * i;          // wynik *= i, ale w sumie jak kto lubi

	return wynik;
};