using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Управляет интерфейсом из двух состояний и 3х кнопок
    /// </summary>
    public class GUIController : MonoSingleton<GUIController>
    {
        #region Fields
        [Header("Start Menu")]
        [SerializeField] private GameObject _startMenu = null;//родительский элемент для стартового состояния интерфейса
        [SerializeField] private Button _newGameBtn = null;//кнопка сброса сохранения игры и генерации нового мира
        [SerializeField] private Button _loadGameBtn = null;//кнопка загрузки старой генерации мира

        [Header("Game Menu")]
        [SerializeField] private GameObject _gameMenu = null;//родительский элемент для игрового состояния интерфейса
        [SerializeField] private Button _toMenuBtn = null;//кнопка возвращения в стартовое меню

        private bool _isStart = false;//метка активного стартового меню
        #endregion

        #region Properties
        /// <summary>
        /// Событие нажатия на кнопку запуска новой игры
        /// </summary>
        public UnityEvent OnClickNewGameButton { get => _newGameBtn.onClick; }
        /// <summary>
        /// Событие нажатия на кнопку загрузки игры
        /// </summary>
        public UnityEvent OnClickLoadGameButton { get => _loadGameBtn.onClick; }
        /// <summary>
        /// Событие нажатия на кнопку выхода из игры в стартовое меню
        /// </summary>
        public UnityEvent OnClickToStartMenuButton { get => _toMenuBtn.onClick; }
        #endregion 

        #region Public Methods
        /// <summary>
        /// Сменяет состояние игрового интерфейса со стартового на игровое и наоборот
        /// </summary>
        public void ChangeMenu()
        {
            if (_isStart)
            {
                _isStart = false;
                _startMenu.SetActive(false);
                _gameMenu.SetActive(true);
            }
            else
            {
                _isStart = true;
                _startMenu.SetActive(true);
                _gameMenu.SetActive(false);

                CheckSavedGameConfig();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Запускает смену состояния на стартовое меню
        /// </summary>
        private void Start()
        {
            _isStart = false;
            ChangeMenu();
        }

        /// <summary>
        /// Проверяет хранится ли сохраненый мир и если нет отключает кнопку загрузки
        /// </summary>
        private void CheckSavedGameConfig()
        {
            if (!GameConfig.Instance.IsSavedGameConfig())
            {
                _loadGameBtn.interactable = false;
            }
            else
            {
                _loadGameBtn.interactable = true;
            }
        }
        #endregion
    }
}
