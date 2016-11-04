using UnityEngine;
using System.Collections;

public class EventSystemScript : MonoBehaviour {

    public Texture2D cursorTexture;

	// Use this for initialization
	void Start () {
        //Cursor.SetCursor(cursorTexture, new Vector2(0f, 0f), CursorMode.Auto);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
