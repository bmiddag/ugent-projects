//Bart Middag
//BinarySearchTree.cpp

//DECLARATIONS
#include <stdlib.h>
#include "BinarySearchTree.h"
#include "ParameterValue.h"

//CONSTRUCTOR
BinarySearchTree::BinarySearchTree(){
	// lege boom om te beginnen
	root = NULL;
}

//DESTRUCTOR
BinarySearchTree::~BinarySearchTree(){
	delete root;
}

//PUBLIC METHODS
void BinarySearchTree::printTree() {
	printTree(root);
}

ParameterValue* BinarySearchTree::get(const long patientID) {
	TreeElement *element = getElement(patientID);
	if(element) {
		return element->getValue()->getValue();
	} else return NULL;
}
void BinarySearchTree::put(long patientID, ParameterValue* e)  {
	Element *element;
	TreeElement *treeElement, *previous;
	if(root == NULL) {
		root = new TreeElement(new Element(patientID,e));
		return;
	}
	treeElement = getElement(patientID);
	if(treeElement != NULL) {
		element = treeElement->getValue();
		while(element->getNextElement()) {
			element = element->getNextElement();
		}
		element->setNextElement(new Element(patientID,e));
	} else {
		//We kunnen treeElement hergebruiken
		previous = root;
		if(patientID < root->getValue()->getPatientID()) {
			treeElement = root->getLeftTree();
		} else treeElement = root->getRightTree();
		while(previous != NULL && treeElement != NULL) {
			previous = treeElement;
			if(patientID < treeElement->getValue()->getPatientID()) {
				treeElement = treeElement->getLeftTree();
			} else treeElement = treeElement->getRightTree();
		}
		//we kunnen treeElement weer hergebruiken
		treeElement = new TreeElement(new Element(patientID,e));
		if(patientID < previous->getValue()->getPatientID()) {
			previous->setLeftTree(treeElement);
		} else previous->setRightTree(treeElement);
		rotate(treeElement);
	}
}
ParameterValue* BinarySearchTree::remove(const long patientID) {
	TreeElement *element, *new_child;
	ParameterValue *out = get(patientID);
	element = getElement(patientID);
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
			Element* to_delete = element->getValue();
			element->setValue(element->getValue()->getNextElement());
			delete to_delete;
		}
	}
	return out;
}

//PRIVATE METHODS
void BinarySearchTree::rotate(TreeElement* axis) {
	TreeElement* parent = axis->getParent();
	TreeElement* parentLeft = axis->getParent()->getLeftTree();
	TreeElement* parentRight = axis->getParent()->getRightTree();
	TreeElement* axisLeft = axis->getLeftTree();
	TreeElement* axisRight = axis->getRightTree();
	TreeElement* parentParent = parent->getParent();

	if (axis == parentLeft) {
		axis->setRightTree(parent);
		//Aangepast van parent->getParent() wat niet klopte als ik bij setRightTree parent al verander.
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

//Hulpfunctie om het TreeElement te bepalen met dit patientID.
TreeElement* BinarySearchTree::getElement(const long patientID) {
	TreeElement* treeElement;
	long elementID;
	treeElement = root;
	while(treeElement != NULL) {
		elementID = treeElement->getValue()->getPatientID();
		if(patientID == elementID) return treeElement;
		if(patientID < elementID) {
			treeElement = treeElement->getLeftTree();
		} else treeElement = treeElement->getRightTree();
	}
	return NULL;
}

void BinarySearchTree::printTree(TreeElement* node) {
	if(node != NULL) {
		printTree(node->getLeftTree());
		Element *element = node->getValue();
		//We overloaden operator<<, zie header.
		cout << *node;
		printTree(node->getRightTree());
	}
}