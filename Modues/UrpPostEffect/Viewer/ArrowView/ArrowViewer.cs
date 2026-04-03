using System;
using System.Buffers;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace NipaFriends.Modules.UrpPostProcess.Viewers.ArrowViews
{
    public class ArrowDrawer : ViewerBase<ArrowDrawer.ArrowData>
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ArrowData
        {
            public Vector2 start; // 始点 (画面座標 UV: 0..1)
            public Vector2 end; // 終点 (画面座標 UV: 0..1)
            public Color color; // 色 (RGBA)
            public float thickness; // 太さ (UV基準。例: 0.005)
            public float headSize; // 矢印の頭のサイズ (UV基準。例: 0.02)
        }


        protected override void Awake()
        {
            base.Awake();
            this.bufferID = Shader.PropertyToID("_ArrowDataBuffer");
            this.bufferCountID = Shader.PropertyToID("_ArrowCount");
        }

        /// <summary>
        /// 矢印をキューに追加します。毎フレーム呼び出す必要があります。
        /// </summary>
        public static void Draw(Vector2 start, Vector2 end, Color color, float thickness = 0.005f,
            float headSize = 0.02f)
        {
            DataReceive.Add(
                new ArrowData()
                {
                    start = start,
                    end = end,
                    color = color,
                    thickness = thickness,
                    headSize = headSize
                }
            );
        }
    }
}
