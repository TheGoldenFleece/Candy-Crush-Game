using UnityEngine;
public class Tile : MonoBehaviour
{
    static public Tile selected;
    public Vector2Int position;
    SpriteRenderer renderer;
    [SerializeField] Color selectColor;
    Color defaultColor;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        defaultColor = renderer.color;
    }

    void Select()
    {
        renderer.color = selectColor;
    }
    void Unselect()
    {
        renderer.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (GridManager.Instance.IsSwaping) return;
        
        if (selected == null)
        {
            selected = this;
            Select();

            return;
        }

        if (selected == this)
        {
            selected = null;
            Unselect();

            return;
        }

        if (Vector2Int.Distance(position, selected.position) == 1)
        {
            GridManager.Instance.Swap(position, selected.position);

            selected.Unselect();
            selected = null;

            return;
        }
        else
        {
            selected.Unselect();
            Select();
            selected = this;
        }
    }

}
