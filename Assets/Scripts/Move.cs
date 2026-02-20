using UnityEngine;
using DG.Tweening;

public class Move : MonoBehaviour
{
    [Header("Точки пути")]
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private Transform _point3; // Добавили точки для пути

    [Header("Настройки движения")]
    [SerializeField] private float _duration = 2f;  // Дюрация
    [SerializeField] private float _speed = 5f; // Скорость
    [SerializeField] private Ease _easeType = Ease.InOutQuad;   // Кривая изменения
    [SerializeField] private float _delay = 1f; // Задержка перед стартом
    [SerializeField] private int _loops = -1;   // -1 - бесконечно

    [Header("Дополнительные эффекты")]
    [SerializeField] private bool _scaleEffect = true;
    [SerializeField] private bool _colorEffect = true;
    [SerializeField] private GameObject _dustEffect;    // Префаб эффекта пыли

    private Renderer _renderer;
    private Color _originalColor;
    private Sequence _mainSequence; // Главная последовательность анимации

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalColor = _renderer.material.color;

        CreatePathAnimation();
    }

    // Анимация движения по пути с DoTween
    private void CreatePathAnimation()
    {
        // массив точек пути
        Vector3[] pathPoints = new Vector3[]
        {
            _point1.position,
            _point2.position,
            _point3.position,
            _point1.position    // Возвращаемся в начало
        };

        // Последовательность для комбинации анимаций
        _mainSequence = DOTween.Sequence();

        // Движение по пути
        // DOVirtual для анимации по пути
        Tween moveTween = transform.DOPath(
            pathPoints,             // Массив точек пути
            _duration * 3,          // Общая длительность (умножаем на кол-во сегментов)
            PathType.CatmullRom,    // Тип пути -плавный
            PathMode.Full3D         // Режим 3D
        )
        .SetEase(_easeType) // Устанавливаем кривую изменения
        .SetDelay(_delay)   // Задержка перед стартом
        .SetLoops(_loops)   // Количество кругов
        .SetSpeedBased(_speed > 0)  // Если указана скорость, используем ее
        .OnStart(() => Debug.Log("Движение началось"))  // старт
        .OnComplete(() => Debug.Log("Движение завершено")); // финиш

        // Добавляем движение в последовательность
        _mainSequence.Append(moveTween);

        // Доп.эффекты

        // Эффект изменения масштаба
        if (_scaleEffect)
        {
            // Пульсация масштаба во время движения
            _mainSequence.Join(
                transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), _duration / 2)
                .SetEase(Ease.InOutBounce)
                .SetLoops(_loops * 2, LoopType.Yoyo)
            );
        }

        // Эффект изменения цвета
        if (_colorEffect && _renderer != null)
        {
            // Циклическое изменение цвета
            _mainSequence.Join(
                _renderer.material.DOColor(Color.red, _duration)
                .SetLoops(_loops * 2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
            );
        }

        // Эффект пыли
        if (_dustEffect != null)
        {
            // Следы при движении
            _mainSequence.Join(
                transform.DOPath(pathPoints, _duration * 3, PathType.CatmullRom)
                .OnWaypointChange((int index) => {
                    // При прохождении каждой точки создаем эффект пыли
                    Instantiate(_dustEffect, transform.position, Quaternion.identity);
                })
            );
        }

        // Запускаем всю последовательность
        _mainSequence.Play();
    }

private void OnDestroy()
    {
        // Очищаем анимацию при уничтожении объекта
        if (_mainSequence != null && _mainSequence.IsActive())
            _mainSequence.Kill();
    }

    // Для гизмоса в редакторе
    private void OnDrawGizmosSelected()
    {
        if (_point1 != null && _point2 != null && _point3 != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_point1.position, _point2.position);
            Gizmos.DrawLine(_point2.position, _point3.position);
            Gizmos.DrawLine(_point3.position, _point1.position);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_point1.position, 0.2f);
            Gizmos.DrawSphere(_point2.position, 0.2f);
            Gizmos.DrawSphere(_point3.position, 0.2f);
        }
    }
}