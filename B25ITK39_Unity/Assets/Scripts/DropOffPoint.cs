using UnityEngine;

public class DropOffPoint : MonoBehaviour
{
    public GameObject highlightEffect;
    public GameObject dropOffText;

    private void Start()
    {
        highlightEffect?.SetActive(true);
        dropOffText?.SetActive(true);
    }
}
