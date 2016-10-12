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

public class ShowMessageActivity extends ListActivity implements
		JSONConnectionListener {

	private String URL;
	private Event event;
	private ArrayAdapter<Message> adapter;
	private ArrayList<Message> listItems;
	
	// Activity request codes
	private static int ADD_MESSAGE_REQUEST = 400;
	private static int EDIT_MESSAGE_REQUEST = 401;
	
	// JSONConnectionHandler request codes
	private static int GET_MESSAGES_REQUEST = 0;
	private static int DELETE_MESSAGE_REQUEST = 1;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		listItems = new ArrayList<Message>();
		setContentView(R.layout.activity_show_message);
		URL = getIntent().getStringExtra("URL");
		adapter = new ArrayAdapter<Message>(this,
				android.R.layout.simple_list_item_1, listItems);
		setListAdapter(adapter);
		registerForContextMenu(getListView());
		createTextViews();
	}

	private void createTextViews() {
		new JSONConnectionHandler("GET", URL, this, this, null, GET_MESSAGES_REQUEST).execute();
	}

	@Override
	public void onCreateContextMenu(ContextMenu menu, View v,
			ContextMenuInfo menuInfo) {
		super.onCreateContextMenu(menu, v, menuInfo);
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.messages, menu);
		// menu.setHeaderTitle("What do you want to do?");
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.show_message, menu);
		return true;
	}

	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		if(requestCode == GET_MESSAGES_REQUEST) {
			try {
				event = Event.parse(json);
				listItems.addAll(event.getMessages());
			} catch (JSONException e) {
				Log.e("JSONBodyException", e.getMessage(), e);
			}
			adapter.notifyDataSetChanged();
		} else if (requestCode == DELETE_MESSAGE_REQUEST) {
			if (json == null || !json.has("error")) {
				listItems.clear();
				createTextViews();
			}
		}
	}

	public void addMessage(MenuItem item) {
		addMessage();
	}

	public void addMessage(View view) {
		addMessage();
	}

	public void addMessage() {
		Intent intent = new Intent(this, AddMessageActivity.class);
		intent.putExtra("URL", event.getMessagesUrl());
		intent.putExtra("method", "POST");
		intent.putExtra("peopleURL",
				getIntent().getExtras().getString("peopleURL"));
		startActivityForResult(intent, ADD_MESSAGE_REQUEST);
	}

	public void back(View view) {
		finish();
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == ADD_MESSAGE_REQUEST || requestCode == EDIT_MESSAGE_REQUEST) {
			if (resultCode == RESULT_OK) {
				setResult(RESULT_OK);
				listItems.clear();
				createTextViews();
			}
		}
	}

	public void onListItemClick(ListView listView, View view, int position,
			long id) {
		listView.showContextMenuForChild(view);
	}

	public void editMessage(MenuItem item) {
		AdapterContextMenuInfo info = (AdapterContextMenuInfo) item.getMenuInfo();
		Message m = listItems.get(info.position);
		Intent intent = new Intent(this, AddMessageActivity.class);
		intent.putExtra("URL", m.getUrl());
		intent.putExtra("method", "PATCH");
		intent.putExtra("peopleURL",
				getIntent().getExtras().getString("peopleURL"));
		intent.putExtra("JSON", m.toJSON(true, false)
				.toString());
		startActivityForResult(intent, EDIT_MESSAGE_REQUEST);
	}

	public void deleteMessage(MenuItem item) {
		AdapterContextMenuInfo info = (AdapterContextMenuInfo) item.getMenuInfo();
		Message m = listItems.get(info.position);
		new JSONConnectionHandler("DELETE", m.getUrl(), this, this, null,
				DELETE_MESSAGE_REQUEST,
				getString(R.string.progress_title),
				getString(R.string.progress_del_msg)).execute();
	}

}
