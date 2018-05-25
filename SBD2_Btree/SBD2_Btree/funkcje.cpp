#include "Header.h"

class rekord
{
public:
	int klucz;
	float a;
	float b;
	float suma;

	rekord(int i)
	{
		klucz = i;
		generuj();
	}

	void generuj()
	{
		a = losujPrawdopodobienstwo();
		b = losujPrawdopodobienstwo();
		suma = wyznaczSume(a, b);
	}

	void wypisz()
	{
		cout << klucz << ". " << a << " " << b << " " << suma << endl;
	}
};

class struk_wpis
{
	int klucz;
	int adres;
	int wsk;
};

class node 
{
	int indeks;		//numer wezla
	int wsk0;		// pierwszy wskaznik w node
	struk_wpis wpisy[2 * D];
	int ile_wpisow;	
};

class drzewo
{
	int stopien;
	int wysokosc;
};

float losujPrawdopodobienstwo()
{
	int los = rand();
	los = los % 1000;
	float prob;
	prob = (float)los;
	prob = prob / 1000;
	return prob;
}

float wyznaczSume(float a, float b)
{
	float min, max, suma, tmp;

	max = a + b;

	if (a > b)
		min = a;
	else if (b > a)
		min = b;
	else
		min = a;

	for (int i;;)
	{
		i = rand();
		i = i % 1000;
		tmp = (float)i;
		tmp = tmp / 1000;
		if (tmp >= min && tmp <= max)
		{
			suma = tmp;
			break;
		}
	}
	return suma;
}

void wybor()
{
	char wybor;
	int klucz;
	
	for (int i = 0; i == 0;)
	{
		cout << "a - dodaj rekord, d - usun rekord, p - przejrzyj, f - wczytaj z pliku, k - koniec";
		cin >> wybor;
		switch (wybor)
		{
		case 'a':
			cin >> klucz;
			break;
		case 'u':
			cin >> klucz;
			break;
		case 'p':
			//przejrzyj plik
			break;
		case 'k':
			++i;
			break;
		case 'f':
			//z pliku
		default:
			cout << "zly wybor" << endl;
			break;
		}
	}
}