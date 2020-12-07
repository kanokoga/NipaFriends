using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common.Debugs;

////////////////////////////////////////////////////////////////////////////////
///<summary>
/// [ROLE] : ギズモやラベルを表示する
/// [note] : -
///</summary> 
public class NipaVisualDebugger : GuiDebuggerMonobehaviour
{

    [SerializeField]
    GUISkin gui;

    public float labelWidth = 500f;

    static Dictionary<string, GizmoData> gizmos_id;
    static List<GizmoData_Timer> gizmos_timer;
    static List<GizmoData_Follow> gizmos_follow;

    static List<LabelData> labels;
    static Dictionary<string, LabelData> labels_id;

    static List<LabelDataFollowTrans> followLabels;
    static Dictionary<string, LabelDataFollowTrans> followLabels_id;

    static List<LineData> lines;
    static Dictionary<string, LineData> lines_id;
    static Dictionary<string, LineArtData> lineArts_id;
    static Dictionary<string, LineData_Follow> lines_follow;

    public bool displayLabel = true;
    public bool displayGizmo = true;

    private void Awake()
    {
        gizmos_id = new Dictionary<string, GizmoData>();
        gizmos_timer = new List<GizmoData_Timer>();
        gizmos_follow = new List<GizmoData_Follow>();

        labels = new List<LabelData>();
        labels_id = new Dictionary<string, LabelData>();

        followLabels = new List<LabelDataFollowTrans>();
        followLabels_id = new Dictionary<string, LabelDataFollowTrans>();

        lines = new List<LineData>();
        lines_id = new Dictionary<string, LineData>();
        lineArts_id = new Dictionary<string, LineArtData>();
        lines_follow = new Dictionary<string, LineData_Follow>();
    }

    private void Start()
    {
        this.Register(DebugMenuCategories.GENERAL, "Label and Gizmos");
    }

    private void Update()
    {

        for (int i = lines.Count - 1; i > -1; i--)
        {
            if (lines[i].endTime < Time.time)
                lines.RemoveAt(i);
        }

        for (int i = gizmos_timer.Count - 1; i > -1; i--)
        {
            if (gizmos_timer[i].endTime < Time.time)
                gizmos_timer.RemoveAt(i);
        }

        for (int i = followLabels.Count - 1; i > -1; i--)
        {
            if (followLabels[i].target == null)
                followLabels.RemoveAt(i);
        }


        for (int i = gizmos_follow.Count - 1; i > -1; i--)
        {
            if (gizmos_follow[i].target == null)
                gizmos_follow.RemoveAt(i);
        }



        var removeIds = new List<string>();

        foreach (var item in followLabels_id)
        {
            if (item.Value.target == null)
                removeIds.Add(item.Key);
        }
        foreach (var item in removeIds)
        {
            followLabels_id.Remove(item);
        }

        removeIds.Clear();
        foreach (var item in lines_follow)
        {
            if (item.Value.start == null)
                removeIds.Add(item.Key);
        }
        foreach (var item in removeIds)
        {
            lines_follow.Remove(item);
        }
    }

    #region ===============================================  Gizmos / Follow Gizmos


    public static void AddGizmo_Timer(ModelType m, Vector3 pos, float size, Color col, float duration = Mathf.Infinity)
    {

#if UNITY_EDITOR
        var g = new GizmoData_Timer();
        g.m = m;
        g.pos = pos;
        g.size = size;
        g.col = col;
        g.endTime = Time.time + duration;
        gizmos_timer.Add(g);
#endif
    }

    public static void AddGizmo_id(string id, ModelType m, Vector3 pos, float size, Color col)
    {

#if UNITY_EDITOR
        var g = new GizmoData();
        g.m = m;
        g.pos = pos;
        g.size = size;
        g.col = col;

        if (gizmos_id.ContainsKey(id))
            gizmos_id[id] = g;
        else
            gizmos_id.Add(id, g);
#endif
    }

    public static void AddGizmo_Follow(ModelType m, Transform target, float size, Color col)
    {

#if UNITY_EDITOR
        var g = new GizmoData_Follow();
        g.m = m;
        g.target = target;
        g.size = size;
        g.col = col;
        gizmos_follow.Add(g);
#endif
    }

    public static void RemoveGizmoWithId(string id)
    {
        if (gizmos_id.ContainsKey(id))
            gizmos_id.Remove(id);
    }

    public static void ClearAllGizmos()
    {
        gizmos_id.Clear();
        gizmos_timer.Clear();
        gizmos_follow.Clear();
    }



    #endregion
    #region ===============================================  Label

    public static void AddLabel_id(string id, string content, Vector2 posOnScreen, Color c)
    {

#if UNITY_EDITOR
        var l = new LabelData();
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = posOnScreen;
        l.isV2 = true;
        if (labels_id.ContainsKey(id))
            labels_id[id] = l;
        else
            labels_id.Add(id, l);
#endif
    }

    public static void AddLabel_id(string id, string content, Vector3 pos, Color c, Vector2 deltaPosPnScreen)
    {

#if UNITY_EDITOR
        var l = new LabelData();
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = deltaPosPnScreen;
        l.pos = pos;

        if (labels_id.ContainsKey(id))
            labels_id[id] = l;
        else
            labels_id.Add(id, l);
#endif
    }

    public static void AddLabel_id(string id, string content, Vector3 pos, Color c)
    {

#if UNITY_EDITOR
        var l = new LabelData();
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = Vector2.zero;
        l.pos = pos;

        if (labels_id.ContainsKey(id))
            labels_id[id] = l;
        else
            labels_id.Add(id, l);
#endif
    }

    public static void AddLabel(string content, Vector3 pos, Color c, Vector2 deltaPosPnScreen)
    {

#if UNITY_EDITOR
        var l = new LabelData();
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = deltaPosPnScreen;
        l.pos = pos;
        labels.Add(l);
#endif
    }

    public static void AddLabel(string content, Vector3 pos, Color c)
    {

#if UNITY_EDITOR
        var l = new LabelData();
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = Vector2.zero;
        l.pos = pos;
        labels.Add(l);
#endif
    }
    public static void RemoveLabelWithId(string id)
    {
        if (labels_id.ContainsKey(id))
            labels_id.Remove(id);
    }

    public static void ClearAllLabels()
    {
        labels.Clear();
        labels_id.Clear();
    }

    public static void ClearLabelsWithNoId()
    {
        labels.Clear();
    }

    #endregion
    #region ===============================================  Follow Label

    public static void AddFollowLabel(Transform followTarget, string content, Color c, Vector3 deltaPos)
    {

#if UNITY_EDITOR
        var l = new LabelDataFollowTrans();
        l.target = followTarget;
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = Vector2.zero;
        l.deltaPos = deltaPos;
        followLabels.Add(l);
#endif
    }

    public static void AddFollowLabel(Transform followTarget, string content, Color c, Vector3 deltaPos, Vector2 deltaPosPnScreen)
    {

#if UNITY_EDITOR
        var l = new LabelDataFollowTrans();
        l.target = followTarget;
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = deltaPosPnScreen;
        l.deltaPos = deltaPos;
        followLabels.Add(l);
#endif
    }

    public static void AddFollowLabel(Transform followTarget, string content, Color c)
    {

#if UNITY_EDITOR
        var l = new LabelDataFollowTrans();
        l.target = followTarget;
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = Vector2.zero;
        l.deltaPos = Vector3.zero;
        followLabels.Add(l);
#endif
    }

    public static void AddFollowLabel_id(string id, Transform followTarget, string content, Color c, Vector3 deltaPos)
    {

#if UNITY_EDITOR
        var l = new LabelDataFollowTrans();
        l.target = followTarget;
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = Vector2.zero;
        l.deltaPos = deltaPos;

        if (followLabels_id.ContainsKey(id))
            followLabels_id[id] = l;
        else
            followLabels_id.Add(id, l);
#endif
    }

    public static void AddFollowLabel_id(string id, Transform followTarget, string content, Color c, Vector3 deltaPos, Vector2 deltaPosPnScreen)
    {

#if UNITY_EDITOR
        var l = new LabelDataFollowTrans();
        l.target = followTarget;
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = deltaPosPnScreen;
        l.deltaPos = deltaPos;

        if (followLabels_id.ContainsKey(id))
            followLabels_id[id] = l;
        else
            followLabels_id.Add(id, l);
#endif
    }

    public static void AddFollowLabel_id(string id, Transform followTarget, string content, Color c)
    {

#if UNITY_EDITOR
        var l = new LabelDataFollowTrans();
        l.target = followTarget;
        l.content = content;
        l.color = c;
        l.deltaPosOnScreen = Vector2.zero;
        l.deltaPos = Vector3.zero;

        if (followLabels_id.ContainsKey(id))
            followLabels_id[id] = l;
        else
            followLabels_id.Add(id, l);
#endif
    }

    public static void RemoveFollowLabelWithId(string id)
    {
        if (followLabels_id.ContainsKey(id))
            followLabels_id.Remove(id);
    }

    public static void ClearAllFollowLabels()
    {
        followLabels_id.Clear();
        followLabels.Clear();
    }

    public static void ClearFollowLabelsWithNoId()
    {
        followLabels.Clear();
    }

    #endregion
    #region ===============================================  Line

    public static void AddLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
    {

#if UNITY_EDITOR
        var line = new LineData();
        line.start = start;
        line.end = end;
        line.color = color;
        line.endTime = Time.time + duration;
        lines.Add(line);
#endif
    }

    public static void AddLine_id(string id, Vector3 start, Vector3 end, Color color)
    {

#if UNITY_EDITOR
        var line = new LineData();
        line.start = start;
        line.end = end;
        line.color = color;
        line.endTime = 0f;

        if (lines_id.ContainsKey(id))
            lines_id[id] = line;
        else
            lines_id.Add(id, line);
#endif
    }

    public static void AddLine_id(string id, Transform start, Vector3 end, Color color)
    {

#if UNITY_EDITOR
        var line = new LineData_Follow();
        line.start = start;
        line.end = end;
        line.color = color;
        line.endTime = 0f;

        if (lines_id.ContainsKey(id))
            lines_follow[id] = line;
        else
            lines_follow.Add(id, line);
#endif
    }

    public static void AddBallArrow(Vector3 start, Vector3 end, float headSize, Color color, float duration = Mathf.Infinity)
    {

#if UNITY_EDITOR
        AddLine(start, end, color, duration);

        AddGizmo_Timer(ModelType.Sphere, end, headSize, color, duration);
#endif
    }

    public static void AddBallArrow(Vector3 start, Vector3 direction, float length, float headSize, Color color, float duration = Mathf.Infinity)
    {

#if UNITY_EDITOR
        var end = start + direction.normalized * length;
        AddLine(start, end, color, duration);

        AddGizmo_Timer(ModelType.Sphere, end, headSize, color, duration);
#endif
    }

    public static void AddLineArt(Vector3[] waypoints, Color colorStart, Color colorEnd, float waypointSymbolSize, float duration = Mathf.Infinity)
    {

#if UNITY_EDITOR
        bool symbl = waypointSymbolSize > 0f;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            var col = Color.Lerp(colorStart, colorEnd, i / (float)waypoints.Length);
            AddLine(waypoints[i], waypoints[i + 1], col, duration);

            if (symbl)
                AddGizmo_Timer(ModelType.Sphere, waypoints[i], waypointSymbolSize, col);
        }
#endif
    }

    public static void AddBallArrow_id(string id, Vector3 start, Vector3 end, float headSize, Color color)
    {

#if UNITY_EDITOR
        AddLine_id(id, start, end, color);
        AddGizmo_id(id, ModelType.Sphere, end, headSize, color);
#endif
    }

    public static void AddBallArrow_id(string id, Vector3 start, Vector3 direction, float length, float headSize, Color color)
    {

#if UNITY_EDITOR
        var end = start + direction.normalized * length;
        AddBallArrow_id(id, start, end, headSize, color);
#endif
    }

    public static void AddLineArt_id(string id, Vector3[] waypoints, Color colorStart, Color colorEnd, float waypointSymbolSize)
    {

#if UNITY_EDITOR
        bool symbl = waypointSymbolSize > 0f;
        var lineArt = new LineArtData();
        lineArt.points = new Vector3[waypoints.Length];
        lineArt.color = new Color[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            var col = Color.Lerp(colorStart, colorEnd, i / (float)waypoints.Length);
            lineArt.points[i] = waypoints[i];
            lineArt.color[i] = col;

            if (symbl)
                AddGizmo_Timer(ModelType.Sphere, waypoints[i], waypointSymbolSize, col);
        }

        if (lineArts_id.ContainsKey(id))
            lineArts_id[id] = lineArt;
        else
            lineArts_id.Add(id, lineArt);
#endif
    }

    public static void ClearAllLines()
    {
        lines.Clear();
        lines_id.Clear();
        lineArts_id.Clear();
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (!displayGizmo)
            return;

        if (lines != null)
        {
            foreach (var line in lines)
            {
                Gizmos.color = line.color;
                Gizmos.DrawLine(line.start, line.end);
            }
        }

        if (lines_id != null)
        {
            foreach (var line in lines_id)
            {
                Gizmos.color = line.Value.color;
                Gizmos.DrawLine(line.Value.start, line.Value.end);
            }
        }

        if (lineArts_id != null)
        {
            foreach (var la in lineArts_id)
            {
                var lineArt = la.Value;
                for (int i = 0; i < lineArt.points.Length - 1; i++)
                {
                    Gizmos.color = lineArt.color[i];
                    Gizmos.DrawLine(lineArt.points[i], lineArt.points[i + 1]);
                }
            }
        }

        if (gizmos_id != null)
            foreach (var g in gizmos_id)
            {
                Gizmos.color = g.Value.col;
                switch (g.Value.m)
                {
                    case ModelType.Sphere:
                        Gizmos.DrawSphere(g.Value.pos, g.Value.size);
                        break;
                    case ModelType.Cube:
                        Gizmos.DrawCube(g.Value.pos, Vector3.one * g.Value.size);
                        break;
                    case ModelType.WireSphere:
                        Gizmos.DrawWireSphere(g.Value.pos, g.Value.size);
                        break;
                    case ModelType.WireCube:
                        Gizmos.DrawWireCube(g.Value.pos, Vector3.one * g.Value.size);
                        break;
                    default:
                        break;
                }
            }

        if (gizmos_follow != null)
            foreach (var g in gizmos_follow)
            {
                Gizmos.color = g.col;
                switch (g.m)
                {
                    case ModelType.Sphere:
                        Gizmos.DrawSphere(g.target.position, g.size);
                        break;
                    case ModelType.Cube:
                        Gizmos.DrawCube(g.target.position, Vector3.one * g.size);
                        break;
                    case ModelType.WireSphere:
                        Gizmos.DrawWireSphere(g.target.position, g.size);
                        break;
                    case ModelType.WireCube:
                        Gizmos.DrawWireCube(g.target.position, Vector3.one * g.size);
                        break;
                    default:
                        break;
                }
            }

        if (gizmos_timer != null)
            foreach (var g in gizmos_timer)
            {
                Gizmos.color = g.col;
                switch (g.m)
                {
                    case ModelType.Sphere:
                        Gizmos.DrawSphere(g.pos, g.size);
                        break;
                    case ModelType.Cube:
                        Gizmos.DrawCube(g.pos, Vector3.one * g.size);
                        break;
                    case ModelType.WireSphere:
                        Gizmos.DrawWireSphere(g.pos, g.size);
                        break;
                    case ModelType.WireCube:
                        Gizmos.DrawWireCube(g.pos, Vector3.one * g.size);
                        break;
                    default:
                        break;
                }
            }
    }

    private void OnGUI()
    {
        GUI.skin = gui;
        if (!enabled || !displayLabel)
            return;

        var sh = Screen.height;

        if (labels != null)
        {

            foreach (var L in labels)
            {
                GUI.color = L.color;
                var screenPos = Camera.main.WorldToScreenPoint(L.pos);
                GUI.Label(new Rect(screenPos.x + L.deltaPosOnScreen.x, sh - screenPos.y + L.deltaPosOnScreen.y, labelWidth, 80), L.content);
            }
        }

        if (labels_id != null)
        {
            foreach (var l in labels_id)
            {
                var L = l.Value;
                GUI.color = L.color;

                if (!L.isV2)
                {
                    var screenPos = Camera.main.WorldToScreenPoint(L.pos);
                    GUI.Label(new Rect(screenPos.x + L.deltaPosOnScreen.x, sh - screenPos.y + L.deltaPosOnScreen.y, labelWidth, 80), L.content);
                }
                else
                {
                    GUI.Label(new Rect(L.deltaPosOnScreen.x, sh - L.deltaPosOnScreen.y, labelWidth, 80), L.content);
                }
            }
        }


        if (followLabels != null)
        {
            foreach (var L in followLabels)
            {
                GUI.color = L.color;
                var screenPos = Camera.main.WorldToScreenPoint(L.target.position + L.deltaPos);
                GUI.Label(new Rect(screenPos.x + L.deltaPosOnScreen.x, sh - screenPos.y + L.deltaPosOnScreen.y, labelWidth, 80), L.content);
            }
        }

        if (followLabels_id != null)
        {
            foreach (var l in followLabels_id)
            {
                var L = l.Value;
                GUI.color = L.color;
                var screenPos = Camera.main.WorldToScreenPoint(L.target.position + L.deltaPos);
                GUI.Label(new Rect(screenPos.x + L.deltaPosOnScreen.x, sh - screenPos.y + L.deltaPosOnScreen.y, labelWidth, 80), L.content);
            }
        }

    }


    protected override void GuiContent()
    {
        this.displayLabel = GUILayout.Toggle(this.displayLabel, "Show Labels");
        this.displayGizmo = GUILayout.Toggle(this.displayGizmo, "Show Gizmos");
    }

    private void OnApplicationQuit()
    {
        ClearAllGizmos();
        ClearAllLabels();
        ClearAllFollowLabels();
        ClearAllLines();
    }

    struct GizmoData
    {
        public ModelType m;
        public Vector3 pos;
        public float size;
        public Color col;
    }
    struct GizmoData_Timer
    {
        public ModelType m;
        public Vector3 pos;
        public float size;
        public Color col;
        public float endTime;
    }
    struct GizmoData_Follow
    {
        public ModelType m;
        public Transform target;
        public float size;
        public Color col;
    }

    struct LabelData
    {
        public string content;
        public Vector3 pos;
        public Vector2 deltaPosOnScreen;
        public Color color;
        public bool isV2;
    }

    struct LabelDataFollowTrans
    {
        public string content;
        public Transform target;
        public Vector3 deltaPos;
        public Vector2 deltaPosOnScreen;
        public Color color;
    }

    struct LineData
    {
        public Vector3 start;
        public Vector3 end;
        public Color color;
        public float endTime;
    }
    struct LineData_Follow
    {
        public Transform start;
        public Vector3 end;
        public Color color;
        public float endTime;
    }
    struct LineArtData
    {
        public Vector3[] points;
        public Color[] color;
    }


    public enum ModelType
    {
        Sphere,
        Cube,
        WireSphere,
        WireCube
    }
}
