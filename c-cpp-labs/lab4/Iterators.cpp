//Bart Middag
//Iterators.cpp

#include "Iterators.h"

// -------------------------------------------- MATRIXROWITERATOR -------------------------------------------- \\

MatrixRowIterator::MatrixRowIterator(const Graph* g, unsigned int row){
	//Initialiseer de iterator.
	this->g = g;
	this->row = row;
	this->currentColumn = 0;
	this->cursor = 0;

	//We gaan naar het eerste element op deze rij en laten de cursor ernaar verwijzen. Als er geen is, blijft de cursor 0.
	GraphElement *element = g->get_start();
	while(element != 0 && this->cursor == 0){
		while( element->get_from() < row && element->get_nextRow() != 0){
			element = element->get_nextRow();
		}
		if( element->get_from() == row){
			this->cursor = element;
		}
		element = element->get_nextColumn();
	}

}

MatrixRowIterator::~MatrixRowIterator(){
	//Hier is niets te zien, omdat we ook niets moeten deleten in dit object.
}

int MatrixRowIterator::next(){
	//Geef de waarde van het eerstvolgende element in de rij terug.
	int count;
	if(this->currentColumn == this->g->get_number_of_nodes()){
		//We gaan buiten de grenzen van de matrix.
		return -1;
	}
	if(this->cursor == 0){
		//Er is geen element > 0 meer in deze rij, dit element is dus 0.
		this->currentColumn++;
		return 0;
	}
	if(this->currentColumn < this->cursor->get_to()){
		//We zijn nog niet aan de cursor aangekomen, dit element is dus 0.
		this->currentColumn++;
		return 0;
	} else {
		//We zijn aan de cursor aangekomen, geef de juiste waarde terug.
		count = cursor->get_count();
		//Pas de cursor aan naar het volgende niet-0 element in de rij (of 0 als er geen is).
		GraphElement *element = this->cursor;
		while(element != 0){
			element = element->get_nextColumn();
			while(element != 0 && element->get_nextRow() != 0 && element->get_from() < cursor->get_from()){
				element = element->get_nextRow();
			}
			if(element != 0 && element->get_from() == cursor->get_from()) this->cursor = element;
		}
		if(element==0) this->cursor = 0;
		this->currentColumn++;
		return count;
	}

}

bool MatrixRowIterator::hasNext() const{
	//Zolang we nog niet aan het einde van de rij zijn, geven we true terug.
	return (this->currentColumn < this->g->get_number_of_nodes());
}

// -------------------------------------------- MATRIXCOLUMNITERATOR -------------------------------------------- \\

MatrixColumnIterator::MatrixColumnIterator(const Graph* g, unsigned int column){
	//Initialiseer de iterator.
	this->g = g;
	this->column = column;
	this->currentRow = 0;
	this->cursor = 0;

	//We gaan naar het eerste element op deze kolom en laten de cursor ernaar verwijzen. Als er geen is, blijft de cursor 0.
	GraphElement *element = g->get_start();
	while(element != 0 && element->get_to() < column){
		element = element->get_nextColumn();
	}
	if(element->get_to() == column){
		this->cursor = element;
	}
}

MatrixColumnIterator::~MatrixColumnIterator(){
	//Hier is niets te zien, omdat we ook niets moeten deleten in dit object.
}

int MatrixColumnIterator::next(){
	//Geef de waarde van het eerstvolgende element in de kolom terug.
	int count;
	if(this->currentRow == this->g->get_number_of_nodes()){
		//We gaan buiten de grenzen van de matrix.
		return -1;
	}
	if(this->cursor == 0){
		//Er is geen element > 0 meer in deze kolom, dit element is dus 0.
		this->currentRow++;
		return 0;
	} 
	if(this->currentRow < this->cursor->get_from()){
		//We zijn nog niet aan de cursor aangekomen, dit element is dus 0.
		this->currentRow++;
		return 0;
	} else {
		//We zijn aan de cursor aangekomen, geef de juiste waarde terug.
		count = cursor->get_count();
		//Pas de cursor aan naar het volgende niet-0 element in de kolom (of 0 als er geen is).
		this->cursor = cursor->get_nextRow();
		this->currentRow++;
		return count;
	}
}

bool MatrixColumnIterator::hasNext() const{
	//Zolang we nog niet aan het einde van de kolom zijn, geven we true terug.
	return (this->currentRow < this->g->get_number_of_nodes());
}