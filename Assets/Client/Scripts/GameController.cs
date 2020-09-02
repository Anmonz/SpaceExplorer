using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram.SpaceExplorer
{
    /// <summary>
    /// Управляет состоянием игры через взаимодействие с интерфейсом
    /// </summary>
    public class GameController : MonoBehaviour
    {
        #region Fields
        private bool _isStartGame = false;//метка запущенной игры
        #endregion

        #region Private Methods
        /// <summary>
        /// Подписывется на нажатие кнопок на интерфейса
        /// </summary>
        private void Start()
        {
            GUIController.Instance.OnClickLoadGameButton.AddListener(LoadGame);
            GUIController.Instance.OnClickNewGameButton.AddListener(NewGame);
            GUIController.Instance.OnClickToStartMenuButton.AddListener(StopGame);
        }

        /// <summary>
        /// Выполняет загрузку сохраненной конфигурации мира 
        /// и запускает игру
        /// </summary>
        private void LoadGame()
        {
            GameConfig.Instance.LoadGameConfig();
            StartGame();
        }

        /// <summary>
        /// Скидывает сохраненную конфигурацию мира
        /// и запускает игру
        /// </summary>
        private void NewGame()
        {
            GameConfig.Instance.ResetGameConfig();
            StartGame();
        }

        /// <summary>
        /// Запускает игру
        /// </summary>
        private void StartGame()
        {
            _isStartGame = true;
            // Активирует игрока
            Player.Instance.ActivatePlayer();
            //Подписывание обновление игрового пространства на перемещение игрока
            Player.Instance.OnMovePlayer.AddListener(() => GameSpaceController.Instance.UpdateDisplacementGameSpace());
            // Генерирует мир по текущей конфигурации
            GameSpaceController.Instance.InitializeGameSpace(GameConfig.Instance.SeedGameSpace);
            // Сменяет состояние интерфейса
            GUIController.Instance.ChangeMenu();
            // Активирует управление масштабированием камеры
            CameraScaler.Instance.ActivateCameraScaler();
        }

        /// <summary>
        /// Останавливает игру
        /// </summary>
        private void StopGame()
        {
            _isStartGame = false;
            /// Сохраняет конфигурацию мира
            GameConfig.Instance.SaveGameConfig(GameSpaceController.Instance.GameSpaceSeed, Player.Instance.PlayerCellPosition);
            /// Отключает масштабируемость камеры
            CameraScaler.Instance.DeactivateCameraScaler();
            /// Отключает игрока
            Player.Instance.DeactivatePlayer();
            //Отписывание обновление игрового пространства от перемещение игрока
            Player.Instance.OnMovePlayer.RemoveListener(() => GameSpaceController.Instance.UpdateDisplacementGameSpace());
            /// Скрывает игровой мир
            GameSpaceController.Instance.HideGameSpace();
            /// Сменяет интерфейс
            GUIController.Instance.ChangeMenu();
        }

        /// <summary>
        /// Обрабатывает событие приостановки приложения 
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            //При запущенной игре выходит в меню
            if(_isStartGame) StopGame();
        }

        /// <summary>
        /// Обрабатывает событие закрытия приложения
        /// </summary>
        private void OnApplicationQuit()
        {
            //При запущенной игре сохраняет конфигурацию мира
            if(_isStartGame) GameConfig.Instance.SaveGameConfig(GameSpaceController.Instance.GameSpaceSeed, Player.Instance.PlayerCellPosition);
        }
        #endregion
    }
}
