using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NipaFriends.WorldSpaces
{
    ///<summary>
    /// 二次元矩形衝突判定
    /// http://marupeke296.com/COL_2D_No8_QuadTree.html
    ///</summary>
    public class NipaCollisionChecker<T> where T : struct
    {
        public Vector2 SpaceSize { get; private set; }

        private Dictionary<int, List<ObjectData>> objectsPerCell = new Dictionary<int, List<ObjectData>>();
        private List<CollisionPair> collidedPairs = new List<CollisionPair>();
        private bool[] isCheckNeeded;
        private int spaceLevelCount;
        private Vector2 lowestLevelCellSize;
        private Stack<ObjectData> collisionTestTargets = new Stack<ObjectData>();
        private int maxCellCoord;


        public NipaCollisionChecker(int spaceLevel, Vector2 spaceSize)
        {
            this.spaceLevelCount = spaceLevel;
            this.SpaceSize = spaceSize;
            this.lowestLevelCellSize = this.SpaceSize / Mathf.Pow(2, this.spaceLevelCount - 1);
            this.isCheckNeeded = new bool[Mathf.RoundToInt(Mathf.Pow(4, this.spaceLevelCount)) + 1];
            this.maxCellCoord = 2 * spaceLevel - 1;
        }

        #region PUBLIC

        ///<summary>
        /// 衝突判定対象オブジェクトを登録する
        /// 座標は左下を原点とした正の場合のみ有効
        ///</summary>
        public void AddObject(T target, Vector2 center, Vector2 size)
        {
            var obj = new ObjectData();
            obj.center = center;
            obj.size = size;
            obj.obj = target;

            var linerCellId = this.GetLinerCellId(obj);
            if(linerCellId < 0 || linerCellId >= this.isCheckNeeded.Length)
            {
                return;
            }

            if(this.objectsPerCell.ContainsKey(linerCellId) == false)
            {
                this.objectsPerCell.Add(linerCellId, new List<ObjectData>());
            }

            this.objectsPerCell[linerCellId].Add(obj);
            this.isCheckNeeded[linerCellId] = true;

            var parentLinerId = linerCellId;
            while(parentLinerId > 0)
            {
                parentLinerId = this.GetParentLinerId(linerCellId);
                this.isCheckNeeded[parentLinerId] = true;
                linerCellId = parentLinerId;
            }
        }

        public void Calc(Action<List<CollisionPair>> callback)
        {
            this.collisionTestTargets = new Stack<ObjectData>();
            this.DetectCollision(0);
            callback(this.collidedPairs);
        }

        public void Reset()
        {
            this.objectsPerCell.Clear();
            for(int i = 0; i < this.isCheckNeeded.Length; i++)
            {
                this.isCheckNeeded[i] = false;
            }

            this.collidedPairs.Clear();
        }

        #endregion

        private int GetLinerCellId(ObjectData obj)
        {
            return this.GetLinerCellId(this.GetLeftTop(obj), this.GetRightBottom(obj));
        }

        private int GetLinerCellId(Vector2 leftTop, Vector2 rightBottom)
        {
            var LT_number = this.Get2DMortonNumber(this.GetCellCoord(leftTop));
            var RB_number = this.Get2DMortonNumber(this.GetCellCoord(rightBottom));
            var envelopper = this.GetLevelAndCellIdEnvelopingObject(LT_number, RB_number);
            return this.GetLinerCellIdFromCellId(envelopper.level, envelopper.id);
        }

        private int GetLinerCellIdFromCellId(int level, int cellId)
        {
            return cellId + Mathf.RoundToInt((Mathf.Pow(4, level) - 1) / 3);
        }

        private Vector2Int GetCellCoord(Vector2 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x / this.lowestLevelCellSize.x),
                Mathf.FloorToInt(pos.y / this.lowestLevelCellSize.y));
        }

        private LevelAndId GetLevelAndCellIdEnvelopingObject(int leftTopMortonNumber, int rightBottomMortonNumber)
        {
            if(leftTopMortonNumber == rightBottomMortonNumber)
            {
                return new LevelAndId(this.spaceLevelCount - 1, leftTopMortonNumber);
            }

            var temp = Convert.ToString(leftTopMortonNumber ^ rightBottomMortonNumber, 2)
                .PadLeft((this.spaceLevelCount - 1) * 2, '0'); //Debug.Log(temp);
            var shiftCount = this.spaceLevelCount - 1;

            for(int i = 0; i < temp.Length; i += 2)
            {
                if(temp.Substring(i, 1) == "1" || temp.Substring(i + 1, 1) == "1")
                {
                    break;
                }

                shiftCount -= 1;
            }

            var id = rightBottomMortonNumber >> shiftCount * 2;
            return new LevelAndId(this.spaceLevelCount - shiftCount - 1, id);
        }

        private int GetParentLinerId(int linerId)
        {
            return Mathf.CeilToInt(linerId * 0.25f) - 1;
        }

        private int BitSeparate32(int n)
        {
            n = Mathf.Min(n, this.maxCellCoord);
            n = (n | (n << 8)) & 0x00ff00ff;
            n = (n | (n << 4)) & 0x0f0f0f0f;
            n = (n | (n << 2)) & 0x33333333;
            return (n | (n << 1)) & 0x55555555;
        }

        private int Get2DMortonNumber(Vector2Int cellCoord)
        {
            return (this.BitSeparate32(cellCoord.x) | (this.BitSeparate32(cellCoord.y) << 1));
        }

        private Vector2 GetLeftTop(ObjectData obj)
        {
            return obj.center - obj.size * 0.5f;
        }

        private Vector2 GetRightBottom(ObjectData obj)
        {
            return obj.center + obj.size * 0.5f;
        }

        private void DetectCollision(int targetLinerCellIndex)
        {
            ObjectData[] objectsInCell;

            var cellContainObjects = false;
            if(this.GetObjectsInCell(targetLinerCellIndex, out objectsInCell))
            {
                for(int i = 0; i < objectsInCell.Length; i++)
                {
                    for(int v = i + 1; v < objectsInCell.Length; v++)
                    {
                        if(this.IsCollide_Rap(objectsInCell[i], objectsInCell[v]))
                        {
                            this.collidedPairs.Add(new CollisionPair(objectsInCell[i].obj, objectsInCell[v].obj));
                        }
                    }
                }

                foreach(var otherObj in this.collisionTestTargets)
                {
                    for(int v = 0; v < objectsInCell.Length; v++)
                    {
                        if(this.IsCollide_Rap(otherObj, objectsInCell[v]))
                        {
                            this.collidedPairs.Add(new CollisionPair(otherObj.obj, objectsInCell[v].obj));
                        }
                    }
                }

                cellContainObjects = true;
            }

            if(targetLinerCellIndex * 4 + 4 < this.isCheckNeeded.Length)
            {
                var childFound = false;

                for(int i = 0; i < 4; i++)
                {
                    var childLinerCellIndex = targetLinerCellIndex * 4 + i + 1;
                    if(this.isCheckNeeded[childLinerCellIndex])
                    {
                        childFound = true;
                        break;
                    }
                }

                if(childFound)
                {
                    if(cellContainObjects)
                    {
                        foreach(var obj in objectsInCell)
                        {
                            this.collisionTestTargets.Push(obj);
                        }
                    }

                    for(int i = 0; i < 4; i++)
                    {
                        var childLinerCellIndex = targetLinerCellIndex * 4 + i + 1;
                        if(this.isCheckNeeded[childLinerCellIndex])
                        {
                            //Debug.LogFormat("{0} -> {1}", targetLinerCellIndex, childLinerCellIndex);
                            this.DetectCollision(childLinerCellIndex);
                        }
                    }

                    if(cellContainObjects)
                    {
                        for(int i = 0; i < objectsInCell.Length; i++)
                        {
                            this.collisionTestTargets.Pop();
                        }
                    }
                }
            }
        }

        private bool GetObjectsInCell(int linerCellId, out ObjectData[] objects)
        {
            if(!this.objectsPerCell.ContainsKey(linerCellId))
            {
                objects = null;
                return false;
            }
            else
            {
                objects = this.objectsPerCell[linerCellId].ToArray();
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

        #region Define

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

        #endregion
    }
}
