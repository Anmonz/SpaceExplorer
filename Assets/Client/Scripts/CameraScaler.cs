﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Изменяет масштаб камеры
    /// </summary>
    public class CameraScaler : MonoSingleton<CameraScaler>
    {
        #region Fields
        [SerializeField] private Camera _mainCamera = null;        //основная камера
        [SerializeField] private float _minSizeCameraScale = 4.5f; //миниальный размер камеры
        [SerializeField] private float _maxSizeCameraScale = 90f; //максимальный размер камеры
        [SerializeField] private float _speedCameraScale = 0.05f;   //скорость изменения размера камеры

        private float _orientationScaleChanger = 1.8f;//значение для изменения масштаба камеры при повороте (16/9 = 1.8)
        private bool _isLandscapeOrientation = false;//метка изменения ориентациии камеры

        private SpaceExplorerInputs _inputActions = null;//класс InputActions

        private Vector2 _lastPositionTouchOne = Vector2.zero;//последняя позиция на экране первого касания
        private Vector2 _lastPositionTouchTwo = Vector2.zero;//последняя позиция на экране второго касания
        private float _lastDistanceTouch = 0;//последнее растояние между касаниями

        private bool _isTouchOne = false;//метка первого нажатия

        private bool _isActivated = false;

        private ActionEvent _onScaleCameraEvent = new ActionEvent();
        #endregion

        #region Properties
        /// <summary>
        /// Событие при масштабировании камеры
        /// </summary>
        public ActionEvent OnScaleCameraSize { get => _onScaleCameraEvent; }

        /// <summary>
        /// Метка изменения размера камеры
        /// </summary>
        public bool IsScalingCamera { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Инициализирует события нажатия на экран
        /// и событие изменения ориентации
        /// </summary>
        public void ActivateCameraScaler()
        {
            _isActivated = true;

            _inputActions.Gameplay.TouchOne.started += OnStartedTouch;
            _inputActions.Gameplay.TouchOne.canceled += OnCanceledTouch;

            _inputActions.Gameplay.TouchTwo.started += OnStartedTouch;
            _inputActions.Gameplay.TouchTwo.canceled += OnCanceledTouch;

            _inputActions.Enable();
            
            //DeviceScreenOrientationChange.Instance.OnOrientationChange.AddListener(OnChangeOrientationScreen);
        }
    
        /// <summary>
        /// Отключает взаимодействие пользователя с масштабированием камеры
        /// </summary>
        public void DeactivateCameraScaler()
        {
            _isActivated = false;

            _inputActions.Disable();

            _inputActions.Gameplay.TouchOne.started -= OnStartedTouch;
            _inputActions.Gameplay.TouchOne.canceled -= OnCanceledTouch;

            _inputActions.Gameplay.TouchTwo.started -= OnStartedTouch;
            _inputActions.Gameplay.TouchTwo.canceled -= OnCanceledTouch;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Проверяет текущую ориентацию экрана
        /// </summary>
        private void Start()
        {
            _inputActions = new SpaceExplorerInputs();
            //OnChangeOrientationScreen(DeviceScreenOrientationChange.Instance.CurrentOrientation);
        }

        /// <summary>
        /// При уничтожении объекта отписывает от подписаных событий
        /// </summary>
        private void OnDestroy()
        {
            if (_isActivated && _inputActions != null)
            {
                _inputActions.Gameplay.TouchOne.started -= OnStartedTouch;
                _inputActions.Gameplay.TouchOne.canceled -= OnCanceledTouch;

                _inputActions.Gameplay.TouchTwo.started -= OnStartedTouch;
                _inputActions.Gameplay.TouchTwo.canceled -= OnCanceledTouch;
                //освобождение экземпляра
                _inputActions.Dispose();
            }

            //DeviceScreenOrientationChange.Instance.OnOrientationChange.RemoveListener(OnChangeOrientationScreen);
        }

        /// <summary>
        /// При активации компонента включает экземпляр данных от системы ввода
        /// </summary>
        private void OnEnable()
        {
            if(_inputActions != null) _inputActions.Enable();
        }

        /// <summary>
        /// При отключении компонента выключает экземпляр данных от системы ввода
        /// </summary>
        private void OnDisable()
        {
            if (_inputActions != null) _inputActions.Disable();
        }

        /// <summary>
        /// Отслеживает событие нажатий на экран
        /// </summary>
        /// <param name="context"></param>
        private void OnStartedTouch(InputAction.CallbackContext context)
        {
            //Проверка на перове нажатие
            if(!_isTouchOne)
            {
                _isTouchOne = true;
                return;
            }

            //Устанавливает начальное положение 
            _lastPositionTouchOne = _inputActions.Gameplay.TouchOnePosition.ReadValue<Vector2>();
            _lastPositionTouchTwo = _inputActions.Gameplay.TouchTwoPosition.ReadValue<Vector2>();
            _lastDistanceTouch = Vector2.Distance(_lastPositionTouchOne, _lastPositionTouchTwo);

            //Запускает корутину изменения размера камеры
            StartCoroutine(ScaleSize());
        }

        /// <summary>
        /// отслеживает отмены нажатия на экран
        /// </summary>
        /// <param name="context"></param>
        private void OnCanceledTouch(InputAction.CallbackContext context)
        {
            _isTouchOne = false;
        }

        /// <summary>
        /// отслеживает событие смены ориентации экрана
        /// </summary>
        /// <param name="orientation"></param>
        private void OnChangeOrientationScreen(DeviceOrientation orientation)
        {
            switch(orientation)
            {
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    if (!_isLandscapeOrientation)
                    {
                        _isLandscapeOrientation = true;
                        ChangeOrientationScaler();
                        _mainCamera.orthographicSize /= _orientationScaleChanger;
                    }
                    break;
                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    if (_isLandscapeOrientation)
                    {
                        _isLandscapeOrientation = false;
                        ChangeOrientationScaler();
                        _mainCamera.orthographicSize *= _orientationScaleChanger;
                    }
                    break;
            }
        }

        /// <summary>
        /// Изменяет значение масштабирования при повороте экрана
        /// </summary>
        private void ChangeOrientationScaler()
        {
            var resolution = DeviceScreenOrientationChange.Instance.CurrentResolution;
            _orientationScaleChanger = resolution.y / resolution.x;
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Корутина изменения размера камеры при движении двух касаний на экране
        /// </summary>
        /// <returns></returns>
        private IEnumerator ScaleSize()
        {
            IsScalingCamera = true;
            CameraFolowTarget.Instance.GoToTarget();
            while (_isTouchOne)
            {
                //вычисление текущих позиций касаний
                var currentTouchOne = _inputActions.Gameplay.TouchOnePosition.ReadValue<Vector2>();
                var currentTouchTwo = _inputActions.Gameplay.TouchTwoPosition.ReadValue<Vector2>();
                var currentDistanceTouch = Vector2.Distance(currentTouchOne, currentTouchTwo);
                
                if((_lastDistanceTouch - currentDistanceTouch) == 0) yield return new WaitForFixedUpdate();

                //вычисление нового значения размера камеры
                var scale = _mainCamera.orthographicSize + (_lastDistanceTouch - currentDistanceTouch) / 2f * _speedCameraScale;

                //изменение рамок масштабирование из-за ориентации
                if(!_isLandscapeOrientation)
                    _mainCamera.orthographicSize = Mathf.Clamp(scale, _minSizeCameraScale, _maxSizeCameraScale);
                else
                    _mainCamera.orthographicSize = Mathf.Clamp(scale, _minSizeCameraScale / _orientationScaleChanger, _maxSizeCameraScale / _orientationScaleChanger);

                OnScaleCameraSize.Invoke();

                //запоминание текущих позиций касаний
                _lastPositionTouchOne = currentTouchOne;
                _lastPositionTouchTwo = currentTouchTwo;
                _lastDistanceTouch = currentDistanceTouch;

                yield return new WaitForFixedUpdate();
            }
            IsScalingCamera = false;
        }
        #endregion
    }
}
