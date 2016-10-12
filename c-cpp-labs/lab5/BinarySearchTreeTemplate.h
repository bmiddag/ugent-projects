//Bart Middag
//BinarySearchTreeTemplate.h

#ifndef BINARYSEARCHTREETEMPLATE_H
#define BINARYSEARCHTREETEMPLATE_H
#include <string>
#include <sstream>
using namespace std;

//Voor commentaar, zie BinarySearchTree.h!

template<class T>
class ElementTemplate{
private:
	long ID;
	T* value;
	ElementTemplate<T>* nextElement;
public:
	ElementTemplate(long ID, T* value): ID(ID), value(value) {
		nextElement = NULL;
	}; 
	~ElementTemplate(){};
	long getID(){
		return ID;
	};
	T* getValue(){
		return value;
	};
	void setNextElement(ElementTemplate<T>* e){
		this->nextElement = e;
	};
	ElementTemplate<T>* getNextElement(){
		return nextElement;
	};
	template<class T>
	friend ostream& operator<<(ostream& stream, const ElementTemplate<T>& element) {
		stream << "- " << *element.value << "\n";
		return stream;
	}
};

template<class T>
class TreeElementTemplate {
private:
	TreeElementTemplate<T>* leftTree;
	TreeElementTemplate<T>* rightTree;
	TreeElementTemplate<T>* parent;
	ElementTemplate<T>* value;
public:
	TreeElementTemplate(ElementTemplate<T>* element){
		this->value = element;
		this->parent = NULL;
		this->leftTree = NULL;
		this->rightTree = NULL;
	};
	~TreeElementTemplate() {
		delete leftTree;
		delete rightTree;
		delete value;
	};

	TreeElementTemplate<T>* getLeftTree(){
		return leftTree;
	};
	TreeElementTemplate<T>* getRightTree(){
		return rightTree;
	};
	TreeElementTemplate<T>* getParent(){
		return parent;
	};
	ElementTemplate<T>* getValue(){
		return this->value;
	};

	void setLeftTree(TreeElementTemplate<T>* leftTree){
		this->leftTree = leftTree;
		if(leftTree!=NULL) leftTree->setParent(this);
	};
	void setRightTree(TreeElementTemplate<T>* rightTree){
		this->rightTree = rightTree;
		if(rightTree!=NULL) rightTree->setParent(this);
	};
	void setParent(TreeElementTemplate<T>* parent){
		this->parent = parent;
	};
	void setValue(ElementTemplate<T>* value){
		this->value = value;
	};
	template<class T>
	friend ostream& operator<<(ostream& out, const TreeElementTemplate<T>& treeElement) {
		ElementTemplate<T> *element = treeElement.value;
		ostringstream IDstream;
		IDstream << " " << element->getID() << " ";
		string ID = IDstream.str();
		int i = 0;
		int length = ID.length();
		int leading = (34 - length) /2;
		int trailing = 34-leading-length;
		for(i = 0; i < leading; i++) out << "=";
		out << ID;
		for(i = 0; i < trailing; i++) out << "=";
		out << "\n";
		while(element != NULL) {
			out << *element;
			element = element->getNextElement();
		}
		return out;
	};
};

template<class T>
class BinarySearchTreeTemplate{
public:
	void put(long ID, T* value);
	T* get(const long ID);
	T* remove(const long ID);
	void printTree();
	BinarySearchTreeTemplate();
	~BinarySearchTreeTemplate();
private:
	TreeElementTemplate<T>* root;
	TreeElementTemplate<T>* getElement(const long ID);
	void printTree(TreeElementTemplate<T>* node);
	void rotate(TreeElementTemplate<T>* axis);
};

#endif