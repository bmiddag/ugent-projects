package com.itech.events;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TextView;

public class AddConfirmationActivity extends Activity implements
		JSONConnectionListener {

	private String URL;
	private String peopleURL;
	protected String method;
	private Spinner spinnerName;
	private Spinner spinnerGoing;
	private List<Person> people;
	private List<String> goingItems;
	
	private static int GET_PEOPLE_REQUEST = 0;
	private static int ADD_CONFIRMATION_REQUEST = 1;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_add_confirmation);
		URL = getIntent().getStringExtra("URL");
		peopleURL = getIntent().getStringExtra("peopleURL");
		method = getIntent().getStringExtra("method");
		people = new ArrayList<Person>();
		goingItems = new ArrayList<String>();
		if(getIntent().hasExtra("JSON")) {
			((TextView)findViewById(R.id.title_add_event_conf)).setText(getString(R.string.action_edit_confirmation));
			setTitle(getString(R.string.title_activity_edit_confirmation));
		}
		fillSpinnerConfirmationName();
		fillSpinnerConfirmationStatus();
	}

	private void fillSpinnerConfirmationStatus() {
		spinnerGoing = (Spinner) findViewById(R.id.spinner_event_conf_status);
		String[] items = new String[] { "attending", "absent" };
		ArrayAdapter<String> adapter2 = new ArrayAdapter<String>(this,
				android.R.layout.simple_spinner_item, items);
		spinnerGoing.setAdapter(adapter2);
	}

	private void fillSpinnerConfirmationName() {
		spinnerName = (Spinner) findViewById(R.id.spinner_event_conf_name);
		new JSONConnectionHandler("GET", peopleURL, this, this, null, GET_PEOPLE_REQUEST).execute();
	}

	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		try {
			if(requestCode == GET_PEOPLE_REQUEST) {
				JSONArray peopleArray = (JSONArray) json.get("people");
				for (int i = 0; i < peopleArray.length(); i++) {
					Person person = Person.parse(peopleArray.getJSONObject(i));
					people.add(person);
					goingItems.add(person.getName());
				}
				ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,
						android.R.layout.simple_spinner_item, goingItems);
				spinnerName.setAdapter(adapter);
				if(getIntent().hasExtra("JSON")) {
					String jsonString = getIntent().getStringExtra("JSON");
					try {
						JSONObject jsonObject = new JSONObject(jsonString);
						Confirmation c = Confirmation.parse(jsonObject);
						// Set name & confirmation status
						spinnerName.setSelection(people.indexOf(c.getPerson()));
						spinnerGoing.setSelection(c.isGoing() ? 0 : 1);
					} catch (Exception e) {
						e.printStackTrace();
					}
				}
			} else if(requestCode == ADD_CONFIRMATION_REQUEST) {
				if (!json.has("error")) {
					setResult(RESULT_OK);
					finish();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	public void post(View view) {
		int personpos = spinnerName.getSelectedItemPosition();
		boolean isgoing = (spinnerGoing.getSelectedItemPosition() == 0);
		Confirmation confirmation = new Confirmation(people.get(personpos),
				isgoing);
		if(getIntent().hasExtra("JSON")) {
			new JSONConnectionHandler(method, URL, this, this,
					confirmation.toFinalJSON(), ADD_CONFIRMATION_REQUEST,
					getString(R.string.progress_title),
					getString(R.string.progress_edit_conf)).execute();
		} else {
			new JSONConnectionHandler(method, URL, this, this,
					confirmation.toFinalJSON(), ADD_CONFIRMATION_REQUEST,
					getString(R.string.progress_title),
					getString(R.string.progress_add_conf)).execute();
		}
	}
	
	public void back(View view) {
		finish();
	}
}
