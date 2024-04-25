//Michael Revit

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// A simple event class.
/// </summary>
/// <typeparam name="T0"></typeparam>
public class EasyEvent<T0> {

	//Script Variables
	private string _eventName;
	private List<KeyValuePair<object, Action<T0>>> _collection = new List<KeyValuePair<object, Action<T0>>>();



	#region Constructor

	public EasyEvent(string eventName) {
		_eventName = eventName;
	}

	#endregion



	#region Subscribe Unsubscribe

	public void Subscribe(object obj, Action<T0> action) {
		_collection.Add(new KeyValuePair<object, Action<T0>>(obj, action));
	}



	public void Unsubscribe(object obj) {
		for (int i = 0; i < _collection.Count; i++)
			if (_collection[i].Key == obj)
				_collection.RemoveAt(i--);
	}



	public void Unsubscribe(Action<T0> action) {
		for (int i = 0; i < _collection.Count; i++)
			if (_collection[i].Value == action) {
				_collection.RemoveAt(i);
				break;
			}
	}

	#endregion



	#region Invoke

	public void Invoke(T0 t0) {
		for (int i = 0; i < _collection.Count; i++) {
			try {
				_collection[i].Value(t0);
			} catch (Exception e) {
				Debug.LogError("Exception in Event [" + _eventName + "] :" + e.Message + "\n---\n" + e.StackTrace + "\n---");
			}
		}
	}

	#endregion

}
