using UnityEngine;
using System.Collections;

public class SpritePrefab2Manager : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject spritePrefab2;
    public float checkDelay = 0.5f;
    private int spritePrefab2Count = 0;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(CheckTagsAfterDelay());
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleRightMouseClick();
        }
    }

    private void HandleRightMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null && hit.collider.CompareTag("SpritePrefab2"))
        {
            Destroy(hit.collider.gameObject);
            spritePrefab2Count--;
            Debug.Log("SpritePrefab2 destroyed. Count: " + spritePrefab2Count);
        }
    }

    private IEnumerator CheckTagsAfterDelay()
    {
        yield return new WaitForSeconds(checkDelay);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null && hit.collider.CompareTag("SpritePrefab2"))
        {
            int spritePrefab2Count = GameObject.FindGameObjectsWithTag("SpritePrefab2").Length;
            Debug.Log("Current SpritePrefab2 Count: " + spritePrefab2Count);
        }
    }

    private void InstantiateSpritePrefab2(Vector3 position)
    {
        Instantiate(spritePrefab2, position, Quaternion.identity);
        spritePrefab2Count++;
        Debug.Log("SpritePrefab2 instantiated. Count: " + spritePrefab2Count);
    }
}
