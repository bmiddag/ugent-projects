#include <windows.h>
#include <iostream>
int main() {
	HKEY hKey;				// Declare a key to store the result
	DWORD buffersize = 1024;	// Declare the size of the data buffer
	char* lpData = new char[buffersize];// Declare the buffer
	/* Open the Registry Key at the location
	HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main
	with read only access
	*/

	HKEY key;
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE, "System\\CurrentControlSet\\Control\\Power\\Timeouts", 0, 0, &key))
	{
		if (RegSetValueEx(key, _T("BattSuspendTimeout"), 0, REG_DWORD, (LPBYTE)&dwData, sizeof(DWORD)))
		{
			RegCloseKey(key);
			return FALSE;
		}

		RegCloseKey(key);
	}

	RegCreateKeyEx(HKEY_CLASSES_ROOT, "VirtualMuseum\\shell\\open\\command", NULL, )
	RegOpenKeyEx(HKEY_CURRENT_USER,
		"Software\\Microsoft\\Internet Explorer\\Main", NULL, KEY_READ, &hKey);

	// Query the registry value
	RegQueryValueEx(hKey, "Start Page", NULL, NULL, (LPBYTE)lpData, &buffersize);

	// Print out the registry value
	std::cout << "Registry Key Open: memory location=" << hKey << "\n";
	std::cout << "Your Internet Start Page is " << lpData << "\n\n";
	// Close the Registry Key
	RegCloseKey(hKey);

	// Pause the system so there is time to read what is going on
	system("Pause");
	delete lpData;
}