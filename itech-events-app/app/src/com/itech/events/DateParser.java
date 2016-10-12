package com.itech.events;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

public class DateParser {
	public static Date parse(String jsonDate) {
		if(jsonDate.contains("T")) {
			return parse(jsonDate,"yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
		} else return parse(jsonDate,"yyyy-MM-dd");
	}
	
	public static Date parse(String dateString, String format) {
		SimpleDateFormat sdf = new SimpleDateFormat(format, Locale.US);
		Date date;
		try {
			date = sdf.parse(dateString);
		} catch (ParseException e) {
			e.printStackTrace();
			date = null;
		}
		return date;
	}
	
	public static String toJSON(Date date) {
		return toJSON(date, false);
	}
	
	public static String toJSON(Date date, boolean dateOnly) {
		if(dateOnly) {
			return toString(date,"yyyy-MM-dd");
		} else return toString(date,"yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
	}
	
	public static String toDateString(Date date) {
		return toString(date,"dd/MM/yyyy");
	}
	
	public static String toDateTimeString(Date date) {
		return toString(date,"dd/MM/yyyy HH:mm");
	}
	
	public static String toString(Date date, String format) {
		try {
			SimpleDateFormat sdf = new SimpleDateFormat(format, Locale.US);
			return sdf.format(date);
		} catch(NullPointerException e) {
			return null;
		}
	}
}
