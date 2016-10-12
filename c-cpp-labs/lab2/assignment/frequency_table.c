#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include "frequency_table.h"
#include "statistics.h"

/* 
* Functie die een nieuwe frequentietabel aanmaakt op basis van een geëncrypteerde tekst
* 
* @param encrypted: string die de geëncrypteerde tekst bevat
*/
frequency_table* ft_create(char* encrypted) {
	
}

/* 
* Functie die het ingenomen geheugen van een frequentietabel vrijgeeft
* 
* @param ft: de frequentietabel
*/
void ft_free(frequency_table* ft) {
	
}

/* 
* Functie h
* 
* @param encrypted: string die de geëncrypteerde tekst bevat
*/
void ft_swap(ft_entry* fc1, ft_entry* fc2) {
	
}

/* 
* Functie die het fixup algoritme implementeert (cfr. p.248-250)
* 
* @param cursor: het knooppunt (p.248-250)
* @param k: k gedefinieerd zoals in de cursus op p.248
* @param ft: de frequentietabel
*/
void ft_fixup(ft_entry* cursor, const unsigned int k, const frequency_table* ft) {
	
}

/* 
* Functie die het fixup algoritme implementeert (cfr. p.248-250)
* 
* @param cursor: het knooppunt (p.248-250)
* @param k: k gedefinieerd zoals in de cursus op p.248
* @param ft: de frequentietabel
*/
void ft_fixdown(ft_entry* cursor, const unsigned int k, const frequency_table* ft) {
	
}

/* 
* Functie die een nieuw element toevoegt aan een frequentietabel of het voorkomen van dat element met 1 verhoogt
* 
* @param c: element dat toegevoegd wordt aan de frequentietabel
* @param ft: frequentietabel waaraan het element wordt toegevoegd
*/
void ft_insert(const char c, frequency_table* ft) {
	
}

/* 
* Functie die het grootste element uit de frequentietabel haalt (cfr. p.248-250)
* 
* @param ft: de frequentietabel
*/
ft_entry* ft_getmax(frequency_table* ft) {
	
}


/* 
* Functie die de index geeft van het grootste element in de array
* 
* @param list: de array
* @param size: grootte van de array
*/
unsigned int get_max_index(double* list, unsigned int size){
	
}

/* 
* Functie die de geëncrypteerde tekst decodeert en terug geeft
* 
* @param ft: frequentietabel van de geëncrypteerde tekst
* @param language: string die de taal voorstelt
* @param text: de geëncrypteerde tekst
*/
char* decrypt(frequency_table* ft, const char* language, const char* text){
	
}
