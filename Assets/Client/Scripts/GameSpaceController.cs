using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace com.AndryKram.SpaceExplorer
{
    public class GameSpaceController : MonoSingleton<GameSpaceController>
    {
        #region Fields
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int _offsetProjection = new Vector2Int(5, 5);
        [SerializeField, Range(0,100)] private int _amountPlanetWithScore = 10;

        [Tooltip("Родитель всех созданых объектов")]
        [SerializeField] private Transform _gameSpaceParent = null;//объект родителя для всех созданных объектов
        [SerializeField] private GameObject _planetPrefab = null;//префаб объекта планет
        [SerializeField] private Vector2 _cellSize = Vector2.one;//Размер ячеек игрового поля
        [SerializeField, Range(0, 100)] private int _chanceSpawnPlanet = 30;//шанс создания в ячейке объекта

        private int _seedGameSpace;//семя игрового поля

        private Dictionary<Vector2Int, Planet> _planets = new Dictionary<Vector2Int, Planet>(); //текущие отображенные планеты
        private Stack<Planet> _poolPlanets = new Stack<Planet>();//пул планет

        private Camera _mainCamera = null;//основная камера
        private float _distanceCameraToGameSpace;//растояние от камеры до поля
        private Vector2Int _leftDownScreenPoint;// Левая нижняя ячейка поля доступная на экране
        private Vector2Int _rightUpScreenPoint;// Правая верхняя ячейка поля доступная на экране
        #endregion

        #region Properties
        /// <summary>
        /// Возвращает семя относительно которого создано игровое поле
        /// </summary>
        public int GameSpaceSeed { get => _seedGameSpace; }
        /// <summary>
        /// Левая нижняя ячейка поля доступная на экране
        /// </summary>
        public Vector2Int LeftDownScreenPosition { get => _leftDownScreenPoint; }
        /// <summary>
        /// Правая верхняя ячейка поля доступная на экране
        /// </summary>
        public Vector2Int RightUpScreenPosition { get => _rightUpScreenPoint - _offsetProjection; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Инициализирует и создает игровое поле
        /// </summary>
        public void InitializeGameSpace(int seed = 0)
        {
            //Проверка на поступление нового Seed
            if (seed == 0) _seedGameSpace = (int)System.DateTime.Now.GetHashCode();
            else _seedGameSpace = seed;

            //Отображение игрового поля
            UpdateAllViewprojection();

            ShowTopPlanet(Player.Instance.PlayerCellPosition, _amountPlanetWithScore);
        }

        /// <summary>
        /// Скрывает объекты с поля
        /// </summary>
        public void HideGameSpace()
        {
            _rightUpScreenPoint = _leftDownScreenPoint;
            CheckObjectsOffViewProjection();
        }

        /// <summary>
        /// Обновляет объекты области отображения при нсмещении
        /// </summary>
        public void UpdateGameSpace()
        {
            UpdateAllViewprojection();

            ShowTopPlanet(Player.Instance.PlayerCellPosition, _amountPlanetWithScore);
        }

        /// <summary>
        /// Получение центра ячейки в глобальных координатах
        /// </summary>
        /// <param name="cell">Ячейка</param>
        /// <returns></returns>
        public Vector3 GetCenterCell(Vector2Int cell)
        {
            var positionX = cell.x * _cellSize.x + _cellSize.x / 2f;
            var positionY = cell.y * _cellSize.y + _cellSize.y / 2f;
            return new Vector3(positionX, positionY, 0f);
        }

        public void ShowTopPlanet(Vector2Int cell, int amountTop)
        {
            var sortPlanet = _planets.OrderBy(kp => Vector2Int.Distance(kp.Key, cell)).ThenByDescending(kp => kp.Value.PlanetScore);

            foreach(var keyValue in sortPlanet)
            {
                if (amountTop > 0)
                {
                    keyValue.Value.ShowScore();
                    amountTop--;
                }
                else
                {
                    keyValue.Value.HideScore();
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Обновляет поле зрения
        /// </summary>
        private void UpdateViewProjection()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;

            _distanceCameraToGameSpace = Vector3.Distance(_mainCamera.transform.position, _gameSpaceParent.position);
            _leftDownScreenPoint = GetCellOnWorld(_mainCamera.ScreenToWorldPoint(new Vector3(0f,0f, _distanceCameraToGameSpace)));
            _rightUpScreenPoint = GetCellOnWorld(_mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _distanceCameraToGameSpace))) + _offsetProjection;
        }

        /// <summary>
        /// Проверяет ячеки внутри поля зрения
        /// </summary>
        private void UpdateAllViewprojection()
        {
            UpdateViewProjection();

            var currentCell = _leftDownScreenPoint;

            for(int x = _leftDownScreenPoint.x; x <= _rightUpScreenPoint.x; x++)
            {
                for (int y = _leftDownScreenPoint.y; y <= _rightUpScreenPoint.y; y++)
                {
                    currentCell = new Vector2Int(x, y);
                    CheckPlanetInCell(ref currentCell);
                }
            }

            //проверка объектов вне поля отображения
            CheckObjectsOffViewProjection();
        }

        /// <summary>
        /// Проверяет и скрывает планеты вне поля зрения
        /// </summary>
        private void CheckObjectsOffViewProjection()
        {
            //получение списка ключей выходящих за поле зрения
            var result = _planets.Keys.Where(k => (k.x < _leftDownScreenPoint.x || k.x > _rightUpScreenPoint.x || k.y < _leftDownScreenPoint.y || k.y > _rightUpScreenPoint.y)).ToList();

            //Скрытие и перенос в пул полученных объектов
            foreach (Vector2Int cell in result)
            {
                var planet = _planets[cell];
                planet.HidePlanet();
                _poolPlanets.Push(planet);
                _planets.Remove(cell);
            }
        }

        /// <summary>
        /// Проверка появления планеты в ячейки 
        /// Создание планеты в ячейке
        /// </summary>
        /// <param name="cell">Ячейка на проверку</param>
        private void CheckPlanetInCell(ref Vector2Int cell)
        {
            //еще не отображены данная планета
            if (!_planets.ContainsKey(cell))
            {
                //проверка шанса появления планеты
                if (CheckChancePlanetSpawn(ref cell))
                {
                    //Получение планеты и установка в ячейкку
                    var objectPlanet = GetPlanet();

                    objectPlanet.InitializePlanet(GetSeedCell(ref cell), GetCenterCell(cell));

                    //добавление новой ячейки с планетой
                    _planets.Add(cell, objectPlanet);
                }
            }
        }
        
        /// <summary>
        /// Проверяет возможность появления планеты в указанной ячейке
        /// </summary>
        /// <param name="cell">Ячейка на проверку</param>
        private bool CheckChancePlanetSpawn(ref Vector2Int cell)
        {
            //Установка seed рандома для конкретной ячейки
            UnityEngine.Random.InitState(GetSeedCell(ref cell));
            //Получение числа шанса появления планеты
            int chance = UnityEngine.Random.Range(0,10000) / 100;

            //проверка появления шанса
            return chance <= _chanceSpawnPlanet;
        }

        private int GetSeedCell(ref Vector2Int cell)
        {
            return (int)System.Math.Floor((float)_seedGameSpace + cell.GetHashCode());
        }

        /// <summary>
        /// Получение новой планеты из пула 
        /// или создание нового экземпляра
        /// </summary>
        /// <returns>Экземпляр GameObject префаба планеты</returns>
        private Planet GetPlanet()
        {
            //проверка на опустошение пула
            if (_poolPlanets.Count == 0)
            {
                //создание новой планеты
                var objectPlanet = GameObject.Instantiate(_planetPrefab, _gameSpaceParent).GetComponent<Planet>();

                //возвращение новой планеты
                return objectPlanet;
            }

            //возвращение планеты из пула
            return _poolPlanets.Pop();
        }

        /// <summary>
        /// Получение ячейки в которую входит позиция
        /// </summary>
        /// <param name="position">позиция на проверку</param>
        /// <returns></returns>
        private Vector2Int GetCellOnWorld(Vector3 position)
        {
            var cellX = (int)System.Math.Floor(position.x / _cellSize.x);
            var cellY = (int)System.Math.Floor(position.y / _cellSize.y);
            return new Vector2Int(cellX, cellY);
        }

        #endregion
    }
}
