namespace UP08
{
    using System.Collections;
    using UnityEngine;

    public class AdditiveScene : Scene
    {
        public override IEnumerator OnEnter(UIEventParam param = null)
        {
            Debug.Log($">>>>> {LayoutName} Enter");
            yield return new WaitForSeconds(1F);
            yield break;
        }

        public override IEnumerator OnExit()
        {
            Debug.Log($"<<<<< {LayoutName} Exit");
            yield break;
        }


        public override string ToString()
        {
            return LayoutName;
        }
    }
}