﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace StarLink.Data
{
	public class StarData
	{
		object value;
		StarDataType type;

		Dictionary<string, StarData> fields = new Dictionary<string, StarData> ();
		List<StarData> array = new List<StarData> ();

		public StarData(object information = null)
		{
			value = information;
			if (value == null) {
				type = StarDataType.NULL;
				return;
			}
			if(information.GetType() == typeof(int))
				type = StarDataType.INTEGER;
			else if(information.GetType() == typeof(float))
				type = StarDataType.FLOAT;
			else if(information.GetType() == typeof(double))
				type = StarDataType.DOUBLE;
			else if(information.GetType() == typeof(bool))
				type = StarDataType.BOOL;
			else if(information.GetType() == typeof(string))
				type = StarDataType.STRING;
		}

		public StarData(Dictionary<string, object> list)
		{
			type = StarDataType.OBJECT;
			foreach (var field in list)
				Add (field.Key, field.Value);
		}

		public StarData(List<object> list)
		{
			type = StarDataType.ARRAY;
			foreach (var obj in list) {
				Add (obj);
			}
		}

		public void Add(object value){
			if (type != StarDataType.NULL && type != StarDataType.ARRAY)
				return;
			type = StarDataType.ARRAY;
			array.Add (value.ToStarData());
		}

		public void Add(string key, object value)
		{
			if (type != StarDataType.NULL && type != StarDataType.OBJECT)
				return;
			type = StarDataType.OBJECT;
			fields.Add (key, value.ToStarData());
		}

		public bool Set(int index, object value){
			if (type != StarDataType.ARRAY ||index >= array.Count)
				return false;
			array [index] = value.ToStarData();
			return true;
		}

		public bool Set(string key, object value){
			if (type != StarDataType.OBJECT || fields.ContainsKey(key) == false)
				return false;
			fields [key] = value.ToStarData();
			return true;
		}
		/*
		static StarData ToStarData(object value){
			if (value.GetType () != typeof(StarData)) {
				if (value.GetType () == typeof(Dictionary<string, object>))
					return new StarData ((Dictionary<string, object>)value);
				else if (value.GetType () == typeof(List<object>))
					return new StarData ((List<object>)value);
				else
					return new StarData (value);
			} else {
				return ((StarData)value);
			} 
		}
		*/

		public StarData Get(object key)
		{
			if (type == StarDataType.ARRAY) {
				if (key.GetType () != typeof(int))
					return null;
				if ((int)key < array.Count)
					return array [(int)key];
				return null;
			} else if (type == StarDataType.OBJECT) {
				if (key.GetType () != typeof(string))
					return null;
				if (fields.ContainsKey ((string)key))
					return fields [(string)key];
				return null;
			} else
				return null;
		}

		public object this[object key] {
			get {
				return Get (key);
			}
			set {
				if (type == StarDataType.ARRAY) 
				{
					Set ((int)key, value);
				} 
				else if (type == StarDataType.OBJECT) 
				{
					if (!Set ((string)key, value))
						Add ((string)key, value);
				} 
				else if (type == StarDataType.NULL) 
				{
					Add ((string)key, value);
				}
			}
		}

		public bool IsObject(){
			return (type == StarDataType.OBJECT);
		}

		public bool IsArray(){
			return (type == StarDataType.ARRAY);
		}

		public bool IsInteger(){
			return (type == StarDataType.INTEGER);
		}

		public bool IsFloat(){
			return (type == StarDataType.FLOAT);
		}

		public bool IsDouble(){
			return (type == StarDataType.DOUBLE);
		}

		public bool IsBoolean(){
			return (type == StarDataType.BOOL);
		}

		public bool IsNull(){
			return (type == StarDataType.NULL);
		}

		public bool IsString(){
			return (type == StarDataType.STRING);
		}

		public string str {
			get {
				if (IsString ())
					return (string)value;
				return string.Empty;
			}
		}

		public float f {
			get {
				if (IsFloat ())
					return (float)value;
				return 0.0f;
			}
		}

		public int i {
			get{
				if (IsInteger ())
					return (int)value;
				return 0;
			}
		}

		public double d {
			get {
				if (IsDouble ())
					return (double)value;
				return 0;
			}
		}

		public bool b {
			get {
				if (IsBoolean ())
					return (bool)value;
				return false;
			}
		}

		public List<StarData> list {
			get {
				var v = new List<StarData> ();
				if (IsArray ()) {
					return array;
				}
				return v;
			}
		}

		public T ToObject<T>(){
			return (T)value;
		}

		public override string ToString ()
		{
			if (type == StarDataType.NULL) {
				return "null";
			}

			if (type == StarDataType.STRING) {
				return ('"' + value.ToString() + '"');
			}

			if (type == StarDataType.ARRAY) {
				string result = "[ ";
				string begin = "";
				foreach (var field in array) {
					result += ( begin + field.ToString() );
					begin = ", ";
				}
				result += " ]";
				return result;
			}

			if (type == StarDataType.OBJECT) {
				string result = "{ ";
				string begin = "";
				foreach (var field in fields) {
					result += ( begin + field.Key + ":" + field.Value.ToString() );
					begin = ", ";
				}
				result += " }";
				return result;
			}

			return value.ToString ();
		}

		public static StarData Parse(string value){
			StarData data;

			if (value.Contains ("{") && value.Contains ("}")) {
				// is object
				data = parseObject(value);
			} 
			else if (value.Contains ("[") && value.Contains ("[")) {
				// is array
				data = parseArray (value);
			} 
			else {
				// is primitive data
				data = parseOne (value);
			}

			return data;
		}

		static StarData parseObject(string value)
		{
			StarData data = new StarData ();

			value = value.Trim (' ');
			value = value.Trim ('{');
			value = value.Trim ('}');
			value = value.Trim (' ');

			List<StarData> objList = new List<StarData> ();
			List<StarData> arrList = new List<StarData> ();

			foreach (var piece in extractObjects(value)) 
			{
				value = value.Replace ("{" + piece + "}", "$obj" + objList.Count.ToString() );
				objList.Add (parseObject (piece));
			}

			foreach (var piece in extractArrays(value)) 
			{
				value = value.Replace ("[" + piece + "]", "$arr" + arrList.Count.ToString() );
				arrList.Add (parseArray (value));
			}

			Console.WriteLine (value);
			//if (!value.Contains (","))
			//return data;			
			var pieces = value.Split (',');

			foreach (var piece in pieces) {
				if (!piece.Contains (":"))
					continue;
				var values = piece.Trim (' ').Split (':');

				if (values [1].Contains ("$obj")) {
					var temp = values [1].Replace ("$obj", string.Empty);
					int index = int.Parse (temp);
					data.Add (values [0], objList [index]);
				} 
				else if (values [1].ToString ().Contains ("$arr")) {
					var temp = values [1].Replace ("$arr", string.Empty);
					int index = int.Parse (temp);
					data.Add (values [0], arrList [index]);
				} 
				else
					data.Add (values [0], parseOne (values [1]));
			}
			return data;
		}

		static List<string> extractObjects(string value)
		{
			List<string> pieces = new List<string> ();
			while (value.Contains ("{")) {
				var split1 = value.Split ('{');
				var split2 = split1 [1].Split ('}');

				pieces.Add (split2 [0]);
				value = value.Replace ("{" + split2 [0] + "}", string.Empty);
			}
			return pieces;
		}

		static List<string> extractArrays(string value)
		{
			List<string> pieces = new List<string> ();
			while (value.Contains ("[")) {
				var split1 = value.Split ('[');
				var split2 = split1 [1].Split (']');

				pieces.Add (split2 [0]);
				value = value.Replace ("[" + split2 [0] + "]", string.Empty);
			}
			return pieces;
		}

		static StarData parseArray(string value)
		{
			StarData data = new StarData ();

			value = value.Trim (' ');
			value = value.Trim ('[');
			value = value.Trim (']');
			value = value.Trim (' ');

			List<StarData> objList = new List<StarData> ();
			List<StarData> arrList = new List<StarData> ();

			foreach (var piece in extractObjects(value)) 
			{
				value = value.Replace ("{" + piece + "}", "$obj" + objList.Count.ToString() );
				objList.Add (parseObject (piece));
			}

			foreach (var piece in extractArrays(value)) 
			{
				value = value.Replace ("[" + piece + "]", "$arr" + arrList.Count.ToString() );
				arrList.Add (parseArray (value));
			}

			//if (!value.Contains (","))
			//return data;			
			var pieces = value.Split (',');

			foreach (var piece in pieces) {
				if (piece.Contains ("$obj")) {
					var temp = piece.Replace ("$obj", string.Empty);
					int index = int.Parse (temp);
					data.Add (objList [index]);
				} 
				else if (piece.Contains ("$arr")) {
					var temp = piece.Replace ("$arr", string.Empty);
					int index = int.Parse (temp);
					data.Add (arrList [index]);
				} 
				else
					data.Add (parseOne (piece.Trim (' ')));
			}

			return data;
		}

		static StarData parseOne(string value)
		{
			StarData data = new StarData ();
			var type = parseType (value);

			if (type == StarDataType.NULL)
				return data;
			else if (type == StarDataType.BOOL)
				return new StarData (Boolean.Parse (value));
			else if (type == StarDataType.DOUBLE)
				return new StarData (Double.Parse (value));
			else if (type == StarDataType.FLOAT)
				return new StarData (float.Parse (value));
			else if (type == StarDataType.INTEGER)
				return new StarData (int.Parse (value));
			else if (type == StarDataType.STRING)
				return new StarData (value.Trim ('"'));
			else if (type == StarDataType.ARRAY) {
				return parseArray (value);
			} 
			else if (type == StarDataType.OBJECT) {
				return parseObject (value);
			}

			return data;
		}

		static StarDataType parseType(string value)
		{
			int n;

			if (value.Contains ("\""))
				return StarDataType.STRING;
			else if (value.Contains ("{"))
				return StarDataType.OBJECT;
			else if (value.Contains ("["))
				return StarDataType.ARRAY;
			else if (value.Contains (","))
				return StarDataType.FLOAT;
			else if (value.Trim (' ').ToLower().Equals ("true") || value.Trim (' ').ToLower().Equals ("false"))
				return StarDataType.BOOL;
			else if (value.Trim (' ').Equals ("null"))
				return StarDataType.NULL;
			else if (int.TryParse (value, out n))
				return StarDataType.INTEGER;

			return StarDataType.NULL;
		}
	}
}

