#include <iostream>
int main()
{
	int ktory;


	do {

		std::cout << "\nKapitanie, ktory podzespol sprawdzic? \n"
			<< "nr 10 - silnik \nnr35 - Stery \nnr 28 - radar\n"
			<< "Podaj kapitanie numer:\n";

		//int ktory;
		std::cin >> ktory;

		switch (ktory)
		{
		case 10:
			std::cout << "sprawdzamy silnik \n";
			break;

		case 28:
			std::cout << "sprawdzamy radar \n";
			break;

		case 35:
			std::cout << "sprawdzamy stery \n";
			break;

		default:
			std::cout << "zazadales nr " << ktory << " - nie znam takiego!";
			break;
		}

		if (ktory == 10 || ktory == 28 || ktory == 35)
			break;
	} while (1);

	return 0;
}