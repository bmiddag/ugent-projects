//Bart Middag
//frequency_table.c

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include "frequency_table.h"
#include "statistics.h"

//Extra includes en defines
#include <ctype.h>
#define EOS '\0'

/* 
* Functie die een nieuwe frequentietabel aanmaakt op basis van een geëncrypteerde tekst
* 
* @param encrypted: string die de geëncrypteerde tekst bevat
*/
frequency_table* ft_create(char* encrypted) {
	frequency_table *ftable;
	unsigned int i;

	/*
	* Alloceer geheugen voor de frequentietabel.
	* Voor de tabel zelf roepen we 1 keer calloc op voor 27 ft_entries ipv verschillende keren realloc.
	* De 27 plaatsen worden niet altijd allemaal gebruikt, maar het is beter om altijd langere stukken
	* geheugen te alloceren in plaats van bij elk nieuw karakter realloc op te roepen voor 1 ft_entry.
	* Dat zou overbodig werk en ook een overvloedig gebruik van realloc zijn.
	*/
	ftable = (frequency_table*)malloc(sizeof(frequency_table));
	ftable->ft = (ft_entry*)calloc(27,sizeof(ft_entry));
	ftable->size = 0;

	//Maak de frequentietabel aan.
	i = 0;
	while(encrypted[i] != EOS) {
		if(isalpha(encrypted[i])) ft_insert(tolower(encrypted[i]), ftable);
		i++;
	}

	return ftable;
}

/* 
* Functie die het ingenomen geheugen van een frequentietabel vrijgeeft
* 
* @param ft: de frequentietabel
*/
void ft_free(frequency_table* ft) {
	if(ft->ft != NULL) free(ft->ft);
}

/* 
* Functie swap
* 
* @param encrypted: string die de geëncrypteerde tekst bevat
*/
void ft_swap(ft_entry* fc1, ft_entry* fc2) {
	//Verwissel aan de hand van een tijdelijke variabele.
	ft_entry fctmp;
	fctmp = *fc2;
	*fc2=*fc1;
	*fc1=fctmp;
}

/* 
* Functie die het fixup algoritme implementeert (cfr. p.248-250)
* 
* @param cursor: het knooppunt (p.248-250)
* @param k: k gedefinieerd zoals in de cursus op p.248
* @param ft: de frequentietabel
*/
void ft_fixup(ft_entry* cursor, const unsigned int k, const frequency_table* ft) {
	//Is aantal voorkomens van dit char groter dan aantal voorkomens van de ouder?
	if (k>1 && cursor->n > ft->ft[k/2].n) {
		//Zoja, verwissel deze twee elementen en blijf fixup uitvoeren voor de top ditzelfde char.
		ft_swap(cursor,&(ft->ft[k/2]));
		ft_fixup(&(ft->ft[k/2]),k/2,ft);
	}
}

/* 
* Functie die het fixdown algoritme implementeert (cfr. p.248-250)
* 
* @param cursor: het knooppunt (p.248-250)
* @param k: k gedefinieerd zoals in de cursus op p.248
* @param ft: de frequentietabel
*/
void ft_fixdown(ft_entry* cursor, const unsigned int k, const frequency_table* ft) {
	unsigned int max_child;
	//Heeft deze node kinderen?
	if(k*2 <= ft->size) {
		//Zoja, bepaal het grootste kind.
		max_child = k*2;
		if(max_child < ft->size && ft->ft[max_child+1].n > ft->ft[max_child].n) max_child++;
		//Komt het grootste kind meer voor dan het huidige char?
		if(ft->ft[max_child].n > cursor->n) {
			//Zoja, verwissel deze twee elementen en blijf fixdown uitvoeren voor de top ditzelfde char.
			ft_swap(cursor,&(ft->ft[max_child]));
			ft_fixdown(&(ft->ft[max_child]),max_child,ft);
		}
	}
}

/* 
* Functie die een nieuw element toevoegt aan een frequentietabel of het voorkomen van dat element met 1 verhoogt
* 
* @param c: element dat toegevoegd wordt aan de frequentietabel
* @param ft: frequentietabel waaraan het element wordt toegevoegd
*/
void ft_insert(const char c, frequency_table* ft) {
	unsigned int i;
	int add_entry;

	add_entry=1;

	//Overloop de tabel om te zien of deze char er al in staat.
	for(i = 1; i <= ft->size; i++) {
		if(ft->ft[i].c == c) {
			//De char staat er al in, hoog gewoon de teller eens op en laat het eventueel stijgen in de boom.
			ft->ft[i].n++;
			add_entry = 0;
			ft_fixup(&ft->ft[i],i,ft);
		}
	}

	//Is het nodig om een nieuwe entry aan te maken?
	if(add_entry) {
		//Hoog eerst de teller op, zodat het nieuwe element op plaats size komt te staan.
		//Positie 0 wordt alleen gebruikt als tijdelijke kopie (zie ft_getmax).
		ft->size++;
		ft->ft[ft->size].c = c;
		ft->ft[ft->size].n = 1;
	}
}

/* 
* Functie die het grootste element uit de frequentietabel haalt (cfr. p.248-250)
* 
* @param ft: de frequentietabel
*/
ft_entry* ft_getmax(frequency_table* ft) {
	ft_entry* to_remove;
	ft_entry* last_el;

	if (ft->size == 0) return NULL;

	//Maak een tijdelijke kopie van het maximum en zet daarna het aantal voorkomens van het maximum op 0.
	ft->ft[0].c = ft->ft[1].c;
	ft->ft[0].n = ft->ft[1].n;
	ft->ft[1].n = 0;

	//Beschouw het eerste en laatste element van de boom en verwissel ze.
	to_remove = &ft->ft[1];
	last_el = &ft->ft[ft->size];
	ft_swap(to_remove,last_el);

	//Laat het nieuwe eerste element zakken in de boom zodat alles terug goed geordend is.
	ft_fixdown(&ft->ft[1],1,ft);

	//Laat het element in de array staan maar we kijken er nooit meer naar omdat size kleiner wordt.
	ft->size--;

	return &ft->ft[0];
}


/* 
* Functie die de index geeft van het grootste element in de array
* 
* @param list: de array
* @param size: grootte van de array
*/
unsigned int get_max_index(double* list, unsigned int size){
	unsigned int i;
	unsigned int max_index;

	max_index = 0;

	//Overloop de lijst en update het maximum als het huidige element groter is.
	for(i=1; i < size; i++) {
		if(list[i] >= list[max_index]) {
			max_index = i;
		}
	}
	return max_index;
}

/* 
* Functie die de geëncrypteerde tekst decodeert en terug geeft
* 
* @param ft: frequentietabel van de geëncrypteerde tekst
* @param language: string die de taal voorstelt
* @param text: de geëncrypteerde tekst
*/
char* decrypt(frequency_table* ft, const char* language, const char* text){
	unsigned int i;
	unsigned int j;
	unsigned int max_index;
	char max_tree;
	char* output;
	char decryption[26] = {0};
	double stats[26] = {0};

	//Alloceer genoeg geheugen voor de tekst en de end of string.
	output = (char*)malloc(strlen(text)+1);

	//Vergelijk met de twee talen.
	for(i = 0; i < 2; i++) {
		if(!strcmp(language,statistics[i].language)) {
			//Kopieer de tabel met statistieken die we nodig hebben zodat we ze kunnen bewerken.
			for(j = 0; j < 26; j++) {
				stats[j] = statistics[i].frequencies[j];
			}

			//Zolang er nog elementen in de heap zitten, zijn er karakters om te vervangen.
			while(ft->size) {
				//Haal de index van de meest voorkomende letter uit de statistieken en vervang dat daarna door 0.
				max_index = get_max_index(stats,26);
				stats[max_index] = 0;
				//Haal ook het meest voorkomende char uit de boom.
				max_tree = ft_getmax(ft)->c;
				//Voeg dit karakter toe aan de vertaaltabel.
				decryption[max_tree-'a']='a'+max_index;
			}
		}
	}

	i = 0;
	//Overloop de tekst, vervang voorkomens van a-z en A-Z met de vertaaltabel en kopieer de rest.
	while(text[i] != EOS) {
		 if(text[i]<='z' && text[i]>='a'){
			 output[i]=decryption[text[i]-'a'];
		 } else if(text[i]<='Z' && text[i]>='A'){
			 output[i]=toupper(decryption[text[i]-'A']);
		 } else output[i]=text[i];
		 i++;
	}

	//Zorg ervoor dat het programma weet waar de outputstring stopt.
	output[i]=EOS;

	return output;
}
