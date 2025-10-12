using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[System.Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}
		
		[SerializeField, ReadOnly, Tooltip("Press 'Create File' in the context menu in Inspector if no Records file exists")]
		private TextAsset _json;

		[field:SerializeField, HideInInspector]
		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			//если ввод Records оказался пустым, то отсуствие этого списка не даст правильно работать ReplayMover, а при инициализации пустого списка избегается NullReferenceException
			if (Records == null)
				Records = new List<Data>();
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			//следующий метод сработает и запросит поле text которого нет из-за пустоты _json
			if (_json == null)
			{
				//gameObject.SetActive(false);
				//return;
#if UNITY_EDITOR
				var guids = AssetDatabase.FindAssets("t:TextAsset");
				foreach (var guid in guids)
				{
					var path = AssetDatabase.GUIDToAssetPath(guid);
					var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
					if (asset != null && asset.name == "Path")
					{
						_json = asset;
						Debug.Log($"Automatically assigned _json: {asset.name}");
						break;
					}
				}
#endif
			}
			if (_json != null && !string.IsNullOrEmpty(_json.text))
			{
				JsonUtility.FromJsonOverwrite(_json.text, this);
			}
		}
        private void OnDestroy()
        {
			SaveToJson();
        }
        private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			//если предыдущая проверка каким-то образом прошла успешно, список не инициализировался, но полученные данные списка пусты, то остановка сценария позволяет не допустить ссылок и запросов на пустой список при позиционировании (первого элемента списка не будет в наличии)
			if (Records == null || Records.Count == 0) return;
			var prev = Records[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			//первый элемент 0 уже обработан в строке 44 в переменной prev, и здесь переходим к следующим
			for (int i = 1; i < Records.Count; i++)
			{
				var curr = Records[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		public void SaveToJson()
		{
			if (_json != null && Records != null && Records.Count > 0)
			{
#if UNITY_EDITOR
				try
				{
					string jsonData = JsonUtility.ToJson(this, true);
					string path = AssetDatabase.GetAssetPath(_json);

					File.WriteAllText(path, jsonData);
					AssetDatabase.Refresh();
					Debug.Log($"Successfully saved {Records.Count} records to {path}");
				}
				catch (Exception e)
				{
					Debug.LogError($"Failed to save records: {e.Message}");
				}
#endif
			}
		}

        [ContextMenu("Create File")]
		private void CreateFile()
		{
#if UNITY_EDITOR
			//todo comment: Что происходит в этой строке?
			//переменная stream назначается равной пути приложения и создаваемому там файлу Path.txt
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			//следующий метод Refresh не сможет осуществиться, т.к. при создании/воссоздании файл будет занят для записи, во избежание этого файл выбрасывается из памяти
			stream.Dispose();
			AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//для избежания обработки пустых эссетов при неудачном поиске и эссетов с именем, не равным "Path"
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					EditorUtility.SetDirty(this);
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					//удовлетворяющий требованиям имени эссет найден, предполагается что он в единственном экземпляре или других с такими именем не имеется, дальнейший перебор бессмыслен
					break;
				}
			}
#endif
		}

		[ContextMenu("Force Save")]
		public void ForceSave()
		{
			SaveToJson();
		}

		[ContextMenu("Debug check records")]
		public void DebugCheckRecords()
		{
			Debug.Log($"Records: {Records != null}, count: {Records?.Count ?? 0}");
			if (Records != null)
			{
				for (int i = 0; i < Mathf.Min(Records.Count, 5); i++)
				{
					Debug.Log($"Record {i}: Pos={Records[i].Position}, Time={Records[i].Time}");
				}
			}
        }
    }
}