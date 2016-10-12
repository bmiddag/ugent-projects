//Bart Middag
//main.c

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <crtdbg.h>
#include "frequency_table.h"

//Extra includes en defines
#define EOS '\0'

/* 
* Functie die de grootte van de file teruggeeft
* 
* @param ifp: het bestand
*/
unsigned long get_file_length(FILE *ifp) {
	unsigned long length;
	//Ga naar het einde van het bestand.
	fseek(ifp, 0L, SEEK_END);
	//Wat is de positie van de cursor?
	length = ftell(ifp);
	//Ga terug naar het begin van het bestand.
	rewind(ifp);
	return length;
}

/* 
* Functie die een bestand inleest en de volledige inhoud in een string opslaat en teruggeeft
* 
* @param filename: string die de bestandsnaam bevat
*/
char* read_file(const char* filename) {
	FILE* ifp;
	char* input;
	unsigned long length;

	//Open het bestand in read-mode.
	ifp = fopen(filename, "r");

	//Geef fout als het inputbestand niet gevonden is.
	if(!ifp){
		 fprintf(stderr,"[ERROR] Input file not found: %s\n", filename);
		 getchar();
		 exit(1);
	}

	//Bepaal de lengte en alloceer genoeg voor de lengte van de tekst en de end of string.
	length = get_file_length(ifp);
	input = (char*) malloc(length+1);

	//Lees het bestand in, sluit het en voeg de end of string toe.
	fread(input,1,length,ifp);
	fclose(ifp);
	input[length]=EOS;

	return input;
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
