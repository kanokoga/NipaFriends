using System;
using System.Buffers;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace NipaFriends.Modules.UrpPostProcess.Viewers
{
    public abstract class ViewerBase<T> : DynamicPostProcess where T : struct
    {
        protected static List<T> DataReceive = new List<T>();
        private static List<T> DataUse = new List<T>();
        private static GraphicsBuffer Buffer;
        private static int BufferCount;

        protected int bufferID;
        protected int bufferCountID;


        protected virtual void Awake()
        {
            Buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1,  Marshal.SizeOf(typeof(T)));
        }

        private void LateUpdate()
        {
            var count = DataReceive.Count;
            if(count == 0)
            {
                return;
            }

            (DataUse, DataReceive) = (DataReceive, DataUse);
            DataReceive.Clear();

            BufferCount = count;
            Buffer.SetData(DataUse);
            DataReceive.Clear();

            this.material.SetBuffer(this.bufferID, Buffer);
            this.material.SetInt(this.bufferCountID, BufferCount);
        }

        protected virtual void OnDestroy()
        {
            Buffer?.Dispose();
            Buffer = null;
        }
    }
}
