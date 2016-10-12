//Bart Middag
//BinarySearchTreeTemplate.cpp

//DECLARATIONS
#include <stdlib.h>
#include "BinarySearchTreeTemplate.h"

//Voor commentaar, zie BinarySearchTree.cpp!

//CONSTRUCTOR
template<class T>
BinarySearchTreeTemplate<T>::BinarySearchTreeTemplate(){
	// lege boom om te beginnen
	root = NULL;
}

//DESTRUCTOR
template<class T>
BinarySearchTreeTemplate<T>::~BinarySearchTreeTemplate(){
	delete root;
}

//PUBLIC METHODS
template<class T>
void BinarySearchTreeTemplate<T>::printTree() {
	printTree(root);
}

template<class T>
T* BinarySearchTreeTemplate<T>::get(const long ID) {
	TreeElementTemplate<T> *element = getElement(ID);
	if(element) {
		return element->getValue()->getValue();
	} else return NULL;
}

template<class T>
void BinarySearchTreeTemplate<T>::put(long ID, T* e)  {
	ElementTemplate<T> *element;
	TreeElementTemplate<T> *treeElement, *previous;
	if(root == NULL) {
		root = new TreeElementTemplate<T>(new ElementTemplate<T>(ID,e));
		return;
	}
	treeElement = getElement(ID);
	if(treeElement != NULL) {
		element = treeElement->getValue();
		while(element->getNextElement()) {
			element = element->getNextElement();
		}
		element->setNextElement(new ElementTemplate<T>(ID,e));
	} else {
		//We kunnen treeElement hergebruiken
		previous = root;
		if(ID < root->getValue()->getID()) {
			treeElement = root->getLeftTree();
		} else treeElement = root->getRightTree();
		while(previous != NULL && treeElement != NULL) {
			previous = treeElement;
			if(ID < treeElement->getValue()->getID()) {
				treeElement = treeElement->getLeftTree();
			} else treeElement = treeElement->getRightTree();
		}
		//we kunnen treeElement weer hergebruiken
		treeElement = new TreeElementTemplate<T>(new ElementTemplate<T>(ID,e));
		if(ID < previous->getValue()->getID()) {
			previous->setLeftTree(treeElement);
		} else previous->setRightTree(treeElement);
		rotate(treeElement);
	}
}

template<class T>
T* BinarySearchTreeTemplate<T>::remove(const long ID) {
	TreeElementTemplate<T> *element, *new_child;
	T *out = get(ID);
	element = getElement(ID);
	if(element != NULL) {
		if(element->getValue()->getNextElement() == NULL) {
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
			ElementTemplate<T>* to_delete = element->getValue();
			element->setValue(element->getValue()->getNextElement());
			delete to_delete;
		}
	}
	return out;
}

//PRIVATE METHODS
template<class T>
void BinarySearchTreeTemplate<T>::rotate(TreeElementTemplate<T>* axis) {
	TreeElementTemplate<T> *parent = axis->getParent();
	TreeElementTemplate<T> *parentLeft = axis->getParent()->getLeftTree();
	TreeElementTemplate<T> *parentRight = axis->getParent()->getRightTree();
	TreeElementTemplate<T> *axisLeft = axis->getLeftTree();
	TreeElementTemplate<T> *axisRight = axis->getRightTree();
	TreeElementTemplate<T> *parentParent = parent->getParent();

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
TreeElementTemplate<T>* BinarySearchTreeTemplate<T>::getElement(const long ID) {
	TreeElementTemplate<T>* treeElement;
	long elementID;
	treeElement = root;
	while(treeElement != NULL) {
		elementID = treeElement->getValue()->getID();
		if(ID == elementID) return treeElement;
		if(ID < elementID) {
			treeElement = treeElement->getLeftTree();
		} else treeElement = treeElement->getRightTree();
	}
	return NULL;
}

template<class T>
void BinarySearchTreeTemplate<T>::printTree(TreeElementTemplate<T>* node) {
	if(node != NULL) {
		printTree(node->getLeftTree());
		ElementTemplate<T> *element = node->getValue();
		cout << *node;
		printTree(node->getRightTree());
	}
}