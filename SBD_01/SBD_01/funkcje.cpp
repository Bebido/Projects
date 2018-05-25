#include "Header.hpp"
#include <cstdio>

bool wyswietlanie;
int zapisy = 0;
int odczyty = 0;
bool licz = false;

using std::cin;
using std::cout;

class rekord {
public:
	float a;
	float b;
	float suma;

	float iloczyn()
	{
		float tmp = a + b - suma;
		return tmp;
	}
};

class bufor {

	int rozmiar;
	FILE *tasma;
	int max_elementow;
	int rozmiar_elementu;
	char *buffer;
	rekord *rek_buffer;
	fpos_t poczatek_pliku;
	fpos_t tmp_pozycja;
	rekord end_rekord;
	char *sciezka_pliku;

	bool pusty;
	bool koniec_bufora;
	bool koniec_pliku;
	bool zapisany;
	int index;
	int ile_w_buforze;
	bool blokada_odczytu;

	bool zapis;
	bool odczyt;

public:
	bufor(char *file_name, char *mode)
	{
		tasma = fopen(file_name, mode);
		fgetpos(tasma, &poczatek_pliku);
		sciezka_pliku = file_name;

		this->max_elementow = 200;
		this->rozmiar_elementu = 12;
		this->rozmiar = this->max_elementow * this->rozmiar_elementu;

		buffer = new char[rozmiar];
		rek_buffer = (rekord*)buffer;
		this->pusty = true;
		this->koniec_pliku = false;
		this->koniec_bufora = false;
		this->index = 0;
		this->zapisany = true;
		this->zapis = false;
		this->odczyt = false;
		blokada_odczytu = false;

		end_rekord.a = -1.0;
		end_rekord.b = -1.0;
		end_rekord.suma = -1.0;
	}

	void zapiszRekord(rekord x)
	{
		blokada_odczytu = false;
		zapis = true;
		koniec_pliku = false;
		if (odczyt == true)
		{
			odczyt = false;
			freopen(sciezka_pliku, "w+b", tasma);
		}
		rek_buffer[index] = x;
		++index;
		this->zapisany = false;
		if (index == max_elementow)
			zapiszBufor();	
	}

	void zapiszBufor() //////////zrob zapisywanie bufora zawsze !
	{
		if (zapisany == false)
		{
			if (licz == true)
				++zapisy;
			blokada_odczytu = false;
			koniec_pliku = false;
			fwrite(buffer, sizeof(rekord), index, tasma);
			index = 0;
			this->zapisany = true;
			zapis = true;
		}
	}

	rekord odczytajRekord()
	{
		if (blokada_odczytu == true)
		{
			return end_rekord;
		}
		odczyt = true;
		if (zapisany == false)
		{
			zapiszBufor();
		}
		if (zapis == true)
		{
			zapis = false;
			pusty = true;
			koniec_bufora = false;
			koniec_pliku = false;
		}

		if (pusty == true)
		{
			fsetpos(tasma, &poczatek_pliku);
			pusty = false;
			ile_w_buforze = wczytajBufor();
			if (ile_w_buforze == 0)
				koniec_pliku = true;
			else 
				koniec_pliku = false;
		}

		if (koniec_pliku == true)
		{
			blokada_odczytu = true;
			index = 0;
			pusty = true;
			koniec_bufora = false;
			koniec_pliku = false;
			return end_rekord;
		}

		if (koniec_bufora == true)
		{
			index = 0;
			ile_w_buforze = wczytajBufor();
			if (ile_w_buforze == 0)
				koniec_pliku = true;
			else
				koniec_bufora = false;
		}

		if (koniec_pliku == true)
		{
			blokada_odczytu = true;
			index = 0;
			//fsetpos(tasma, &poczatek_pliku);
			pusty = true;
			koniec_bufora = false;
			koniec_pliku = false;
			return end_rekord;
		}

		if (koniec_bufora == false)
		{
			++index;
			if (index == ile_w_buforze || index == max_elementow)
				koniec_bufora = true;
			
			return rek_buffer[index-1];
		}
	}
	//l
	int wczytajBufor()
	{
		int zwrot, blad;
		fgetpos(tasma, &tmp_pozycja);
		for (;;)
		{
			zwrot = fread(buffer, rozmiar_elementu, max_elementow, tasma);
			if (licz == true && zwrot != 0)
				++odczyty;
			blad = ferror(tasma);
			if (blad == 0)
				break;
			else
			{
				fsetpos(tasma, &tmp_pozycja);
				clearerr(tasma);
			}
		}
		return zwrot;
	}

	~bufor()
	{
		if (zapisany == false)
			zapiszBufor();
		delete[] buffer;
		fclose(tasma);
	}
};

void wybor()
{
	char wybor;
	cout << "Czy chcesz wyswietlac plik po kazdej fazie sortowania? (t/n): \n";
	cin >> wybor;

	switch (wybor)
	{
	case 't':
		wyswietlanie = true;
		break;
	case 'n':
		wyswietlanie = false;
		break;
	default:
		wyswietlanie = false;
		break;
	}

	cout << "Wybierz metoda wprowadzania:\n m - manualnie\n l - losowo\n f - z pliku\n";
	cin >> wybor;

	switch (wybor)
	{
	case 'l':
		losowo();
		break;
	case 'm':
		manual();
		break;
	case 'f':
		cout << "plik\n";
		break;
	default:
		cout << "plik\n";
		break;
	}
}

void losowo()
{
	bufor tape_1("tasma1.test", "w+b");

	int liczba_rekordow = rand();
	liczba_rekordow = liczba_rekordow % 20;
	liczba_rekordow = liczba_rekordow + 20;
	liczba_rekordow = 51155;

	cout << "Podaj liczbe rekordow do posortowania: ";
	cin >> liczba_rekordow;

	rekord tmp;

	for (int i = 0; i < liczba_rekordow; ++i)
	{
		tmp.a = losujPrawdopodobienstwo();
		tmp.b = losujPrawdopodobienstwo();
		tmp.suma = wyznaczSume(tmp.a, tmp.b);
		tape_1.zapiszRekord(tmp);
	}
}

void manual()
{
	bufor tape1("tasma1.test", "w+");
	
	int liczba_rekordow;
	cout << "Podaj liczbe rekordow: ";
	cin >> liczba_rekordow;

	rekord tmp;
	for (int i = 0; i < liczba_rekordow; ++i)
	{
		cout << "Prawd. pierwszego zdarzenia: ";
		cin >> tmp.a;

		cout << "Prawd. drugiego zdarzenia: ";
		cin >> tmp.b;

		cout << "Prawd. sumy zdarzen: ";
		cin >> tmp.suma;
		
		tape1.zapiszRekord(tmp);
	}
}

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

void sortuj()
{
	
	bool koniec;
	int ile_faz = 0;

	for (;;)
	{
		if (wyswietlanie == true)
		{
			licz = false;
			wyswietlTasme();
		}
		++ile_faz;
		licz = true;
		koniec = dystrybucja();
		if (koniec == true)
			break;
		koniec = scalanie();
		if (koniec == true)
			break;
	}
	if (wyswietlanie == true)
	{
		licz = false;
		wyswietlTasme();
	}

	cout << "Liczba faz do posortowania: " << ile_faz << "\n";
	cout << "Liczba zapisow na dysk: " << zapisy << "\nLiczba odczytow z dysku : " << odczyty << "\n";
}

bool dystrybucja()
{
	bufor tasma1("tasma1.test", "r+b");
	bufor tasma2("tasma2.test", "w+b");
	bufor tasma3("tasma3.test", "w+b");

	rekord rekord_1, rekord_do_zapis, rekord_poprz;
	rekord_poprz.a = 0.0;
	rekord_poprz.b = 0.0;
	rekord_poprz.suma = 0.0;
	bool byl_koniec_serii = false;
	bool koniec_serii = false;
	bool koniec_pliku = false;
	bool posortowane = true;
	int aktywna_tasma = 1;
	////////dystrybucja


	//rekord_do_zapis = rekord_0;
	for (int i = 0;; ++i)
	{
		rekord_1 = tasma1.odczytajRekord();
		//++odczyty;
		rekord_do_zapis = rekord_1;

		if (rekord_1.a == -1.0)
		{
			//ustawic wszystko
			//--odczyty;
			koniec_pliku = true;
			break;
		}

		if (rekord_do_zapis.iloczyn() < rekord_poprz.iloczyn())
			koniec_serii = true;

		if (koniec_serii == true)
		{
			posortowane = false;
			byl_koniec_serii = true;
			koniec_serii = false;
			aktywna_tasma = aktywna_tasma * (-1);
		}

		if (aktywna_tasma == 1)
		{
			tasma2.zapiszRekord(rekord_do_zapis);
			//++zapisy;
			rekord_poprz = rekord_do_zapis;
		}
		else
		{
			tasma3.zapiszRekord(rekord_do_zapis);
			//++zapisy;
			rekord_poprz = rekord_do_zapis;
		}
	}
	return posortowane;
}

bool scalanie()
{
	bufor tasma1("tasma1.test", "w+b");
	bufor tasma2("tasma2.test", "r+b");
	bufor tasma3("tasma3.test", "r+b");

	rekord rekord_0, rekord_1, rekord_2, rekord_do_zapis, rekord_poprz;
	rekord_poprz.a = 0.0;
	rekord_poprz.b = 0.0;
	rekord_poprz.suma = 0.0;
	bool koniec_serii = false;
	bool koniec_pliku = false;
	bool posortowane = true;
	int aktywna_tasma = 1;
	bool koniec_serii_t2 = false;
	bool koniec_serii_t3 = false;
	bool koniec_pliku_t2 = false;
	bool koniec_pliku_t3 = false;

	rekord_0 = rekord_poprz;
	rekord_1 = rekord_0;

	rekord_do_zapis = rekord_0;

	rekord_poprz = rekord_0;
	rekord_do_zapis = rekord_0;
	rekord rekord_poprz_1, rekord_poprz_2;
	rekord_poprz_1 = rekord_0;
	rekord_poprz_2 = rekord_0;
	rekord_1 = rekord_0;
	rekord_2 = rekord_0;

	rekord_1 = tasma2.odczytajRekord();
	//++odczyty;
	rekord_2 = tasma3.odczytajRekord();
	//++odczyty;

	for (;;)
	{
		if (rekord_1.iloczyn() <= rekord_2.iloczyn())
		{
			rekord_do_zapis = rekord_1;
			aktywna_tasma = 2;
		}
		else
		{
			rekord_do_zapis = rekord_2;
			aktywna_tasma = 3;
		}
		tasma1.zapiszRekord(rekord_do_zapis);
		//++zapisy;

		if (aktywna_tasma == 2)
		{
			rekord_poprz_1 = rekord_1;
			rekord_1 = tasma2.odczytajRekord();
			//++odczyty;

			if (rekord_1.a == -1)
			{
				koniec_pliku_t2 = true;
				//--odczyty;
			}
			else if (rekord_poprz_1.iloczyn() > rekord_1.iloczyn())
				koniec_serii_t2 = true;
		}
		else if (aktywna_tasma == 3)
		{
			rekord_poprz_2 = rekord_2;
			rekord_2 = tasma3.odczytajRekord();
			//++odczyty;

			if (rekord_2.a == -1)
			{
				//--odczyty;
				koniec_pliku_t3 = true;
			}
			else if (rekord_poprz_2.iloczyn() > rekord_2.iloczyn())
				koniec_serii_t3 = true;
		}

		if (koniec_serii_t2 == true)
		{
			posortowane = false;
			for (;;)
			{
				if (rekord_2.a == -1)
				{
					//--odczyty;
					koniec_pliku_t3 = true;
					break;
				}
				else if (rekord_poprz_2.iloczyn() > rekord_2.iloczyn())
				{
					koniec_serii_t2 = false;
					break;
				}
				else if (rekord_poprz_2.iloczyn() <= rekord_2.iloczyn())
				{
					tasma1.zapiszRekord(rekord_2);
					//++zapisy;
				}
				rekord_poprz_2 = rekord_2;
				rekord_2 = tasma3.odczytajRekord();
				//++odczyty;
			}
		}

		if (koniec_serii_t3 == true)
		{
			posortowane = false;
			for (;;)
			{

				if (rekord_1.a == -1)
				{
					//--odczyty;
					koniec_pliku_t2 = true;
					break;
				}
				else if (rekord_poprz_1.iloczyn() > rekord_1.iloczyn())
				{
					koniec_serii_t3 = false;
					break;
				}
				else if (rekord_poprz_1.iloczyn() <= rekord_1.iloczyn())
				{
					tasma1.zapiszRekord(rekord_1);
					//++zapisy;
				}
				rekord_poprz_1 = rekord_1;
				rekord_1 = tasma2.odczytajRekord();
				//++odczyty;
			}
		}

		if (koniec_pliku_t2 == true) //dodaj serie
		{
			for (;;)
			{

				if (rekord_2.a == -1)
				{
					//--odczyty;
					koniec_pliku_t2 = false;
					break;
				}
				else
				{
					rekord_poprz = rekord_2;
					tasma1.zapiszRekord(rekord_2);
					//++zapisy;
				}
				rekord_2 = tasma3.odczytajRekord();
				//++odczyty;
				if ((rekord_poprz.iloczyn() > rekord_2.iloczyn()) && rekord_2.a != -1)
					posortowane = false;
			}
			break;
		}

		if (koniec_pliku_t3 == true)
		{
			for (;;)
			{
				if (rekord_1.a == -1)
				{
					//--odczyty;
					koniec_pliku_t3 = false;
					break;
				}
				else
				{
					rekord_poprz = rekord_1;
					tasma1.zapiszRekord(rekord_1);
					//++zapisy;
				}
				rekord_1 = tasma2.odczytajRekord();
				//++odczyty;
				if ((rekord_poprz.iloczyn() > rekord_1.iloczyn()) && rekord_1.a != -1)
					posortowane = false;
			}
			break;
		}
	}
	return posortowane;
}

void wyswietlTasme()
{
	bufor tasma1("tasma1.test", "r+b");
	rekord rekord_1;
	for (int i = 0;; ++i)
	{
		rekord_1 = tasma1.odczytajRekord();
		if (rekord_1.a == -1)
			break;
		cout << rekord_1.a << " " << rekord_1.b << " " << rekord_1.suma << " " << rekord_1.iloczyn() << "\n";
	}
	cout << "\n";
}
