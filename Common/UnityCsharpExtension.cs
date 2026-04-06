using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NipaFriends
{
    public static class UnityCsharpExtension
    {
     ///<summary>
        ///[ROLE] : vector3をvector2に （x,z）
        ///[note] : 高度（y）は切り捨て
        ///</summary>
        public static Vector2 ToV2_XZ(this Vector3 _v)
        {
            return new Vector2(_v.x, _v.z);
        }

        public static Vector2 ToV2_XY(this Vector3 _v)
        {
            return new Vector2(_v.x, _v.y);
        }

        public static Vector3 SetY(this Vector3 _v, float y)
        {
            return new Vector3(_v.x, y, _v.z);
        }

        public static Vector3 SetZ(this Vector3 _v, float z)
        {
            return new Vector3(_v.x, _v.y, z);
        }

        public static Vector3 SwapYZ(this Vector3 _v)
        {
            return new Vector3(_v.x, _v.z, _v.y);
        }

        ///<summary>
        ///[ROLE] : Vector2をvector3
        ///[note] : 高度を指定する
        ///</summary>
        public static Vector3 ToV3_Y(this Vector2 _v, float _y)
        {
            return new Vector3(_v.x, _y, _v.y);
        }

        public static Vector3 ToV3_Z(this Vector2 _v, float z)
        {
            return new Vector3(_v.x, _v.y, z);
        }

        public static Vector3 Scale(this Vector3 _v, float sx, float sy, float sz)
        {
            return new Vector3(_v.x * sx, _v.y * sy, _v.z * sz);
        }

        public static Vector3 Sum(this IEnumerable<Vector3> vectors)
        {
            return vectors.Aggregate(Vector3.zero, (acc, v) => acc + v);
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            var count = vectors.Count();
            if(count == 0)
            {
                return Vector3.zero;
            }

            return vectors.Sum() / count;
        }

        /// <summary>
        /// Spherical linear interpolation for Vector2 (angle interpolation).
        /// </summary>
        public static Vector2 Vector2Slerp(Vector2 from, Vector2 to, float t)
        {
            float angle = Vector2.SignedAngle(from, to);
            float rad = angle * Mathf.Deg2Rad * t;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);

            Vector2 dir = new Vector2(
                from.x * cos - from.y * sin,
                from.x * sin + from.y * cos
            );
            return dir.normalized * Mathf.Lerp(from.magnitude, to.magnitude, t);
        }


        ///<summary>
        ///[ROLE] : Vector2をvector3
        ///[note] : 高度を指定する
        ///</summary>
        public static Vector3[] ConvertV2ToV3(this Vector2[] _v, float _y)
        {
            var v3 = new Vector3[_v.Length];
            for(var i = 0; i < _v.Length; i++)
            {
                v3[i] = new Vector3(_v[i].x, _y, _v[i].y);
            }

            return v3;
        }

        public static T CloneDeep<T>(this T target) where T : class
        {
            object clone = null;
            using(var stream = new System.IO.MemoryStream())
            {
                var formatter =
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

}

