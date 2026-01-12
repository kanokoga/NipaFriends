using UnityEngine;
using System.Collections.Generic;

namespace NipaFriends.Meshes
{

    /// <summary>
    /// generate line mesh flat on XZ plane
    /// </summary>
    public static class FlatLineMeshGenerator
    {
        /// <summary>
        /// 座標リストからメッシュを生成する
        /// </summary>
        public static Mesh GenerateMesh(List<Vector3> points, float lineWidth)
        {
            if(points.Count < 2)
            {
                return null;
            }

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();
            var normals = new List<Vector3>();

            for(var i = 0; i < points.Count; i++)
            {
                Vector3 tangent;

                if(i == 0)
                {
                    // 始点：次の点への方向のみ
                    tangent = (points[i + 1] - points[i]).normalized;
                }
                else if(i == points.Count - 1)
                {
                    // 終点：前の点からの方向のみ
                    tangent = (points[i] - points[i - 1]).normalized;
                }
                else
                {
                    // 中間点：入ってくる方向と出ていく方向の平均をとる
                    var vIn = (points[i] - points[i - 1]).normalized;
                    var vOut = (points[i + 1] - points[i]).normalized;
                    tangent = (vIn + vOut).normalized;
                }

                // 帯を「真上」に向けるための横方向ベクトル
                // tangentに対して常に垂直なsideベクトルを作る
                var side = Vector3.Cross(Vector3.up, tangent).normalized;

                // 【重要】角度による幅の補正 (Miter Joint)
                // 直角に曲がる際、単純な垂直だと内側が細くなるのを防ぎたい場合は
                // ここで計算を足しますが、まずは基本の平均化で試します。

                vertices.Add(points[i] + side * (lineWidth / 2f)); // 右
                vertices.Add(points[i] - side * (lineWidth / 2f)); // 左

                normals.Add(Vector3.up);
                normals.Add(Vector3.up);

                var u = (float)i / (points.Count - 1);
                uvs.Add(new Vector2(u, 1));
                uvs.Add(new Vector2(u, 0));

                if(i < points.Count - 1)
                {
                    var root = i * 2;
                    triangles.Add(root);
                    triangles.Add(root + 2);
                    triangles.Add(root + 1);

                    triangles.Add(root + 1);
                    triangles.Add(root + 2);
                    triangles.Add(root + 3);
                }
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh GenerateMesh(List<Vector3> points,
            float lineWidth,
            float headV,
            float headLength,
            float tailV,
            float tailLength)
        {
            if(points.Count < 2)
            {
                return null;
            }

            // --- 1. 全体の距離と各ポイントの累積距離を計算 ---
            float totalDist = 0;
            var dists = new float[points.Count];
            for(var i = 1; i < points.Count; i++)
            {
                totalDist += Vector3.Distance(points[i - 1], points[i]);
                dists[i] = totalDist;
            }

            // 指定された長さが全体の長さを超えないようクランプ
            tailLength = Mathf.Clamp(tailLength, 0, totalDist);
            headLength = Mathf.Clamp(headLength, 0, totalDist - tailLength);

            // --- 2. UVと位置を保持する一時リストの作成 (頂点挿入ロジック) ---
            var customPoints = new List<(Vector3 pos, float v)>();
            customPoints.Add((points[0], 0f)); // 始点

            // Head位置の挿入
            for(var i = 0; i < points.Count - 1; i++)
            {
                if(dists[i] < tailLength && dists[i + 1] >= tailLength)
                {
                    var t = (tailLength - dists[i]) / (dists[i + 1] - dists[i]);
                    customPoints.Add((Vector3.Lerp(points[i], points[i + 1], t), tailV));
                    break;
                }
            }

            // 中間点の挿入 (HeadとTailの間にある元のポイント)
            for(var i = 1; i < points.Count - 1; i++)
            {
                if(dists[i] > tailLength && dists[i] < (totalDist - headLength))
                {
                    // headV から tailV の間を線形補間
                    var t = (dists[i] - tailLength) / (totalDist - tailLength - headLength);
                    var v = Mathf.Lerp(tailV, headV, t);
                    customPoints.Add((points[i], v));
                }
            }

            // Tail位置の挿入
            var tailDistPos = totalDist - headLength;
            for(var i = 0; i < points.Count - 1; i++)
            {
                if(dists[i] < tailDistPos && dists[i + 1] >= tailDistPos)
                {
                    var t = (tailDistPos - dists[i]) / (dists[i + 1] - dists[i]);
                    customPoints.Add((Vector3.Lerp(points[i], points[i + 1], t), headV));
                    break;
                }
            }

            customPoints.Add((points[points.Count - 1], 1f)); // 終点

            // --- 3. メッシュデータの構築 ---
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            for(var i = 0; i < customPoints.Count; i++)
            {
                var pos = customPoints[i].pos;
                var v = customPoints[i].v;

                // 接線の計算 (前後のポイントから平均を算出)
                Vector3 tangent;
                if(i == 0)
                {
                    tangent = (customPoints[i + 1].pos - pos).normalized;
                }
                else if(i == customPoints.Count - 1)
                {
                    tangent = (pos - customPoints[i - 1].pos).normalized;
                }
                else
                {
                    var vIn = (pos - customPoints[i - 1].pos).normalized;
                    var vOut = (customPoints[i + 1].pos - pos).normalized;
                    tangent = (vIn + vOut).normalized;
                }

                // Y-up固定のsideベクトル
                var side = Vector3.Cross(Vector3.up, tangent).normalized;

                // 左右の頂点
                vertices.Add(pos + side * (lineWidth / 2f)); // 右
                vertices.Add(pos - side * (lineWidth / 2f)); // 左

                // UV (Uは横幅0~1, Vは進行方向)
                uvs.Add(new Vector2(1, v));
                uvs.Add(new Vector2(0, v));

                // インデックスの作成
                if(i < customPoints.Count - 1)
                {
                    var r = i * 2;
                    triangles.Add(r);
                    triangles.Add(r + 2);
                    triangles.Add(r + 1);
                    triangles.Add(r + 1);
                    triangles.Add(r + 2);
                    triangles.Add(r + 3);
                }
            }

            // --- 4. メッシュの作成と返却 ---
            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
