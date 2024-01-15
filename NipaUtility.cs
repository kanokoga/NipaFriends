using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;
using Random = UnityEngine.Random;

namespace NipaFriends
{
    public static class NipaUtility
    {
        #region *** Definition ***

        private struct floatId
        {
            public floatId(int _id, float _value)
            {
                this.id = _id;
                this.value = _value;
            }

            public int id;
            public float value;
        }

        #endregion

        #region *** 距離と時間 ***

        ///<summary>
        /// [ROLE] : 指定された座標は指定距離内にあるか
        /// [note] : -
        ///</summary>
        public static bool IsTargetInRange(Vector3 _from, Vector3 _target, float _range)
        {
            return (_target - _from).sqrMagnitude < (_range * _range);
        }

        public static bool IsTargetInRangeHorizonal(Vector3 a, Vector3 b, float sqrRange)
        {
            a.y = 0;
            b.y = 0;
            return (a - b).sqrMagnitude < sqrRange;
        }


        ///<summary>
        /// [ROLE] : 指定の速度（処理ではdeltatimeで移動）で移動した場合指定の座標まで何秒かかるか
        /// [note] : 回転は無視する
        ///</summary>
        public static float GetTimeToReachTarget(Vector3 _from, Vector3 _to, float _speed)
        {
            return (_to - _from).magnitude / _speed;
        }


        ///<summary>
        /// [ROLE] : 同一水平面上にいるとして二乗距離を返す
        /// [note] : -
        ///</summary>
        public static float GetHorizonalSqrDistance(Vector3 a, Vector3 b)
        {
            a.y = b.y;
            return (a - b).sqrMagnitude;
        }

        #endregion

        #region *** 角度 ***

        ///<summary>
        /// [ROLE] : 座標が自身から見て何度の方角にいるか(0~180) 正面が０度
        /// [note] : オプションで高度差を加味
        ///</summary>
        public static float GetAngleDirection_Abs(Transform _observer, Vector3 _target, bool isAltitudeInclude = false)
        {
            var dir = _observer.TransformDirection(Vector3.forward);

            if(!isAltitudeInclude)
            {
                var pos = _target;
                pos.y = _observer.position.y;
                _target = pos;

                if(dir.y > Mathf.Epsilon)
                {
                    var posD = dir;
                    posD.y = 0f;
                    dir = posD;
                }
            }

            var tgtDir = _target - _observer.position;


            var ang = Vector3.Angle(tgtDir, dir);


            return ang;
        }


        ///<summary>
        /// [ROLE] : 単純にある２点間の角度をワールド基準で返す（0~360）
        /// [note] : ０時の方向（北）を０とする
        ///</summary>
        public static float GetAngle_Absolute(Vector3 _from, Vector3 _to)
        {
            var ang = Vector3.Angle(Vector3.forward, _to - _from);
            if(_to.x < _from.x)
            {
                ang = 360f - ang;
            }

            return ang;
        }


        ///<summary>
        /// [ROLE] : 座標が自身から見て何度の方角にいるか(-180~180) 正面が０度
        /// [note] : オプションで高度差を加味
        ///</summary>
        public static float GetAngleDirection(Transform _observer, Vector3 _target, bool isAltitudeInclude = false)
        {
            var dir = _observer.TransformDirection(Vector3.forward);

            if(!isAltitudeInclude)
            {
                var pos = _target;
                pos.y = _observer.position.y;
                _target = pos;

                if(dir.y > Mathf.Epsilon)
                {
                    var posD = dir;
                    posD.y = 0f;
                    dir = posD;
                }
            }

            var tgtDir = _target - _observer.position;
            var ang = Vector3.Angle(tgtDir, dir);

            var relative = _observer.InverseTransformPoint(_target);
            if(relative.x < 0)
            {
                ang = -ang;
            }

            return ang;
        }


        ///<summary>
        /// [ROLE] : 目標に対して正面を向けているか
        /// [note] : -
        ///</summary>
        public static bool IsFaceToTaget(Transform _observer, Vector3 _tgtPos, float _limitAngle,
            bool _ignoreHeight = false)
        {
            if(_ignoreHeight)
            {
                _tgtPos.y = _observer.position.y;
            }

            return Quaternion.Angle(_observer.rotation, Quaternion.LookRotation(_tgtPos - _observer.position)) <
                   _limitAngle;
        }


        ///<summary>
        /// [ROLE] : 基準点から見て地点Aと地点Bがなす角度は何度か
        /// [note] : -
        ///</summary>
        public static float GetAngleBetween(Vector3 _observer, Vector3 _posA, Vector3 _posB)
        {
            return Vector3.Angle((_posA - _observer), (_posB - _observer));
        }


        ///<summary>
        /// [ROLE] : あるベクトルに直行する単位ベクトルを返す
        /// [note] : -
        ///</summary>
        public static Vector2 GetDirectionVertical(Vector2 _originalDir)
        {
            return new Vector2(1 / _originalDir.x, -1 / _originalDir.y).normalized;
        }


        ///<summary>
        /// [ROLE] : あるベクトルと指定された角度をなす単位ベクトルを返す
        /// [note] : -
        ///</summary>
        public static Vector2 GetDirectionAngleToOriginalDir(Vector2 _original, float _ang)
        {
            _original = _original.normalized;
            _ang = _ang * Mathf.PI / 180f;
            return new Vector2(_original.x * Mathf.Cos(_ang) - _original.y * Mathf.Sin(_ang),
                _original.x * Mathf.Sin(_ang) + _original.y * Mathf.Cos(_ang));
        }

        #endregion

        #region *** 座標 ***

        // ----- 参照：http://iot-kyoto.com/technical/satoh/hittest-002, http://www5d.biglobe.ne.jp/~tomoya03/shtml/algorithm.html
        // ----- http://1st.geocities.jp/shift486909/program/program_menu.html


        ///<summary>
        /// [ROLE] : 指定地点Aから一番近い、指定地点Bから指定距離離れた地点を返す
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionOffToTarget(Vector3 _fromPos, Vector3 _targetPos, float _dist)
        {
            var dir = (_fromPos - _targetPos).normalized;
            return _targetPos + dir * _dist;
        }


        ///<summary>
        /// [ROLE] : 平面とベクトルの交点
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionRayHitPlane(Vector3 planePos, Vector3 planeNormalDir, Vector3 rayStartPos,
            Vector3 rayDir)
        {
            return rayStartPos + ((Vector3.Dot(planeNormalDir, planePos) - Vector3.Dot(planeNormalDir, rayStartPos)) /
                                  (Vector3.Dot(planeNormalDir, rayDir))) * rayDir;
        }


        ///<summary>
        /// [ROLE] : 線分FG上でかつ、指定地点Bから指定距離離れた地点を返す
        /// [note] : http://homepage1.nifty.com/gfk/circle-line.htm
        ///</summary>
        public static bool GetPositionOffToTarget(Vector3 _posFrom, Vector3 _posGoal, Vector3 _posCenter, float _dist,
            out Vector3 _result)
        {
            _result = _posFrom;

            Vector3 o;
            if(!GetPositionOnLineFromPoint(_posFrom, _posGoal, _posCenter, out o)) //中心から線分への垂線の足
            {
                return false;
            }

            var distSqr = _dist * _dist - (o - _posCenter).sqrMagnitude;
            Debug.Log(distSqr);
            if(distSqr < 0)
            {
                return false;
            }

            var dist = Mathf.Pow(distSqr, 0.5f);
            _result = GetPositionBetweenDist(_posFrom, _posGoal, dist);
            return true;
        }


        ///<summary>
        /// [ROLE] : 地点Aと地点Bを結ぶ線上で地点Aから指定比率離れた地点を返す
        /// [note] : 比率指定無しで中間地点(0.5)
        ///</summary>
        public static Vector3 GetPositionBetweenRatio(Vector3 _posA, Vector3 _posB, float _ratio = 0.5f)
        {
            var relative = (_posB - _posA);
            return _posA + relative * _ratio;
        }


        ///<summary>
        /// [ROLE] : 地点Aと地点Bを結ぶ線上で地点Aから指定距離離れた地点を返す
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionBetweenDist(Vector3 _posA, Vector3 _posB, float _distFromAtoB)
        {
            var dir = (_posB - _posA).normalized;
            return _posA + dir * _distFromAtoB;
        }


        ///<summary>
        /// [ROLE] : 指定された地点から指定距離内にある座標をランダムで返す
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionRandomInRange(Vector3 _origin, float _range)
        {
            var delta = Random.insideUnitCircle * _range;
            var result = _origin + new Vector3(delta.x, 0f, delta.y);
            return result;
        }


        ///<summary>
        /// [ROLE] : 指定の速度で現在の進路を維持した場合、何秒後にどこに到達しているか
        /// [note] :
        ///</summary>
        public static Vector3 GetPositonInFuture(Transform _mover, float _speed, float _duration)
        {
            return _mover.position + _mover.forward * _speed * _duration;
        }


        ///<summary>
        /// [ROLE] : 指定の速度で現在の進路を維持した場合、何秒後にどこに到達しているか
        /// [note] :
        ///</summary>
        public static Vector3 GetPositonInFuture(Vector3 position, Quaternion rotation, float _speed, float _duration)
        {
            return position; // + _mover.forward * _speed * _duration;
        }


        ///<summary>
        /// [ROLE] : 与えれた座標から単ベクトル方向に指定距離はなれた座標を返す
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionDirectionTo(Vector3 _original, Vector3 _dir, float _dist)
        {
            return _original + _dir * _dist;
        }


        ///<summary>
        /// [ROLE] : 座標が指定多角形内の外か内か
        /// [note] : http://www.hiramine.com/programming/graphics/2d_ispointinpolygon.html
        /// レイが多角形の辺を何回横切るかを数え、偶数回横切るとき、点は多角形の外側、奇数回横切るとき、点は多角形の内側と判定
        ///</summary>
        public static bool IsInsidePolygon(Vector2 pos, List<Vector2> vertexs, float rayLength = 100)
        {
            var crossedCount = 0;
            var rayEnd = Vector2.right * rayLength + pos;

            for(int i = 0; i < vertexs.Count; i++)
            {
                if(IsLineCross(pos, rayEnd, vertexs[i], vertexs[i == vertexs.Count - 1 ? 0 : i + 1]))
                {
                    crossedCount++;
                }
            }

            return crossedCount % 2 != 0;
        }

        public static bool IsInsidePolygon(Vector2 pos, List<Vector2> vertexs, float noise, float rayLength)
            => IsInsidePolygon(pos, vertexs.Select(v => v + Random.insideUnitCircle * noise).ToList(), rayLength);

        public static Vector2 RotatePoint(Vector2 target, Vector2 rotNormarized)
        {
            return new Vector2(target.x * rotNormarized.x - target.y * rotNormarized.y,
                target.x * rotNormarized.y + target.y * rotNormarized.x);
        }

        public static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            int polygonLength = polygon.Count, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = point.x, pointY = point.y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;
            while(i < polygonLength)
            {
                startX = endX;
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;
                endY = endPoint.y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }

            return inside;
        }


        ///<summary>
        /// [ROLE] : ベクトルXはベクトルAとベクトルBの間にあるか
        /// [note] : https://stackoverflow.com/questions/13640931/how-to-determine-if-a-vector-is-between-two-other-vectors
        ///</summary>
        public static bool IsVectorInside(Vector2 X, Vector2 A, Vector2 B)
        {
            var crossAB = A.x * B.y - B.x * A.y;
            var crossAX = A.x * X.y - X.x * A.y;

            if(crossAB * crossAX < 0)
            {
                return false;
            }

            var crossBA = B.x * A.y - A.x * B.y;
            var crossBX = B.x * X.y - X.x * B.y;

            if(crossBA * crossBX < 0)
            {
                return false;
            }

            return true;
        }


        ///<summary>
        /// [ROLE] : XからAへのベクトルとXからBへのベクトルの内積
        /// [note] :１だと同じ方向
        ///</summary>
        public static float GetDot(Vector2Int X, Vector2Int A, Vector2Int B)
        {
            var XA = new Vector2(A.x - X.x, A.y - X.y).normalized;
            var XB = new Vector2(B.x - X.x, B.y - X.y).normalized;
            return Vector2.Dot(XA, XB);
        }


        ///<summary>
        /// [ROLE] : 線分ABに点Cから垂直に下ろした線分の交点を求める.高度は位置にかかわらず点Aと同じ
        /// [note] : http://www.sousakuba.com/Programming/gs_near_pos_on_line.html
        /// 交点が線分上にあるなら正を返す
        ///</summary>
        public static bool GetPositionOnLineFromPoint(Vector3 _posA, Vector3 _posB, Vector3 _posC, out Vector3 result)
        {
            var posA = ConvertV3ToV2_IgnoreY(_posA);
            var posB = ConvertV3ToV2_IgnoreY(_posB);
            var posC = ConvertV3ToV2_IgnoreY(_posC);

            var v = (posB - posA).normalized;
            var v_AC = posC - posA;
            var dist = Vector2.Dot(v, v_AC);

            result = _posA + ConvertV2ToV3(v, 0f) * dist;

            if(dist < 0 || dist * dist > (posB - posA).sqrMagnitude)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool GetCrossPoint(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, out Vector3 result)
        {
            Vector2 result2 = Vector2.zero;
            float ratio;
            var found = GetCrossPoint(a1.ToV2XZ(), a2.ToV2XZ(), b1.ToV2XZ(), b2.ToV2XZ(), out result2, out ratio);
            result = result2.ToV3Y(Mathf.Lerp(a1.y, a2.y, ratio));
            return found;
        }

        public static bool GetCrossPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 result)
        {
            float dummy = 0f;
            return GetCrossPoint(a1, a2, b1, b2, out result, out dummy);
        }


        ///<summary>
        /// [ROLE] : ２つの線分の交点
        /// [note] : -
        ///</summary>
        public static bool GetCrossPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 result,
            out float ratio)
        {
            ratio = 0f;
            var v = b1 - a1;
            var va = a2 - a1;
            var vb = b2 - b1;

            var t1 = Cross(v, vb) / Cross(va, vb);
            var t2 = Cross(v, va) / Cross(va, vb);

            result = a1;
            if(t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                result = Vector2.Lerp(a1, a2, t1);
                ratio = t1;
                return true;
            }
            else
            {
                return false;
            }
        }


        ///<summary>
        /// [ROLE] : 線分ABに対して点Cの対象な点を求める
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionOppositeLine(Vector3 _posA, Vector3 _posB, Vector3 _posC)
        {
            Vector3 posO;
            GetPositionOnLineFromPoint(_posA, _posB, _posC, out posO);
            return GetPositionBetweenRatio(_posC, posO, 2f);
        }


        ///<summary>
        /// [ROLE] : 観察者から見た角度で観察者から指定距離離れた場所の座標を返す(-180~180)
        /// [note] : 高度は観察者と同じ
        ///</summary>
        public static Vector3 GetPositionAngleOffsetFrom(Transform _observer, float _angle, float _dist)
        {
            var tgtDir = _observer.TransformDirection(Vector3.forward);
            var resultVector = Vector3.zero;
            var angInRagian = _angle * Mathf.PI / 180f;

            resultVector.y = 0f;
            resultVector.x = tgtDir.x * Mathf.Cos(angInRagian) - tgtDir.z * Mathf.Sin(-angInRagian);
            resultVector.z = tgtDir.x * Mathf.Sin(-angInRagian) + tgtDir.z * Mathf.Cos(angInRagian);

            return resultVector * _dist + _observer.position;
        }


        ///<summary>
        /// [ROLE] : ２つの座標のうち近い方を返す
        /// [note] : -
        ///</summary>
        public static Vector3 GetPositionNearest(Vector3 _from, Vector3 _goalA, Vector3 _goalB)
        {
            if((_goalA - _from).sqrMagnitude > (_goalB - _from).sqrMagnitude)
            {
                return _goalB;
            }
            else
            {
                return _goalA;
            }
        }


        ///<summary>
        /// [ROLE] : 与えられた制御点からベジェ曲線を作成し、与えられた数分の線上の点を返す
        /// [note] : http://qiita.com/hart_edsf/items/cec5af01a70b62ca93f2, http://dixq.net/g/s_02.html
        /// http://www.dfx.co.jp/dftalk/?p=10576
        ///</summary>
        public static Vector2[] GetPointOnBezier(Vector2[] _ctrlPoints, int _resultNumber)
        {
            //---------------------------------- 先に係数を計算する

            var cpLength = _ctrlPoints.Length;
            var funcPara = GetPascalTriangle(_ctrlPoints.Length);

            //---------------------------------- ベジェ曲線を計算する

            var delta = 1f / (_resultNumber - 1);
            var para = 0f;
            var paraO = 0f;
            var result = new Vector2[_resultNumber];

            for(int r = 1; r < _resultNumber - 1; r++)
            {
                para = delta * r;
                paraO = 1f - para;
                var p = Vector2.zero;
                for(int i = 0; i < cpLength; i++)
                {
                    p.x += funcPara[i] * Mathf.Pow(paraO, cpLength - 1 - i) * Mathf.Pow(para, i) * _ctrlPoints[i].x;
                    p.y += funcPara[i] * Mathf.Pow(paraO, cpLength - 1 - i) * Mathf.Pow(para, i) * _ctrlPoints[i].y;
                }

                result[r] = p;
            }

            result[0] = _ctrlPoints[0];
            result[_resultNumber - 1] = _ctrlPoints[cpLength - 1];
            return result;
        }


        ///<summary>
        /// [ROLE] : 与えられた座標群と目的地から横隊陣形の座標を返す
        /// [note] : -
        ///</summary>
        public static Vector3[] GetFormation_Line(Vector3[] _positions, Vector3 _desti, float _interval,
            bool _sort = false)
        {
            Vector3 center = Vector3.zero;
            foreach(var item in _positions)
            {
                center += item;
            }

            center /= _positions.Length;
            var delta = _desti - center;
            delta.y = 0f;
            var dir = (delta).normalized;
            var dirNorm = new Vector3(dir.z, 0f, -dir.x);

            var formationWidthHalf = _interval * (_positions.Length - 1) / 2f;
            var results = new Vector3[_positions.Length];

            for(int i = 0; i < results.Length; i++)
            {
                results[i] = _desti + dirNorm * (formationWidthHalf - _interval * i);
            }

            if(!_sort)
            {
                return results;
            }

            ;

            //---------------------------------- ソートする場合
            //http://marupeke296.com/DXG_No19_ViweBCoordinate.html

            // var ang = Vector3.Angle(dir, Vector3.forward) * Mathf.PI / 180f;
            // var cos = Mathf.Cos(ang);
            // var sin = Mathf.Sin(ang);

            var cos = 1f / dir.x;
            var sin = 1f / dir.z;

            ///<summary> フォーメーション座標系からみたもとの点の横上での位置 </summary>
            var posXOnFormationWorld = new floatId[_positions.Length];

            for(int i = 0; i < posXOnFormationWorld.Length; i++)
            {
                posXOnFormationWorld[i] = new floatId(i, _positions[i].x * cos - _positions[i].z * sin);
            }

            if(sin > 0f && cos > 0f || sin < 0f && cos < 0f)
            {
                posXOnFormationWorld = posXOnFormationWorld.OrderByDescending(v => v.value).ToArray();
            }
            else
            {
                posXOnFormationWorld = posXOnFormationWorld.OrderBy(v => v.value).ToArray();
            }

            var resultsSorted = new Vector3[_positions.Length];

            for(int i = 0; i < resultsSorted.Length; i++)
            {
                resultsSorted[posXOnFormationWorld[i].id] = results[i];
            }

            return resultsSorted;
        }


        ///<summary>
        /// [ROLE] : Bsprine関数を描く
        /// [note] : http://1st.geocities.jp/shift486909/program/Interpolation2.html
        /// http://maicommon.ciao.jp/ss/Jalgo/Bspline/index.htm
        /// http://d.hatena.ne.jp/Zellij/20120705/p1
        ///</summary>
        public static float GetVector3Dot(Vector3 origin, Vector3 a, Vector3 b)
            => Vector3.Dot((a - origin).normalized, (b - origin).normalized);

        public static bool IsBothFront(Vector3 origin, Vector3 a, Vector3 b)
            => GetVector3Dot(origin, a, b) > 0f;

        public static bool IsBetween(Vector3 origin, Vector3 a, Vector3 b)
            => GetVector3Dot(origin, a, b) < 0f;

        #endregion

        #region *** その他 ***

        public static void CopyArray<T>(T[] origin, ref T[] target)
        {
            if(target == null || target.Length != origin.Length)
            {
                target = new T[origin.Length];
            }

            for(int i = 0; i < origin.Length; i++)
            {
                target[i] = origin[i];
            }
        }

        public static bool IsEquel(float a, float b)
            => Mathf.Abs(a - b) < 0.1f;


        ///<summary>
        ///[ROLE] : モノクロのテクスチャをカラーにする
        ///[note] : -
        ///</summary>///
        public static void ColorTexture(Texture2D _tex, Color _color)
        {
            for(int x = 0; x < _tex.width; x++)
            {
                for(int z = 0; z < _tex.height; z++)
                {
                    var c = _tex.GetPixel(x, z).r * _color;
                    _tex.SetPixel(x, z, c);
                }
            }

            _tex.Apply();
        }


        ///<summary>
        ///[ROLE] : テクスチャを加算する
        ///[note] : -
        ///</summary>///
        public static Texture2D CombineTexture2d(Texture2D _texA, Texture2D _texB)
        {
            Texture2D tex = new Texture2D(_texA.width, _texA.height);

            for(int x = 0; x < _texA.width; x++)
            {
                for(int z = 0; z < _texA.height; z++)
                {
                    var c = _texA.GetPixel(x, z) + _texB.GetPixel(x, z);
                    tex.SetPixel(x, z, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /////////////////////////////////////////////////////////////////
        ///<summary>
        ///[ROLE] : 子ども化し、位置とスケールを０００、１１１にする
        ///[note] : 任意のスケールも設定可能
        ///</summary>
        public static void SetParent(Transform _parent, Transform _cild, float _scale = 1f)
        {
            _cild.transform.SetParent(_parent);
            _cild.localPosition = Vector3.zero;
            _cild.localScale = Vector3.one * _scale;
        }


        ///<summary>
        ///[ROLE] : カラーの透明度を変更する
        ///[note] : -
        ///</summary>///
        static public Color ChangeColorAlpha(Color _coloer, float _alpha)
        {
            return new Color(_coloer.r, _coloer.g, _coloer.b, _alpha);
        }


        ///<summary>
        /// [ROLE] : 数値が指定範囲内に収まっているか
        /// [note] : -
        ///</summary>
        public static bool IsValueWithinRange(float _value, float _min, float _max)
        {
            return _value >= _min && _value <= _max;
        }


        ///<summary>
        /// [ROLE] : 数値が指定範囲内に収まっているか
        /// [note] : -
        ///</summary>
        public static bool IsValueWithinRange(int _value, int _min, int _max)
        {
            return _value >= _min && _value <= _max;
        }


        ///<summary>
        /// [ROLE] : パスカルの三角形を計算する
        /// [note] : _level = 0 ~ レベル０で1が返る。
        ///</summary>
        public static int[] GetPascalTriangle(int _level)
        {
            var temp = new int[_level];
            var result = new int[_level];

            for(int i = 0; i < result.Length; i++)
            {
                temp[i] = 1;
                result[i] = 1;
            }

            for(int i = 1; i < _level - 1; i++)
            {
                //Debug.Log("level" + i);
                for(int v = 1; v < i + 1; v++)
                {
                    result[v] = temp[v - 1] + temp[v];
                }

                for(int v = 1; v < i + 1; v++)
                {
                    temp[v] = result[v];
                }
            }

            return result;
        }


        ///<summary>
        /// [ROLE] : 彼我の間に障害物がないか
        /// [note] : -
        ///</summary>
        public static bool IsClearToTarget(Vector3 _from, Transform _target, float _maxDist = 10000f)
        {
            var isClear = false;
            var dir = _target.position + Vector3.up * 0.5f - _from;
            Ray ray = new Ray(_from, dir);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, _maxDist))
            {
                if(hit.transform == _target)
                {
                    isClear = true;
                }
            }

            return isClear;
        }

        public static bool IsLineCross(Vector3 _lineAStart, Vector3 _lineAEnd, Vector3 _lineBStart, Vector3 _lineBEnd)
            => IsLineCross(_lineAStart.ToV2XZ(), _lineAEnd.ToV2XZ(), _lineBStart.ToV2XZ(), _lineBEnd.ToV2XZ());


        ///<summary>
        /// [ROLE] : ２つの線分は交差するか
        /// [note] : http://marupeke296.com/COL_2D_No10_SegmentAndSegment.html
        ///</summary>
        public static bool IsLineCross(Vector2 _lineAStart, Vector2 _lineAEnd, Vector2 _lineBStart, Vector2 _lineBEnd)
        {
            var v = _lineBStart - _lineAStart;
            var v1 = _lineAEnd - _lineAStart;
            var v2 = _lineBEnd - _lineBStart;

            var t2 = Cross(v, v1) / Cross(v1, v2);
            if(!IsValueWithinRange(t2, 0f, 1f))
            {
                return false;
            }

            var t1 = Cross(v, v2) / Cross(v1, v2);
            if(!IsValueWithinRange(t1, 0f, 1f))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        ///<summary>
        ///[ROLE] : カメラのレイヤーを設定する
        ///[note] : -
        ///</summary>///
        public static void SetCameraLayer(Camera _cam, int _layerIndex)
        {
            _cam.cullingMask = (1 << _layerIndex);
        }

        public static int GetLayerMask(int layerIndex)
        {
            //// Bit shift the index of the layer (8) to get a bit mask
            //int layerMask = 1 << 8;

            //// This would cast rays only against colliders in layer 8.
            //// But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;
            var result = (1 << layerIndex);
            return result;
        }

        public static int GetLayerMaskExcept(int layerIndex)
        {
            var result = (1 << layerIndex);
            return ~result;
        }

        ///<summary>
        ///[ROLE] : カメラのレイヤーを設定する
        ///[note] : -
        ///</summary>///
        public static void SetCameraLayer(Camera _cam, string _layerName)
        {
            _cam.cullingMask = (1 << LayerMask.NameToLayer(_layerName));
        }


        ///<summary>
        /// [ROLE] : ２次元クロス積を求める
        /// [note] :
        ///</summary>
        public static float Cross(Vector2 _v1, Vector2 _v2)
        {
            return _v1.x * _v2.y - _v1.y * _v2.x;
        }


        ///<summary>
        /// [ROLE] : 座標の高さを指定の座標の高さに合わせる
        /// [note] : -
        ///</summary>
        public static Vector3 MakeSameAltitude(Vector3 _original, Vector3 _goal)
        {
            _original.y = _goal.y;
            return _original;
        }


        ///<summary>
        ///[ROLE] : ±ランダム値を返す
        ///[note] : -
        ///</summary>
        public static float RandomRange(float _v)
        {
            return UnityEngine.Random.Range(-_v, _v);
        }


        ///<summary>
        ///[ROLE] : ファイルまたはディレクトリが存在するか
        ///[note] : -
        ///</summary>
        public static bool IsPathExists(string path)
        {
            var exist = System.IO.File.Exists(path);
            if(!exist)
            {
                Debug.LogWarning("No path exists' " + path);
            }

            return exist;
        }


        ///<summary>
        ///[ROLE] : vector3をvector2に （x,z）
        ///[note] : 高度（y）は切り捨て
        ///</summary>
        public static Vector2 ConvertV3ToV2_IgnoreY(Vector3 _v)
        {
            return new Vector2(_v.x, _v.z);
        }


        ///<summary>
        ///[ROLE] : Vector2をvector3
        ///[note] : 高度を指定する
        ///</summary>
        public static Vector3 ConvertV2ToV3(Vector2 _v, float _y)
        {
            return new Vector3(_v.x, _y, _v.y);
        }


        ///<summary>
        ///[ROLE] : Vector2をvector3
        ///[note] : 高度を指定する
        ///</summary>
        public static Vector3[] AddV3(Vector3[] _v, Vector3 _addtional)
        {
            for(int i = 0; i < _v.Length; i++)
            {
                _v[i] += _addtional;
            }

            return _v;
        }


        ///<summary>
        ///[ROLE] : 指定したインデックスから指定した数はなれたインデックスを返す
        ///[note] : 長さの数倍離れると対応できない
        ///</summary>
        public static int NeighborIndex(int _arrayLength, int _tgtIndex, int _dist)
        {
            int result = _tgtIndex + _dist;
            if(result < 0)
            {
                result = _arrayLength + result;
            }
            else if(result >= _arrayLength)
            {
                result = (result - _arrayLength);
            }

            return Mathf.Clamp(result, 0, _arrayLength - 1);
        }


        ///<summary>
        ///[ROLE] : 要素を与えると一つのインデックスを返す
        ///[note] : 要素の値が大きいほど選ばれる確率が高まる
        ///</summary>///
        public static int RandomIndex(float[] _elements)
        {
            var sum = 0f;
            for(int i = 0; i < _elements.Length; i++)
            {
                sum += _elements[i];
            }

            var v = Random.value * sum;
            var current = 0f;
            var result = 0;
            for(int i = 0; i < _elements.Length; i++)
            {
                current += _elements[i];

                if(v < current && v > current - _elements[i])
                {
                    result = i;
                    //for (int t = i; t >= 0; t--)
                    //{
                    //    if (_elements[i] - _elements[t] < Mathf.Epsilon)
                    //        result = t;
                    //    else
                    //        break;
                    //}
                    break;
                }
            }

            return result;
        }


        ///<summary>
        ///[ROLE] : 指定範囲以内の整数をランダムで返す
        ///[note] : -
        ///</summary>
        public static int RandomInt(int _min, int _max)
        {
            return Mathf.FloorToInt(UnityEngine.Random.Range(_min, _max + 1));
        }


        ///<summary>
        ///[ROLE] : 指定範囲以内の整数をランダムで返す
        ///[note] : -
        ///</summary>
        public static int RandomInt(int _min, int _max, int _except)
        {
            var ints = new int[_max - _min];
            var index = 0;
            for(int i = _min; i <= _max; i++)
            {
                if(i == _except)
                {
                    continue;
                }

                ints[index] = i;
                index++;
            }

            return ints[Mathf.FloorToInt(Random.value * ints.Length)];
        }


        ///<summary>
        /// [ROLE] : 指定した文字を改行コードに差し替える
        /// [note] : -
        ///</summary>
        public static string ReplaceWithNewLine(string _text, string _target)
        {
            return _text.Replace(_target, System.Environment.NewLine);
        }


        ///<summary>
        ///[ROLE] : 配列をシャッフルして指定の要素数まで削る
        ///[note] : -
        ///</summary>///
        public static T[] ShuffleArray<T>(T[] _array, int _count)
        {
            var ary2 = new T[_count];
            _array = _array.OrderBy(i => System.Guid.NewGuid()).ToArray();
            System.Array.Copy(_array, ary2, _count);
            return ary2;
        }


        ///<summary>
        ///[ROLE] : コンポネントを新しいGOにコピーする
        ///[note] : http://answers.unity3d.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html, http://answers.unity3d.com/questions/458207/copy-a-component-at-runtime.html
        ///</summary>
        public static void CopyComponent(Component _original, GameObject _dest)
        {
            System.Type compoType = _original.GetType();

            var former = _dest.GetComponent(compoType);
            if(former != null)
            {
                Component.Destroy(former);
            }

            Component newCompo = _dest.AddComponent(compoType);

            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public |
                                                   System.Reflection.BindingFlags.NonPublic |
                                                   System.Reflection.BindingFlags.Instance |
                                                   System.Reflection.BindingFlags.Default |
                                                   System.Reflection.BindingFlags.DeclaredOnly;

            System.Reflection.PropertyInfo[] pinfos = compoType.GetProperties(flags);
            foreach(var pinfo in pinfos)
            {
                if(pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(newCompo, pinfo.GetValue(_original, null), null);
                    }
                    catch { }
                }
            }

            System.Reflection.FieldInfo[] fileds = compoType.GetFields();
            foreach(var field in fileds)
            {
                field.SetValue(newCompo, field.GetValue(_original));
            }
        }


        ///<summary>
        ///[ROLE] : 円状に展開した場合の座標を返す
        ///[note] : -
        ///</summary>
        public static Vector3[] CirclePosition(int _obejectCount, float _radius, Vector3 _cetnerPos)
        {
            var results = new Vector3[_obejectCount];

            float para;
            float posX;
            float posZ;

            for(int i = 0; i < _obejectCount; i++)
            {
                para = i * ((Mathf.PI * 2.0f) / _obejectCount);
                posX = _radius * Mathf.Cos(para);
                posZ = _radius * Mathf.Sin(para);

                // Position
                results[i] = new Vector3(posX, 0f, posZ) + _cetnerPos;
            }

            return results;
        }


        ///<summary>
        ///[ROLE] : 円状に展開した場合の座標を返す
        ///[note] : 中心からの距離がランダム
        ///</summary>
        public static Vector3[] CirclePositionRandomRange(int _obejectCount, Vector2 _radiusRange, Vector3 _cetnerPos)
        {
            var results = new Vector3[_obejectCount];

            float para;
            float posX;
            float posZ;

            for(int i = 0; i < _obejectCount; i++)
            {
                var range = Random.Range(_radiusRange.x, _radiusRange.y);
                para = i * ((Mathf.PI * 2.0f) / _obejectCount);
                posX = range * Mathf.Cos(para);
                posZ = range * Mathf.Sin(para);
                results[i] = new Vector3(posX, 0f, posZ) + _cetnerPos;
            }

            return results;
        }


        ///<summary>
        /// [ROLE] : -
        /// [note] : 順番は逆になることがある
        ///</summary>
        public static List<Vector2Int> CellLine(Vector2Int start, Vector2Int end, bool fixStart = false)
        {
            var result = new List<Vector2Int>();
            var steep = Mathf.Abs(end.y - start.y) > Mathf.Abs(end.x - start.x);
            var swaped = false;
            if(steep)
            {
                var s = start.x;
                start.x = start.y;
                start.y = s;

                s = end.x;
                end.x = end.y;
                end.y = s;
            }

            if(start.x > end.x)
            {
                swaped = true;

                var s = start.x;
                start.x = end.x;
                end.x = s;

                s = start.y;
                start.y = end.y;
                end.y = s;
            }

            int deltax = end.x - start.x;

            int deltay = Mathf.Abs(end.y - start.y);
            int error = Mathf.RoundToInt(deltax / 2f);
            int ystep = 0;
            int y = start.y;

            if(start.y < end.y)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }

            for(int x = start.x; x < end.x; x++)
            {
                if(steep)
                {
                    result.Add(new Vector2Int(y, x));
                }
                else
                {
                    result.Add(new Vector2Int(x, y));
                }

                error -= deltay;

                if(error < 0)
                {
                    y += ystep;
                    error += deltax;
                }
            }

            if(fixStart == true && swaped == true)
            {
                result.Reverse();
            }

            return result;
        }

        static public string ReadTextFile(string _pathToTextFile)
        {
            var result = "";
            try
            {
                // Open the text file using a stream reader.
                using(StreamReader sr = new StreamReader(_pathToTextFile))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    result = line;
                }
            }
            catch(Exception e)
            {
                Debug.LogWarning("The file could not be read:");
                Debug.LogWarning(e.Message);
            }

            return result;
        }


        ///<summary>
        ///[ROLE] : テキストファイルを書き出す
        ///[note] : 同名ファイルには書き足す
        ///</summary>
        public static bool WriteTextFile(string _dirPath, string _fileName, string _content,
            StringDateType _minTimeType, string _extension = ".txt")
        {
            _content += System.Environment.NewLine;


            var fileNameDate = "";

            switch(_minTimeType)
            {
                case StringDateType.NoTime:
                    break;
                case StringDateType.DayLevel:
                    fileNameDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case StringDateType.HourLevel:
                    fileNameDate = DateTime.Now.ToString("yyyy-MM-dd-HH");
                    break;
                case StringDateType.MinuteLevel:
                    fileNameDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
                    break;
                case StringDateType.SecondLevel:
                    fileNameDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    break;
                default:
                    break;
            }

            if(fileNameDate != "")
            {
                _fileName += "_" + fileNameDate;
            }

            _fileName += _extension;

            var path = System.IO.Path.Combine(_dirPath, _fileName);

            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();

            try
            {
                sw.Write(_content);
            }
            catch(Exception ex)
            {
                Debug.Log("filed to output log " + ex.ToString());
                //UtilityTeam.LogToDIWManager_General(UtilityTeam.LogKind.ErrorLog, "filed to output log " + ex.ToString());
                return false;
                //throw;
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }

            return true;
        }

        public static Vector2 GetScreenPosition(Camera camera, Vector3 position, float width = 200f,
            float height = 200f)
        {
            var viewPos = camera.WorldToViewportPoint(position);
            var screenPos = new Vector2(
                Mathf.Clamp(Screen.width * viewPos.x, 0f, Screen.width - width),
                Mathf.Clamp(Screen.height * (1f - viewPos.y), 0f, Screen.height - height)
            );
            return screenPos;
        }

        #endregion

        #region *** Camera ***

        public static Vector3 GetCursorPositionOnPlane(Camera camera, float height = 0f)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            return GetPositionRayHitPlane(Vector3.up * height, Vector3.up, camera.transform.position, ray.direction);
        }

        #endregion
    }

    public static class NipaCSharpExtention
    {
        ///<summary>
        ///[ROLE] : vector3をvector2に （x,z）
        ///[note] : 高度（y）は切り捨て
        ///</summary>
        public static Vector2 ToV2XZ(this Vector3 _v)
        {
            return new Vector2(_v.x, _v.z);
        }

        public static Vector2 ToV2XY(this Vector3 _v)
        {
            return new Vector2(_v.x, _v.y);
        }

        public static Vector3 SetY(this Vector3 _v, float y)
        {
            return new Vector3(_v.x, y, _v.z);
        }

        public static Vector3 SwapYZ(this Vector3 _v)
        {
            return new Vector3(_v.x, _v.z, _v.y);
        }

        ///<summary>
        ///[ROLE] : Vector2をvector3
        ///[note] : 高度を指定する
        ///</summary>
        public static Vector3 ToV3Y(this Vector2 _v, float _y)
        {
            return new Vector3(_v.x, _y, _v.y);
        }

        public static Vector3 ToV3Z(this Vector2 _v, float z)
        {
            return new Vector3(_v.x, _v.y, z);
        }

        public static Vector3 Scale(this Vector3 _v, float sx, float sy, float sz)
        {
            return new Vector3(_v.x * sx, _v.y * sy, _v.z * sz);
        }


        ///<summary>
        ///[ROLE] : Vector2をvector3
        ///[note] : 高度を指定する
        ///</summary>
        public static Vector3[] ConvertV2ToV3(this Vector2[] _v, float _y)
        {
            var v3 = new Vector3[_v.Length];
            for(int i = 0; i < _v.Length; i++)
            {
                v3[i] = new Vector3(_v[i].x, _y, _v[i].y);
            }

            return v3;
        }

        public static T CloneDeep<T>(this T target) where T : class
        {
            object clone = null;
            using(System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, target);
                stream.Position = 0;
                clone = formatter.Deserialize(stream);
            }

            return (T)clone;
        }

        public static bool CompareColors(Color c, Vector3 color255, float threshold = 1f)
        {
            var delta = (color255 - new Vector3(c.r * 255, c.g * 255, c.b * 255)).sqrMagnitude;
            return delta < threshold;
        }

        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static void SetParentZero(this Transform _cild, Transform _parent, float _scale = 1f)
        {
            _cild.transform.SetParent(_parent);
            _cild.localPosition = Vector3.zero;
            _cild.localScale = Vector3.one * _scale;
            _cild.localRotation = Quaternion.identity;
        }
    }

    ///<summary>
    /// [ROLE] : 日付の細かさのレベル
    /// [note] : -
    ///</summary>
    public enum StringDateType
    {
        NoTime,
        DayLevel,
        HourLevel,
        MinuteLevel,
        SecondLevel
    }
}
