package com.itech.events;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Class representing a confirmation
 * @author Bart Middag & Mitch De Wilde
 */
public class Confirmation {
	private Person person;
	private boolean going;
	private String url;
	
	/**
	 * Create object from the full representation of the confirmation
	 * @param person	the person who confirmed
	 * @param going		whether or not the person is going to this event
	 * @param url		the URL of this confirmation
	 */
	public Confirmation(Person person, boolean going, String url) {
		this.person = person;
		this.going = going;
		this.url = url;
	}
	
	/**
	 * Used to create a new confirmation object
	 * @param person	the person who wants to confirm
	 * @param going		whether or not the person is going to this event
	 */
	public Confirmation(Person person, boolean going) {
		this(person, going, null);
	}
	
	public Person getPerson() {
		return person;
	}
	
	public boolean isGoing() {
		return going;
	}
	
	public String getUrl() {
		return url;
	}
	
	public String toString() {
		return person.getName() + " is " + (going ? "attending" : "absent");
	}
	
	/**
	 * Convert this to the final JSON object
	 * @return	the final JSON object
	 */
	public JSONObject toFinalJSON() {
		JSONObject json = new JSONObject();
		try {
			json.put("confirmation",toJSON());
			return json;
		} catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Convert this confirmation to the default JSON object
	 * @return	a JSON object containing this confirmation
	 */
	public JSONObject toJSON(){
		return toJSON(false);
	}
	
	/**
	 * Convert this confirmation to a JSON object with extra information
	 * @return	a JSON object containing this confirmation
	 */
	public JSONObject toJSON(boolean includeUrl){
		JSONObject json = new JSONObject();
		try {
			json.put("person", person.toJSON(true, false, false));
			json.put("going", going);
			if(url != null && includeUrl) json.put("url", url);
			return json;
	    } catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Parse a JSON object to create a confirmation object
	 * @param json	the JSON object to parse
	 * @return		a confirmation object
	 */
	public static Confirmation parse(JSONObject json) throws JSONException {
		Person person = Person.parse(json.getJSONObject("person"));
		boolean going = json.getBoolean("going");
		String url = null;
		if(json.has("url")) url = json.getString("url");
		return new Confirmation(person, going, url);
	}
}
