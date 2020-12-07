using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

////////////////////////////////////////////////////////////////////////////////
///<summary>
/// [ROLE] : ギズモやラベルを表示する
/// [note] : -
///</summary> 
public class NipaVisualDebuggerAcnycHelper
{

    static List<GizmoData> gizmos;
    static List<LabelData> labels;
    static List<LabelData> solidlabels;
    static List<LabelDataFollowTrans> followLabels;
    static List<string> gizmosToRemove;
    static List<ArrowData> arrows = new List<ArrowData>();
    static List<LineArt> linearts = new List<LineArt>();
    static List<LineData> lines = new List<LineData>();
    static List<LineData_Timer> lines_timer = new List<LineData_Timer>();
    private static object requestsLock = new object();

    public NipaVisualDebuggerAcnycHelper()
    {
        gizmos = new List<GizmoData>();
        labels = new List<LabelData>();
        solidlabels = new List<LabelData>();
        followLabels = new List<LabelDataFollowTrans>();
        gizmosToRemove = new List<string>();
        MainSystem.Main.OnUnityUpdate.AddListener(SendRequestsToDebugger);
    }

    void SendRequestsToDebugger()
    {

#if UNITY_EDITOR
        lock (requestsLock)
        {
            foreach (var item in gizmos)
            {
                if (item.id != "")
                    NipaVisualDebugger.AddGizmo_id(item.id, item.m, item.pos, item.size, item.col);
                else
                    NipaVisualDebugger.AddGizmo_Timer(item.m, item.pos, item.size, item.col, item.duration);
            }
            foreach (var item in labels)
            {
                if (item.id != "")
                    NipaVisualDebugger.AddLabel_id(item.id, item.content, item.pos, item.color, item.deltaPosOnScreen);
                else
                    NipaVisualDebugger.AddLabel(item.content, item.pos, item.color, item.deltaPosOnScreen);
            }
            foreach (var item in followLabels)
            {
                //if (item.id != "")
                //    NipaVisualDebugger.AddFollowLabel_id(item.id, item.target, item.content, item.color, item.deltaPos, item.deltaPosOnScreen);
                //else
                //    NipaVisualDebugger.AddFollowLabel(item.target, item.content, item.color, item.deltaPos, item.deltaPosOnScreen);

                NipaVisualDebugger.AddLabel_id(item.id, item.content, item.deltaPos, item.color);
            }
            foreach (var item in solidlabels)
            {
                if (item.id != "")
                    NipaVisualDebugger.AddLabel_id(item.id, item.content, item.deltaPosOnScreen, item.color);
                //else
                //    NipaVisualDebugger.AddLabel(item.content, item.pos, item.color, item.deltaPosOnScreen);
            }
            foreach (var item in gizmosToRemove)
            {
                NipaVisualDebugger.RemoveGizmoWithId(item);
            }
            foreach (var item in arrows)
            {
                NipaVisualDebugger.AddBallArrow_id(item.id, item.start, item.dir, item.lengh, item.head, item.color);
            }
            foreach (var item in linearts)
            {
                NipaVisualDebugger.AddLineArt_id(item.id, item.points, item.start, item.end, 0f);
            }
            foreach (var item in lines)
            {
                NipaVisualDebugger.AddLine_id(item.id, item.start, item.end, item.color);
            }
            foreach (var item in lines_timer)
            {
                NipaVisualDebugger.AddLine(item.start, item.end, item.color, item.duration);
            }
            arrows.Clear();
            linearts.Clear();
            lines.Clear();
            ClearAllGizmos();
            ClearAllLabels();
            ClearAllFollowLabels();
            solidlabels.Clear();
            gizmosToRemove.Clear();
            lines_timer.Clear();
        }
#endif
    }

    #region ===============================================  Gizmos


    public static void AddGizmo_id(string id, NipaVisualDebugger.ModelType m, Vector3 pos, float size, Color col)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            var g = new GizmoData();
            g.m = m;
            g.pos = pos;
            g.size = size;
            g.col = col;
            g.id = id;
            gizmos.Add(g);
            Monitor.Exit(requestsLock);
        }
#endif
    }

    public static void AddGizmo_timer(NipaVisualDebugger.ModelType m, Vector3 pos, float size, Color col, float duration = Mathf.Infinity)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            var g = new GizmoData();
            g.m = m;
            g.pos = pos;
            g.size = size;
            g.col = col;
            g.id = "";
            g.duration = duration;
            gizmos.Add(g);
            Monitor.Exit(requestsLock);
        }
#endif
    }


    public static void RemoveGizmoWithId(string id)
    {
        gizmosToRemove.Add(id);
    }

    public static void ClearAllGizmos()
    {
        gizmos.Clear();
    }


    #endregion
    #region ===============================================  Label


    public static void AddLabelWithId(string id, string content, Vector3 pos, Color c, Vector2 deltaPosPnScreen)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            var l = new LabelData();
            l.content = content;
            l.color = c;
            l.deltaPosOnScreen = deltaPosPnScreen;
            l.pos = pos;
            l.id = id;
            labels.Add(l);
            Monitor.Exit(requestsLock);
        }
#endif
    }

    public static void AddSolidLabelWithId(string id, string content, Vector2 pos, Color c)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            var l = new LabelData();
            l.content = content;
            l.color = c;
            l.deltaPosOnScreen = pos;
            l.id = id;
            solidlabels.Add(l);
            Monitor.Exit(requestsLock);
        }
#endif
    }

    public static void RemoveLabelWithId(string id)
    {

    }

    public static void ClearAllLabels()
    {
        labels.Clear();
    }


    #endregion
    #region ===============================================  Follow Label



    public static void AddFollowLabelWithId(string id, Vector3 pos, string content, Color c, Vector2 deltaPosPnScreen)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            var l = new LabelDataFollowTrans();
           // l.target = followTarget;
            l.content = content;
            l.color = c;
            l.deltaPosOnScreen = deltaPosPnScreen;
            l.deltaPos = pos;
            l.id = id;
            followLabels.Add(l);
            Monitor.Exit(requestsLock);
        }
#endif
    }


    public static void RemoveFollowLabelWithId(string id)
    {

    }

    public static void ClearAllFollowLabels()
    {
        followLabels.Clear();
    }


    #endregion
    #region ===============================================  Line

    public static void AddBallArrow_id(string id, Vector3 start, Vector3 dir, float len, float headSize, Color color)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            arrows.Add(new ArrowData() { id = id, start = start, dir = dir, lengh = len, head = headSize, color = color });
            Monitor.Exit(requestsLock);
        }
#endif
    }

    public static void AddLineArt_id(string id, Vector3[] waypoints, Color colorStart, Color colorEnd, float waypointSymbolSize)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            linearts.Add(new LineArt() { id = id, points = waypoints, start = colorStart, end = colorEnd });
            Monitor.Exit(requestsLock);
        }
#endif
    }

    public static void AddLine_id(string id, Vector3 start, Vector3 end, Color color)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            lines.Add(new LineData() { id = id, start = start, end = end, color = color });
            Monitor.Exit(requestsLock);
        }
#endif
    }
    public static void AddLine_Timer(string id, Vector3 start, Vector3 end, Color color, float duration)
    {

#if UNITY_EDITOR
        if (Monitor.TryEnter(requestsLock))
        {
            lines_timer.Add(new LineData_Timer() { id = id, start = start, end = end, color = color, duration = duration });
            Monitor.Exit(requestsLock);
        }
#endif
    }

    #endregion

    struct GizmoData
    {
        public string id;
        public NipaVisualDebugger.ModelType m;
        public Vector3 pos;
        public float size;
        public Color col;
        public float duration;
    }

    struct LabelData
    {
        public string id;
        public string content;
        public Vector3 pos;
        public Vector2 deltaPosOnScreen;
        public Color color;
    }

    struct LabelDataFollowTrans
    {
        public string id;
        public string content;
        public Transform target;
        public Vector3 deltaPos;
        public Vector2 deltaPosOnScreen;
        public Color color;
    }

    struct ArrowData
    {
        public string id;
        public Vector3 start;
        public Vector3 dir;
        public Color color;
        public float lengh;
        public float head;
    }

    struct LineData
    {
        public string id;
        public Vector3 start;
        public Vector3 end;
        public Color color;
    }
    struct LineData_Timer
    {
        public string id;
        public Vector3 start;
        public Vector3 end;
        public Color color;
        public float duration;
    }
    struct LineArt
    {
        public Vector3[] points;
        public Color start;
        public Color end;
        public string id;
    }

}
