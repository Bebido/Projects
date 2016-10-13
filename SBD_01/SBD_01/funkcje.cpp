#include "Header.hpp"
#include <cstdio>

using std::cin;
using std::cout;


void wybor()
{
	char wybor;
	cout << "Wybierz metoda wprowadzania:\n m - manualnie\n l - losowo\n f - z pliku\n";
	wybor = getchar();

	switch (wybor)
	{
	case 'l':
		cout << "los\n";
		break;
	case 'm':
		cout << "manual\n";
		break;
	case 'f':
		cout << "plik\n";
		break;
	default:
		// skopiuj z plik
		cout << "plik\n";
		break;
	}
}