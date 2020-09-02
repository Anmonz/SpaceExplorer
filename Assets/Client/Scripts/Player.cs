using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// обрабатывает логику игрока
    /// </summary>
    public class Player : MonoSingleton<Player>
    {
        #region Fields
        [SerializeField] private GameObject _playerObject = null;//главный объект игрока
        [SerializeField] private Transform _playerTransform = null;//компонент перемещения игрока
        [SerializeField] private SwipeController _playerController = null;//контролер свайпов для управления игроком
        [SerializeField] private float _timeMove = 1f;//время перемещения по позициям
        [SerializeField] private AnimationCurve _curveMove = AnimationCurve.Linear(0,0,1,1);//кривая движения

        private bool _isMove = false;//метка текущего активного перемещения
        private Vector2Int _playerPositionCell = Vector2Int.zero;//текущая клетка расположения игрока
        #endregion

        #region Properties
        /// <summary>
        /// Текущая позиция расположения игрока
        /// </summary>
        public Vector2Int PlayerCellPosition { get => _playerPositionCell; }

        public ActionEvent OnMovePlayer { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Активирует работу игрока и отслеживание свайпов
        /// </summary>
        public void ActivatePlayer()
        {
            //устанавливает начальную позицию из конфигурации
            _playerPositionCell = GameConfig.Instance.PlayerPosition;
            _playerTransform.position = GameSpaceController.Instance.GetCenterCell(_playerPositionCell);

            //включает отслеживание свайпов экрана
            _playerController.enabled = true;
            _playerController.OnSwipe.AddListener(MovePlayerTo);

            //Отображает игрока
            _playerObject.SetActive(true);
        }

        /// <summary>
        /// Останавливает работу игрока
        /// </summary>
        public void DeactivatePlayer()
        {
            //отключает отслеживание свайпов по экрану
            _playerController.enabled = false;
            _playerController.OnSwipe.RemoveListener(MovePlayerTo);

            //скрвает игрока
            _playerObject.SetActive(false);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Находит данные если они небыли установленны в инспекторе
        /// </summary>
        private void Start()
        {
            //проверяет главный объект игрока
            if (_playerObject == null) _playerObject = this.gameObject;

            //скрывает игрока
            _playerObject.SetActive(false);

            //проверяет компонент перемещения игрока
            if (_playerTransform == null) _playerTransform = this.transform;

            //проверяет контролер свайпов для управления игроком
            if (_playerController == null) _playerController = _playerObject.AddComponent<SwipeController>();
            _playerController.enabled = false;
        }

        /// <summary>
        /// Перемещает игрока в указанном направлении
        /// </summary>
        /// <param name="direction"></param>
        private void MovePlayerTo(Vector2 direction)
        {
            //проверка текущего движения
            if (!_isMove)
            {
                _isMove = true;

                Vector2Int moveDirection;

                //вычисление конкретного вектора движения из общего направления
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    moveDirection = Vector2Int.right * (int)Mathf.Sign(direction.x);
                }
                else
                {
                    moveDirection = Vector2Int.up * (int)Mathf.Sign(direction.y);
                }

                //смена позиции игрока
                _playerPositionCell += moveDirection;

                //Запуск перемещения игрока
                StartCoroutine(MoveCoroutine(GameSpaceController.Instance.GetCenterCell(_playerPositionCell), _timeMove));
            }
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Корутина перемещения игрока в позицию за время
        /// </summary>
        /// <param name="newPosition">Новая позиция</param>
        /// <param name="time">Время перемещения</param>
        /// <returns></returns>
        private IEnumerator MoveCoroutine(Vector3 newPosition, float time)
        {
            float startTime = Time.time;
            float t;
            Vector3 startPosition = _playerTransform.position;

            //Перемещение
            while((t = (Time.time - startTime) / time) < 1)
            {
                _playerTransform.position = Vector3.Lerp(startPosition, newPosition, _curveMove.Evaluate(t));
                yield return new WaitForFixedUpdate();
            }

            _playerTransform.position = newPosition;
            _isMove = false;

            //выполнение события перемещения игрока
            OnMovePlayer.Invoke();
        }
        #endregion
    }
}
