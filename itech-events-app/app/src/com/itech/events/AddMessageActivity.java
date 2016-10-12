package com.itech.events;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

public class AddMessageActivity extends Activity implements JSONConnectionListener {
	
	private String URL;
	private String peopleURL;
	protected String method;
	private Spinner spinnerName;
	private List<Person> people;
	
	private static int GET_PEOPLE_REQUEST = 0;
	private static int ADD_MESSAGE_REQUEST = 1;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_add_message);
		Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		peopleURL = intent.getStringExtra("peopleURL");
		method = intent.getStringExtra("method");
		people = new ArrayList<Person>();
		if (method.equals("PATCH")) {
			((TextView) findViewById(R.id.title_add_event_mesg))
					.setText(getString(R.string.action_edit_message));
			setTitle(getString(R.string.title_activity_edit_message));
		}
		fillSpinnerMessageName();
	}

	private void fillSpinnerMessageName() {
		spinnerName = (Spinner) findViewById(R.id.spinner_event_mesg_name);
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
				}
				ArrayAdapter<Person> adapter = new ArrayAdapter<Person>(this,
						android.R.layout.simple_spinner_item, people);
				spinnerName.setAdapter(adapter);
				if (method.equals("PATCH")) {
					String jsonString = getIntent().getStringExtra("JSON");
					try {
						JSONObject jsonObject = new JSONObject(jsonString);
						Message message = Message.parse(jsonObject);
						// Set message & person
						((EditText) findViewById(R.id.et_message)).setText(message.getText());
						spinnerName.setSelection(people.indexOf(message.getPerson()));
					} catch (JSONException e) {
						e.printStackTrace();
					}
				}
			} else if(requestCode == ADD_MESSAGE_REQUEST) {
				if (json == null || !json.has("error")) {
					setResult(RESULT_OK);
					this.finish();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public void post(View view) {
		int personpos = spinnerName.getSelectedItemPosition();
		
		EditText editName = (EditText) findViewById(R.id.et_message);
		String messageText = editName.getText().toString();
		if (messageText == null || messageText.isEmpty()) {
			Toast.makeText(AddMessageActivity.this, "Please enter a message.",
					Toast.LENGTH_LONG).show();
			return;
		}
		
		Message message = new Message(messageText, people.get(personpos));
		if(method.equals("PATCH")) {
			new JSONConnectionHandler(method, URL, this, this,
					message.toFinalJSON(), ADD_MESSAGE_REQUEST,
					getString(R.string.progress_title),
					getString(R.string.progress_edit_msg)).execute();
		} else {
			new JSONConnectionHandler(method, URL, this, this,
					message.toFinalJSON(), ADD_MESSAGE_REQUEST,
					getString(R.string.progress_title),
					getString(R.string.progress_add_msg)).execute();
		}
	}
	
	public void back(View view) {
		finish();
	}
}
