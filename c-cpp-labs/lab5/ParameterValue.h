//Bart Middag
//ParameterValue.h

#ifndef PARAMETERVALUE_H
#define PARAMETERVALUE_H
#include <string>
#include <sstream> 
#include <iostream>
using namespace std;

class Timestamp{
private:
	string date;
	string hour;

public:
	Timestamp() {};
	~Timestamp() {};

	void setDate(string date){
		this->date = date;
	}

	string getDate(){
		return date;
	}
	
	void setHour(string hour){
		this->hour = hour;
	}

	string getHour(){
		return hour;
	}

	friend ostream& operator<<(ostream& stream, const Timestamp& t) {
		stream << t.date << ", " << t.hour;
		return stream;
	}
};

class Value{
private:
	float value;
	string unit;

public:
	Value() {};
	~Value() {};

	void setValue(float value){
		this->value = value;
	}

	float getValue(){
		return value;
	}

	void setUnit(string unit){
		this->unit = unit;
	}

	string getUnit(){
		return unit;
	}

	friend ostream& operator<<(ostream& stream, const Value& v) {
		stream << v.value << " " << v.unit;
		return stream;
	}
};

class ParameterValue{
public:
	ParameterValue() {};
	virtual ~ParameterValue() {};

	void setTimestamp(Timestamp timestamp){
		this->timestamp = timestamp;
	}

	Timestamp getTimestamp(){
		return timestamp;
	}

	void setValue(Value value){
		this->value = value;
	}

	Value getValue(){
		return value;
	}

	virtual string getDescription() {
		return "Waarde";
	}

	virtual string getDiagnosis() {
		return "???";
	}

	virtual void rescale(float factor) {
		//Omdat we enkel moeten aantonen dat we overweg kunnen met het virtual-aspect,
		//heb ik deze methode niet overal geïmplementeerd - enkel in HeartRate en BodyTemperature.
		//Vermits de bedoeling van rescale is om de waarde te herschalen en niet te veranderen, doet
		//de methode niets als de factor niet gelijk is aan één van de voorgeprogrammerde factors. 
	}

	virtual ostream& printTo(ostream& stream) {
		stream << getDescription() << ": " << value << " [" << getDiagnosis() << "] (" << timestamp << ")";
		return stream;
	}

	friend ostream& operator<<(ostream& stream, ParameterValue& v) {
		//We overloaden de operator<< voor makkelijke prints
		//die blijven werken als we niet weten wat ons element bevat (zie template)
		//Deze operator kunnen we echter niet overerven, dus verwijzen we naar een virtual-methode in de klasse zelf.
		return v.printTo(stream);
	}

protected:
	Timestamp timestamp;
	Value value;
};

#endif