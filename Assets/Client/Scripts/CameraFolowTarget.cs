using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Передвигает камеру к цели
    /// </summary>
    public class CameraFolowTarget : MonoSingleton<CameraFolowTarget>
    {
        #region Fields
        [SerializeField] private Transform _target = null;//цель которую отслеживает камера
        [Range(1f, 40f),SerializeField] private float _moveLaziness = 10f;//задержка движения камеры
        [Range(0.01f,0.5f),SerializeField] private float _stickingDistance = 0.1f;//растояние до камеры при котором останавливается движение
        [SerializeField] private bool _lookAtTarget = true;//направление камеры в цель
        [SerializeField] private bool _takeOffsetFromInitialPos = true;//запомнить начальное смещение до цели
        [SerializeField] private Vector3 _generalOffset;//смещение до цели

        private Transform _cameraTransform = null;//трансформ камеры
        private Vector3 _whereCameraShouldBe;//точка к которой перемещаются
        private bool _isMove = false;//метка перемещения

        private ActionEvent _onMoveToTarget = new ActionEvent();
        #endregion

        #region Properties
        /// <summary>
        /// Событие окончания передвижения камеры к цели
        /// </summary>
        public ActionEvent OnMoveToTarget { get => _onMoveToTarget; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Мгновенно переместить камеру к объекту
        /// </summary>
        public void GoToTarget()
        {
            _whereCameraShouldBe = _target.position + _generalOffset;
            _cameraTransform.position = _whereCameraShouldBe;
        }

        /// <summary>
        /// Плавное перемещение камеры к цели
        /// </summary>
        public void MoveToTarget()
        {
            _whereCameraShouldBe = _target.position + _generalOffset;
            if (!_isMove)
                StartCoroutine(Move());
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Запоминание начальных натсроек
        /// </summary>
        private void Start()
        {
            _cameraTransform = transform;
            if (_takeOffsetFromInitialPos && _target != null) _generalOffset = _cameraTransform.position - _target.position;
        }

        /// <summary>
        /// Перемещение к цели
        /// </summary>
        /// <returns></returns>
        private IEnumerator Move()
        {
            _isMove = true;
            _whereCameraShouldBe = _target.position + _generalOffset;

            //Перемещение
            while(_isMove)
            {
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _whereCameraShouldBe, 1 / _moveLaziness);

                if (_lookAtTarget) transform.LookAt(_target);

                if (Vector3.Distance(_cameraTransform.position, _whereCameraShouldBe) < _stickingDistance)
                {
                    _isMove = false;
                    break;
                }

                _onMoveToTarget.Invoke();
                yield return new WaitForFixedUpdate();
            }

            _cameraTransform.position = _whereCameraShouldBe;
        }
        #endregion
    }
}
