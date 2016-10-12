//Bart Middag
//BodyTemperature.h

#ifndef BODYTEMPERATURE_H
#define BODYTEMPERATURE_H

class BodyTemperature : public ParameterValue{
public:
	BodyTemperature() {};
	~BodyTemperature() {};

	void setTemperature(Value temperature){
		ParameterValue::setValue(temperature);
	}

	Value getTemperature(){
		return ParameterValue::getValue();
	}

	string getDescription() {
		return "Lichaamstemperatuur";
	}

	string getDiagnosis() {
		if(value.getUnit() == "Celsius") {
			if((value.getValue() > 42) || (value.getValue() < 27)) {
				return "Dodelijk";
			} else if(value.getValue() > 37.7){
				return "Koorts";
			} else if(value.getValue() < 35) {
				return "Onderkoeling";
			} else {
				return "OK";
			}
		} else return "???";
	}

	void rescale(float factor) {
		//Celsius naar Fahrenheit converteren of omgekeerd met enkel een factor gaat niet
		if((factor == 0.8) && (value.getUnit() == "Celsius")) {
			value.setValue(value.getValue()*factor);
			value.setUnit("Réaumur");
		} else if((factor == 1.25) && (value.getUnit() == "Réaumur")) {
			value.setValue(value.getValue()*factor);
			value.setUnit("Celsius");
		}
	}
};

#endif