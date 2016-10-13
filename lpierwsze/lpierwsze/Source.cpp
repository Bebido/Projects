#include <iostream>

int main()
{
	int wejscie, testy;
	std::cin >> testy;
	while (testy)
	{
		std::cin >> wejscie;
		if (wejscie == 1)
		{
			std::cout << "NIE\n";
			--testy;
			continue;
		}

		for (int i = 1; i <= wejscie; ++i)
		{
			if ( wejscie%i == 0 && i != wejscie && i != 1 && i != wejscie)
			{
				std::cout << "NIE\n";
				i =+ wejscie;
				continue;
			}
			if (i == wejscie)
				std::cout << "TAK\n";
		}
		--testy;
	}
	return 0;

}