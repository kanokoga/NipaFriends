using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NipaFriends.WorldSpaces
{
    ///<summary>
    /// [ROLE] : 二次元矩形衝突判定
    /// [note] : http://marupeke296.com/COL_2D_No8_QuadTree.html
    ///</summary> 
    public class NipaCollisionDetector<T> where T : class
    {
        private Dictionary<int, List<ObjectData>> _cellAndObjectsIn;
        private List<CollisionPair> _result;
        private bool[] _isCheckNeeded;

        public int _spaceLevelCount;

        public Vector2 _spaceSize;
        private Vector2 _lowestLevelCellSize;
        private Stack<ObjectData> _collisionTestTargets;
        private int _maxCellCoord;

        public NipaCollisionDetector(int spaceLevel, Vector2 fieldSize)
        {
            this._spaceLevelCount = spaceLevel;
            this._spaceSize = fieldSize;
            this._lowestLevelCellSize = this._spaceSize / Mathf.Pow(2, this._spaceLevelCount - 1);
            this._isCheckNeeded = new bool[Mathf.RoundToInt(Mathf.Pow(4, this._spaceLevelCount)) + 1];
            this._maxCellCoord = 2 * spaceLevel - 1;
        }

        #region ***登録***

        public void Reset()
        {
            if (this._cellAndObjectsIn != null)
            {
                this._cellAndObjectsIn.Clear();
            }

            this._cellAndObjectsIn = new Dictionary<int, List<ObjectData>>();

            for (int i = 0; i < this._isCheckNeeded.Length; i++)
            {
                this._isCheckNeeded[i] = false;
            }
            if (this._result != null)
            {
                this._result.Clear();
            }

            this._result = new List<CollisionPair>();
        }

        ///<summary>
        /// [ROLE] : 衝突判定対象オブジェクトを登録する
        /// [note] : 座標は左下を原点とした正の場合のみ有効
        ///</summary> 
        public void Register(T objectClass, Vector2 center, Vector2 size)
        {
            //  Debug.LogFormat("registerd c {0}  s {1}", center, size);
            var obj = new ObjectData();
            obj.center = center;
            obj.size = size;
            obj.obj = objectClass;
            var linerCellId = this.GetLinerCellId(obj);
            //  Debug.LogFormat("liner cell index {0}", linerCellId);
            if (linerCellId < 0 || linerCellId >= this._isCheckNeeded.Length)
            {
                return;
            }

            if (this._cellAndObjectsIn.ContainsKey(linerCellId) == false)
            {
                this._cellAndObjectsIn.Add(linerCellId, new List<ObjectData>());
            }

            this._cellAndObjectsIn[linerCellId].Add(obj);
            this._isCheckNeeded[linerCellId] = true;

            var parentLinerId = linerCellId;
            while (parentLinerId > 0)
            {
                parentLinerId = this.GetParentLinerId(linerCellId);
                this._isCheckNeeded[parentLinerId] = true;
                linerCellId = parentLinerId;
            }
        }

        private int GetLinerCellId(ObjectData obj)
        {
            return this.GetLinerCellId(this.GetLT(obj), this.GetRB(obj));
        }

        private int GetLinerCellId(Vector2 leftTop, Vector2 rightBottom)
        {
            var LT_number = this.Get2DMortonNumber(this.GetCellCoord(leftTop));
            var RB_number = this.Get2DMortonNumber(this.GetCellCoord(rightBottom));
            //   Debug.LogFormat("morton lt {0} rb {1}", LT_number, RB_number);
            var envelopper = this.GetLevelAndCellIdEnvelopingObject(LT_number, RB_number);
            //    Debug.LogFormat("<color=white>level {0} id {1}</color>", envelopper.level, envelopper.id);
            return this.GetLinerCellIdFromCellId(envelopper.level, envelopper.id);
        }

        private int GetLinerCellIdFromCellId(int level, int cellId)
        {
            return cellId + Mathf.RoundToInt((Mathf.Pow(4, level) - 1) / 3);
        }

        private Vector2Int GetCellCoord(Vector2 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x / this._lowestLevelCellSize.x), Mathf.FloorToInt(pos.y / this._lowestLevelCellSize.y));
        }

        private LevelAndId GetLevelAndCellIdEnvelopingObject(int leftTopMortonNumber, int rightBottomMortonNumber)
        {
            if (leftTopMortonNumber == rightBottomMortonNumber)
            {
                return new LevelAndId(this._spaceLevelCount - 1, leftTopMortonNumber);
            }

            var temp = Convert.ToString(leftTopMortonNumber ^ rightBottomMortonNumber, 2).PadLeft((this._spaceLevelCount - 1) * 2, '0'); //Debug.Log(temp);
            var shiftCount = this._spaceLevelCount - 1;

            for (int i = 0; i < temp.Length; i += 2)
            {
                if (temp.Substring(i, 1) == "1" || temp.Substring(i + 1, 1) == "1")
                {
                    break;
                }

                shiftCount -= 1;
            }
            var id = rightBottomMortonNumber >> shiftCount * 2;
            return new LevelAndId(this._spaceLevelCount - shiftCount - 1, id);
        }

        private int GetParentLinerId(int linerId)
        {
            return Mathf.CeilToInt(linerId * 0.25f) - 1;
        }

        private int BitSeparate32(int n)
        {
            n = Mathf.Min(n, this._maxCellCoord);
            n = (n | (n << 8)) & 0x00ff00ff;
            n = (n | (n << 4)) & 0x0f0f0f0f;
            n = (n | (n << 2)) & 0x33333333;
            return (n | (n << 1)) & 0x55555555;
        }

        private int Get2DMortonNumber(Vector2Int cellCoord)
        {
            return (this.BitSeparate32(cellCoord.x) | (this.BitSeparate32(cellCoord.y) << 1));
        }

        private Vector2 GetLT(ObjectData obj)
        {
            return obj.center - obj.size * 0.5f;
        }

        private Vector2 GetRB(ObjectData obj)
        {
            return obj.center + obj.size * 0.5f;
        }

        #endregion


        #region ***衝突判定***

        public void StartDetectingCollision(System.Action<List<CollisionPair>> callback)
        {
            this._collisionTestTargets = new Stack<ObjectData>();
            this.DetectCollision(0);

            //foreach (var item in result)
            //{
            //    Debug.LogFormat("<color=yellow>collider {0}<>{1}</color>", item.x, item.y);
            //}
            callback(this._result);
        }

        private void DetectCollision(int targetLinerCellIndex)
        {
            ObjectData[] objectsInCell;

            var cellContainObjects = false;
            if (this.GetObjectsInCell(targetLinerCellIndex, out objectsInCell))
            {
                for (int i = 0; i < objectsInCell.Length; i++)
                {
                    for (int v = i + 1; v < objectsInCell.Length; v++)
                    {
                        if (this.IsCollide_Rap(objectsInCell[i], objectsInCell[v]))
                        {
                            this._result.Add(new CollisionPair(objectsInCell[i].obj, objectsInCell[v].obj));
                        }
                    }
                }

                foreach (var otherObj in this._collisionTestTargets)
                {
                    for (int v = 0; v < objectsInCell.Length; v++)
                    {
                        if (this.IsCollide_Rap(otherObj, objectsInCell[v]))
                        {
                            this._result.Add(new CollisionPair(otherObj.obj, objectsInCell[v].obj));
                        }
                    }
                }
                cellContainObjects = true;
            }

            if (targetLinerCellIndex * 4 + 4 < this._isCheckNeeded.Length)
            {
                var childFound = false;

                for (int i = 0; i < 4; i++)
                {
                    var childLinerCellIndex = targetLinerCellIndex * 4 + i + 1;
                    if (this._isCheckNeeded[childLinerCellIndex])
                    {
                        childFound = true;
                        break;
                    }
                }

                if (childFound)
                {
                    if (cellContainObjects)
                    {
                        foreach (var obj in objectsInCell)
                        {
                            this._collisionTestTargets.Push(obj);
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        var childLinerCellIndex = targetLinerCellIndex * 4 + i + 1;
                        if (this._isCheckNeeded[childLinerCellIndex])
                        {
                            //Debug.LogFormat("{0} -> {1}", targetLinerCellIndex, childLinerCellIndex);
                            this.DetectCollision(childLinerCellIndex);
                        }
                    }

                    if (cellContainObjects)
                    {
                        for (int i = 0; i < objectsInCell.Length; i++)
                        {
                            this._collisionTestTargets.Pop();
                        }
                    }
                }
            }
        }

        private bool GetObjectsInCell(int linerCellId, out ObjectData[] objects)
        {
            if (!this._cellAndObjectsIn.ContainsKey(linerCellId))
            {
                objects = null;
                return false;
            }
            else
            {
                objects = this._cellAndObjectsIn[linerCellId].ToArray();
                return true;
            }
        }

        private bool IsCollide_Rap(ObjectData objA, ObjectData objB)
        {
            var distX = Mathf.Abs(objA.center.x - objB.center.x);
            var distY = Mathf.Abs(objA.center.y - objB.center.y);
            var width = objA.size.x * 0.5f + objB.size.x * 0.5f;
            var height = objA.size.y * 0.5f + objB.size.y * 0.5f;

            return (distX < width) && (distY < height);
        }


        #endregion

        private struct LevelAndId
        {
            public LevelAndId(int level_, int id_)
            {
                this.id = id_;
                this.level = level_;
            }
            public int level;
            public int id;
        }

        private struct ObjectData
        {
            public T obj;
            public Vector2 center;
            public Vector2 size;
        }

        public struct CollisionPair
        {
            public CollisionPair(T a, T b)
            {
                this.objA = a;
                this.objB = b;
            }
            public T objA;
            public T objB;
        }

    }
}