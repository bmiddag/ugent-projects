//Bart Middag
//image.c

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "image.h"

//Zodat init_image_default de functie init_image kan gebruiken, vermelden we die hier al.
void init_image(image* im, int width, int height);

void init_image_default(image* im){
	//Gebruik de functie init_image met de default-waarden.
	init_image(im,DEFAULT_IMAGE_WIDTH,DEFAULT_IMAGE_HEIGHT);
}

void init_image(image* im, int width, int height){
	int i, j;
	im->width=width;
	im->height=height;

	//Alloceer geheugen voor de afbeelding.
	im->data = (unsigned char***) calloc(width,sizeof(unsigned char**));
	for (i=0; i<width; i++){
		im->data[i] = (unsigned char**) calloc(height,sizeof(unsigned char*));
		for (j=0; j<height; j++) im->data[i][j] = (unsigned char*) calloc(3,sizeof(unsigned char));
	}
}

void free_image(image* im){
	int i, j;

	//Laat het gebruikte geheugen vrij en zet het op NULL.
	for (i=0; i<im->width; i++){
		for (j=0; j<im->height; j++) {
			free(im->data[i][j]);
			im->data[i][j] = NULL;
		}
		free(im->data[i]);
		im->data[i] = NULL;
	}
	free(im->data);
	im->data = NULL;
}

void negative_image(image* im, pixel* from, pixel* to){
	int i, j, k;

	//Overloop alle posities in de rechthoek en verander elke R/G/B-waarde naar 255-waarde (de negatieve waarde).
	for (i=from->x; i <= to->x; i++) {
		for (j = from->y; j <= to->y; j++) {
			for(k = 0; k < 3; k++) im->data[i][j][k] = 255-im->data[i][j][k];
		}
	}
}

void flip_image_horizontally(image* im, pixel* from, pixel* to){
	int i, j;
	unsigned char* tmp;

	//Overloop alle pixels in de linkerhelft van de rechthoek en verwissel ze met de juist pixel in de rechterhelft.
	for(i = 0; i <= (to->x-from->x)/2; i++) {
		for(j = from->y; j <= to->y; j++) {
			tmp=im->data[from->x+i][j];
			im->data[from->x+i][j] = im->data[to->x-i][j];
			im->data[to->x-i][j] = tmp;
		}
	}
}

void flip_image_vertically(image* im, pixel* from, pixel* to){
	int i, j;
	unsigned char* tmp;

	//Overloop alle pixels in de bovenste helft van de rechthoek en verwissel ze met de juist pixel in de onderste helft.
	for(i = 0; i <= (to->y-from->y)/2; i++) {
		for(j = from->x; j <= to->x; j++) {
			tmp=im->data[j][from->y+i];
			im->data[j][from->y+i] = im->data[j][to->y-i];
			im->data[j][to->y-i] = tmp;
		}
	}
}

void dynamic_resize_image (image* im, int new_width, int new_height, unsigned char r, unsigned char g, unsigned char b) {
	int i, j;

	if(im->width>new_width) {
		//De afbeelding wordt kleiner in de breedte, dus laat het overbodige geheugen vrij en zet het op NULL.
		for(i = im->width-1; i>=new_width; i--) {
			for (j = 0; j < im->height; j++) {
				free(im->data[i][j]);
				im->data[i][j] = NULL;
			}
			free(im->data[i]);
			im->data[i] = NULL;
		}
		im->width = new_width;
		im->data = (unsigned char***) realloc(im->data,new_width*sizeof(unsigned char**));
	} else if (new_width>im->width) {
		//De afbeelding wordt groter in de breedte, dus alloceer extra geheugen en vul de juiste waarden in.
		im->data = (unsigned char***) realloc(im->data,new_width*sizeof(unsigned char**));
		for(i = im->width; i<new_width; i++) {
			im->data[i] = (unsigned char**)calloc(im->height,sizeof(unsigned char*));
			for (j = 0; j < im->height; j++) {
				im->data[i][j] = (unsigned char*) calloc(3,sizeof(unsigned char));
				im->data[i][j][0] = r;
				im->data[i][j][1] = g;
				im->data[i][j][2] = b;
			}
		}
		im->width = new_width;
	}
	if(im->height>new_height) {
		//De afbeelding wordt kleiner in de hoogte, dus laat het overbodige geheugen vrij en zet het op NULL.
		for (i = 0 ; i < im->width ; i++ ) {
			for(j = im->height-1; j>new_height-1; j--) {
				free(im->data[i][j]);
				im->data[i][j] = NULL;
			}
			im->data[i] = (unsigned char**) realloc(im->data[i],new_height*sizeof(unsigned char*));
		}
		im->height = new_height;
	} else if (new_height>im->height) {
		//De afbeelding wordt groter in de hoogte, dus alloceer extra geheugen en vul de juiste waarden in.
		for(i = 0; i<im->width; i++) {
			im->data[i] = (unsigned char**) realloc(im->data[i],new_height*sizeof(unsigned char*));
			for(j = im->height; j<new_height; j++) {
				im->data[i][j] = (unsigned char*) calloc(3,sizeof(unsigned char));
				im->data[i][j][0] = r;
				im->data[i][j][1] = g;
				im->data[i][j][2] = b;
			}
		}
		im->height = new_height;
	}
}

void extractLeastSignificantBit(unsigned char input, int shift, unsigned char* output){
	unsigned char bit = input & 0x1;
	*output = *output | (bit << shift);
}

void decode_message (image* im, char* message, int max_len){
	int i, j;
	int index = 0; // index in the message
	int phase = 0; // phase 0 => first 3 bits // phase 1 => second 3 bits // phase 2 => last 2 bits
	unsigned char kar = 0x0; // char used for constructing characters

	// iterate the grid, starting with position (10,10), then (20,10) and so on
	for(j=1; j<(im->height/30); j++){
		for(i = 1; i<(im->width/30); i++){
			int x, y;

			if(index > max_len){
				// reached the maximum length of the string
				// jump out of inner for loop
				break;
			}

			// pixel postion to handle
			x = 30 * i;
			y = 30 * j;

			// character to handle

			if(phase == 0){
				// store first 3 bits
				extractLeastSignificantBit(im->data[x][y][0],7,&kar);
				extractLeastSignificantBit(im->data[x][y][1],6,&kar);
				extractLeastSignificantBit(im->data[x][y][2],5,&kar);
				phase = 1;
			} else if(phase == 1){
				// store second 3 bits
				extractLeastSignificantBit(im->data[x][y][0],4,&kar);
				extractLeastSignificantBit(im->data[x][y][1],3,&kar);
				extractLeastSignificantBit(im->data[x][y][2],2,&kar);
				phase = 2;
			}else if(phase == 2){
				// store last 2 bits
				extractLeastSignificantBit(im->data[x][y][0],1,&kar);
				extractLeastSignificantBit(im->data[x][y][1],0,&kar);

				// kar is complete => store
				message[index] = kar;

				if(kar == 0x0){
					// make the loop break, since end char was found!
					index = max_len+1;
				}

				// reset phase, prepare iteration for new charachter
				kar = 0x0;
				index++;
				phase=0;
			}
		}
		if(index > max_len){
			// reached the maximum length of the string
			// jump out of outer for loop
			break;
		}
	}
}

void writeLeastSignificantBit(unsigned char input, int bitIndex, unsigned char* output){
	unsigned char mask;
	unsigned char bit;
	switch(bitIndex){
	case 0:
		mask = 0x01; // mask = 0000 0001
		break;
	case 1:
		mask = 0x02; // mask = 0000 0010
		break;
	case 2:
		mask = 0x04;// mask = 0000 0100
		break;
	case 3:
		mask = 0x08;// mask = 0000 1000
		break;
	case 4:
		mask = 0x10;// mask = 0001 0000
		break;
	case 5:
		mask = 0x20;// mask = 0010 0000
		break;
	case 6:
		mask = 0x40;// mask = 0100 0000
		break;
	case 7:
		mask = 0x80;// mask = 1000 0000
		break;
	default:
		printf("Wrong bit index, must be in [0,7]\n");
	}

	// take the requested bit by applying the mask and shifting the bit towards the rightmost position
	bit = (input & mask) >> bitIndex ;
	// overwrite the rightmost bit from the output by AND with 1111 11110 and OR with the bit
	*output = (*output & 0xfe)  | bit;
}

void encode_message (image* im, char* message){
	int i, j;
	int index = 0; // index in the message
	int phase = 0; // phase 0 => first 3 bits // phase 1 => second 3 bits // phase 2 => last 2 bits

	// iterate the grid, starting with position (10,10), then (20,10) and so on
	for(j=1; j<(im->height/30); j++){
		for(i = 1; i<(im->width/30); i++){
			int x, y;

			if(index > strlen(message)){
				// reached the maximum length of the string
				// jump out of inner for loop
				break;
			}

			// pixel postion to handle
			x = 30 * i;
			y = 30 * j;

			// character to handle

			if(phase == 0){
				// store first 3 bits
				writeLeastSignificantBit(message[index],7,&im->data[x][y][0]);
				writeLeastSignificantBit(message[index],6,&im->data[x][y][1]);
				writeLeastSignificantBit(message[index],5,&im->data[x][y][2]);
				phase = 1;
			} else if(phase == 1){
				// store second 3 bits
				writeLeastSignificantBit(message[index],4,&im->data[x][y][0]);
				writeLeastSignificantBit(message[index],3,&im->data[x][y][1]);
				writeLeastSignificantBit(message[index],2,&im->data[x][y][2]);
				phase = 2;
			}else if(phase == 2){
				// store last 2 bits
				writeLeastSignificantBit(message[index],1,&im->data[x][y][0]);
				writeLeastSignificantBit(message[index],0,&im->data[x][y][1]);

				// reset phase, prepare iteration for new charachter
				index++;
				phase=0;
			}
		}
		if(index > strlen(message)){
			// reached the maximum length of the string
			// jump out of outer for loop
			break;
		}
	}

}



void load_image(image* im, char* file_name){
	int i,j;
	char kar[1];
	FILE * pFile;


	// Open file
	pFile = fopen(file_name, "rb");
	if (pFile == NULL) {
		printf("error opening file %s\n", file_name);
	}

	// skip first header line
	*kar = 0x00;
	while (*kar != 0x0a) {
		fread(kar, 1, 1, pFile);
	}

	// read first char, if # skip comment line
	fread(kar, 1, 1, pFile);
	while (*kar == 0x23 ) {
		fread(kar, 1, 1, pFile);
		while (*kar != 0x0a) {
			fread(kar, 1, 1, pFile);
		}
		fread(kar, 1, 1, pFile);
	}

	// skip 2nd header line
	while (*kar != 0x0a) {
		fread(kar, 1, 1, pFile);
	}

	// skip 3rd header line
	fread(kar, 1, 1, pFile);
	while (*kar != 0x0a) {
		fread(kar, 1, 1, pFile);
	}

	for (j = 0; j < im->height; j++) {
		for (i = 0; i < im->width; i++) {
			fread(im->data[i][j], 1, 3, pFile);
		}
	}

	fclose(pFile);
}

void save_image(image* im, char* file_name){
	int i,j;
	FILE * pFile;


	// Open file
	pFile = fopen(file_name, "wb");
	if (pFile == NULL) {
		printf("error opening file %s\n", file_name);
	}

	// Write header
	fprintf(pFile, "P6\n# Created by IrfanView\n%d %d\n255\n", im->width, im->height);

	// Write pixel data
	for (j = 0; j < im->height; j++){
		for (i = 0; i < im->width; i++){
			fwrite(im->data[i][j], 1, 3, pFile);
		}
	}

	// Close file
	fclose(pFile);
}