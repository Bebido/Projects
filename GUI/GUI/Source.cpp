#include <windows.h>
#include <fstream>

using namespace std;

int wczytaj();

int main()
{
	
	char b[10];
	int licz;
	licz = wczytaj();
	for (int kod;;)
	{
		_itoa_s(licz, b, 10);
		kod = MessageBox(NULL, b, "Licznik", MB_RETRYCANCEL);
		if (kod == IDCANCEL)
			break;
		else if (kod == IDRETRY)
			licz = wczytaj();
	}
	system("pause");
	return 0;
}

int wczytaj()
{
	int liczba;
	fstream plik;
	plik.open("licz.txt", std::ios::in | std::ios::out);
	plik >> liczba;
	plik.close();
	return liczba;
}