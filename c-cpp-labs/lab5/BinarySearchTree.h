//Bart Middag
//BinarySearchTree.h

#ifndef BINARYSEARCHTREE_H
#define BINARYSEARCHTREE_H
#include "ParameterValue.h"
using namespace std;

class Element{
private:
	long patientID;
	ParameterValue* value;
	Element* nextElement;
public:
	Element(long patientID, ParameterValue* value): patientID(patientID), value(value) {
		nextElement = NULL;
	}; 
	~Element(){};
	long getPatientID(){
		return patientID;
	};
	ParameterValue* getValue(){
		return value;
	};
	void setNextElement(Element* e){
		this->nextElement = e;
	};
	Element* getNextElement(){
		return nextElement;
	};
	//We overloaden de operator<< voor makkelijke prints
	//die blijven werken als we niet weten wat ons element bevat (zie template)
	friend ostream& operator<<(ostream& stream, const Element& element) {
		stream << "- " << *element.value << "\n";
		return stream;
	}
};

class TreeElement {
private:
	TreeElement* leftTree;
	TreeElement* rightTree;
	TreeElement* parent;
	Element* value;
public:
	TreeElement(Element* element){
		this->value = element;
		this->parent = NULL;
		this->leftTree = NULL;
		this->rightTree = NULL;
	};
	~TreeElement() {
		//Om gemakkelijk de hele boom te kunnen verwijderen, doen we ook recursief left-en rightTree weg.
		//Anders zetten we gewoon leftTree en rightTree eerst op NULL.
		delete leftTree;
		delete rightTree;
		delete value;
	};

	TreeElement* getLeftTree(){
		return leftTree;
	};
	TreeElement* getRightTree(){
		return rightTree;
	};
	TreeElement* getParent(){
		return parent;
	};
	Element* getValue(){
		return this->value;
	};

	void setLeftTree(TreeElement* leftTree){
		//We zetten parent van de nieuwe leftTree al direct op this om minder code te moeten typen
		this->leftTree = leftTree;
		if(leftTree!=NULL) leftTree->setParent(this);
	};
	void setRightTree(TreeElement* rightTree){
		this->rightTree = rightTree;
		if(rightTree!=NULL) rightTree->setParent(this);
	};
	void setParent(TreeElement* parent){
		this->parent = parent;
	};
	void setValue(Element* value){
		this->value = value;
	};
	//We overloaden de operator<< voor makkelijke prints
	//die blijven werken als we niet weten wat ons element bevat (zie template)
	//Deze functie is niet zo triviaal dankzij mijn mooiere prints, maar vermits
	//ik alle andere operator<< functies en alle andere TreeElement-functies in de
	//header definieer, doe ik dit hier ook. Een uitzondering dus.
	friend ostream& operator<<(ostream& out, const TreeElement& treeElement) {
		//Bepaal het aantal "="-tekens dat we moeten printen om mooi hetzelfde te hebben als
		//de prints in main.
		Element *element = treeElement.value;
		ostringstream IDstream;
		IDstream << " " << element->getPatientID() << " ";
		string patientID = IDstream.str();
		int i = 0;
		int length = patientID.length();
		int leading = (34 - length) /2;
		int trailing = 34-leading-length;
		for(i = 0; i < leading; i++) out << "=";
		out << patientID;
		for(i = 0; i < trailing; i++) out << "=";
		out << "\n";
		while(element != NULL) {
			out << *element;
			element = element->getNextElement();
		}
		return out;
	};
};

class BinarySearchTree{
public:
	void put(long patientID, ParameterValue* value);
	ParameterValue* get(const long patientID);
	ParameterValue* remove(const long patientID);
	void printTree();
	BinarySearchTree();
	~BinarySearchTree();
private:
	TreeElement* root;
	TreeElement* getElement(const long patientID);
	void printTree(TreeElement* node);
	void rotate(TreeElement* axis);
};

#endif