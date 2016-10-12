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
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class EventsActivity extends ListActivity implements OnItemClickListener, JSONConnectionListener {
	private String URL;
	private ArrayList<Event> events = new ArrayList<Event>();
    private ArrayAdapter<Event> adapter;
    
    private static int EDIT_EVENT_REQUEST = 100;
    private static int SHOW_EVENT_REQUEST = 101;
    private static int ADD_EVENT_REQUEST = 102;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_events);
		adapter = new ArrayAdapter<Event>(this,
	            android.R.layout.simple_list_item_2,
	            android.R.id.text1,
	            events) {
			@Override
			public View getView(int position, View convertView, ViewGroup parent) {
				// Must always return just a View.
				View view = super.getView(position, convertView, parent);
				// If you look at the android.R.layout.simple_list_item_2 source, you'll see
				// it's a TwoLineListItem with 2 TextViews - text1 and text2.
				//TwoLineListItem listItem = (TwoLineListItem) view;
				TextView text1 = (TextView) view.findViewById(android.R.id.text1);
				TextView text2 = (TextView) view.findViewById(android.R.id.text2);
				text1.setText(getItem(position).getTitle());
				text2.setText("Starting at " + DateParser.toDateTimeString(getItem(position).getStartDate()));
				return view;
			}
		};
	    setListAdapter(adapter);
	    getListView().setOnItemClickListener(this);
	    
	    Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		
	    addItems();
	}
	
	public void addEvent() {
		Intent intent = new Intent(this, AddEventActivity.class);
		intent.putExtra("URL", URL);
		intent.putExtra("method", "POST");
		startActivityForResult(intent,ADD_EVENT_REQUEST);
	}
	
	public void addEvent(MenuItem m) {
		addEvent();
	}
	
	public void addEvent(View v) {
		addEvent();
	}
	
	public void addItems() {
		new JSONConnectionHandler("GET",URL,this,this,null).execute();
    }

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.events, menu);
		return true;
	}
	
	@Override
	public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
		Intent intent = new Intent(this, ShowEventActivity.class);
		intent.putExtra("URL", adapter.getItem(position).getUrl());
		intent.putExtra("peopleURL", getIntent().getExtras().getString("peopleURL"));
		startActivityForResult(intent, SHOW_EVENT_REQUEST);
	}

	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		try {
			JSONArray eventsArray = (JSONArray) json.get("events");
			for (int i = 0; i < eventsArray.length(); i++) {
				Event event = Event.parse(eventsArray.getJSONObject(i));
				events.add(event);
			}
		} catch (JSONException e) {
			Log.e("JSONBodyException",e.getMessage(),e);
		}
		adapter.notifyDataSetChanged();
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    if (requestCode == EDIT_EVENT_REQUEST || requestCode == SHOW_EVENT_REQUEST
	    		|| requestCode == ADD_EVENT_REQUEST) {
	        if (resultCode == RESULT_OK) {
	            events.clear();
	            addItems();
	        }
	    }
	}
	
	public void back(View view) {
		finish();
	}
}
