using UnityEngine;

public class DestroyAfter : MonoBehaviour{
    public float destroyTime = -1;

    private void OnEnable()
    {
        if(destroyTime > 0){
            Invoke(nameof(HandleDestroy), destroyTime);
        }
    }

    private void HandleDestroy()
    {
        //KYLE TODO: Network destroy
    }
}