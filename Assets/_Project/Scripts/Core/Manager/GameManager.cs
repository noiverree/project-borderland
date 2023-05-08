using System.Collections.Generic;
using UnityEngine;

namespace ProjectBorderland.Core.Manager
{
    /// <summary>
    /// Handles overall game states.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        //==============================================================================
        // Variables
        //==============================================================================
        #region singletonDDOL
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();

                    if (instance == null)
                    {
                        GameObject newGameObject = new GameObject();
                        newGameObject.name = typeof(GameManager).Name;
                        instance = newGameObject.AddComponent<GameManager>();
                    }
                }

                return instance;
            }
        }
        #endregion

        private Camera mainCamera;
        public Camera MainCamera { get { return mainCamera; } }

        [Header("Object References")]
        [SerializeField] private GameObject freeRoamPlayer;
        [SerializeField] private Camera freeRoamCamera;
        [SerializeField] private GameObject pointAndClickPlayer;
        [SerializeField] private Camera pointAndClickCamera;
        [SerializeField] private GameObject crosshair;

        private FreeRoam.PlayerMovement freeRoamPlayerMovement;
        private FreeRoam.PlayerCamera freeRoamPlayerCamera;
        private FreeRoam.Interaction freeRoamInteraction;
        private FreeRoam.PlayerItemHolder freeRoamPlayerItemHolder;

        private PointAndClick.Interaction pointAndClickInteraction;
        private PointAndClick.PlayerCameraPan pointAndClickPlayerCameraPan;

        private Inspection.Inspection freeRoamInspection;

        private GameState currentGameState;
        private List<ActionState> currentActionStates = new List<ActionState>();

        
        
        //==============================================================================
        // Functions
        //==============================================================================
        #region MonoBehaviour methods
        private void Awake()
        {
            #region singletonDDOL
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion

            freeRoamPlayerMovement = freeRoamPlayer.GetComponentInChildren<FreeRoam.PlayerMovement>();
            freeRoamPlayerCamera = freeRoamPlayer.GetComponentInChildren<FreeRoam.PlayerCamera>();
            freeRoamInteraction = freeRoamPlayer.GetComponentInChildren<FreeRoam.Interaction>();
            freeRoamPlayerItemHolder = freeRoamPlayer.GetComponentInChildren<FreeRoam.PlayerItemHolder>();
            freeRoamInspection = freeRoamPlayer.GetComponentInChildren<Inspection.Inspection>();

            pointAndClickInteraction = pointAndClickPlayer.GetComponentInChildren<PointAndClick.Interaction>();
            pointAndClickPlayerCameraPan = pointAndClickPlayer.GetComponentInChildren<PointAndClick.PlayerCameraPan>();
        }



        private void Start()
        {
            SwitchGamestate(GameState.FreeRoam);
        }
        #endregion



        #region ProjectBorderland methods
        /// <summary>
        /// Switches between game states.
        /// </summary>
        /// <param name="gameState"></param>
        public static void SwitchGamestate(GameState gameState)
        {
            if (gameState == GameState.FreeRoam)
            {
                instance.currentGameState = GameState.FreeRoam;
                Cursor.lockState = CursorLockMode.Locked;
                instance.mainCamera = instance.freeRoamCamera;

                EnableFreeRoam();
                DisablePointAndClick();
            }

            if (gameState == GameState.PointAndClick)
            {
                instance.currentGameState = GameState.PointAndClick;
                Cursor.lockState = CursorLockMode.None;
                instance.mainCamera = instance.pointAndClickCamera;

                EnablePointAndClick();
                DisableFreeRoam();
            }
        }



        /// <summary>
        /// Checks if current action states contains given action state.
        /// </summary>
        /// <param name="actionState"></param>
        private bool CheckActionState(ActionState actionState)
        {
            return currentActionStates.Contains(actionState);
        }



        /// <summary>
        /// Adds an action state to action states list.
        /// </summary>
        private void AddActionState(ActionState actionState)
        {
            currentActionStates.Add(actionState);
        }



        /// <summary>
        /// Removes an action state from action states list.
        /// </summary>
        private void RemoveActionState(ActionState actionState)
        {
            currentActionStates.Remove(actionState);
        }



        /// <summary>
        /// Enables free roam properties.
        /// </summary>
        private static void EnableFreeRoam()
        {
            instance.freeRoamCamera.gameObject.SetActive(true);
            instance.freeRoamPlayerMovement.enabled = true;
            instance.freeRoamPlayerCamera.enabled = true;
            instance.freeRoamInteraction.enabled = true;
            instance.freeRoamPlayerItemHolder.enabled = true;
        }



        /// <summary>
        /// Disables free roam properties.
        /// </summary>
        private static void DisableFreeRoam()
        {
            instance.freeRoamCamera.gameObject.SetActive(false);
            instance.freeRoamPlayerMovement.enabled = false;
            instance.freeRoamPlayerCamera.enabled = false;
            instance.freeRoamInteraction.enabled = false;
            instance.freeRoamPlayerItemHolder.enabled = false;
        }



        /// <summary>
        /// Disables point and click properties.
        /// </summary>
        private static void EnablePointAndClick()
        {
            instance.pointAndClickCamera.gameObject.SetActive(true);
            instance.pointAndClickPlayerCameraPan.enabled = true;
            instance.pointAndClickInteraction.enabled = true;
        }



        /// <summary>
        /// Disables point and click properties.
        /// </summary>
        private static void DisablePointAndClick()
        {
            instance.pointAndClickCamera.gameObject.SetActive(false);
            instance.pointAndClickPlayerCameraPan.enabled = false;
            instance.pointAndClickInteraction.enabled = false;
        }



        /// <summary>
        /// Sets up point and click camera position.
        /// </summary>
        /// <param name="cameraPosition"></param>
        public static void SetUpPointAndClickCamera(Transform cameraPosition)
        {
            instance.pointAndClickPlayerCameraPan.AnchoredPosition = cameraPosition.position;
            instance.pointAndClickPlayerCameraPan.transform.position = cameraPosition.position;
            instance.pointAndClickPlayerCameraPan.transform.rotation = cameraPosition.rotation;
        }



        /// <summary>
        /// Enters dialogue mode.
        /// </summary>
        public static void EnterDialogue()
        {
            instance.AddActionState(ActionState.Dialogue);

            if (instance.currentGameState == GameState.FreeRoam)
            {
                if (!instance.CheckActionState(ActionState.Inspection))
                {
                    Cursor.lockState = CursorLockMode.None;
                    instance.crosshair.SetActive(false);

                    instance.freeRoamPlayerMovement.Stop();
                    DisableFreeRoam();
                    instance.freeRoamCamera.gameObject.SetActive(true);
                }

                else
                {
                    instance.pointAndClickInteraction.enabled = false;
                    instance.freeRoamInspection.enabled = false;
                }
            }

            if (instance.currentGameState == GameState.PointAndClick)
            {
                if (!instance.CheckActionState(ActionState.Inspection))
                {
                    instance.pointAndClickInteraction.enabled = false;
                }

                else
                {

                }
            }
        }



        /// <summary>
        /// Exits dialogue mode.
        /// </summary>
        public static void ExitDialogue()
        {
            instance.RemoveActionState(ActionState.Dialogue);

            if (instance.currentGameState == GameState.FreeRoam)
            {
                if (!instance.CheckActionState(ActionState.Inspection))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    instance.crosshair.SetActive(true);

                    EnableFreeRoam();
                }

                else
                {
                    instance.pointAndClickInteraction.enabled = true;
                    instance.freeRoamInspection.enabled = true;
                }
            }

            if (instance.currentGameState == GameState.PointAndClick)
            {
                if (!instance.CheckActionState(ActionState.Inspection))
                {
                    instance.pointAndClickInteraction.enabled = true;
                }

                else
                {

                }
            }
        }



        /// <summary>
        /// Enters inspection mode.
        /// </summary>
        public static void EnterInspection()
        {
            instance.AddActionState(ActionState.Inspection);

            if (instance.currentGameState == GameState.FreeRoam)
            {
                Cursor.lockState = CursorLockMode.None;
                instance.crosshair.SetActive(false);

                instance.pointAndClickInteraction.enabled = true;
                instance.pointAndClickInteraction.PointAndClickCamera = instance.freeRoamCamera;

                instance.freeRoamPlayerMovement.Stop();
                
                DisableFreeRoam();
                instance.freeRoamCamera.gameObject.SetActive(true);
            }
        }



        /// <summary>
        /// Exits inspection mode.
        /// </summary>
        public static void ExitInspection()
        {
            instance.RemoveActionState(ActionState.Inspection);

            if (instance.currentGameState == GameState.FreeRoam)
            {
                Cursor.lockState = CursorLockMode.Locked;
                instance.crosshair.SetActive(true);

                instance.pointAndClickInteraction.enabled = false;

                EnableFreeRoam();
            }
        }
        #endregion
    }
}