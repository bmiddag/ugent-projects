//Bart Middag
//BinarySearchTreeSTL.h

#ifndef BINARYSEARCHTREESTL_H
#define BINARYSEARCHTREESTL_H
#include <string>
#include <sstream>
#include <list>
using namespace std;

//Voor commentaar, zie BinarySearchTree.h!
//Het grote verschil met de Template-versie is dat ik hier ID in TreeElementSTL heb gestoken
//en Element niet meer bestaat (dit is nu gewoon een deel van list).

template<class T>
class TreeElementSTL {
private:
	TreeElementSTL<T>* leftTree;
	TreeElementSTL<T>* rightTree;
	TreeElementSTL<T>* parent;
	list<T*> values;
	long ID;
public:
	TreeElementSTL(T* element, long ID) {
		this->ID = ID;
		this->values.push_back(element);
		this->parent = NULL;
		this->leftTree = NULL;
		this->rightTree = NULL;
	};
	~TreeElementSTL() {
		delete leftTree;
		delete rightTree;
	};

	long getID(){
		return ID;
	};

	TreeElementSTL<T>* getLeftTree(){
		return leftTree;
	};
	TreeElementSTL<T>* getRightTree(){
		return rightTree;
	};
	TreeElementSTL<T>* getParent(){
		return parent;
	};
	T* getValue(){
		return this->values.front();
	};
	list<T*>* getValues(){
		return &this->values;
	}

	void setLeftTree(TreeElementSTL<T>* leftTree){
		this->leftTree = leftTree;
		if(leftTree!=NULL) leftTree->setParent(this);
	};
	void setRightTree(TreeElementSTL<T>* rightTree){
		this->rightTree = rightTree;
		if(rightTree!=NULL) rightTree->setParent(this);
	};
	void setParent(TreeElementSTL<T>* parent){
		this->parent = parent;
	};
	/*void setValue(ElementSTL<T>* value){
		this->value = value;
	};*/
	template<class T>
	friend ostream& operator<<(ostream& out, const TreeElementSTL<T>& treeElement) {
		ostringstream IDstream;
		IDstream << " " << treeElement.ID << " ";
		string ID = IDstream.str();
		int i = 0;
		int length = ID.length();
		int leading = (34 - length) /2;
		int trailing = 34-leading-length;
		for(i = 0; i < leading; i++) out << "=";
		out << ID;
		for(i = 0; i < trailing; i++) out << "=";
		out << "\n";
		//Overloop de lijst met een iterator
		for(list<T*>::const_iterator iterator = treeElement.values.begin(), end = treeElement.values.end(); iterator != end; ++iterator) {
			out << "- " << **iterator << "\n";
		}
		return out;
	};
};

template<class T>
class BinarySearchTreeSTL{
public:
	void put(long ID, T* value);
	T* get(const long ID);
	T* remove(const long ID);
	void printTree();
	BinarySearchTreeSTL();
	~BinarySearchTreeSTL();
private:
	TreeElementSTL<T>* root;
	TreeElementSTL<T>* getElement(const long ID);
	void printTree(TreeElementSTL<T>* node);
	void rotate(TreeElementSTL<T>* axis);
};

#endif