package com.itech.events;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;

import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpRequestBase;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.ProgressDialog;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

/**
 * AsyncTask that handles the HTTP requests and returns a JSON object.
 * @author Bart Middag & Mitch De Wilde
 */
public class JSONConnectionHandler extends AsyncTask<Void, Void, Void> {
	private String method;
	private String URL;
	private Activity activity;
	private JSONConnectionListener listener;
	private JSONObject jsonSend;
	private JSONObject jsonReceive;
	private int requestCode;
	private String progressTitle;
	private String progressContent;
	private ProgressDialog progress;
	
	/**
	 * Create a connection handler object.
	 * @param method	The HTTP method to use: GET, POST, PATCH or DELETE
	 * @param URL		The URL to send the request to
	 * @param activity	The calling activity
	 * @param listener	The listener object that will handle the received JSON body
	 * @param jsonSend	The JSON object that should be sent (null if using GET or DELETE)
	 */
	public JSONConnectionHandler(String method, String URL, Activity activity,
			JSONConnectionListener listener, JSONObject jsonSend) {
		this(method, URL, activity, listener, jsonSend, 0,
				activity.getString(R.string.progress_title),
				activity.getString(R.string.progress_loading));
	}
	
	/**
	 * Create a connection handler object with custom progress strings.
	 * @param method			The HTTP method to use: GET, POST, PATCH or DELETE
	 * @param URL				The URL to send the request to
	 * @param activity			The calling activity
	 * @param listener			The listener object that will handle the received JSON body
	 * @param jsonSend			The JSON object that should be sent (null if using GET or DELETE)
	 * @param progressTitle		The title of the progress dialog
	 * @param progressContent	The content of the progress dialog
	 */
	public JSONConnectionHandler(String method, String URL, Activity activity,
			JSONConnectionListener listener, JSONObject jsonSend,
			String progressTitle, String progressContent) {
		this(method, URL, activity, listener, jsonSend, 0, progressTitle, progressContent);
	}
	
	/**
	 * Create a connection handler object with a request code.
	 * @param method		The HTTP method to use: GET, POST, PATCH or DELETE
	 * @param URL			The URL to send the request to
	 * @param activity		The calling activity
	 * @param listener		The listener object that will handle the received JSON body
	 * @param jsonSend		The JSON object that should be sent (null if using GET or DELETE)
	 * @param requestCode	The request code for this object
	 */
	public JSONConnectionHandler(String method, String URL, Activity activity,
			JSONConnectionListener listener, JSONObject jsonSend, int requestCode) {
		this(method, URL, activity, listener, jsonSend, requestCode,
				activity.getString(R.string.progress_title),
				activity.getString(R.string.progress_loading));
	}
	
	/**
	 * Create a connection handler object with a request code and custom progress strings.
	 * @param method			The HTTP method to use: GET, POST, PATCH or DELETE
	 * @param URL				The URL to send the request to
	 * @param activity			The calling activity
	 * @param listener			The listener object that will handle the received JSON body
	 * @param jsonSend			The JSON object that should be sent (null if using GET or DELETE)
	 * @param requestCode		The request code for this object
	 * @param progressTitle		The title of the progress dialog
	 * @param progressContent	The content of the progress dialog
	 */
	public JSONConnectionHandler(String method, String URL, Activity activity,
			JSONConnectionListener listener, JSONObject jsonSend, int requestCode,
			String progressTitle, String progressContent) {
		this.method = method;
		this.URL = URL;
		this.activity = activity;
		this.listener = listener;
		this.jsonSend = jsonSend;
		this.requestCode = requestCode;
		this.progressTitle = progressTitle;
		this.progressContent = progressContent;
	}
	
	@Override
	protected Void doInBackground(Void... params) {
		DefaultHttpClient client = new DefaultHttpClient();
		HttpRequestBase request;
		if(method.equals("POST")) {
			request = new HttpPost(URL);
		} else if(method.equals("PATCH")) {
			request = new HttpPatch(URL);
		} else if(method.equals("DELETE")){
			request = new HttpDelete(URL);
		} else request = new HttpGet(URL);
		request.setHeader("Accept", "application/json");
		if(method.equals("POST") || method.equals("PATCH")) {
			request.setHeader("Content-Type", "application/json");
		}
		try {
			if(method.equals("POST") || method.equals("PATCH")) {
				((HttpEntityEnclosingRequestBase)request).setEntity(
						new StringEntity(jsonSend.toString()));
			}
			HttpResponse response = client.execute(request);
			if(response.getEntity() != null) {
				InputStream bodyStream = response.getEntity().getContent();
				BufferedReader bodyReader = new BufferedReader(
						new InputStreamReader(bodyStream));
				StringBuilder body = new StringBuilder();
				String chunk;
				while ((chunk = bodyReader.readLine()) != null)
					body.append(chunk);
				jsonReceive = new JSONObject(body.toString());
				if(jsonReceive.has("error")) {
					activity.runOnUiThread(new Runnable() {
						  public void run() {
							  try {
								Toast.makeText(activity, "Error " + jsonReceive.getString("status") + ": "
										  + jsonReceive.getString("error"),Toast.LENGTH_LONG).show();
							} catch (JSONException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}
						  }
					});
				}
			}
		} catch (Exception e) {
			Log.e("JSONConnectionException",e.getLocalizedMessage(),e);
			activity.runOnUiThread(new Runnable() {
				  public void run() {
					  Toast.makeText(activity, "There has been an error.",
							  Toast.LENGTH_LONG).show();
				  }
			});
			request.abort();
		}
		return null;
	}
	
	@Override
	protected void onPreExecute() {
		progress = ProgressDialog.show(activity,
				progressTitle,
				progressContent);
	}
	
	@Override
	protected void onPostExecute(Void arg) {
		if (progress != null && progress.isShowing()) {
			progress.dismiss();
		}
		listener.handleJSONBody(jsonReceive, requestCode);
	}
	
	@Override
    protected void onCancelled() {
		if (progress != null && progress.isShowing()) {
			progress.dismiss();
		}
        super.onCancelled();
    }
}
