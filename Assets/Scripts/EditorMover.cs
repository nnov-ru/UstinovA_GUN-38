using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

		//todo comment: Что произойдёт, если _delay > _duration?
		//метод сработает лишь один первый раз, а второй раз не наступит, т. к. интервал _delay окончится позднее всей продолжительности работы программы _duration
		[Range(0.2f, 1.0f)]
		private float _delay = 1f;
		[Min(0.2f)]
		private float _duration = 10f;

		private void Start()
		{
			if (_duration <= _delay)
			{
				_duration = _delay * 5f;
				Debug.LogWarning($"Duration was adjusted to {_duration} to exceed the delay of {_delay}");
			}
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			//Update происходит каждый кадр, а найти компонент PositionSaver достаточно один раз в начале работы. Поиск каждый кадр нагружает процессор.
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();
        }

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished. Total records: {_save.Records.Count}");
				return;
			}

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //линейное сокращение длительности интервала между записями исказит журнал записей и в итоге последние записи будут производиться с разницей в доли секунды или интервал станет отрицательным. цель же сокращать оставшееся время работы, а не интервал
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				var newData = new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени?
					//для фиксации момента, в который записываются координаты - а значит и скорости передвижения
					Time = Time.time,
				};
				_save.Records.Add(newData);
				Debug.Log($"Added record #{_save.Records.Count} at position : {newData.Position}");
			}
		}
        private void OnDisable()
        {
			if (_save != null)
			{
				_save.SaveToJson();
			}
        }
    }
}