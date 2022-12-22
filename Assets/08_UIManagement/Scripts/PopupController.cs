namespace UP08
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    // PoolManager
    using UP01;

    // SmartCoroutine
    using UP04;

    public class PopupController : UIController<Popup>
    {
        public static List<Popup> PopupStack { get; protected set; } = new List<Popup>();


        public static void Show(string name, UIEventParam param = null)
        {
            Popup popup = null;

            if (!layouts.TryGetValue(name, out popup))
            {
                var poolObj = PoolManager.Instance.Get(name);
                if (poolObj == null) return;
                popup = poolObj.GetComponent<Popup>();
                popup.transform.parent = instance.transform;
            }

            if (popup == null) return;

            SmartCoroutine.Create(CoShow(popup));

            IEnumerator CoShow(Popup popup)
            {
                popup.Order = Current == null ? 0 : Current.Order + 1;

                Current = popup;
                PopupStack.Add(popup);

                yield return popup.OnEnter(param);
            }
        }

        public static void Hide()
        {
            SmartCoroutine.Create(CoHide());

            IEnumerator CoHide()
            {
                var hiddenPopup = PopupStack[PopupStack.Count - 1];

                PopupStack.RemoveAt(PopupStack.Count - 1);
                Current = PopupStack.Count == 0 ? null : PopupStack[PopupStack.Count - 1];

                hiddenPopup.gameObject.SetActive(false);
                yield return hiddenPopup.OnExit();

                var poolObj = hiddenPopup.GetComponent<PoolObject>();
                poolObj?.Return();
            }
        }

        public static void Hide(string name)
        {
            if (Focus(name)) Hide();
        }
        public static void Hide(Popup popup)
        {
            if (Focus(popup)) Hide();
        }


        public static bool Focus(string name)
        {
            int idx = FindIndex(name);
            if (idx < 0) return false;

            Focus(idx);
            return true;
        }
        public static bool Focus(Popup popup)
        {
            int idx = FindIndex(popup);
            if (idx < 0) return false;

            Focus(idx);
            return true;
        }
        private static void Focus(int idx)
        {
            (int prev, int current) order = (PopupStack[idx].Order, PopupStack[PopupStack.Count - 1].Order);

            var temp = PopupStack[idx];
            PopupStack[idx] = PopupStack[PopupStack.Count - 1];
            PopupStack[PopupStack.Count - 1] = temp;

            PopupStack[idx].Order = order.prev;
            PopupStack[PopupStack.Count - 1].Order = order.current;

            Current = PopupStack[PopupStack.Count - 1];
        }

        public static int FindIndex(string name)
        {
            return PopupStack.FindIndex(p => p.LayoutName.CompareTo(name) == 0);
        }
        public static int FindIndex(Popup popup)
        {
            return PopupStack.FindIndex(p => ReferenceEquals(p, popup));
        }

        public static bool IsOpened(string name)
        {
            return FindIndex(name) >= 0;
        }
        public static bool IsOpened(Popup popup)
        {
            return FindIndex(popup) >= 0;
        }
    }
}