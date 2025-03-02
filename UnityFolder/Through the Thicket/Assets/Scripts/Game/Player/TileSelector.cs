using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private GameObject hoverHighlight;
    [SerializeField] private GameObject selectedHighlight;
    private GameObject hoveredObject;
    private GameObject selectedObject;
    [SerializeField] LayerMask tileMask;
    [SerializeField] private IWorldState world;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;
    public void Update()
    {
        if (!world.IsActiveAndOpen())
        {
            HideAll();
            return;
        }
        ShowAll();
        UpdateSelectedTile();
    }
    public void LClick()
    {
        if (!world.IsActiveAndOpen())
        {
            return;
        }
        //select the hovered object - even if it is null - this way clicking off of anything will deselect
        selectedObject = hoveredObject;
        //move selected highlight to hover highlight
        selectedHighlight.transform.position = hoverHighlight.transform.position;
        //make the selected highlight show / hide depending on if a tile is hovered
        selectedHighlight.SetActive(hoverHighlight.activeSelf);

        if (hoverHighlight.activeSelf)
        {
            playerController.StartMovingPlayer();
        }
    }
    public void RClick()
    {
        if (!world.IsActiveAndOpen())
        {
            return;
        }
        //select the hovered object - even if it is null - this way clicking off of anything will deselect
        selectedObject = hoveredObject;
        //move selected highlight to hover highlight
        selectedHighlight.transform.position = hoverHighlight.transform.position;
        //make the selected highlight show / hide depending on if a tile is hovered
        selectedHighlight.SetActive(hoverHighlight.activeSelf);
        TileInteractionMenu menu = selectedObject.GetComponent<TileDataHolder>().thisTileData.GetInteractionOptions();
        gameManager.EnterTileInteractionMode(menu, selectedObject);

    }
    public void UpdateSelectedTile()
    {
        if (!playerController.IsTakingInput())
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
        {
            hoveredObject = hit.collider.gameObject.transform.parent.gameObject;
            hoverHighlight.SetActive(true);
            hoverHighlight.transform.position = new Vector3(hoveredObject.transform.position.x, hoveredObject.transform.position.y, hoveredObject.transform.position.z);
        }
        else
        {
            hoveredObject=null;
            hoverHighlight.SetActive(false);
        }
    }
    public Tile GetSelectedTile()
    {
        return new Tile((int)selectedObject.transform.position.x, (int)selectedObject.transform.position.z, (int)selectedObject.transform.position.y, new Grass());
    }
    private void HideAll()
    {
        hoverHighlight.SetActive(false);
        selectedHighlight.SetActive(false);
    }
    private void ShowAll()
    {
        hoverHighlight.SetActive(true);
        selectedHighlight.SetActive(true);
    }
}
