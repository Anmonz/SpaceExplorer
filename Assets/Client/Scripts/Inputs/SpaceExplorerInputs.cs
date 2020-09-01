// GENERATED AUTOMATICALLY FROM 'Assets/Client/Scripts/Inputs/SpaceExplorerInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace com.AndryKram.SpaceExplorer
{
    public class @SpaceExplorerInputs : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @SpaceExplorerInputs()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""SpaceExplorerInputs"",
    ""maps"": [
        {
            ""name"": ""CameraScaler"",
            ""id"": ""f05406fb-1ac4-4e54-939a-32e3c4758e64"",
            ""actions"": [
                {
                    ""name"": ""TouchOne"",
                    ""type"": ""Button"",
                    ""id"": ""82a722f3-e985-48b1-b97d-9d69b93d3100"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TouchTwo"",
                    ""type"": ""Button"",
                    ""id"": ""9f265cf9-c5ff-4d0b-baa7-009a0f37d58d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TouchOnePosition"",
                    ""type"": ""Value"",
                    ""id"": ""1c5de4f5-8b25-42e8-a68c-09e24864fc80"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TouchTwoPosition"",
                    ""type"": ""Value"",
                    ""id"": ""5affaae8-6e4f-4c02-b7e3-b62b38798aad"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""31ee849f-4731-4ccc-ad81-41322474d5c6"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c0360059-36cf-48ad-9019-ed7b025c2227"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchTwo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b3c868f-be12-4884-8538-400f4e084734"",
                    ""path"": ""<Touchscreen>/touch0/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchOnePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77536ba8-1037-4187-9e6e-843f2735c962"",
                    ""path"": ""<Touchscreen>/touch1/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchTwoPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // CameraScaler
            m_CameraScaler = asset.FindActionMap("CameraScaler", throwIfNotFound: true);
            m_CameraScaler_TouchOne = m_CameraScaler.FindAction("TouchOne", throwIfNotFound: true);
            m_CameraScaler_TouchTwo = m_CameraScaler.FindAction("TouchTwo", throwIfNotFound: true);
            m_CameraScaler_TouchOnePosition = m_CameraScaler.FindAction("TouchOnePosition", throwIfNotFound: true);
            m_CameraScaler_TouchTwoPosition = m_CameraScaler.FindAction("TouchTwoPosition", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // CameraScaler
        private readonly InputActionMap m_CameraScaler;
        private ICameraScalerActions m_CameraScalerActionsCallbackInterface;
        private readonly InputAction m_CameraScaler_TouchOne;
        private readonly InputAction m_CameraScaler_TouchTwo;
        private readonly InputAction m_CameraScaler_TouchOnePosition;
        private readonly InputAction m_CameraScaler_TouchTwoPosition;
        public struct CameraScalerActions
        {
            private @SpaceExplorerInputs m_Wrapper;
            public CameraScalerActions(@SpaceExplorerInputs wrapper) { m_Wrapper = wrapper; }
            public InputAction @TouchOne => m_Wrapper.m_CameraScaler_TouchOne;
            public InputAction @TouchTwo => m_Wrapper.m_CameraScaler_TouchTwo;
            public InputAction @TouchOnePosition => m_Wrapper.m_CameraScaler_TouchOnePosition;
            public InputAction @TouchTwoPosition => m_Wrapper.m_CameraScaler_TouchTwoPosition;
            public InputActionMap Get() { return m_Wrapper.m_CameraScaler; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CameraScalerActions set) { return set.Get(); }
            public void SetCallbacks(ICameraScalerActions instance)
            {
                if (m_Wrapper.m_CameraScalerActionsCallbackInterface != null)
                {
                    @TouchOne.started -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchOne;
                    @TouchOne.performed -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchOne;
                    @TouchOne.canceled -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchOne;
                    @TouchTwo.started -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchTwo;
                    @TouchTwo.performed -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchTwo;
                    @TouchTwo.canceled -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchTwo;
                    @TouchOnePosition.started -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchOnePosition;
                    @TouchOnePosition.performed -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchOnePosition;
                    @TouchOnePosition.canceled -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchOnePosition;
                    @TouchTwoPosition.started -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchTwoPosition;
                    @TouchTwoPosition.performed -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchTwoPosition;
                    @TouchTwoPosition.canceled -= m_Wrapper.m_CameraScalerActionsCallbackInterface.OnTouchTwoPosition;
                }
                m_Wrapper.m_CameraScalerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @TouchOne.started += instance.OnTouchOne;
                    @TouchOne.performed += instance.OnTouchOne;
                    @TouchOne.canceled += instance.OnTouchOne;
                    @TouchTwo.started += instance.OnTouchTwo;
                    @TouchTwo.performed += instance.OnTouchTwo;
                    @TouchTwo.canceled += instance.OnTouchTwo;
                    @TouchOnePosition.started += instance.OnTouchOnePosition;
                    @TouchOnePosition.performed += instance.OnTouchOnePosition;
                    @TouchOnePosition.canceled += instance.OnTouchOnePosition;
                    @TouchTwoPosition.started += instance.OnTouchTwoPosition;
                    @TouchTwoPosition.performed += instance.OnTouchTwoPosition;
                    @TouchTwoPosition.canceled += instance.OnTouchTwoPosition;
                }
            }
        }
        public CameraScalerActions @CameraScaler => new CameraScalerActions(this);
        public interface ICameraScalerActions
        {
            void OnTouchOne(InputAction.CallbackContext context);
            void OnTouchTwo(InputAction.CallbackContext context);
            void OnTouchOnePosition(InputAction.CallbackContext context);
            void OnTouchTwoPosition(InputAction.CallbackContext context);
        }
    }
}
