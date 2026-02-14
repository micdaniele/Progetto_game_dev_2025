using UnityEngine;

public class Knife : MonoBehaviour
{
    public Transform top; //coltello alto
    public Transform bottom; //coltello basso
    public float speed = 5f; //velocità di avvicinamento
    public float gap = 1f; //distanza tra i due coltelli

    private float leftEdge;// Bordo sinistro dello schermo

    //
    private void Start()
    {
        //coordinate per l'angolo in basso a sinistra
        //ScreenToWorldPoint() converte da screen space a world space
        //ed aggiunge un margine per distruggere l'oggetto leggermente fuori schermo per evitare il pop visivo
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f; 
        top.position += Vector3.up * gap / 2;
        bottom.position += Vector3.down * gap / 2;
    }

    private void Update()
    {
        //velocità di avvicinamento dei coltelli
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }

}
