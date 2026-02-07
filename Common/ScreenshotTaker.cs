using System.Collections;
using System.IO;
using UnityEngine;

namespace NipaFriends
{
    public class ScreenshotTaker : MonoBehaviour
    {
        public KeyCode captureKey = KeyCode.F12;
        public string fileName = "Screenshot";


        private void Update()
        {
            // Sキーを押したらスクリーンショットを撮る
            if(Input.GetKeyDown(this.captureKey))
            {
                this.StartCoroutine(this.CaptureRT());
            }
        }

        private IEnumerator CaptureRT()
        {
            // フレームのレンダリングが完全に終了するまで待機
            yield return new WaitForEndOfFrame();

            // 画面のサイズでテクスチャを作成
            var width = Screen.width;
            var height = Screen.height;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            // 現在の画面（バックバッファ）を読み込む
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            // PNGに変換して保存
            var bytes = tex.EncodeToPNG();
            var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desktopPath,
                $"{this.fileName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
            File.WriteAllBytes(path, bytes);

            Debug.Log($"保存完了: {path}");

            // メモリ解放
            Destroy(tex);
        }
    }
}
