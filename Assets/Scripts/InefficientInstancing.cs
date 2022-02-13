using UnityEngine;

public class InefficientInstancing : MonoBehaviour
{
    public int AmountToSpawn = 10000;
    public GameObject Prefab;
    
    public void Start()
    {
        for (int i = 0; i < AmountToSpawn; i++)
        {
            GameObject.Instantiate(Prefab, this.transform);
        }

    }
}
