package com.itech.events;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.TextView;

public class MainActivity extends Activity implements JSONConnectionListener {
	// Main site URL
	public String URL = "http://events.restdesc.org";
	// Obtained with HTTP request
	private String title;
	private String eventsURL;
	private String peopleURL;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		new JSONConnectionHandler("GET",URL,this,this,null).execute();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	public void goToEvents(View view) {
		Intent intent = new Intent(this, EventsActivity.class);
		intent.putExtra("URL", eventsURL);
		intent.putExtra("peopleURL", peopleURL);
		startActivity(intent);
	}

	public void goToPeople(View view) {
		Intent intent = new Intent(this, PeopleActivity.class);
		intent.putExtra("URL", peopleURL);
		startActivity(intent);
	}

	@Override
	public void handleJSONBody(JSONObject json, int requestCode) {
		try {
			title = json.getString("title");
			eventsURL = json.getString("events");
			peopleURL = json.getString("people");
			((TextView)findViewById(R.id.menu_title)).setText(title);
		} catch (JSONException e) {
			Log.e("JSONBodyException",e.getMessage(),e);
		}
	}

}
