package com.itech.events;

import java.util.Calendar;
import java.util.Date;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.DatePickerDialog;
import android.app.DatePickerDialog.OnDateSetListener;
import android.app.TimePickerDialog;
import android.app.TimePickerDialog.OnTimeSetListener;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.TimePicker;
import android.widget.Toast;

public class AddEventActivity extends Activity implements OnDateSetListener,
		JSONConnectionListener, OnTimeSetListener {
	TimePickerDialog tpdialog;
	protected String URL;
	protected String method;
	protected String error = null;
	private boolean isStartPicker;
	private boolean fired;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_add_event);
		Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		method = intent.getStringExtra("method");

		if (method.equals("PATCH")) {
			((TextView) findViewById(R.id.title_add_event))
					.setText(getString(R.string.event_edit_title));
			setTitle(getString(R.string.title_activity_edit_event));
			// progressString = getString(R.string.progress_edit_person);
			String jsonString = intent.getStringExtra("JSON");
			try {
				JSONObject json = new JSONObject(jsonString);
				Event event = Event.parse(json);
				// Set date
				TextView startdate_text = (TextView) findViewById(R.id.txt_start_date);
				startdate_text.setText(DateParser.toDateTimeString(event.getStartDate()));
				TextView enddate_text = (TextView) findViewById(R.id.txt_end_date);
				enddate_text.setText(DateParser.toDateTimeString(event.getEndDate()));
				// Set name & description
				((EditText) findViewById(R.id.et_eventname)).setText(event
						.getTitle());
				((EditText) findViewById(R.id.et_description)).setText(event
						.getDescription());
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		if(json==null || !json.has("error")) {
			setResult(RESULT_OK);
			this.finish();
		}
	}

	public void setStartDate(View view) {
		TextView dateText = (TextView) findViewById(R.id.txt_start_date);
		Calendar startCalendar = Calendar.getInstance();
		Date startDate = DateParser.parse(dateText.getText().toString(), "dd/MM/yyyy HH:mm");
		if(startDate != null) startCalendar.setTime(startDate);
		fired = false;
		isStartPicker = true;
		new DatePickerDialog(this, this,
				startCalendar.get(Calendar.YEAR),
				startCalendar.get(Calendar.MONTH),
				startCalendar.get(Calendar.DAY_OF_MONTH)).show();
	}

	public void setEndDate(View view) {
		TextView dateText = (TextView) findViewById(R.id.txt_end_date);
		Calendar endCalendar = Calendar.getInstance();
		Date endDate = DateParser.parse(dateText.getText().toString(), "dd/MM/yyyy HH:mm");
		if(endDate != null) endCalendar.setTime(endDate);
		fired = false;
		isStartPicker = false;
		new DatePickerDialog(this, this,
				endCalendar.get(Calendar.YEAR),
				endCalendar.get(Calendar.MONTH),
				endCalendar.get(Calendar.DAY_OF_MONTH)).show();
	}

	@Override
	public void onDateSet(DatePicker picker, int year, int month, int day) {
		// Android Time & Datepicker Double Fire bug protection
		if(fired) return;
		fired = true;
		TextView dateText;
		Calendar c = Calendar.getInstance();
		if (isStartPicker) {
			dateText = (TextView) findViewById(R.id.txt_start_date);
		} else dateText = (TextView) findViewById(R.id.txt_end_date);
		Date date = DateParser.parse(dateText.getText().toString(),"dd/MM/yyyy HH:mm");
		if(date != null) {
			c.setTime(date);
		}
		c.set(Calendar.YEAR, year);
		c.set(Calendar.MONTH, month);
		c.set(Calendar.DAY_OF_MONTH, day);
		dateText.setText(DateParser.toDateTimeString(c.getTime()));
		tpdialog = new TimePickerDialog(this, this, c.get(Calendar.HOUR_OF_DAY), c.get(Calendar.MINUTE), true);
		tpdialog.show();
	}

	public void post(View view) {
		TextView dateText = (TextView) findViewById(R.id.txt_start_date);
		Date startDate = DateParser.parse(dateText.getText().toString(), "dd/MM/yyyy HH:mm");
		if(startDate == null) {
			Toast.makeText(AddEventActivity.this,
					"The start date is incorrect.", Toast.LENGTH_LONG).show();
			return;
		}
		TextView dateText2 = (TextView) findViewById(R.id.txt_end_date);
		Date endDate = DateParser.parse(dateText2.getText().toString(), "dd/MM/yyyy HH:mm");
		if(endDate == null) {
			Toast.makeText(AddEventActivity.this, "The end date is incorrect.",
					Toast.LENGTH_LONG).show();
			return;
		}
		if (endDate.before(startDate)) {
			Toast.makeText(this,
					"Make sure that the start date is before the end date!",
					Toast.LENGTH_LONG).show();
			return;
		}

		EditText editName = (EditText) findViewById(R.id.et_eventname);
		String name = editName.getText().toString();
		if (name == null || name.isEmpty()) {
			Toast.makeText(AddEventActivity.this, "Please enter a name.",
					Toast.LENGTH_LONG).show();
			return;
		}
		EditText editDesc = (EditText) findViewById(R.id.et_description);
		String description = editDesc.getText().toString();
		if (description == null || description.isEmpty()) {
			Toast.makeText(AddEventActivity.this,
					"Please enter a description.", Toast.LENGTH_LONG).show();
			return;
		}

		Event event = new Event(name, description, startDate, endDate);
		
		if (method.equals("PATCH")) {
			new JSONConnectionHandler(method, URL, this, this, event.toFinalJSON(),
					getString(R.string.progress_title),
					getString(R.string.progress_edit_event)).execute();
		} else {
			new JSONConnectionHandler(method, URL, this, this, event.toFinalJSON(),
					getString(R.string.progress_title),
					getString(R.string.progress_add_event)).execute();
		}
	}
	
	public void back(View view) {
		finish();
	}

	@Override
	public void onTimeSet(TimePicker picker, int hourOfDay, int minute) {
		TextView dateText;
		if (isStartPicker) {
			dateText = (TextView) findViewById(R.id.txt_start_date);
		} else dateText = (TextView) findViewById(R.id.txt_end_date);
		Calendar c = Calendar.getInstance();
		Date date = DateParser.parse(dateText.getText().toString(), "dd/MM/yyyy HH:mm");
		if(date != null) c.setTime(date);
		c.set(Calendar.HOUR_OF_DAY, hourOfDay);
		c.set(Calendar.MINUTE, minute);
		dateText.setText(DateParser.toDateTimeString(c.getTime()));
	}
}
