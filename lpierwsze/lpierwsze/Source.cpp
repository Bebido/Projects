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
			if ( wejscie%i == 0 && i != wejscie && i != 1 && i != wejscie)      // 2x i != wejscie
			{
				std::cout << "NIE\n";
				i =+ wejscie;       // cos tu nie gra, to jest rownowazne i = wejscie, a nie i = i + wejscie, na pewno zamierzone?
				continue;
			}
			if (i == wejscie)
				std::cout << "TAK\n";
		}
		--testy;
	}
	return 0;
    //  ogolnie ten jest stylowo lepszy od poprzedniego, wypracuj 1 styl i staraj sie nie zmieniac
}