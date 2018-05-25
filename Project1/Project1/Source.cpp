#include <windows.h>
#include <string>
#include <cstdio>

using namespace std;

DWORD GetDWORDRegKey(HKEY hKey, const string &strValueName)
{
	unsigned long cos = REG_DWORD;
	DWORD dwBufferSize(sizeof(DWORD));
	DWORD nResult = 3453;
	LONG nError = ::RegQueryValueEx(hKey,
		strValueName.c_str(),
		NULL,
		NULL,
		(LPBYTE)&nResult,
		&dwBufferSize);
	if (nError == ERROR_SUCCESS)
	{
		printf("");
	}
	return nResult;
}

int main()
{
	HKEY hKey;
	LONG lRes = RegOpenKeyEx(HKEY_CURRENT_USER, "aaa", 0, KEY_READ, &hKey);
	DWORD nValue = 7;
	if (lRes == ERROR_SUCCESS)
	{

		std::string valueName = "bb";
		//std::wstring wsTmp(valueName.begin(), valueName.end());

		
		nValue = GetDWORDRegKey(hKey, valueName);
		RegCloseKey(hKey);
	}

	printf("%d", nValue);
	return 0;
}