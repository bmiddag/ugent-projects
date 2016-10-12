package com.itech.events;

import java.util.ArrayList;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView.AdapterContextMenuInfo;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;

public class ShowEventActivity extends ListActivity implements
		JSONConnectionListener {
	private String URL;
	private Event event;
	private ArrayAdapter<Confirmation> adapter;
	private ArrayList<Confirmation> listItems;

	// Activity request codes
	private static int EDIT_EVENT_REQUEST = 300;
	private static int ADD_CONFIRMATION_REQUEST = 301;
	private static int EDIT_CONFIRMATION_REQUEST = 302;
	// JSONConnectionHandler requests
	private static int GET_PERSON_REQUEST = 0;
	private static int DELETE_EVENT_REQUEST = 1;
	private static int DELETE_CONFIRMATION_REQUEST = 2;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		listItems = new ArrayList<Confirmation>();
		setContentView(R.layout.activity_show_event);
		adapter = new ArrayAdapter<Confirmation>(this,
				android.R.layout.simple_list_item_1, listItems);
		setListAdapter(adapter);
		registerForContextMenu(getListView());

		Intent intent = getIntent();
		URL = intent.getStringExtra("URL");
		createTextViews();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.show_event, menu);
		return true;
	}
	
	@Override
	public void onCreateContextMenu(ContextMenu menu, View v,ContextMenuInfo menuInfo) {
		super.onCreateContextMenu(menu, v, menuInfo);
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.confirmations, menu);
		menu.setHeaderTitle("What do you want to do?");
	}

	private void createTextViews() {
		new JSONConnectionHandler("GET", URL, this, this, null).execute();
	}

	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		if (requestCode == GET_PERSON_REQUEST) {
			try {
				event = Event.parse(json);
				TextView txtEventTitle = (TextView) findViewById(R.id.show_event_title);
				TextView txtEventDescription = (TextView) findViewById(R.id.show_event_description);
				TextView txtEventStart = (TextView) findViewById(R.id.show_event_start_date);
				TextView txtEventEnd = (TextView) findViewById(R.id.show_event_end_date);

				txtEventTitle.setText(event.getTitle());
				txtEventDescription.setText(event.getDescription());
				txtEventStart.setText(DateParser.toDateTimeString(event
						.getStartDate()));
				txtEventEnd.setText(DateParser.toDateTimeString(event
						.getEndDate()));
				listItems.addAll(event.getConfirmations());
			} catch (JSONException e) {
				Log.e("JSONBodyException", e.getMessage(), e);
			}
			adapter.notifyDataSetChanged();
		} else if (requestCode == DELETE_EVENT_REQUEST) {
			if (json == null || !json.has("error")) {
				setResult(RESULT_OK);
				this.finish();
			}
		} else if (requestCode == DELETE_CONFIRMATION_REQUEST) {
			if (json == null || !json.has("error")) {
				listItems.clear();
				createTextViews();
			}
		}
	}

	public void editEvent(View view) {
		Intent intent = new Intent(this, AddEventActivity.class);
		intent.putExtra("URL", URL);
		intent.putExtra("method", "PATCH");
		intent.putExtra("JSON", event.toJSON(true, true, true, true)
				.toString());
		startActivityForResult(intent, EDIT_EVENT_REQUEST);
	}

	public void showMessages(View view) {
		Intent intent = new Intent(this, ShowMessageActivity.class);
		intent.putExtra("URL", URL);
		intent.putExtra("method", "POST");
		intent.putExtra("peopleURL",
				getIntent().getExtras().getString("peopleURL"));
		startActivity(intent);
	}
	
	public void addConfirmation(MenuItem item) {
		addConfirmation();
	}
	
	public void addConfirmation(View view) {
		addConfirmation();
	}

	public void addConfirmation() {
		Intent intent = new Intent(this, AddConfirmationActivity.class);
		intent.putExtra("URL", event.getConfirmationsUrl());
		intent.putExtra("method", "POST");
		intent.putExtra("peopleURL",
				getIntent().getExtras().getString("peopleURL"));
		startActivityForResult(intent, ADD_CONFIRMATION_REQUEST);
	}

	public void delete(View view) {
		new JSONConnectionHandler("DELETE", URL, this, this, null,
				DELETE_EVENT_REQUEST, getString(R.string.progress_title),
				getString(R.string.progress_del_event)).execute();
	}

	public void back(View view) {
		finish();
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == EDIT_EVENT_REQUEST || requestCode == ADD_CONFIRMATION_REQUEST
				|| requestCode == EDIT_CONFIRMATION_REQUEST) {
			if (resultCode == RESULT_OK) {
				setResult(RESULT_OK);
				listItems.clear();
				createTextViews();
			}
		}
	}

	@Override
	public void onListItemClick(ListView listView, View view, int position, long id) {
		listView.showContextMenuForChild(view);
	}
	
	public void editConfirmation(MenuItem item) {
		AdapterContextMenuInfo info = (AdapterContextMenuInfo) item.getMenuInfo();
		Confirmation c = listItems.get(info.position);
		Intent intent = new Intent(this, AddConfirmationActivity.class);
		intent.putExtra("URL", event.getConfirmationsUrl());
		intent.putExtra("method", "POST");
		intent.putExtra("peopleURL",
				getIntent().getExtras().getString("peopleURL"));
		intent.putExtra("JSON", c.toJSON(true).toString());
		startActivityForResult(intent, EDIT_CONFIRMATION_REQUEST);
	}
	
	public void deleteConfirmation(MenuItem item) {
		AdapterContextMenuInfo info = (AdapterContextMenuInfo) item.getMenuInfo();
		Confirmation c = listItems.get(info.position);
		new JSONConnectionHandler("DELETE", c.getUrl(), this, this, null,
				DELETE_CONFIRMATION_REQUEST, getString(R.string.progress_title),
				getString(R.string.progress_del_conf)).execute();
	}
}
