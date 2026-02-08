using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance; //traccia dell’unica istanza attiva del MusicManager

    void Awake()
    {
        if (instance == null)
        {
            instance = this; //Salva il riferimento a questa istanza nella variabile statica
            //Evita che il GameObject venga distrutto quando carichi una nuova scena così che la musica continua a suonare tra scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Previene duplicazioni di musica quando torni in una scena che ne contiene un’altra copia
            Destroy(gameObject);
        }
    }
}