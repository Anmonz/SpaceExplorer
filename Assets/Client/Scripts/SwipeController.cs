using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Отслеживает свайп игрока пальцем по экрану
    /// </summary>
    public class SwipeController : MonoBehaviour
    {
        #region Fields
        [SerializeField] private float _distancePixelsForSwipe = 100;//растояние в пикселях для свайпа экрана

        private SpaceExplorerInputs _inputActions = null;//клас InputAsset
        private bool _isTouch = false;//метка нажатия на экран

        private float _currentSwipeValue = 0;//значение прохождения растояния для свайпа
        private Vector2 _swipeDirection = Vector2.zero;//направление свайпа

        private ActionEvent<Vector2> _onSwipe = new ActionEvent<Vector2>();//событие свайпа
        #endregion

        #region Properties
        /// <summary>
        /// Событие при свайпе по экрану на заданное растояние
        /// </summary>
        public ActionEvent<Vector2> OnSwipe { get => _onSwipe; }
        #endregion

        #region Private Methods
        /// <summary>
        /// Подписывает на события ввода нажатие на экран
        /// </summary>
        private void Awake()
        {
            _inputActions = new SpaceExplorerInputs();

            _inputActions.Player.Touch.started += OnStartedTouch;
            _inputActions.Player.Touch.canceled += OnCanceledTouch;
        }

        /// <summary>
        /// При уничтожении очищает экземпляр ввода
        /// </summary>
        private void OnDestroy()
        {
            _onSwipe.RemoveAllListener();

            if (_inputActions != null)
            {
                _inputActions.Player.Touch.started -= OnStartedTouch;
                _inputActions.Player.Touch.canceled -= OnCanceledTouch;

                _inputActions.Dispose();
            }
        }

        /// <summary>
        /// При включении востанавливает рассылку события ввода
        /// </summary>
        private void OnEnable()
        {
            if (_inputActions != null) _inputActions.Enable();
        }

        /// <summary>
        /// При отключении приостанавливает рассылку события ввода
        /// </summary>
        private void OnDisable()
        {
            if (_inputActions != null) _inputActions.Disable();
        }

        /// <summary>
        /// Обрабатывает события нажатия на экран
        /// </summary>
        /// <param name="context"></param>
        private void OnStartedTouch(InputAction.CallbackContext context)
        {
            _isTouch = true;
            //запускает корутину отслеживания свайпа
            StartCoroutine(Swipe());
        }

        /// <summary>
        /// Обрабатывает событие отпускания нажатия на экран
        /// </summary>
        /// <param name="context"></param>
        private void OnCanceledTouch(InputAction.CallbackContext context)
        {
            _isTouch = false;

            //При прохождении растояния свайпа
            if(_currentSwipeValue >= 1)
            {
                //вызов события свайпа
                _onSwipe.Invoke(_swipeDirection);

                //сброс текущего значения проходжения рстрояния свайпа
                _currentSwipeValue = 0;
            }
        }
        #endregion

        #region Coroutine
        /// <summary>
        /// Корутина отслеживания растояния между начальной позицией нажатия и текущей
        /// </summary>
        /// <returns></returns>
        private IEnumerator Swipe()
        {
            //начальная позиция
            var startTouchPosition = _inputActions.Player.TouchPosition.ReadValue<Vector2>();

            while (_isTouch)
            {
                //текущая позиция
                var currentTouchPosition = _inputActions.Player.TouchPosition.ReadValue<Vector2>();
                //направление передвижения
                _swipeDirection = currentTouchPosition - startTouchPosition;
                //значение пройденного растояния
                _currentSwipeValue = _swipeDirection.magnitude / _distancePixelsForSwipe;

                yield return new WaitForFixedUpdate();
            }
        }
        #endregion
    }
}
