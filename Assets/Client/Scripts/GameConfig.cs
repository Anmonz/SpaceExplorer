using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Управляет сохранением и загрузкой информации о игровом мире
    /// </summary>
    public class GameConfig : MonoSingleton<GameConfig>
    {
        #region Fields
        private const string IS_SAVED_CONFIG_KEY = "IsSavedConfig";//ключ выполения сохранения

        private const string SEED_KEY = "SeedGameSpace";//ключ семени генерации мира
        private int _seedGameSpace = 0;//семя генерации мира

        private const string PLAYER_POSITION_X_KEY = "PlayerPositionX";//ключ позиции игрока по X
        private const string PLAYER_POSITION_Y_KEY = "PlayerPositionY";//ключ позиции игрока по Y
        private Vector2Int _playerPosition = Vector2Int.zero;//позиция игрока

        private const string RESOURCES_PATH_SPRITES_PLANETS = "Sprites/Planets";
        private List<Sprite> _planetsSprites = null;
        #endregion

        #region Properties
        /// <summary>
        /// Семя генерации мира
        /// </summary>
        public int SeedGameSpace { get => _seedGameSpace; }
        /// <summary>
        /// Позиция игрока
        /// </summary>
        public Vector2Int PlayerPosition { get => _playerPosition; }
        /// <summary>
        /// Различные картинки планет
        /// </summary>
        public List<Sprite> PlanetsSprites { get => _planetsSprites; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Сохраняет данные мира
        /// </summary>
        /// <param name="seed">семя мира</param>
        /// <param name="playerPosition">позиция игрока</param>
        public void SaveGameConfig(int seed,Vector2Int playerPosition)
        {
            _seedGameSpace = seed;
            _playerPosition = playerPosition;

            SaveConfig();
        }

        /// <summary>
        /// Загружает данные если они были сохранены
        /// </summary>
        public void LoadGameConfig()
        {
            LoadConfig();
        }

        /// <summary>
        /// Проверяет были ли сохранены данные
        /// </summary>
        /// <returns></returns>
        public bool IsSavedGameConfig()
        {
            return PlayerPrefs.HasKey(IS_SAVED_CONFIG_KEY);
        }

        /// <summary>
        /// Обнуляет все сохраненные данные
        /// </summary>
        public void ResetGameConfig()
        {
            _seedGameSpace = 0;
            _playerPosition = Vector2Int.zero;

            PlayerPrefs.DeleteAll();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Загружает спрайты планет из ресурсов
        /// </summary>
        private void Start()
        {
            _planetsSprites = new List<Sprite>(Resources.LoadAll<Sprite>(RESOURCES_PATH_SPRITES_PLANETS));
        }

        /// <summary>
        /// Выполненяе загрузку сохраненных данных
        /// </summary>
        private void LoadConfig()
        {
            if (!IsSavedGameConfig()) return;
            
            if(PlayerPrefs.HasKey(SEED_KEY))
            {
                _seedGameSpace = PlayerPrefs.GetInt(SEED_KEY);
            }

            if(PlayerPrefs.HasKey(PLAYER_POSITION_X_KEY))
            {
                _playerPosition = new Vector2Int(PlayerPrefs.GetInt(PLAYER_POSITION_X_KEY), PlayerPrefs.GetInt(PLAYER_POSITION_Y_KEY));
            }
        }

        /// <summary>
        /// Сохраняет данные
        /// </summary>
        private void SaveConfig()
        {
            PlayerPrefs.SetInt(SEED_KEY, _seedGameSpace);
            PlayerPrefs.SetInt(PLAYER_POSITION_X_KEY, _playerPosition.x);
            PlayerPrefs.SetInt(PLAYER_POSITION_Y_KEY, _playerPosition.y);
            PlayerPrefs.SetInt(IS_SAVED_CONFIG_KEY, 1);
        }
        #endregion
    }
}
