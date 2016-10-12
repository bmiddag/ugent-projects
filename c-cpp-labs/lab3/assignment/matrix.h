#ifndef _MATRIX
#define _MATRIX

#define DEFAULT_MATRIX_NUM_ROWS 3
#define DEFAULT_MATRIX_NUM_COLS 2


typedef struct {
	int num_rows;
	int num_cols;
	int** data;
} matrix;

// initializes the matrix
// given the number of rows and columns
// sets all elements to zero
void init_matrix(matrix* m, int num_rows, int num_cols);

// initializes the matrix
// using the default number or rows and colums
// sets all elements to zero
void init_matrix_default(matrix* m);

// initializes the square matrix
// with number of rows and columns set to dimension
// sets all elements to zero except
// the main diagonal elements are set to one
void init_identity_matrix(matrix* m, int dimension);

// frees the allocated memory of the matrix
void free_matrix(matrix* m);

// prints the matrix' elements
void print_matrix(matrix* m);

// transpose matrix without making a copy
// only for square matrices!
// use *(matrix +i) notation
void transpose_matrix_pa(matrix* m);

// transpose matrix without making a copy
// only for square matrices!
// use matrix[i] notation
void transpose_matrix_sn(matrix* m);

// initializes the result matrix with the correct dimensions
// multiplies a and b into the result matrix
// suppose the dimensions of a and b are compatible for multiplication
void multiply_matrices(matrix* a, matrix* b, matrix* result);

// dynamically expands (makes it bigger, new_num_rows >= original number of rows
// and new_num_cols >= originial number of columns)
// the new elements are set to zero
void dynamic_expand(matrix* m, int new_num_rows, int new_num_cols);

#endif