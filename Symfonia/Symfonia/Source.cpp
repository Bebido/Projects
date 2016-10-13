#include <iostream>

int main()
{
	int stopy;
	double metry;
	double przelicznik = 0.3;

	std::cout << "Wprowadz stopy \n";
	std::cin >> stopy;
	metry = przelicznik * stopy;

	std::cout << stopy << " stop to" << metry << " metrow \n";
	system("pause");
}