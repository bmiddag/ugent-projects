package com.itech.events;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class ShowPersonActivity extends ListActivity implements JSONConnectionListener {
	private String URL;
	private Person person;
	private ArrayAdapter<Event> adapter;
	private List<Event> events;
	// Activity request codes
	private static int EDIT_PERSON_REQUEST = 200;
	// JSONConnectionHandler request codes
	private static int GET_PERSON_REQUEST = 0;
	private static int DELETE_PERSON_REQUEST = 1;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_show_person);
		events = new ArrayList<Event>();
		adapter = new ArrayAdapter<Event>(this,
				android.R.layout.simple_list_item_1, events);
		setListAdapter(adapter);
		
		Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		createTextViews();
	}
	
	private void createTextViews() {
		new JSONConnectionHandler("GET",URL,this,this,null,GET_PERSON_REQUEST).execute();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.people, menu);
		return true;
	}
	
	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
			if(requestCode == GET_PERSON_REQUEST) {
			try {
				person = Person.parse(json);
				TextView txtPersonName = (TextView) findViewById(R.id.show_person_name);
				TextView txtPersonEmail = (TextView) findViewById(R.id.show_person_email);
				TextView txtPersonBirthDate = (TextView) findViewById(R.id.show_person_birth_date);
				
				txtPersonName.setText(person.getName());
				txtPersonEmail.setText(person.getEmail());
				txtPersonBirthDate.setText(DateParser.toDateString(person.getBirthDate()));
				for (Event e: person.getEvents()) {
					events.add(e);
				}
			} catch (JSONException e) {
				Log.e("JSONBodyException",e.getMessage(),e);
			}
			adapter.notifyDataSetChanged();
		} else if(requestCode == DELETE_PERSON_REQUEST) {
			if(json==null || !json.has("error")) {
				setResult(RESULT_OK);
				this.finish();
			}
		}
	}
	
	public void back(View view) {
		finish();
	}
	
	public void edit(View view) {
		Intent intent = new Intent("com.itech.events.ADD_PERSON");
		intent.putExtra("URL", person.getUrl());
		intent.putExtra("method", "PATCH");
		intent.putExtra("JSON", person.toJSON(true,false,false).toString());
		startActivityForResult(intent,EDIT_PERSON_REQUEST);
	}
	
	public void delete(View view) {
		new JSONConnectionHandler("DELETE",URL,this,this,null,DELETE_PERSON_REQUEST,
				getString(R.string.progress_title),
				getString(R.string.progress_del_person)).execute();
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    if (requestCode == EDIT_PERSON_REQUEST) {
	        if (resultCode == RESULT_OK) {
	        	setResult(RESULT_OK);
	            events.clear();
	            createTextViews();
	        }
	    }
	}

}