using UnityEngine;

namespace NipaFriends.Modules.UrpPostProcess.Viewers.ArrowViews
{
  public class ArrowViiewTester : MonoBehaviour
  {
      [SerializeField] private Color arrowColor = Color.red;
      [SerializeField] private float arrowWidth = 0.005f;
      [SerializeField] private float arrowHeadSize = 0.02f;

      private void Update()
      {
          Vector2 start = new Vector2(0.5f, 0.5f);
          Vector2 end = start + new Vector2(Mathf.Cos(Time.time), Mathf.Sin(Time.time)) * 0.2f;
          ArrowDrawer.Draw(start, end, arrowColor, arrowWidth, arrowHeadSize);
      }
  }
}
