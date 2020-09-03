﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram.SpaceExplorer
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
	{

		#region Fields
		private static T instance = null;

		#endregion

		#region Properties
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<T>();
					if (instance == null)
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name;
						instance = obj.AddComponent<T>();
					}
				}
				return instance;
			}
		}

		#endregion

		#region Methods
		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = this as T;
			}
			else
			{
				Destroy(gameObject);
			}
		}
		#endregion
	}
}