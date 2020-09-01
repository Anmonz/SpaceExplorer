using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace com.AndryKram.SpaceExplorer
{
    public class GameSpaceController : MonoSingleton<GameSpaceController>
    {
        #region Fields
        [Header("View Settings")]

        [Tooltip("Установить объект камеры или игрока")]
        [SerializeField] private Transform _objectCenterViewPosition; //точка центра отображения игрового пространства (Камеру поставить)
        [Tooltip("Растояние от объекта до края куба области отображения объектов")]
        [SerializeField] private int _radiusCubeViewSpace; //радиус окружности от центра отображения игрового пространства

        [Header("Grid Settings")]

        [Tooltip("Родитель всех созданых объектов")]
        [SerializeField] private Transform _gameSpaceParent;//объект родителя для всех созданных объектов
        [SerializeField] private GameObject _planetPrefab;//префаб объекта планет
        [SerializeField] private Vector2 _cellSize;//Размер ячеек игрового поля
        [SerializeField, Range(0, 100)] private int _chanceSpawnPlanet = 30;//шанс создания в ячейке объекта

        private int _seedGameSpace;//семя игрового поля

        private Dictionary<Vector2Int, GameObject> _planets = new Dictionary<Vector2Int, GameObject>(); //текущие отображенные планеты
        private Stack<GameObject> _poolPlanets = new Stack<GameObject>();//пул планет

        private Vector2Int _lastCenterView;//последняя позиция объекта-центра области отображения
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
            CheckAllCellViewGameSpace();
        }

        /// <summary>
        ///Обновляет всю область отображения
        /// </summary>
        public void UpdateAllGameSpace()
        {
            CheckAllCellViewGameSpace();
        }

        /// <summary>
        /// Обновляет объекты области отображения при нсмещении
        /// </summary>
        public void UpdateDisplacementGameSpace()
        {
            CheckCellDiplacementCenterViewGameSpace();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Проверка ячеек входящих в растояние перемещения центра области отображения
        /// </summary>
        private void CheckCellDiplacementCenterViewGameSpace()
        {
            //получение ячейки для позиции объекта центра области отображения
            var centerCell = GetCellOnWorld(_objectCenterViewPosition.position);

            //вектор смещения относительно старого центра
            var displacementCenter = centerCell - _lastCenterView;

            //проверка на нулевое смещение
            if (displacementCenter == Vector2Int.zero) return;

            //вычисление дистанций смещения по осям
            var offsetX = Mathf.Abs(displacementCenter.x);
            var offsetY = Mathf.Abs(displacementCenter.y);

            //проверка смещения больше чем радиус куба области отображения
            if (offsetX >= _radiusCubeViewSpace || offsetY >= _radiusCubeViewSpace)
            {
                //выполнение глобальной проверки для области отображения
                CheckAllCellViewGameSpace();
                return;
            }

            //вычисление знака смещения по оси X
            var signX = (int)Mathf.Sign(displacementCenter.x);

            //проверка ячеек попавших в смещение по оси X
            for (int x = (_radiusCubeViewSpace - offsetX);  x <= _radiusCubeViewSpace; x++)
            {
                for (int i = -(_radiusCubeViewSpace); i < (_radiusCubeViewSpace); i++)
                {
                    var currentCell = centerCell + new Vector2Int(x * signX, i);
                    CheckPlanetInCell(ref currentCell);
                }
            }

            //вычисление знака смещения по оси Y
            var signY = (int)Mathf.Sign(displacementCenter.y);

            //проверка ячеек попавших в смещение по оси Y
            for (int y = (_radiusCubeViewSpace - offsetY); y <= _radiusCubeViewSpace; y++)
            {
                for (int i = -(_radiusCubeViewSpace); i < (_radiusCubeViewSpace); i++)
                {
                    var currentCell = centerCell + new Vector2Int(i, y * signY);
                    CheckPlanetInCell(ref currentCell);
                }
            }

            //сохранение центра
            _lastCenterView = centerCell;

            //проверка объектов вне поля отображения
            CheckPlanetsOffViewSpace();
        }

        /// <summary>
        /// Проверяет все ячейки входящие в область отображения
        /// </summary>
        private void CheckAllCellViewGameSpace()
        {
            //получение ячейки для позиции объекта центра области отображения
            var centerCell = GetCellOnWorld(_objectCenterViewPosition.position);

            //проверка всех ячеек по радиусу куба
            for (int r = 0; r <= _radiusCubeViewSpace; r++)
            {
                for (int i = -r; i < r; i++)
                {
                    //нижняя грань
                    var currentCell = centerCell + new Vector2Int(i, -r);
                    CheckPlanetInCell(ref currentCell);

                    //правая грань
                    currentCell = centerCell + new Vector2Int(r, i);
                    CheckPlanetInCell(ref currentCell);

                    //верхняя грань
                    currentCell = centerCell + new Vector2Int(-i, r);
                    CheckPlanetInCell(ref currentCell);

                    //левая грань
                    currentCell = centerCell + new Vector2Int(-r, -i);
                    CheckPlanetInCell(ref currentCell);
                }
            }

            //сохранение центра
            _lastCenterView = centerCell;

            //проверка объектов вне поля отображения
            CheckPlanetsOffViewSpace();
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

                    objectPlanet.transform.position = GetCenterCell(cell);
                    objectPlanet.SetActive(true);

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
        private GameObject GetPlanet()
        {
            //проверка на опустошение пула
            if (_poolPlanets.Count == 0)
            {
                //создание новой планеты
                var objectPlanet = GameObject.Instantiate(_planetPrefab, _gameSpaceParent);

                //возвращение новой планеты
                return objectPlanet;
            }

            //возвращение планеты из пула
            return _poolPlanets.Pop();
        }

        /// <summary>
        /// Скрывает планеты вне поля отображения объектов
        /// </summary>
        private void CheckPlanetsOffViewSpace()
        {
            //вычисление крайних углов диагонали
            var leftDown = _lastCenterView + new Vector2Int(-1, -1) * (int)_radiusCubeViewSpace;
            var rightUp = _lastCenterView + new Vector2Int(1, 1) * (int)_radiusCubeViewSpace;

            //получение списка ключей выходящих за поле зрения
            var result = _planets.Keys.Where(k => (k.x < leftDown.x || k.x > rightUp.x || k.y < leftDown.y || k.y > rightUp.y) ).ToList();

            //Скрытие и перенос в пул полученных объектов
            foreach(Vector2Int cell in result)
            {
                var planet = _planets[cell];
                planet.SetActive(false);
                _poolPlanets.Push(planet);
                _planets.Remove(cell);
            }
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

        /// <summary>
        /// Получение центра ячейки в глобальных координатах
        /// </summary>
        /// <param name="cell">Ячейка</param>
        /// <returns></returns>
        private Vector3 GetCenterCell(Vector2Int cell)
        {
            var positionX = cell.x * _cellSize.x + _cellSize.x / 2f;
            var positionY = cell.y * _cellSize.y + _cellSize.y / 2f;
            return new Vector3(positionX, positionY, 0f);
        }
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Отображает в редакторе размер поля отображения объектов
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_objectCenterViewPosition.position, _cellSize * _radiusCubeViewSpace * 2f); 
        }
#endif
    }
}
