using DG.Tweening;
using TMPro.EditorUtilities;
using UnityEngine;

public enum DoorDirection { PosX, NegX, PosZ, NegZ }

public class Doorway : MonoBehaviour
{
    public DoorDirection direction;
    public bool isConnected = false;
    private GameObject FOW;
    private SpriteRenderer spriteRenderer;

    public Vector2 Offset()
    {
        return new Vector2(transform.position.x / 10, transform.position.z / 10);
    }

    private void Start()
    {
        FOW = GetComponentInParent<RoomProperties>().gameObject.transform.Find("FOW")?.gameObject;
        if (FOW != null)
        {
            spriteRenderer = FOW.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0, 0, 0, 1);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (spriteRenderer != null && !spriteRenderer.color.Equals(new Color(0, 0, 0, 0)))
        {
            spriteRenderer.DOColor(new Color(0, 0, 0, 0), 0.1f);
        }
        GetComponentInParent<RoomProperties>().onDoorEntered(collider.gameObject);
    }
}
