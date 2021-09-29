using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{

    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f);
    private static Color normalColor = Color.white;

    private static float moveDuration = 0.05f;

    private static TileController previousSelected = null;

    public int id;

    private BoardManager board;
    private SpriteRenderer render;
    private bool isSelected = false;

    private void Awake()
    {
        board = BoardManager.Instance;
        render = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // Non Selectable conditions
        if (render.sprite == null || board.IsAnimating)
        {
            
            return;
        }
        //Already Selected this tile?
        if (isSelected)
        {
            Deselect();
        }
        //
        else
        {
            if(previousSelected == null)
            {
                Select();
            }
            //try swaping
            else
            {
                //swap tile
                SwapTile(previousSelected);

                // if cant swap than change selected 
                previousSelected.Deselect();
                Select();
              
            }

        }
    }

    public void ChangeId(int id, int x, int y)
    {
        render.sprite = board.tileTypes[id];
        this.id = id;

        name = "TILE_" + id + " (" + x + ", " + y + ")";
    }

    private void Select()
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = this;
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = normalColor;
        previousSelected = null;
    }

    public void SwapTile(TileController otherTile, System .Action onCompleted)
    {
        StartCoroutine(board.SwapTilePosition(this, otherTile, onCompleted));
    }

    public  IEnumerator MoveTilePosition(Vector2 targetPosition, System.Action onComplete)
    {
        Vector2 startPosition = transform.position;
        float duration = moveDuration;
        float time = 0.0f;

        //run animation on next frame for safety reason;
        yield return new WaitForEndOfFrame();

        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPosition;

        onComplete?.Invoke();
    }
}

