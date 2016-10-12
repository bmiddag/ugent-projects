//Bart Middag
//BloodPressure.h

#ifndef BLOODPRESSURE_H
#define BLOODPRESSURE_H

//Dit is een speciaal geval. BloodePressure heeft 2 values.
//We houden dus de twee values bij inde klasse zelf en geven het niet door aan de basisklasse.

class BloodPressure : public ParameterValue {
public:
	BloodPressure() {};
	~BloodPressure() {};

	void setSystolicPressure(Value systolic){
		this->systolic = systolic;
	}

	Value getSystolicPressure(){
		return systolic;
	}

	void setDiastolicPressure(Value diastolic){
		this->diastolic = diastolic;
	}

	Value getDiastolicPressure(){
		return diastolic;
	}

	string getDescription() {
		return "Bloeddruk";
	}

	string getDiagnosis() {
		if((systolic.getUnit() == "mmHg") && (diastolic.getUnit() == "mmHg")) {
			if((systolic.getValue() > 140) || (diastolic.getValue() > 90)) {
				return "Hypertensie";
			} else if((systolic.getValue() < 90) || (diastolic.getValue() < 60)) {
				return "Hypotensie";
			} else {
				return "OK";
			}
		} else return "???";
	}

	ostream& printTo(ostream& stream) {
		stream << getDescription() << ": bovendruk: " << systolic << ", onderdruk: " << diastolic << " [" << getDiagnosis() << "] (" << timestamp << ")";
		return stream;
	}

private:
	Value systolic;
	Value diastolic;
};

#endif