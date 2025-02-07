using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIRenderer : Graphic
{
    public UIHolder UIHolder; // Reference to the inventory system
    public Color availableColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Empty slots
    public Color occupiedColor = new Color(0.8f, 0.1f, 0.1f, 1f); // Items placed
    public float cellSize = 30f; // Size of each grid cell
    public float padding = 2f; // Spacing between cells

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (UIHolder == null) return;
        foreach(Inventory inventory in UIHolder.inventories)
        {
            Vector2 startPos = inventory.GetLocation();
            InventorySlot[,] slots = inventory.GetSlots();
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    if (!slots[x, y].IsValid())
                    {
                        continue;
                    }
                    bool isAvailable = slots[x,y].IsEmpty();
                    DrawCell(vh, startPos, x, y, isAvailable ? availableColor : occupiedColor);
                }
            }
        }
    }

    private void DrawCell(VertexHelper vh, Vector2 startPos, int x, int y, Color color)
    {
        float xPos = startPos.x + x * (cellSize + padding);
        float yPos = startPos.y + y * (cellSize + padding);

        Vector3 v0 = new Vector3(xPos, yPos);
        Vector3 v1 = new Vector3(xPos, yPos + cellSize);
        Vector3 v2 = new Vector3(xPos + cellSize, yPos + cellSize);
        Vector3 v3 = new Vector3(xPos + cellSize, yPos);

        int startIndex = vh.currentVertCount;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = v0; vh.AddVert(vert);
        vert.position = v1; vh.AddVert(vert);
        vert.position = v2; vh.AddVert(vert);
        vert.position = v3; vh.AddVert(vert);

        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }

    public void Refresh()
    {
        SetVerticesDirty(); // Forces Unity to re-render the UI
    }

    public void ApplyUIHolder(UIHolder holder)
    {
        UIHolder = holder;
    }
}
