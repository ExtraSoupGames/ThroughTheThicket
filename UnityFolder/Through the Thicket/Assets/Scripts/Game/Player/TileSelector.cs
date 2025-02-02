using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject;
    private GameObject hoveredObject;
    [SerializeField] LayerMask tileMask;
    public void Update()
    {
        UpdateSelectedTile();
    }
    public void UpdateSelectedTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
        {
            hoveredObject = hit.collider.gameObject;
            highlightObject.transform.position = new Vector3(hoveredObject.transform.position.x, 1.5f, hoveredObject.transform.position.z);
        }
    }
    public Tile GetSelectedTile()
    {
        return new Tile((int)hoveredObject.transform.position.x, (int)hoveredObject.transform.position.z, (int)hoveredObject.transform.position.y, TileTypes.Grass);
    }
}
