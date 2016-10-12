//Bart Middag
//matrix.c

#include <stdlib.h>
#include <stdio.h>
#include "matrix.h"

void init_matrix(matrix* m, int num_rows, int num_cols){
	int i;

	//Sla het aantal rijen en kolommen op.
	m->num_rows = num_rows;
	m->num_cols = num_cols;

	//Alloceer geheugen voor de matrix.
	m->data = (int**)calloc(num_rows,sizeof(int*));
	for(i=0; i<num_rows; i++) m->data[i] = (int*)calloc(num_cols,sizeof(int));
}

void init_matrix_default(matrix* m){
	//Voer de normale init_matrix-methode uit met de default-waarden.
	init_matrix(m,DEFAULT_MATRIX_NUM_ROWS,DEFAULT_MATRIX_NUM_COLS);
}

void init_identity_matrix(matrix* m, int dimension){
	int i;

	//Voer de normale init_matrix-methode uit en zet daarna de alle getallen op de hoofddiagonaal op 1.
	init_matrix(m,dimension,dimension);
	for(i=0; i<dimension; i++) m->data[i][i] = 1;
}

void free_matrix(matrix* m){
	int i;

	//Laat het gebruikte geheugen vrij en zet het op NULL.
	for(i=0; i<m->num_rows; i++) {
		free(m->data[i]);
		m->data[i] = NULL;
	}
	free(m->data);
	m->data = NULL;

}

void print_matrix(matrix* m){
	int i, j;

	//Print de matrix uit.
	for(i=0; i<m->num_rows; i++) {
		for(j = 0; j<m->num_cols; j++) {
			printf("%3d ",m->data[i][j]);
		}
		printf("\n");
	}
}

void transpose_matrix_pa(matrix* m){
	int i, j, tmp;

	//Transponeer een vierkante matrix met pointer-arithmetiek.
	for(i=0;i<m->num_rows;i++){
		for(j=i+1;j<m->num_cols;j++){
			tmp=*((*(m->data+i))+j);
			*((*(m->data+i))+j)=*((*(m->data+j))+i);
			*((*(m->data+j))+i)=tmp;
		}
    }
}

void transpose_matrix_sn(matrix* m){
	int i, j, tmp;

	//Transponeer een vierkante matrix met de matrix[i][j]-notatie.
	for(i=0;i<m->num_rows;i++){
		for(j=i+1;j<m->num_cols;j++){
			tmp=m->data[i][j];
			m->data[i][j] = m->data[i][j];
			m->data[i][j] = tmp;
		}
    }
}

void multiply_matrices(matrix* a, matrix* b, matrix* result){
	int i, j, k, sum;

	//Maak een nieuwe matrix aan van de juiste grootte.
	init_matrix(result,a->num_rows,b->num_cols);

	//Overloop de matrix om het product te maken.
	for(i=0;i<a->num_rows;i++){
		for(j=0;j<b->num_cols;j++){
			sum = 0;

			//Tel de producten op om de juiste waarde te bekomen.
			for(k=0; k < a->num_cols ; k++){
				sum+=a->data[i][k]*b->data[k][j]; 
			}
			result->data[i][j]=sum;
		}
	}
}

void dynamic_expand(matrix* m, int new_num_rows, int new_num_cols){
	int i, j;

	//Moeten er rijen bijkomen?
	if(new_num_rows>m->num_rows) {

		//Alloceer extra geheugen voor de rijen.
		m->data = (int**) realloc(m->data,new_num_rows*sizeof(int*));

		//Alloceer genoeg geheugen zodat de kolommen ook langer kunnen worden.
		for(i = m->num_rows; i<new_num_rows; i++) {
			m->data[i] = (int*)calloc(m->num_cols,sizeof(int));
		}
		m->num_rows = new_num_rows;
	}

	//Moeten er kolommen bijkomen?
	if(new_num_cols>m->num_cols) {

		//Alloceer extra geheugen voor de kolommen.
		for(i = 0; i<m->num_rows; i++) {
			m->data[i] = (int*) realloc(m->data[i],new_num_cols*sizeof(int));

			//Zet de waarden van de nieuwe posities op 0. Dit moet niet bij calloc, want dat doet dat zelf.
			for(j = m->num_cols; j<new_num_cols; j++) m->data[i][j]=0;
		}
		m->num_cols = new_num_cols;
	}
}