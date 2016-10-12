//Bart Middag
//Graph.cpp

#include "Graph.h"
#include "Iterators.h"

Graph::Graph(unsigned int number_of_nodes){
	//Initialiseer de graaf.
	this->number_of_nodes = number_of_nodes;
	this->start = 0;
}


GraphElement* hdc_next_row(GraphElement *element, GraphElement *next_column){
	//Hulpmethode om een deep copy van de graaf te maken (voor de volgende elementen in dezelfde kolom).
	GraphElement *new_element, *next_row;

	new_element = new GraphElement(element->get_from(),element->get_to(), element->get_count());
	new_element->set_nextColumn(next_column);

	if(element->get_nextRow() != 0){
		next_row = hdc_next_row(element->get_nextRow(), next_column);
		new_element->set_nextRow(next_row);
	}

	return new_element;
}

GraphElement* hdc(GraphElement *element){
	//Hulpmethode om een deep copy van de graaf te maken (voor eerste element in elke kolom).
	GraphElement *new_element, *next_column, *next_row;
	next_column = 0;

	new_element = new GraphElement(element->get_from(), element->get_to(), element->get_count());

	if( element->get_nextColumn() != 0){
		next_column = hdc(element->get_nextColumn());
		new_element->set_nextColumn(next_column);
	}
	if ( element->get_nextRow() != 0){
		next_row = hdc_next_row(element->get_nextRow(), next_column);
		new_element->set_nextRow(next_row);
	}

	return new_element;
}

Graph::Graph(const Graph& g){
	//Maak een deep copy van de gegeven graaf.
	this->number_of_nodes = g.number_of_nodes;
	this->start = hdc(g.get_start());
}

Graph::~Graph(){
	//Verwijder elk aangemaakt element uit de graaf adhv. hun nextRow en nextColumn-referenties.
	GraphElement *element, *next;
	element=this->get_start();
	while(element != 0) {
		next = element->get_nextRow();
		if(next == 0) next = element->get_nextColumn();
		delete element;
		element = next;
	}
}

int Graph::put_path(unsigned int from, unsigned int to, unsigned int count){
	//Voeg een nieuw element toe aan de graaf en pas de graaf aan zodat de verbindingen correct zijn.
	GraphElement *element, *new_element;
	//Als het element 0 keer moet voorkomen voegen we het niet toe, maar daarom gebeurt er geen fout.
	if(count==0) return 0;
	//Er gebeurt wel een fout als de from-en to-argumenten te groot zijn voor deze graaf.
	if(from>get_number_of_nodes() || to>get_number_of_nodes()) return -1;

	element = this->get_start();
	//Als de graaf leeg is, maken we onze start aan.
	if(element == 0) {
		new_element = new GraphElement(from,to,count);
		this->start = new_element;
		return 0;
	}
	//Ook als onze bestaande start na het element moet komen dat we toevoegen, passen we start aan.
	if(element->get_to() > to) {
		new_element = new GraphElement(from,to,count);
		new_element->set_nextColumn(element);
		this->start = new_element;
		return 0;
	}
	if(element->get_from() > from && element->get_to() == to) {
		new_element = new GraphElement(from,to,count);
		new_element->set_nextRow(element);
		new_element->set_nextColumn(element->get_nextColumn());
		this->start = new_element;
		return 0;
	}
	//We gaan naar de kolom en rij juist voor of gelijk aan de rij die we nodig hebben.
	while(element->get_nextColumn() != 0 && element->get_nextColumn()->get_to() <= to) {
		element = element->get_nextColumn();
	}
	while(element->get_nextRow() != 0 && element->get_nextRow()->get_from() <= from) {
		element = element->get_nextRow();
	}
	//Als we op de juiste positie staan, tellen we count gewoon bij het bestaande element op.
	if(element != 0 && element->get_from() == from && element->get_to() == to){
		element->set_count(element->get_count()+count);
		return 0;
	}
	//Ontbreekt de kolom die we nodig heben, of moet ons element komen voor het eerste element van de juiste kolom?
	if(element->get_to() < to || element->get_from() > from) {
		new_element = new GraphElement(from,to,count);
		//In beide gevallen wordt de nextColumn-verwijzing gelijkgesteld aan element->getnextColumn().
		new_element->set_nextColumn(element->get_nextColumn());
		if(element->get_to() == to) {
			//Als we in de juiste kolom zitten moet onze nextRow dus het element worden dat we bekomen zijn.
			new_element->set_nextRow(element);
		}
		//Ga naar de laatst bestaande kolom voor de kolom die we zoeken en verander daar voor elk element de nextColumn.
		element = this->get_start();
		while(element->get_nextColumn() != 0 && element->get_nextColumn()->get_to() < to) {
			element = element->get_nextColumn();
		}
		while(element != 0){
			element->set_nextColumn(new_element);
			element = element->get_nextRow();
		}
		return 0;
	}
	//Nu zitten we zeker in de juiste kolom en zal ons nieuwe element niet het eerste element van de kolom zijn.
	new_element = new GraphElement(from,to,count);
	new_element->set_nextColumn(element->get_nextColumn());
	new_element->set_nextRow(element->get_nextRow());
	element->set_nextRow(new_element);
	return 0;
}

int Graph::number_of_transitions(unsigned int node) const{
	int count = 0;
	//Tel het aantal keren dat er een verbinding uit deze node vertrekt.
	MatrixRowIterator ri(this, node);
	while(ri.hasNext()){
		count+=ri.next();
	}
	//Tel het aantal keren dat er een verbinding in deze node toekomt.
	MatrixColumnIterator ci(this, node);
	while(ci.hasNext()){
		count+=ci.next();
	}
	return count;
}

const Graph Graph::operator+(const Graph& other) const{
	const Graph *copy, *iterate;
	int i;
	unsigned int row;
	//Over welke graaf gaan we itereren en van welke gaan we een deep copy maken?
	if(other.get_number_of_nodes() > this->number_of_nodes) {
		copy = &other;
		iterate = this;
	} else {
		copy = this;
		iterate = &other;
	}
	//Maak een deep copy van de grootste graaf.
	Graph graph = Graph(*copy);
	//Itereer over elke kolom.
	//De ColumnIterator moet niet zoveel nodes overlopen om de cursor te verzetten, dus deze is efficiënter.
	for(unsigned int c = 0; c < iterate->get_number_of_nodes(); c++) {
		MatrixColumnIterator ci(iterate, c);
		row = 0;
		while(ci.hasNext()){
			i = 0;
			do {
				//Sla alle 0'en over. We zijn enkel geïnteresseerd in de waarden die groter zijn dan 0.
				i = ci.next();
				row++;
			} while((ci.hasNext()) && (i == 0));
			//Maak deze verbinding ook aan in de zopas aangemaakte graaf.
			if(i > 0) graph.put_path(row-1,c,i);
        }
	}
	return graph;
}

