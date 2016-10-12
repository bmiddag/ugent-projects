package com.itech.events;

import java.util.ArrayList;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;

public class PeopleActivity extends ListActivity implements JSONConnectionListener {
	private String URL;
	private ArrayList<Person> people;
    private ArrayAdapter<Person> adapter;
    
    private static int EDIT_PERSON_REQUEST = 200;
    private static int SHOW_PERSON_REQUEST = 201;
    private static int ADD_PERSON_REQUEST = 202;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_people);
		people = new ArrayList<Person>();
		adapter=new ArrayAdapter<Person>(this,
	            android.R.layout.simple_list_item_1,
	            people);
		setListAdapter(adapter);
		
		Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		
	    addItems();
	}
	
	public void addItems() {
		new JSONConnectionHandler("GET",URL,this,this,null).execute();
    }

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.people, menu);
		return true;
	}
	
	@Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int itemId = item.getItemId();
        switch (itemId) {
            case R.id.action_add_person:
                addPerson();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }
	
	private void addPerson() {
		Intent intent = new Intent("com.itech.events.ADD_PERSON");
		intent.putExtra("URL", URL);
		intent.putExtra("method", "POST");
		startActivityForResult(intent, ADD_PERSON_REQUEST);
	}
	
	public void addPerson(View view) {
		addPerson();
	}
	
	public void back(View view) {
		finish();
	}
	
	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		try {
			JSONArray peopleArray = (JSONArray) json.get("people");
			for (int i = 0; i < peopleArray.length(); i++) {
				Person person = Person.parse(peopleArray.getJSONObject(i));
				people.add(person);
			}
		} catch (JSONException e) {
			Log.e("JSONBodyException",e.getMessage(),e);
		}
		adapter.notifyDataSetChanged();
	}

	@Override
	public void onListItemClick(ListView l, View view, int position, long id) {
		Intent intent = new Intent("com.itech.events.SHOW_PERSON");
		intent.putExtra("URL", adapter.getItem(position).getUrl());
		startActivityForResult(intent, SHOW_PERSON_REQUEST);
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    if (requestCode == EDIT_PERSON_REQUEST || requestCode == SHOW_PERSON_REQUEST
	    		|| requestCode == ADD_PERSON_REQUEST) {
	        if (resultCode == RESULT_OK) {
	            people.clear();
	            addItems();
	        }
	    }
	}
}