package com.itech.events;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 * Class representing an event
 * @author Bart Middag & Mitch De Wilde
 */
public class Event {
	private String title;
	private String description;
	private Date startDate;
	private Date endDate;
	private Date createdDate;
	private Date updatedDate;
	private String url;
	private String confirmationsUrl;
	private String messagesUrl;
	private List<Confirmation> confirmations;
	private List<Message> messages;
	
	/**
	 * Create object from the full representation of the event
	 * @param title			Title of the event
	 * @param description	Description of this event
	 * @param startDate		Start date of the event
	 * @param endDate		End date of the event
	 * @param createdDate	Creation date of the event
	 * @param updatedDate	Date of last update to this event
	 * @param url			URL of this event
	 */
	public Event(String title, String description, Date startDate, Date endDate, Date createdDate,
			Date updatedDate, String url, String confirmationsUrl, String messagesUrl) {
		this.title = title;
		this.description = description;
		this.startDate = startDate;
		this.endDate = endDate;
		this.createdDate = createdDate;
		this.updatedDate = updatedDate;
		this.url = url;
		this.confirmationsUrl = confirmationsUrl;
		this.messagesUrl = messagesUrl;
		confirmations = new ArrayList<Confirmation>();
		messages = new ArrayList<Message>();
	}
	
	/**
	 * Create object from the list representation of the event
	 * @param title		Title of the event
	 * @param startDate	Start date of the event
	 * @param url		URL of the event
	 */
	public Event(String title, Date startDate, String url) {
		this(title, null, startDate, null, null, null, url, null, null);
	}
	
	/**
	 * Create object from the shortest possible representation of the event
	 * @param title	Title of the event
	 * @param url	URL of the event
	 */
	public Event(String title, String url) {
		this(title, null, null, null, null, null, url, null, null);
	}
	
	/**
	 * Used to create a new event
	 * @param title			Title of the event
	 * @param description	Description of the event
	 * @param startDate		Start date of the event
	 * @param endDate		End date of the event
	 */
	public Event(String title, String description, Date startDate, Date endDate) {
		this(title, description, startDate, endDate, null, null, null, null, null);
	}
	
	public String getTitle() {
		return title;
	}
	
	public String getDescription() {
		return description;
	}
	
	public Date getStartDate() {
		return startDate;
	}
	
	public Date getEndDate() {
		return endDate;
	}
	
	public Date getCreatedDate() {
		return createdDate;
	}
	
	public Date getUpdatedDate() {
		return updatedDate;
	}
	
	public String getUrl() {
		return url;
	}
	
	public String getMessagesUrl() {
		return messagesUrl;
	}
	
	public String getConfirmationsUrl() {
		return confirmationsUrl;
	}
	
	public List<Confirmation> getConfirmations() {
		return confirmations;
	}
	
	public List<Message> getMessages() {
		return messages;
	}
	
	public void addConfirmation(Confirmation c) {
		confirmations.add(c);
	}
	
	public void addMessage(Message m) {
		messages.add(m);
	}
	
	/**
	 * Convert this to the final JSON object
	 * @return	the final JSON object
	 */
	public JSONObject toFinalJSON() {
		JSONObject json = new JSONObject();
		try {
			json.put("event",toJSON());
			return json;
		} catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Convert this event to the default JSON object
	 * @return	a JSON object containing this event
	 */
	public JSONObject toJSON(){
		return toJSON(false, false, false, false);
	}
	
	/**
	 * Convert this event to a JSON object with extra information
	 * @return	a JSON object containing this event
	 */
	public JSONObject toJSON(boolean includeUrl, boolean includeConfirmations,
			boolean includeMessages, boolean includeDates){
		JSONObject json = new JSONObject();
		try {
			json.put("title", title);
			if(description != null) json.put("description", description);
			if(startDate != null) json.put("start", DateParser.toJSON(startDate));
			if(endDate != null) json.put("end", DateParser.toJSON(endDate));
			if(includeDates) {
				if(createdDate != null) json.put("created_at", DateParser.toJSON(createdDate));
				if(updatedDate != null) json.put("updated_at", DateParser.toJSON(updatedDate));
			}
			if(includeUrl && url != null) json.put("url", url);
			if(includeConfirmations && confirmationsUrl != null) {
				JSONObject confirmationsObject = new JSONObject();
				JSONArray confirmationsArray = new JSONArray();
				for(Confirmation c: confirmations) {
					confirmationsArray.put(c.toJSON());
				}
				confirmationsObject.put("list", confirmationsArray);
				confirmationsObject.put("url", confirmationsUrl);
				json.put("confirmations", confirmationsObject);
			}
			if(includeMessages && messagesUrl != null) {
				JSONObject messagesObject = new JSONObject();
				JSONArray messagesArray = new JSONArray();
				for(Message m: messages) {
					messagesArray.put(m.toJSON());
				}
				messagesObject.put("list", messagesArray);
				messagesObject.put("url", messagesUrl);
				json.put("messages", messagesObject);
			}
			return json;
	    } catch (JSONException e) {
	        e.printStackTrace();
	        return null;
	    }
	}
	
	/**
	 * Parse a JSON object to create an event object
	 * @param json	the JSON object to parse
	 * @return		an event object
	 */
	public static Event parse(JSONObject json) throws JSONException {
		String title = json.getString("title");
		String url = json.getString("url");
		if(!json.has("start")) {
			// Use shortest representation
			return new Event(title,url);
		}
		Date startDate = DateParser.parse(json.getString("start"));
		if(!json.has("end")) {
			// Use second shortest representation
			return new Event(title,startDate,url);
		}
		Date endDate = DateParser.parse(json.getString("end"));
		Date createdDate = DateParser.parse(json.getString("created_at"));
		Date updatedDate = DateParser.parse(json.getString("updated_at"));
		String description = json.getString("description");
		// String index = (String)json.get("index");
		JSONObject confirmationsObject = json.getJSONObject("confirmations");
		JSONObject messagesObject = json.getJSONObject("messages");
		String confirmationsUrl = confirmationsObject.getString("url");
		String messagesUrl = messagesObject.getString("url");
		Event event = new Event(title, description, startDate, endDate,
				createdDate, updatedDate, url, confirmationsUrl, messagesUrl);
		JSONArray confirmationsArray = confirmationsObject.getJSONArray("list");
		JSONArray messagesArray = messagesObject.getJSONArray("list");
		for(int i = 0; i < confirmationsArray.length(); i++) {
			event.addConfirmation(Confirmation.parse(confirmationsArray.getJSONObject(i)));
		}
		for(int i = 0; i < messagesArray.length(); i++) {
			event.addMessage(Message.parse(messagesArray.getJSONObject(i)));
		}
		return event;
	}
	
	public String toString() {
		return title;
	}
}
