#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "image.h"

void init_image_default(image* im){
	// TODO complete
}

void init_image(image* im, int width, int height){
	// TODO complete
}

void free_image(image* im){
	// TODO complete
}

void negative_image(image* im, pixel* from, pixel* to){
	// TODO complete
}

void flip_image_horizontally(image* im, pixel* from, pixel* to){
	// TODO complete
}

void flip_image_vertically(image* im, pixel* from, pixel* to){
	// TODO complete
}

void dynamic_resize_image (image* im, int new_width, int new_height, unsigned char r, unsigned char g, unsigned char b) {
	// TODO complete
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
				// TODO add calls to extractSignificantBit
				phase = 1;
			} else if(phase == 1){
				// store second 3 bits
				// TODO add calls to extractSignificantBit
				phase = 2;
			}else if(phase == 2){
				// store last 2 bits
				// TODO add calls to extractSignificantBit

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
	// TODO complete
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