//Bart Middag
//HeartRate.h

#ifndef HEARTRATE_H
#define HEARTRATE_H

class HeartRate : public ParameterValue{
public:
	HeartRate() {};
	~HeartRate() {};

	void setHeartRate(Value heartrate){
		ParameterValue::setValue(heartrate);
	}

	Value getHeartRate(){
		return ParameterValue::getValue();
	}

	string getDescription() {
		return "Hartslag";
	}

	string getDiagnosis() {
		if(value.getUnit() == "bpm") {
			if(value.getValue() > 100){
				return "Tachycardie";
			} else if(value.getValue() < 60) {
				return "Bradycardie";
			} else {
				return "OK";
			}
		} else return "???";
	}

	void rescale(float factor) {
		if((factor == 60) && (value.getUnit() == "bpm")) {
			value.setValue(value.getValue()*factor);
			value.setUnit("bps");
		} else if((factor == 1/60) && (value.getUnit() == "bps")) {
			value.setValue(value.getValue()*factor);
			value.setUnit("bpm");
		}
	}
};

#endif