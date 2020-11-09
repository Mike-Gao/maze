using UnityEngine;

public class Destructable : MonoBehaviour
{

    void Start()
    {

    }

    public void OnDie()
    {
        // this will call the OnDestroy function
        Destroy(gameObject);
    }
}
