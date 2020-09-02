using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.AndryKram.SpaceExplorer
{
    public class Planet : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Text _scoreText = null;

        private GameObject _planetGameObject = null;
        private Transform _planetTransform = null;
        #endregion

        #region Properties
        public int PlanetScore { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Инициализация планеты
        /// </summary>
        /// <param name="seedPlanet"></param>
        /// <param name="position"></param>
        public void InitializePlanet(int seedPlanet, Vector3 position)
        {
            if (_planetGameObject == null)
            {
                _planetGameObject = gameObject;
                _planetTransform = transform;
            }

            //расположение планеты
            _planetTransform.position = position;
            _planetGameObject.SetActive(true);

            //Установка seed рандома для планеты
            UnityEngine.Random.InitState(seedPlanet);

            //Установка рейтинга планеты
            PlanetScore = UnityEngine.Random.Range(0, 10000);
            _scoreText.text = PlanetScore.ToString();
            _scoreText.enabled = false;
        }

        /// <summary>
        /// Скрытие планеты
        /// </summary>
        public void HidePlanet()
        {
            _planetGameObject.SetActive(false);
        }

        /// <summary>
        /// Отображение рейтинга планеты
        /// </summary>
        public void ShowScore()
        {
            _scoreText.enabled = true;
        }

        /// <summary>
        /// Скрытие рейтинга планеты
        /// </summary>
        public void HideScore()
        {
            _scoreText.enabled = false;
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
