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
        [SerializeField] private Vector2Int _offsetEndScreenPlayerMove; //количество ячеек до края экрана до которых может добраться игрок

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
            //Перемещение камеры к игроку
            CameraFolowTarget.Instance.GoToTarget();
            // Генерирует мир по текущей конфигурации
            GameSpaceController.Instance.InitializeGameSpace(GameConfig.Instance.SeedGameSpace);
            // Сменяет состояние интерфейса
            GUIController.Instance.ChangeMenu();
            // Активирует управление масштабированием камеры
            CameraScaler.Instance.ActivateCameraScaler();

            //Подписывание обновление игрового пространства на перемещение игрока
            Player.Instance.OnMovePlayer.AddListener(GameSpaceController.Instance.UpdateGameSpace);
            Player.Instance.OnMovePlayer.AddListener(CheckDistanceFromPlayerToEndScreen);
            CameraScaler.Instance.OnScaleCameraSize.AddListener(GameSpaceController.Instance.UpdateGameSpace);
            CameraFolowTarget.Instance.OnMoveToTarget.AddListener(GameSpaceController.Instance.UpdateGameSpace);
        }

        /// <summary>
        /// Останавливает игру
        /// </summary>
        private void StopGame()
        {
            _isStartGame = false;
            /// Сохраняет конфигурацию мира
            GameConfig.Instance.SaveGameConfig(GameSpaceController.Instance.GameSpaceSeed, Player.Instance.PlayerCellPosition);

            //Отписывание обновление игрового пространства от перемещение игрока
            Player.Instance.OnMovePlayer.RemoveListener(GameSpaceController.Instance.UpdateGameSpace);
            Player.Instance.OnMovePlayer.RemoveListener(CheckDistanceFromPlayerToEndScreen);
            CameraScaler.Instance.OnScaleCameraSize.RemoveListener(GameSpaceController.Instance.UpdateGameSpace);
            CameraFolowTarget.Instance.OnMoveToTarget.RemoveListener(GameSpaceController.Instance.UpdateGameSpace);
            /// Отключает масштабируемость камеры
            CameraScaler.Instance.DeactivateCameraScaler();
            /// Отключает игрока
            Player.Instance.DeactivatePlayer();
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


        private void CheckDistanceFromPlayerToEndScreen()
        {
            var playerPosition = Player.Instance.PlayerCellPosition;
            var leftDownScreen = GameSpaceController.Instance.LeftDownScreenPosition;
            var rightUpScreen = GameSpaceController.Instance.RightUpScreenPosition;
            if(playerPosition.x >= rightUpScreen.x - _offsetEndScreenPlayerMove.x ||
                playerPosition.x <= leftDownScreen.x + _offsetEndScreenPlayerMove.x||
                playerPosition.y >= rightUpScreen.y - _offsetEndScreenPlayerMove.y||
                playerPosition.y <= leftDownScreen.y + _offsetEndScreenPlayerMove.y)
            {
                CameraFolowTarget.Instance.GoToTarget();
            }
        }
        #endregion
    }
}
