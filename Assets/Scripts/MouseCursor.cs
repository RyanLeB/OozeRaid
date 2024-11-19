using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    // ---- Cursor textures ----
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D cursorTextureClick;
   

    private Vector2 cursorHotSpot;

    
    // Start is called before the first frame update
    void Start()
    {
        cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);

    }

    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.SetCursor(cursorTextureClick, cursorHotSpot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
        }
    }


}
