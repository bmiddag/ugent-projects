#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <crtdbg.h>
#include "frequency_table.h"

/* 
* Functie die de grootte van de file teruggeeft
* 
* @param ifp: het bestand
*/
unsigned long get_file_length(FILE *ifp) {
	
}

/* 
* Functie die een bestand inleest en de volledige inhoud in een string opslaat en teruggeeft
* 
* @param filename: string die de bestandsnaam bevat
*/
char* read_file(const char* filename) {
	
}

void actions(const char* filename) {
	frequency_table* ft;
	unsigned int i=0;
	char *input, *language, *text, *decrypted;

	//Leest tekst uit het bestand
	input = read_file(filename);
	//Splitten van de ingelezen tekst om taal en geëncrypteerde tekst te verkrijgen
	language = strtok(input,"#");
	text = strtok(NULL,"#");
	//Printen van taal en geëncrypteerde tekst
	printf("Found language: %s\n\n",language);
	printf("Encoded text:\n=============\n%s\n\n\n",text);

	//Frequentietabel aanmaken
	ft = ft_create(text);
	
	//Ontcijferen van de geëncrypteerde tekst
	decrypted = decrypt(ft, language, text);
	printf("Decoded text:\n=============\n%s\n",decrypted);

	//Vrijmaken van het gealloceerde geheugen
	ft_free(ft);
	free(ft);
	free(input);
	free(decrypted);

	//Wachten op invoer van gebruiker om het venster te sluiten
	getchar();
}


int main(int argc, char * argv []) {
	//In case of memory leaks the number of the datablock can be used here
	//_CrtSetBreakAlloc({NR}); 

	actions("tekst1.txt");

	//Memory leaks test
	_CrtDumpMemoryLeaks();
	return 0;
}
