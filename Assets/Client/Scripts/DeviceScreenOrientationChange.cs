using System.Collections;
using System;
using UnityEngine;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Отслеживает изменения ориентации и разрешения экрана устройства
    /// </summary>
    public class DeviceScreenOrientationChange : MonoSingleton<DeviceScreenOrientationChange>
    {
        #region Fields
        private static ActionEvent<Vector2> _onResolutionChange = new ActionEvent<Vector2>();    //Событие смены разрещения устройства
        private static ActionEvent<DeviceOrientation> _onOrientationChange = new ActionEvent<DeviceOrientation>();     //Событие смены ориентации устройства

        private static Vector2 _currentResolution = Vector2.zero;  //Тукущее разрешение устройства
        private static DeviceOrientation _currentOrientation = DeviceOrientation.Unknown;//Тукущая ориентация устройства

        private const float _checkDelay = 0.5f; //задержка между проверками
        private bool _isChecking = true;    //метка активации корутины
        #endregion

        #region Properties
        /// <summary>
        /// Событие смены разрещения устройства
        /// </summary>
        public ActionEvent<Vector2> OnResolutionChanged { get => _onResolutionChange; }

        /// <summary>
        /// Событие смены ориентации устройства
        /// </summary>
        public ActionEvent<DeviceOrientation> OnOrientationChange { get => _onOrientationChange; }

        /// <summary>
        /// Тукущее разрешение устройства
        /// </summary>
        public Vector2 CurrentResolution { get => _currentResolution; }

        /// <summary>
        /// Тукущая ориентация устройства
        /// </summary>
        public DeviceOrientation CurrentOrientation { get => _currentOrientation; }
        #endregion

        #region Private Methods
        /// <summary>
        /// При активации объекта запускается корутина
        /// </summary>
        private void Start()
        {
            StartCoroutine(CheckForChange());
        }

        /// <summary>
        /// При уничтожении объекта останавливается корутина
        /// </summary>
        void OnDestroy()
        {
            _isChecking = false;
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Выполняет постоянную проверку изменений ориентации и разрешения экрана
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckForChange()
        {
            _currentResolution = new Vector2(Screen.width, Screen.height);
            _currentOrientation = Input.deviceOrientation;

            while (_isChecking)
            {
                //Проверка изменения разрешения
                if (_currentResolution.x != Screen.width || _currentResolution.y != Screen.height)
                {
                    //Смена разрешения и вызов события
                    _currentResolution = new Vector2(Screen.width, Screen.height);
                    _onResolutionChange.Invoke(_currentResolution);
                }

                //Проверка ориентации экрана
                switch (Input.deviceOrientation)
                {
                    case DeviceOrientation.Unknown:
                    case DeviceOrientation.FaceUp:
                    case DeviceOrientation.FaceDown:
                        break;
                    default:
                        //Проверка изменения только экрана
                        if (_currentOrientation != Input.deviceOrientation)
                        {
                            //Смена ориентации и вызов события
                            _currentOrientation = Input.deviceOrientation;
                            _onOrientationChange.Invoke(_currentOrientation);
                        }
                        break;
                }

                yield return new WaitForSeconds(_checkDelay);
            }
        }
        #endregion
    }
}
