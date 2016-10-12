//Bart Middag
//BinarySearchTreeSTL.cpp

//DECLARATIONS
#include <stdlib.h>
#include "BinarySearchTreeSTL.h"

//Voor commentaar, zie BinarySearchTree.cpp!
//Het grote verschil met de Template-versie is dat ik hier ID in TreeElementSTL heb gestoken
//en Element niet meer bestaat (dit is nu gewoon een deel van list).

//CONSTRUCTOR
template<class T>
BinarySearchTreeSTL<T>::BinarySearchTreeSTL(){
	// lege boom om te beginnen
	root = NULL;
}

//DESTRUCTOR
template<class T>
BinarySearchTreeSTL<T>::~BinarySearchTreeSTL(){
	delete root;
}

//PUBLIC METHODS
template<class T>
void BinarySearchTreeSTL<T>::printTree() {
	printTree(root);
}

template<class T>
T* BinarySearchTreeSTL<T>::get(const long ID) {
	TreeElementSTL<T> *element = getElement(ID);
	if(element) {
		return element->getValue();
	} else return NULL;
}

template<class T>
void BinarySearchTreeSTL<T>::put(long ID, T* e)  {
	TreeElementSTL<T> *treeElement, *previous;
	if(root == NULL) {
		root = new TreeElementSTL<T>(e,ID);
		return;
	}
	treeElement = getElement(ID);
	if(treeElement != NULL) {
		treeElement->getValues()->push_back(e);
	} else {
		//We kunnen treeElement hergebruiken
		previous = root;
		if(ID < root->getID()) {
			treeElement = root->getLeftTree();
		} else treeElement = root->getRightTree();
		while(previous != NULL && treeElement != NULL) {
			previous = treeElement;
			if(ID < treeElement->getID()) {
				treeElement = treeElement->getLeftTree();
			} else treeElement = treeElement->getRightTree();
		}
		//we kunnen treeElement weer hergebruiken
		treeElement = new TreeElementSTL<T>(e,ID);
		if(ID < previous->getID()) {
			previous->setLeftTree(treeElement);
		} else previous->setRightTree(treeElement);
		rotate(treeElement);
	}
}

template<class T>
T* BinarySearchTreeSTL<T>::remove(const long ID) {
	TreeElementSTL<T> *element, *new_child;
	T *out = get(ID);
	element = getElement(ID);
	if(element != NULL) {
		if(element->getValues()->size() == 1) {
			if(element->getLeftTree() == NULL || element->getRightTree() == NULL) {
				if(element->getRightTree() == NULL) {
					new_child = element->getLeftTree();
				} else new_child = element->getRightTree();
			} else {
				new_child = element->getRightTree();
				while(new_child->getLeftTree() != NULL) {
					new_child = new_child->getLeftTree();
				}
				if(new_child->getParent()->getLeftTree() == new_child) new_child->getParent()->setLeftTree(NULL);
				if(new_child->getParent()->getRightTree() == new_child) new_child->getParent()->setRightTree(NULL);
				new_child->setLeftTree(element->getLeftTree());
				new_child->setRightTree(element->getRightTree());

			}
			if(element->getParent() != NULL) {
				if(element->getParent()->getLeftTree() == element) element->getParent()->setLeftTree(new_child);
				if(element->getParent()->getRightTree() == element) element->getParent()->setRightTree(new_child);
			}
			if(element == root) root = new_child;
			element->setLeftTree(NULL);
			element->setRightTree(NULL);
			delete element;
		} else {
			element->getValues()->erase(element->getValues()->begin());
		}
	}
	return out;
}

//PRIVATE METHODS
template<class T>
void BinarySearchTreeSTL<T>::rotate(TreeElementSTL<T>* axis) {
	TreeElementSTL<T> *parent = axis->getParent();
	TreeElementSTL<T> *parentLeft = axis->getParent()->getLeftTree();
	TreeElementSTL<T> *parentRight = axis->getParent()->getRightTree();
	TreeElementSTL<T> *axisLeft = axis->getLeftTree();
	TreeElementSTL<T> *axisRight = axis->getRightTree();
	TreeElementSTL<T> *parentParent = parent->getParent();

	if (axis == parentLeft) {
		axis->setRightTree(parent);
		axis->setParent(parentParent);
		parent->setLeftTree(axisRight);
		parent->setParent(axis);
		if (axisRight != NULL) {
			axisRight->setParent(parent);
		}
		
		if (parentParent != NULL) {
			if (parentParent->getRightTree() == parent) {
				parentParent->setRightTree(axis);
			} else {
				parentParent->setLeftTree(axis);
			}
		}
	}
	else if (axis == parentRight) {
		axis->setLeftTree(parent);
		axis->setParent(parentParent);
		parent->setRightTree(axisLeft);
		parent->setParent(axis);
		if (axisLeft != NULL) {
			axisLeft->setParent(parent);
		}
		
		if (parentParent != NULL) {
			if (parentParent->getRightTree() == parent) {
				parentParent->setRightTree(axis);
			} else {
				parentParent->setLeftTree(axis);
			}
		}
	}

	if (parent != root) {
		rotate(axis);
	} else {
		root = axis;
	}
}

template<class T>
TreeElementSTL<T>* BinarySearchTreeSTL<T>::getElement(const long ID) {
	TreeElementSTL<T>* treeElement;
	treeElement = root;
	while(treeElement != NULL) {
		if(ID == treeElement->getID()) return treeElement;
		if(ID < treeElement->getID()) {
			treeElement = treeElement->getLeftTree();
		} else treeElement = treeElement->getRightTree();
	}
	return NULL;
}

template<class T>
void BinarySearchTreeSTL<T>::printTree(TreeElementSTL<T>* node) {
	if(node != NULL) {
		printTree(node->getLeftTree());
		cout << *node;
		printTree(node->getRightTree());
	}
}