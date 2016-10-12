package com.itech.events;

import java.util.Date;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Class representing a message
 * @author Bart Middag & Mitch De Wilde
 */
public class Message {
	private String text;
	private Person person;
	private Date createdDate;
	private String url;
	
	/**
	 * Create object from the full representation of a message
	 * @param text			Text of this message
	 * @param person		Author of this message
	 * @param createdDate	Creation date of this message
	 * @param url			URL of this message
	 */
	public Message(String text, Person person, Date createdDate, String url) {
		this.text = text;
		this.person = person;
		this.createdDate = createdDate;
		this.url = url;
	}
	
	/**
	 * Used to create a new message
	 * @param text		Text of this message
	 * @param person	Author of this message
	 */
	public Message(String text, Person person) {
		this(text, person, null, null);
	}
	
	public String getText() {
		return text;
	}
	
	public Person getPerson() {
		return person;
	}
	
	public Date getCreatedDate() {
		return createdDate;
	}
	
	public String getUrl() {
		return url;
	}
	
	public String toString() {
		return person.getName() + ":\n" + text;
	}
	
	/**
	 * Convert this to the final JSON object
	 * @return	the final JSON object
	 */
	public JSONObject toFinalJSON() {
		JSONObject json = new JSONObject();
		try {
			json.put("message",toJSON());
			return json;
		} catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Convert this message to the default JSON object
	 * @return	a JSON object containing this message
	 */
	public JSONObject toJSON(){
		return toJSON(false, false);
	}
	
	/**
	 * Convert this message to a JSON object with extra information
	 * @return	a JSON object containing this message
	 */
	public JSONObject toJSON(boolean includeUrl, boolean includeDates){
		JSONObject json = new JSONObject();
		try {
			json.put("text", text);
			json.put("person", person.toJSON(true, false, false));
			if(createdDate != null && includeDates) json.put("created_at", DateParser.toJSON(createdDate));
			if(url != null && includeUrl) json.put("url", url);
			return json;
	    } catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Parse a JSON object to create a message object
	 * @param json	the JSON object to parse
	 * @return		a message object
	 */
	public static Message parse(JSONObject json) throws JSONException {
		String text = json.getString("text");
		Date createdDate = null;
		if(json.has("created_at")) createdDate = DateParser.parse(json.getString("created_at"));
		Person person = Person.parse(json.getJSONObject("person"));
		String url = null;
		if(json.has("url")) url = json.getString("url");
		return new Message(text, person, createdDate, url);
	}
}
