#include <Windows.h>
#include <iostream>

#define MAX_ROZMIAR 15

using namespace std;

class Rekord {
public:
	int index;
	int max;
	int rozmiar;
	int *wpisy;

	Rekord(int ind, int n, int tab[])       // raczej wyrzucaj juz do osobnego pliku, lepiej miec .h z samymi deklaracjami i cpp z implementacja
	{
		this->index = ind;
		this->rozmiar = n;
		this->max = -1;
		this->wpisy = new int[rozmiar];
		for (int i = 0; i < this->rozmiar; ++i)
		{
			wpisy[i] = tab[i];
			if (wpisy[i] > this->max)       // takie ify to zlo, zawsze uzywaj klamer
				this->max = wpisy[i];
		}
	}

	void wypisz()
	{
		cout << "Index: " << this->index << ". ";
		for (int i = 0; i < this->rozmiar; ++i)     // klamry
			cout << wpisy[i] << " ";
		cout << endl;
	}

	~Rekord()
	{
		delete[] wpisy;
	}


};

class Bp_drzewo {

};


int main()
{
	Bp_drzewo drzewo;
	int tab[5] = { 82, 3, 32, 12, 40 };
	Rekord rekord(1, 5, tab);
	rekord.wypisz();
	system("pause");
	return 0;
}