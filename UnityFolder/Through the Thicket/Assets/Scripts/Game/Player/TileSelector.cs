using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private GameObject hoverHighlight;
    [SerializeField] private GameObject selectedHighlight;
    private GameObject hoveredObject;
    private GameObject selectedObject;
    [SerializeField] LayerMask tileMask;
    public void Update()
    {
        UpdateSelectedTile();
    }
    public void Click()
    {
        //select the hovered object - even if it is null - this way clicking off of anything will deselect
        selectedObject = hoveredObject;
        //move selected highlight to hover highlight
        selectedHighlight.transform.position = hoverHighlight.transform.position;
        //make the selected highlight show / hide depending on if a tile is hovered
        selectedHighlight.SetActive(hoverHighlight.activeSelf);
    }
    public void UpdateSelectedTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
        {
            hoveredObject = hit.collider.gameObject;
            hoverHighlight.transform.position = new Vector3(hoveredObject.transform.position.x, 1.5f, hoveredObject.transform.position.z);
        }
        else
        {
            hoveredObject=null;
            hoverHighlight.SetActive(false);
        }
    }
    public Tile GetSelectedTile()
    {
        return new Tile((int)selectedObject.transform.position.x, (int)selectedObject.transform.position.z, (int)selectedObject.transform.position.y, TileTypes.Grass);
    }
}
