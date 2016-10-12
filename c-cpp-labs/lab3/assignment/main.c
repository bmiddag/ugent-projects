#include <stdio.h>
#include <stdlib.h>
#include <crtdbg.h>
#include "matrix.h"
#include "image.h"

void fill_matrix_a(matrix* m){
	m->data[0][0] = 1;
	m->data[0][1] = 0;
	m->data[0][2] = 2;
	m->data[1][0] = 0;
	m->data[1][1] = 3;
	m->data[1][2] = 1;
}

void fill_matrix_b(matrix* m){
	m->data[0][0] = 0;
	m->data[0][1] = 3;
	m->data[1][0] = 2;
	m->data[1][1] = 1;
	m->data[2][0] = 0;
	m->data[2][1] = 4;
}

void ex_1_matrix_operations(){
	matrix a,b,c;

	// init a(2,3) and fill with elements
	init_matrix(&a,2,3);
	fill_matrix_a(&a);

	// init b (default) and fill with elements
	init_matrix_default(&b);
	fill_matrix_b(&b);

	// c = a * b
	multiply_matrices(&a, &b, &c);

	// c = c'
	transpose_matrix_pa(&c);

	// free matrix a and b for reuse
	free_matrix(&a);
	free_matrix(&b);

	// create unity matrix
	init_identity_matrix(&a,2);

	// b = c * a
	multiply_matrices(&c, &a, &b);

	// expand b to (3,4)
	dynamic_expand(&b,3,4);

	// print result b
	print_matrix(&b);

	// free all resources
	free_matrix(&a);
	free_matrix(&b);
	free_matrix(&c);
}

void ex_2_image_processing(){
	image im;
	pixel a,b;
	char message[1000];

	// init image with default settings
	init_image_default(&im);
	
	// load image
	load_image(&im,"input.ppm");

	// some operations on image
	a.x=25;
	a.y=25;
	b.x=331;
	b.y=532;
	negative_image(&im, &a, &b);
	save_image(&im,"output_0.ppm");

	a.x=160;
	a.y=387;
	b.x=525;
	b.y=489;
	flip_image_horizontally(&im, &a, &b);
	save_image(&im,"output_1.ppm");

	a.x=160;
	a.y=35;
	b.x=525;
	b.y=288;
	flip_image_vertically(&im, &a, &b);
	save_image(&im,"output_2.ppm");

	a.x=160;
	a.y=195;
	b.x=525;
	b.y=435;
	negative_image(&im, &a, &b);
	save_image(&im,"output_3.ppm");

	dynamic_resize_image (&im, DEFAULT_IMAGE_WIDTH+25, DEFAULT_IMAGE_HEIGHT+25, 0, 102, 187);

	decode_message(&im, message,1000);
	printf("Decoded message: %s\n", message);
	encode_message(&im, "Plaats hier je voornaam en naam!");

	// save image and free
	save_image(&im,"output.ppm");
	free_image(&im);
}

int main(int argc, const char * argv[]) {
	ex_1_matrix_operations();
	ex_2_image_processing();
	_CrtDumpMemoryLeaks();
}