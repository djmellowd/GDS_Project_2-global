using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing3 : MonoBehaviour
{
    [SerializeField] private TilemapVisual tilemapVisual;
    private Tilemap tilemap;
    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    private void Start()
    {
        tilemap = new Tilemap(20, 10, 10f, Vector3.zero);

        tilemap.SetTilemapVisual(tilemapVisual);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            tilemap.SetTilemapSprite(mouseWorldPosition, tilemapSprite);
        }

        if(Input.GetKeyDown(T))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.None;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(Y))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Ground;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(U))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Path;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(I))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Dirt;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
    }
}
