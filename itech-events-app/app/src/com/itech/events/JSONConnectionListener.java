package com.itech.events;

import org.json.JSONObject;

public interface JSONConnectionListener {
	public void handleJSONBody(JSONObject json, int requestCode);
}
