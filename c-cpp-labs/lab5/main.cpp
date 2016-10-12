//Bart Middag
//main.cpp

#include "BinarySearchTree.h"
#include "BinarySearchTreeTemplate.h"
#include "BinarySearchTreeSTL.h"
#include "BinarySearchTree.h"
#include "BinarySearchTreeTemplate.h"
#include "BodyTemperature.h"
#include "BloodPressure.h"
#include "HeartRate.h"
#include "ParameterValue.h"
#include <iostream>

int main() {

	//BinarySearchTree* bst = new BinarySearchTree();
	//BinarySearchTreeTemplate<ParameterValue>* bst = new BinarySearchTreeTemplate<ParameterValue>();
	BinarySearchTreeSTL<ParameterValue>* bst = new BinarySearchTreeSTL<ParameterValue>();

	HeartRate* h = new HeartRate();
	BodyTemperature* b = new BodyTemperature();
	BloodPressure* bp = new BloodPressure();
	Timestamp* t = new Timestamp();
	Timestamp* t2 = new Timestamp();
	Timestamp* t3 = new Timestamp();
	Value* v = new Value();
	Value* v2 = new Value();
	Value* v3 = new Value();
	Value* v4 = new Value();
	
	t->setDate("15/11/2012");
	t->setHour("11:07");
	t2->setDate("15/11/2012");
	t2->setHour("11:37");
	t3->setDate("15/11/2012");
	t3->setHour("14:20");
	v->setValue(58);
	v->setUnit("bpm");
	v2->setValue(36);
	v2->setUnit("Celsius");
	v3->setValue(112);
	v3->setUnit("mmHg");
	v4->setValue(89);
	v4->setUnit("mmHg");
	h->setHeartRate(*v);
	h->setTimestamp(*t);
	b->setTemperature(*v2);
	b->setTimestamp(*t2);
	bp->setSystolicPressure(*v3);
	bp->setDiastolicPressure(*v4);
	bp->setTimestamp(*t3);

	bst->put(1000, h);
	bst->put(500, b);
	bst->put(300, bp);
	bst->put(700, bp);
	bst->put(700, b);
	bst->put(1500, b);
	bst->put(700, h);
	bst->put(1300, bp);
	bst->put(2000, bp);
	bst->printTree();

	cout << "==================================\n";

	bst->remove(700);
	bst->remove(700);
	bst->remove(700);
	bst->remove(2000);
	bst->remove(1300);
	bst->remove(1000);
	bst->remove(1500);
	bst->remove(300);
	bst->remove(500);

	cout << "FINAL FINAL FINAL FINAL FINAL FINAL\n";
	cout << "==================================\n";
	bst->printTree();

	delete v;
	delete v2;
	delete v3;
	delete v4;

	delete t;
	delete t2;
	delete t3;

	delete h;
	delete b;
	delete bp;

	delete bst;
	getchar();
	_CrtDumpMemoryLeaks();
	return 0;
}