using UnityEngine;

public class BookSpawner : MonoBehaviour {
    public GameObject[] stacks;
    public GameObject[] open;
    public GameObject[] single;

    private void Awake() {
        int type = Random.Range(0, 100);
        if (10 < type && type < 40) {
            // Open Book
            int bookIndex = Random.Range(0, open.Length);
            Instantiate(open[bookIndex], transform);
        } else if (type < 80) {
            // Stack of books
            int bookIndex = Random.Range(0, stacks.Length);
            GameObject stack = Instantiate(stacks[bookIndex], transform);
        } else {
            // Single book
            int height = Random.Range(0, 4);
            float heightOffset = 0;
            for (int i = 0; i < height; i++) {
                int bookIndex = Random.Range(0, single.Length);
                GameObject stack = Instantiate(single[bookIndex], transform);
                BoxCollider collider = stack.GetComponent<BoxCollider>();
                stack.transform.localPosition = new Vector3(0, heightOffset, 0);
                heightOffset += collider.size.z;
                stack.transform.localEulerAngles = new Vector3(-90, Random.Range(0, 360), 0);
            }
        }
    }
}