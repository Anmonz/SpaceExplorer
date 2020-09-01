using System;
using UnityEngine.Events;

namespace com.AndryKram.SpaceExplorer
{
    public class ActionEvent
    {
        private event Action OnEventAction;

        public void AddListener(Action action)
        {
            OnEventAction += action;
        }

        public void RemoveListener(Action action)
        {
            OnEventAction -= action;
        }

        public void RemoveAllListener()
        {
            OnEventAction = null;
        }

        public void Invoke()
        {
            OnEventAction?.Invoke();
        }
    }

    public class ActionEvent<T0>
    {
        private event Action<T0> OnEventAction;

        public void AddListener(Action<T0> action)
        {
            OnEventAction += action;
        }

        public void RemoveListener(Action<T0> action)
        {
            OnEventAction -= action;
        }

        public void RemoveAllListener()
        {
            OnEventAction = null;
        }

        public void Invoke(T0 message)
        {
            OnEventAction?.Invoke(message);
        }
    }

    public class ActionEvent<T0,T1>
    {
        private event Action<T0,T1> OnEventAction;

        public void AddListener(Action<T0,T1> action)
        {
            OnEventAction += action;
        }

        public void RemoveListener(Action<T0,T1> action)
        {
            OnEventAction -= action;
        }

        public void RemoveAllListener()
        {
            OnEventAction = null;
        }

        public void Invoke(T0 message1, T1 message2)
        {
            OnEventAction?.Invoke(message1,message2);
        }
    }

    public class ActionEvent<T0, T1, T2>
    {
        private event Action<T0, T1, T2> OnEventAction;

        public void AddListener(Action<T0, T1, T2> action)
        {
            OnEventAction += action;
        }

        public void RemoveListener(Action<T0, T1, T2> action)
        {
            OnEventAction -= action;
        }

        public void RemoveAllListener()
        {
            OnEventAction = null;
        }

        public void Invoke(T0 message1, T1 message2, T2 message3)
        {
            OnEventAction?.Invoke(message1, message2, message3);
        }
    }

    public class ActionEvent<T0, T1, T2, T3>
    {
        private event Action<T0, T1, T2, T3> OnEventAction;

        public void AddListener(Action<T0, T1, T2, T3> action)
        {
            OnEventAction += action;
        }

        public void RemoveListener(Action<T0, T1, T2, T3> action)
        {
            OnEventAction -= action;
        }

        public void RemoveAllListener()
        {
            OnEventAction = null;
        }

        public void Invoke(T0 message1, T1 message2, T2 message3, T3 message4)
        {
            OnEventAction?.Invoke(message1, message2, message3, message4);
        }
    }
}
