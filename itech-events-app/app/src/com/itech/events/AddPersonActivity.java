package com.itech.events;

import java.util.Calendar;
import java.util.Date;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.DatePickerDialog;
import android.app.DatePickerDialog.OnDateSetListener;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

public class AddPersonActivity extends Activity implements OnDateSetListener, JSONConnectionListener {
	private String URL;
	private String method;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_add_person);
		Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		// method will be "POST" when adding and "PATCH" when editing
		method = intent.getStringExtra("method");
		if(method.equals("PATCH")) {
			((TextView)findViewById(R.id.title_add_person)).setText(getString(R.string.person_edit_title));
			setTitle(getString(R.string.title_activity_edit_person));
			//progressString = getString(R.string.progress_edit_person);
			String jsonString = intent.getStringExtra("JSON");
			try {
				JSONObject json = new JSONObject(jsonString);
				Person person = Person.parse(json);
				// Set date
				TextView date_text = (TextView)findViewById(R.id.txt_birth_date);
				date_text.setText(DateParser.toDateString(person.getBirthDate()));
				// Set name & email
				((EditText)findViewById(R.id.et_name)).setText(person.getName());
				((EditText)findViewById(R.id.et_email)).setText(person.getEmail());
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	public void post(View view) {
		TextView dateText = (TextView)findViewById(R.id.txt_birth_date);
		Date birthDate = DateParser.parse(dateText.getText().toString(), "dd/MM/yyyy");
		if(birthDate == null) {
			Toast.makeText(AddPersonActivity.this, "The date of birth is incorrect.", Toast.LENGTH_LONG).show();
			return;
		}
		EditText editName = (EditText)findViewById(R.id.et_name);
		String name = editName.getText().toString();
		if(name == null || name.isEmpty()) {
			Toast.makeText(AddPersonActivity.this, "Please enter a name.", Toast.LENGTH_LONG).show();
			return;
		}
		EditText editEmail = (EditText)findViewById(R.id.et_email);
		String email = editEmail.getText().toString();
		if(email == null || email.isEmpty()) {
			Toast.makeText(AddPersonActivity.this, "Please enter an e-mail address.", Toast.LENGTH_LONG).show();
			return;
		}
		
		Person person = new Person(name, email, birthDate);
		if(method.equals("PATCH")) {
			new JSONConnectionHandler(method,URL,this,this,person.toFinalJSON(),
					getString(R.string.progress_title),
					getString(R.string.progress_edit_person)).execute();
		} else {
			new JSONConnectionHandler(method,URL,this,this,person.toFinalJSON(),
					getString(R.string.progress_title),
					getString(R.string.progress_add_person)).execute();
		}
	}
	
	public void setBirthDate(View view) {
		TextView dateText = (TextView)findViewById(R.id.txt_birth_date);
		Date birthDate = DateParser.parse(dateText.getText().toString(), "dd/MM/yyyy");
		Calendar c = Calendar.getInstance();
		if(birthDate != null) c.setTime(birthDate);
		DatePickerDialog date = new DatePickerDialog(this, this, c.get(Calendar.YEAR),
				c.get(Calendar.MONTH), c.get(Calendar.DAY_OF_MONTH));
		date.show();
	}
	
	public void back(View view) {
		finish();
	}

	@Override
	public void onDateSet(DatePicker picker, int year, int month, int day) {
		// Get start of today
		Calendar c = Calendar.getInstance();
		c.set(Calendar.HOUR, 0);
		c.set(Calendar.MINUTE, 0);
		c.set(Calendar.SECOND, 0);
		Date today = c.getTime();
		
		c.set(Calendar.YEAR, year);
		c.set(Calendar.MONTH, month);
		c.set(Calendar.DAY_OF_MONTH, day);
		Date picked = c.getTime();
		
		if(picked.before(today)) {
			// set selected date into Text View
			TextView date_text = (TextView)findViewById(R.id.txt_birth_date);
			date_text.setText(String.format("%02d/%02d/%04d", day, month+1, year));
		} else {
			Toast.makeText(this, "This person is a paradox if (s)he was born in the future!", Toast.LENGTH_LONG).show();
		}
	}
	
	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		if(json==null || !json.has("error")) {
			setResult(RESULT_OK);
			this.finish();
		}
	}
	
}