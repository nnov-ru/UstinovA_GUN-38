using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
			////todo comment: зачем нужны эти проверки?
			//если почему-то при выполнении исходного условия о наличии PositionSaver всё-таки нет в наличии PositionSaver или записи не передались или не зафиксировались, то дальнейшее обращение к ним в Update вызовет ошибку
			if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				//чтобы не допустить исполнения метода Update при ошибочных условиях
				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)? 
			//Replay воспроизводит положения объекта в журнале в тот же момент с начала работы, что и момент, когда объект достиг их при регистрации журнала. здесь отыскивается момент текущего времени, когда пора уже прекратить нахождение в записанном месте и следовать к следующей точке 
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				//чтобы остановить воспроизведение, если проверяемый элемент по счету уже превысил количество записей в журнале местоположений
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			//% прошедшего после предыдущей точки времени ко всему времени между двумя последовательными зарегистированными точками
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //при регистрации двух точек в одно и то же время и получении бесконечности при делении на 0 в расчете переменной delta - delta вручную приравнивается к нулю 
            if (float.IsNaN(delta)) delta = 0f;
			//todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			//линейная интерполяция между двумя координатами - т. е. прорисовка плавной линии между двумя точками журнала пропорционально времени прохода между двумя точками (т.е.с той же скоростью, что и объект двигался при регистрации точек в журнал)
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}