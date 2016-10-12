package com.itech.events;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 * Class representing a person
 * @author Bart Middag & Mitch De Wilde
 */
public class Person {

	private String name;
	private String email;
	private Date birthDate;
	private Date createdDate;
	private Date updatedDate;
	private String url;
	private List<Event> events;
	
	/**
	 * Create object from the full representation of the person
	 * @param name			This person's name
	 * @param email			This person's e-mail address
	 * @param birthDate		This person's date of birth
	 * @param createdDate	The date this person was added to the server
	 * @param updatedDate	The date of the last update to this person on the server
	 * @param url			The URL of this person
	 */
	public Person(String name, String email, Date birthDate, Date createdDate,
			Date updatedDate, String url) {
		this.name = name;
		this.email = email;
		this.birthDate = birthDate;
		this.createdDate = createdDate;
		this.updatedDate = updatedDate;
		this.url = url;
		events = new ArrayList<Event>();
	}
	
	/**
	 * Used to create a new person
	 * @param name		This person's name
	 * @param email		This person's e-mail address
	 * @param birthDate	This person's date of birth
	 */
	public Person(String name, String email, Date birthDate) {
		this(name,email,birthDate,null,null,null);
	}

	/**
	 * Create an object from the short representation of a person
	 * @param name	The name of this person
	 * @param url	The URL of this person
	 */
	public Person(String name, String url) {
		this(name,null,null,null,null,url);
	}
	
	public String getName() {
		return name;
	}
	
	public String getUrl() {
		return url;
	}
	
	public String getEmail() {
		return email;
	}
	
	public Date getBirthDate() {
		return birthDate;
	}
	
	public Date getCreatedDate() {
		return createdDate;
	}
	
	public Date getUpdatedDate() {
		return updatedDate;
	}
	
	public List<Event> getEvents() {
		return events;
	}
	
	public void addEvent(Event e) {
		events.add(e);
	}
	
	/**
	 * Convert this to the final JSON object
	 * @return	the final JSON object
	 */
	public JSONObject toFinalJSON() {
		JSONObject json = new JSONObject();
		try {
			json.put("person",toJSON());
			return json;
		} catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Convert this person to the default JSON object
	 * @return	a JSON object containing this person
	 */
	public JSONObject toJSON(){
		return toJSON(false, false, false);
	}
	
	/**
	 * Convert this person to a JSON object with extra information
	 * @return	a JSON object containing this person
	 */
	public JSONObject toJSON(boolean includeUrl, boolean includeEvents, boolean includeDates){
		JSONObject json = new JSONObject();
		try {
			json.put("name", name);
			if(email != null) json.put("email", email);
			if(birthDate != null) json.put("birth_date", DateParser.toJSON(birthDate));
			if(includeDates){
				if(createdDate != null) json.put("created_at", DateParser.toJSON(createdDate));
				if(updatedDate != null) json.put("updated_at", DateParser.toJSON(updatedDate));
			}
			if(url != null && includeUrl) json.put("url", url);
			if(!events.isEmpty() && includeEvents) {
				JSONArray eventsArray = new JSONArray();
				for(Event e: events) {
					eventsArray.put(e.toJSON(includeUrl, false, false, false));
				}
				json.put("events", eventsArray);
			}
			return json;
	    } catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Parse a JSON object to create a person object
	 * @param json	the JSON object to parse
	 * @return		a person object
	 */
	public static Person parse(JSONObject json) throws JSONException {
		String name = json.getString("name");
		String url = json.getString("url");
		if(!json.has("email")) {
			// Use short representation
			return new Person(name, url);
		}
		String email = json.getString("email");
		Date birthDate = null;
		if(json.has("birth_date")) birthDate = DateParser.parse(json.getString("birth_date"));
		Date createdDate = null;
		Date updatedDate = null;
		if(json.has("created_at") && json.has("updated_at")) {
			createdDate = DateParser.parse(json.getString("created_at"));
			updatedDate = DateParser.parse(json.getString("updated_at"));
		}
		// String index = (String)json.get("index");
		Person person = new Person(name, email, birthDate, createdDate, updatedDate, url);
		if(json.has("events")) {
			JSONArray eventsArray = json.getJSONArray("events");
			for(int i = 0; i < eventsArray.length(); i++) {
				person.addEvent(Event.parse(eventsArray.getJSONObject(i)));
			}
		}
		return person;
	}
	
	public String toString() {
		return name;
	}
	
	public boolean equals(Object o) {
		if(url == null || o == null) return false;
		if(o instanceof Person) {
			Person p = (Person)o;
			if(p.getUrl() == null) return false;
			return p.getUrl().equals(url);
		} else return false;
	}
}
