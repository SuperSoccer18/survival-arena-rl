using UnityEngine;

[ExecuteAlways]
public class FlowFieldDebug : MonoBehaviour
{
    [SerializeField] private FlowFieldNavigator navigator;
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private bool drawFlow = true;
    [SerializeField] private float arrowScale = 0.35f;

    private void OnDrawGizmos()
    {
        if (navigator == null) return;

        var ty = navigator.GetType();

        // Try to reflect grid info
        var worldOriginField = ty.GetField("worldOrigin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var gridWidthField = ty.GetField("gridWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var gridHeightField = ty.GetField("gridHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var cellSizeField = ty.GetField("cellSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        if (worldOriginField == null || gridWidthField == null || gridHeightField == null || cellSizeField == null) return;

        Vector2 origin = (Vector2)worldOriginField.GetValue(navigator);
        int width = (int)gridWidthField.GetValue(navigator);
        int height = (int)gridHeightField.GetValue(navigator);
        float size = (float)cellSizeField.GetValue(navigator);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 center2 = origin + new Vector2((x + 0.5f) * size, (y + 0.5f) * size);
                Vector3 center3 = new Vector3(center2.x, center2.y, 0f);
                Gizmos.color = Color.gray;
                if (drawGrid) Gizmos.DrawWireCube(center3, Vector3.one * size * 0.98f);

                if (drawFlow)
                {
                    Vector2 f = navigator.FlowAtCell(x, y);
                    if (f != Vector2.zero)
                    {
                        Gizmos.color = Color.cyan;
                        Vector3 to = center3 + new Vector3(f.x, f.y, 0f) * size * arrowScale;
                        Gizmos.DrawLine(center3, to);
                    }
                }
            }
        }
    }
}
