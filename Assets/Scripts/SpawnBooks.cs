
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SpawnBooks : MonoBehaviour {
    public GameObject[] shelves;
    public GameObject[] books;
    public float[] widths;
    public float[] heights;
    private void Awake() {
        // Init the array
        widths = new float[books.Length];
        heights = new float[books.Length];
        
        // Get the normalized width for each book
        for (int i = 0; i < books.Length; i++) {
            GameObject prefab = books[i];
            MeshRenderer renderer = prefab.GetComponent<MeshRenderer>();
            widths[i] = renderer.bounds.size.z;
            heights[i] = renderer.bounds.size.y;
        }
        
        // Populate each shelf
        foreach (GameObject shelf in shelves) {
            SpawnBookRow(shelf);
        }
    }

    private void SpawnBookRow(GameObject shelf) {
        // Calculate Shelf Properties
        BoxCollider shelfBounds = shelf.GetComponent<BoxCollider>();
        Vector3 shelfSize = shelfBounds.size;
        Vector3 shelfPos = shelfBounds.center;
        shelfPos.x += shelfSize.x * 0.5f;
        shelfPos.y -= shelfSize.y * 0.5f;
        shelfPos.z -= shelfSize.z * 0.5f;
        
        // Calculate the average book width for this shelf
        float average = 0;
        foreach (float width in widths) {
            average += width;
        }

        average /= widths.Length;

        float offset = Random.Range(0f, average);
        int count = 0;
        while (offset < shelfSize.z) {
            int bookIndex = Random.Range(0, books.Length);
            float bookScale = shelfSize.y / heights[bookIndex] * Random.Range(0.7f, 1f);
            float bookWidth = widths[bookIndex] * shelf.transform.lossyScale.z * bookScale;
            if (offset + bookWidth < shelfSize.z) {
                GameObject book = Instantiate(books[bookIndex]);
                Vector3 position = shelfPos;
                position.z += offset;
                position.x -= Random.Range(0f, 0.1f);
                book.transform.localPosition = position;
                book.transform.localScale = new Vector3(bookScale, bookScale, bookScale);
                book.transform.SetParent(shelf.transform, false);
                offset += bookWidth * Random.Range(1f, 1.3f);
            }

            count++;
            
            if (offset < average || count > 100)
                break;
        }

        
        // Create books
        /*float allocatedSize = 0f;
        int index = 0;
        while (true) {
            index++;
            if (index > 5000)
                break;
            
            int prefabIndex = Random.Range(0, books.Length);
            float scale = 1; // shelfSize.y / heights[prefabIndex];
            float bookWidth = widths[prefabIndex] * scale;
            if (allocatedSize + bookWidth > shelfSize.z)
                break;
            
            if (true) {// (Random.Range(0f, 1f) > 0.05f && allocatedSize < shelfSize.x * 0.95f) {
                GameObject book = Instantiate(books[Random.Range(0, books.Length)], shelf.transform, false);

                book.transform.Rotate(0, 180, 0);

                Vector3 bookLocalScale = book.transform.localScale * scale; // * Random.Range(0.7f, 1f);
                allocatedSize += bookWidth; // * Random.Range(1f, 1.5f);
                Vector3 bookPosition = new Vector3(shelfPos.x, shelfPos.y, shelfPos.z + allocatedSize);

                book.transform.localScale = bookLocalScale;
                book.transform.position = bookPosition;
            } else {
                allocatedSize += average * Random.Range(0.6f, 1.5f);
            }
        }*/
    }
}