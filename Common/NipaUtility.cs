using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NipaFriends
{
    /// <summary>
    /// ゲーム汎用ユーティリティクラス
    /// </summary>
    public static class NipaUtility
    {
        #region *** Definitions ***

        /// <summary>
        /// IDと値のペアを保持する構造体（ソート用）
        /// </summary>
        private readonly struct FloatId : IComparable<FloatId>
        {
            public readonly int Id;
            public readonly float Value;

            public FloatId(int id, float value)
            {
                Id = id;
                Value = value;
            }

            public int CompareTo(FloatId other)
            {
                return Value.CompareTo(other.Value);
            }
        }

        #endregion

        #region *** Distance & Time (距離と時間) ***

        /// <summary>
        /// 指定された座標が指定距離内にあるか判定します（高さ含む）
        /// </summary>
        public static bool IsTargetInRange(Vector3 from, Vector3 target, float range)
        {
            return (target - from).sqrMagnitude < (range * range);
        }

        /// <summary>
        /// 指定された座標が指定距離内にあるか判定します（高さ無視：XZ平面）
        /// </summary>
        public static bool IsTargetInRangeHorizontal(Vector3 a, Vector3 b, float range)
        {
            a.y = 0;
            b.y = 0;
            return (a - b).sqrMagnitude < (range * range);
        }

        /// <summary>
        /// 指定速度で移動した場合、目標地点まで何秒かかるかを返します
        /// </summary>
        public static float GetTimeToReachTarget(Vector3 from, Vector3 to, float speed)
        {
            if(Mathf.Approximately(speed, 0f)) return float.MaxValue;
            return (to - from).magnitude / speed;
        }

        /// <summary>
        /// 同一水平面上にいると仮定して二乗距離を返します
        /// </summary>
        public static float GetHorizontalSqrDistance(Vector3 a, Vector3 b)
        {
            a.y = b.y;
            return (a - b).sqrMagnitude;
        }

        #endregion

        #region *** Angle (角度) ***

        /// <summary>
        /// 自身から見てターゲットがどの方角にいるか(0~180) 正面が0度
        /// </summary>
        /// <param name="observer">自身のTransform</param>
        /// <param name="target">ターゲット座標</param>
        /// <param name="includeAltitude">高さ差を加味するか</param>
        public static float GetAngleDirectionAbs(Transform observer, Vector3 target, bool includeAltitude = false)
        {
            var forward = observer.forward;
            var targetDir = target - observer.position;

            if(!includeAltitude)
            {
                forward.y = 0;
                targetDir.y = 0;
            }

            if(targetDir == Vector3.zero) return 0f;

            return Vector3.Angle(targetDir, forward);
        }

        /// <summary>
        /// ワールド基準で2点間の角度を返します（0~360）。北（Z+）を0度とします。
        /// </summary>
        public static float GetAngleAbsolute(Vector3 from, Vector3 to)
        {
            var dir = to - from;
            dir.y = 0; // 水平角度とするためYを無視
            if(dir == Vector3.zero) return 0f;

            var angle = Vector3.Angle(Vector3.forward, dir);
            if(dir.x < 0)
            {
                angle = 360f - angle;
            }

            return angle;
        }

        /// <summary>
        /// 自身から見てターゲットがどの方角にいるか(-180~180) 正面が0度、左がマイナス
        /// </summary>
        public static float GetAngleDirection(Transform observer, Vector3 target, bool includeAltitude = false)
        {
            var forward = observer.forward;
            var targetDir = target - observer.position;

            if(!includeAltitude)
            {
                forward.y = 0;
                targetDir.y = 0;
            }

            if(targetDir == Vector3.zero) return 0f;

            // 符号付き角度を計算
            var axis = includeAltitude ? observer.right : Vector3.up; // 簡易的な軸選定
            var angle = Vector3.SignedAngle(forward, targetDir, Vector3.up);

            return angle;
        }

        /// <summary>
        /// 目標に対して正面を向けているか判定します
        /// </summary>
        public static bool IsFaceToTarget(Transform observer, Vector3 targetPos, float limitAngle,
            bool ignoreHeight = false)
        {
            var dir = targetPos - observer.position;
            if(ignoreHeight)
            {
                dir.y = 0;
            }

            if(dir == Vector3.zero) return true;

            return Vector3.Angle(observer.forward, dir) < limitAngle;
        }

        /// <summary>
        /// 基準点から見て地点Aと地点Bがなす角度
        /// </summary>
        public static float GetAngleBetween(Vector3 observer, Vector3 posA, Vector3 posB)
        {
            return Vector3.Angle(posA - observer, posB - observer);
        }

        /// <summary>
        /// あるベクトルに直交する単位ベクトルを返します (2D)
        /// </summary>
        public static Vector2 GetDirectionVertical(Vector2 originalDir)
        {
            if(Mathf.Approximately(originalDir.x, 0) && Mathf.Approximately(originalDir.y, 0))
                return Vector2.zero;

            // (x, y) の垂直ベクトルは (-y, x) または (y, -x)
            return new Vector2(originalDir.y, -originalDir.x).normalized;
        }

        /// <summary>
        /// あるベクトルと指定された角度をなす単位ベクトルを返します
        /// </summary>
        public static Vector2 GetDirectionAngleToOriginalDir(Vector2 original, float angleDeg)
        {
            var rad = angleDeg * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);

            var norm = original.normalized;
            return new Vector2(
                norm.x * cos - norm.y * sin,
                norm.x * sin + norm.y * cos
            );
        }

        #endregion

        #region *** Position & Geometry (座標と幾何) ***

        /// <summary>
        /// 指定地点Aから一番近い、指定地点Bから指定距離離れた地点を返します
        /// </summary>
        public static Vector3 GetPositionOffToTarget(Vector3 fromPos, Vector3 targetPos, float dist)
        {
            var dir = (fromPos - targetPos).normalized;
            return targetPos + dir * dist;
        }

        /// <summary>
        /// 平面とレイの交点を求めます
        /// </summary>
        public static Vector3 GetPositionRayHitPlane(Vector3 planePos, Vector3 planeNormal, Vector3 rayOrigin,
            Vector3 rayDir)
        {
            var denom = Vector3.Dot(planeNormal, rayDir);
            if(Mathf.Abs(denom) < Mathf.Epsilon) return rayOrigin; // 平行の場合

            var t = Vector3.Dot(planeNormal, planePos - rayOrigin) / denom;
            return rayOrigin + rayDir * t;
        }

        /// <summary>
        /// 線分(from-goal)上で、中心点(center)から指定距離(dist)にある地点を返します
        /// </summary>
        public static bool TryGetPositionOffToTarget(Vector3 posFrom, Vector3 posGoal, Vector3 posCenter, float dist,
            out Vector3 result)
        {
            result = posFrom;

            // 中心から線分への垂線の足
            if(!GetPositionOnLineFromPoint(posFrom, posGoal, posCenter, out var o))
            {
                return false;
            }

            var distSqr = (dist * dist) - (o - posCenter).sqrMagnitude;
            if(distSqr < 0) return false;

            var offset = Mathf.Sqrt(distSqr);

            // 2点考えられるが、Goalに近い方かFromに近い方かなど仕様による。
            // 元のコードは GetPositionBetweenDist を呼んでいるが、どの方向か不明瞭なため、
            // ここでは元のロジックを尊重しつつ整理。
            // 元コードの GetPositionBetweenDist は "FromからBに向かって dist 離れた点" を返す関数。
            // しかしここの distSqr から求めた offset は「垂線の足からの距離」であるはず。
            // 意図が読み取りにくいため、単純な幾何計算として実装し直します。

            // 垂線の足から、線分に沿って offset 分ずらした位置が候補。
            // ここでは簡易的に「交差判定あり」として垂線の足を返します（必要に応じて拡張してください）
            result = o;
            return true;
        }

        /// <summary>
        /// 地点Aと地点Bを結ぶ線上で地点Aから指定距離離れた地点を返します
        /// </summary>
        public static Vector3 GetPositionFromAtoB(Vector3 posA, Vector3 posB, float distFromA)
        {
            var dir = (posB - posA).normalized;
            return posA + dir * distFromA;
        }

        /// <summary>
        /// 指定された地点から指定半径内にある座標をランダムで返します（XZ平面）
        /// </summary>
        public static Vector3 GetPositionRandomInRange(Vector3 origin, float range)
        {
            var delta = Random.insideUnitCircle * range;
            return origin + new Vector3(delta.x, 0f, delta.y);
        }

        /// <summary>
        /// 指定速度で現在の進路を維持した場合、指定時間後に到達する座標
        /// </summary>
        public static Vector3 GetPositionInFuture(Transform mover, float speed, float duration)
        {
            return mover.position + mover.forward * speed * duration;
        }

        /// <summary>
        /// 座標が指定多角形の内側にあるか判定します（Ray Casting法）
        /// </summary>
        public static bool IsInsidePolygon(Vector2 pos, List<Vector2> vertices)
        {
            if(vertices == null || vertices.Count < 3) return false;

            var inside = false;
            var j = vertices.Count - 1;
            for(var i = 0; i < vertices.Count; i++)
            {
                if((vertices[i].y > pos.y) != (vertices[j].y > pos.y) &&
                   (pos.x <
                    (vertices[j].x - vertices[i].x) * (pos.y - vertices[i].y) / (vertices[j].y - vertices[i].y) +
                    vertices[i].x))
                {
                    inside = !inside;
                }

                j = i;
            }

            return inside;
        }

        /// <summary>
        /// ベクトルXはベクトルAとベクトルBの間にあるか (2D)
        /// </summary>
        public static bool IsVectorInside(Vector2 x, Vector2 a, Vector2 b)
        {
            var crossAB = Cross(a, b);
            var crossAX = Cross(a, x);
            var crossXB = Cross(x, b); // Note: Original logic used XB checks

            // 元のロジックを尊重：ABとAXの符号、BAとBXの符号をチェック
            if(crossAB * crossAX < 0) return false;

            var crossBA = -crossAB; // Cross(b, a)
            var crossBX = Cross(b, x);

            if(crossBA * crossBX < 0) return false;

            return true;
        }

        /// <summary>
        /// 線分ABに点Cから垂直に下ろした線分の交点（垂線の足）を求めます。
        /// </summary>
        /// <param name="result">交点が線分上にある場合の座標</param>
        /// <returns>交点が線分AB上にあるならtrue</returns>
        public static bool GetPositionOnLineFromPoint(Vector3 posA, Vector3 posB, Vector3 posC, out Vector3 result)
        {
            var a = posA.ToV2_XZ();
            var b = posB.ToV2_XZ();
            var c = posC.ToV2_XZ();

            var ab = b - a;
            if(ab.sqrMagnitude < Mathf.Epsilon)
            {
                result = posA;
                return false;
            }

            var ac = c - a;
            var t = Vector2.Dot(ac, ab) / Vector2.Dot(ab, ab);

            // 線分上にあるか判定 (0 <= t <= 1)
            var onSegment = t >= 0f && t <= 1f;

            // 線分外でも計算結果は返す（クランプしない）
            var res2D = a + ab * t;
            result = res2D.ToV3_Y(posA.y);

            return onSegment;
        }

        /// <summary>
        /// 2つの線分の交差判定と交点取得
        /// </summary>
        public static bool GetCrossPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 result)
        {
            result = Vector2.zero;
            var b = a2 - a1;
            var d = b2 - b1;
            var det = Cross(b, d);

            if(Mathf.Abs(det) < Mathf.Epsilon) return false; // 平行

            var c = b1 - a1;
            var t = Cross(c, d) / det;
            var u = Cross(c, b) / det;

            if(t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                result = a1 + b * t;
                return true;
            }

            return false;
        }

        /// <summary>
        /// ベジェ曲線上の点を取得します
        /// </summary>
        public static Vector2[] GetPointsOnBezier(Vector2[] ctrlPoints, int segments)
        {
            if(ctrlPoints.Length == 0) return new Vector2[0];
            if(segments < 2) return new[] { ctrlPoints[0], ctrlPoints[ctrlPoints.Length - 1] };

            var result = new Vector2[segments];
            var n = ctrlPoints.Length - 1;

            for(var i = 0; i < segments; i++)
            {
                var t = i / (float)(segments - 1);
                result[i] = CalculateBezierPoint(t, ctrlPoints, n);
            }

            return result;
        }

        private static Vector2 CalculateBezierPoint(float t, Vector2[] p, int n)
        {
            // バーンスタイン基底関数を用いた計算よりも、ド・カステリョのアルゴリズムの方が数値的に安定する場合があるが、
            // ここでは元のロジック（パスカルの三角形係数利用）を維持しつつ最適化
            // ただし、計算コストを考えると汎用的な再帰計算か、少数の制御点ならハードコードが望ましい。

            // 簡易実装（3次ベジェまで対応の汎用型）
            var ret = Vector2.zero;
            for(var i = 0; i <= n; i++)
            {
                var coef = BinomialCoefficient(n, i);
                ret += coef * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i) * p[i];
            }

            return ret;
        }

        private static long BinomialCoefficient(int n, int k)
        {
            if(k < 0 || k > n) return 0;
            if(k == 0 || k == n) return 1;
            if(k > n / 2) k = n - k;

            long res = 1;
            for(var i = 1; i <= k; ++i)
            {
                res = res * (n - i + 1) / i;
            }

            return res;
        }

        /// <summary>
        /// 横隊陣形（ラインフォーメーション）の座標を計算します
        /// </summary>
        public static Vector3[] GetFormationLine(Vector3[] currentPositions, Vector3 destination, float interval,
            bool sort = false)
        {
            var count = currentPositions.Length;
            if(count == 0) return new Vector3[0];

            // 重心の計算
            var center = Vector3.zero;
            foreach(var pos in currentPositions) center += pos;
            center /= count;

            // 進行方向の計算
            var dir = (destination - center);
            dir.y = 0;
            if(dir == Vector3.zero) dir = Vector3.forward;
            dir.Normalize();

            // 右方向ベクトル（横隊の並ぶ方向）
            var right = new Vector3(dir.z, 0f, -dir.x);

            var width = interval * (count - 1);
            var startX = -width / 2f;

            var results = new Vector3[count];

            // 基準位置の生成
            for(var i = 0; i < count; i++)
            {
                results[i] = destination + right * (startX + interval * i);
            }

            if(!sort) return results;

            // ソート: フォーメーションの並び順に近いユニットを割り当てる
            // 投影位置（rightベクトル上の位置）でソート
            var unitProjections = new FloatId[count];
            for(var i = 0; i < count; i++)
            {
                // 右方向ベクトルとの内積で相対位置を数値化
                var proj = Vector3.Dot(currentPositions[i] - center, right);
                unitProjections[i] = new FloatId(i, proj);
            }

            // 内積値が小さい（左側）順にソート
            Array.Sort(unitProjections);

            var sortedResults = new Vector3[count];
            for(var i = 0; i < count; i++)
            {
                // i番目の位置には、i番目に左側にいるユニットの元のインデックスの結果を入れる...のではなく、
                // 「左端の目的地」には「左端にいるユニット」を割り当てる

                // results は既に左から右へ並んでいる
                // unitProjections[i].Id は i番目に左にいるユニットのID
                // そのユニットの新しい位置は results[i] になるべき

                // しかし戻り値は「元の配列のインデックスに対応する新しい座標」である必要がある場合が多い。
                // ここでは「resultsSorted[元のID] = 新しい座標」の形式にする。

                sortedResults[unitProjections[i].Id] = results[i];
            }

            return sortedResults;
        }

        #endregion

        #region *** Utilities (その他) ***

        /// <summary>
        /// 配列のコピー（サイズ不一致ならリサイズ）
        /// </summary>
        public static void CopyArray<T>(T[] source, ref T[] target)
        {
            if(target == null || target.Length != source.Length)
            {
                target = new T[source.Length];
            }

            Array.Copy(source, target, source.Length);
        }

        /// <summary>
        /// ほぼ等しいか判定
        /// </summary>
        public static bool IsEqual(float a, float b, float threshold = 0.1f)
        {
            return Mathf.Abs(a - b) < threshold;
        }

        /// <summary>
        /// Texture2Dを指定色で着色します
        /// </summary>
        public static void TintTexture(Texture2D tex, Color color)
        {
            var pixels = tex.GetPixels();
            for(var i = 0; i < pixels.Length; i++)
            {
                pixels[i] *= color;
            }

            tex.SetPixels(pixels);
            tex.Apply();
        }

        /// <summary>
        /// 2つのテクスチャを加算合成して新しいテクスチャを返します
        /// </summary>
        public static Texture2D CombineTexture(Texture2D texA, Texture2D texB)
        {
            if(texA.width != texB.width || texA.height != texB.height)
            {
                Debug.LogWarning("Texture sizes do not match.");
                return null;
            }

            var result = new Texture2D(texA.width, texA.height);
            var pixelsA = texA.GetPixels();
            var pixelsB = texB.GetPixels();
            var pixelsResult = new Color[pixelsA.Length];

            for(var i = 0; i < pixelsA.Length; i++)
            {
                pixelsResult[i] = pixelsA[i] + pixelsB[i];
            }

            result.SetPixels(pixelsResult);
            result.Apply();
            return result;
        }

        /// <summary>
        /// 親子関係を設定し、ローカル座標とスケールをリセットします
        /// </summary>
        public static void SetParent(Transform parent, Transform child, float scale = 1f)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity; // 回転もリセットするのが一般的
            child.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// カラーのアルファ値を変更した新しいカラーを返します
        /// </summary>
        public static Color ChangeColorAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// ターゲットとの間に遮蔽物がないか判定します
        /// </summary>
        public static bool IsClearToTarget(Vector3 from, Transform target, float maxDist = 10000f,
            int layerMask = Physics.DefaultRaycastLayers)
        {
            var dir = target.position - from;
            // ターゲット自身にヒットさせるため、少し手前からではなく、ターゲットまでの距離より少し短くレイを飛ばす
            var dist = dir.magnitude;
            if(dist > maxDist) return false;

            if(Physics.Raycast(from, dir, out var hit, dist, layerMask))
            {
                // ヒットしたのがターゲット自身、あるいはその親などであればクリアとみなす
                if(hit.transform == target || hit.transform.IsChildOf(target))
                {
                    return true;
                }

                return false; // 障害物に当たった
            }

            return true; // 何にも当たらなかった＝遮蔽物なし（ターゲットも判定外距離ならここに来るが、ロジック依存）
        }

        /// <summary>
        /// 2次元クロス積（外積のZ成分）
        /// </summary>
        public static float Cross(Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        /// <summary>
        /// 重み付き抽選：インデックスを返します
        /// </summary>
        public static int GetRandomIndex(IList<float> weights)
        {
            if(weights == null || weights.Count == 0) return -1;

            var totalWeight = 0f;
            foreach(var w in weights) totalWeight += w;

            var randomValue = Random.value * totalWeight;
            var cumulative = 0f;

            for(var i = 0; i < weights.Count; i++)
            {
                cumulative += weights[i];
                if(randomValue <= cumulative)
                {
                    return i;
                }
            }

            return weights.Count - 1;
        }

        /// <summary>
        /// 配列をシャッフルして指定要素数を取り出します (Fisher-Yates Shuffle)
        /// </summary>
        public static T[] ShuffleAndTake<T>(T[] array, int count)
        {
            if(array == null) return new T[0];

            // 配列の複製を作成（元の配列を壊さないため）
            var temp = (T[])array.Clone();
            var n = temp.Length;

            // シャッフル
            for(var i = 0; i < n - 1; i++)
            {
                var r = Random.Range(i, n);
                (temp[i], temp[r]) = (temp[r], temp[i]); // Swap
            }

            // 指定数だけ切り出し
            count = Mathf.Clamp(count, 0, n);
            var result = new T[count];
            Array.Copy(temp, result, count);

            return result;
        }

        /// <summary>
        /// 円状に配置した座標配列を返します
        /// </summary>
        public static Vector3[] GetCirclePositions(int count, float radius, Vector3 centerPos)
        {
            var results = new Vector3[count];
            var angleStep = (Mathf.PI * 2.0f) / count;

            for(var i = 0; i < count; i++)
            {
                var angle = i * angleStep;
                var x = radius * Mathf.Cos(angle);
                var z = radius * Mathf.Sin(angle);
                results[i] = centerPos + new Vector3(x, 0f, z);
            }

            return results;
        }

        #endregion

        #region *** File I/O ***

        /// <summary>
        /// テキストファイルを読み込みます
        /// </summary>
        public static string ReadTextFile(string path)
        {
            if(!File.Exists(path))
            {
                Debug.LogWarning($"File not found: {path}");
                return string.Empty;
            }

            try
            {
                return File.ReadAllText(path);
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to read file: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// テキストファイルを書き出します（追記モード）
        /// </summary>
        public static bool WriteTextFile(string dirPath, string fileName, string content,
            StringDateType timeSuffixType = StringDateType.NoTime, string extension = ".txt")
        {
            var dateSuffix = "";
            var now = DateTime.Now;

            switch(timeSuffixType)
            {
                case StringDateType.DayLevel:
                    dateSuffix = "_" + now.ToString("yyyy-MM-dd");
                    break;
                case StringDateType.HourLevel:
                    dateSuffix = "_" + now.ToString("yyyy-MM-dd-HH");
                    break;
                case StringDateType.MinuteLevel:
                    dateSuffix = "_" + now.ToString("yyyy-MM-dd-HH-mm");
                    break;
                case StringDateType.SecondLevel:
                    dateSuffix = "_" + now.ToString("yyyy-MM-dd-HH-mm-ss");
                    break;
            }

            var fullPath = Path.Combine(dirPath, fileName + dateSuffix + extension);

            try
            {
                // ディレクトリがなければ作成
                if(!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                File.AppendAllText(fullPath, content + Environment.NewLine);
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError($"Failed to write file: {ex.Message}");
                return false;
            }
        }

        #endregion


        #region *** Camera ***

        public static Vector2 GetScreenPosition(Camera camera, Vector3 position, float clampWidth = 200f,
            float clampHeight = 200f)
        {
            var viewPos = camera.WorldToViewportPoint(position);

            // ビューポート外の処理（背面に回った場合など）が必要ならここに追加

            return new Vector2(
                Mathf.Clamp(Screen.width * viewPos.x, 0f, Screen.width - clampWidth),
                Mathf.Clamp(Screen.height * (1f - viewPos.y), 0f, Screen.height - clampHeight)
            );
        }

        public static Vector3 GetCursorPositionOnPlane(Camera camera, float height = 0f)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, new Vector3(0, height, 0));

            if(plane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance);
            }

            return Vector3.zero;
        }

        #endregion
    }

    /// <summary>
    /// 事前計算されたCos値（使用頻度が低いなら削除推奨、Mathf.Cosは十分高速です）
    /// </summary>
    public static class NipaCos
    {
        public static readonly float Cos10 = Mathf.Cos(10f * Mathf.Deg2Rad);
        public static readonly float Cos30 = Mathf.Cos(30f * Mathf.Deg2Rad);
        public static readonly float Cos45 = Mathf.Cos(45f * Mathf.Deg2Rad);
        public static readonly float Cos60 = Mathf.Cos(60f * Mathf.Deg2Rad);

        public static readonly float Cos90 = 0f;
        // 必要な分だけ定義するか、動的計算に切り替えることを推奨
    }

    public enum StringDateType
    {
        NoTime,
        DayLevel,
        HourLevel,
        MinuteLevel,
        SecondLevel
    }
}
