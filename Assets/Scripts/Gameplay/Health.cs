using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour{

    public UnityEvent OnHit;
    public UnityEvent OnDestroy;

    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private int maxHealth;
    public float HealthPercent => (float)currentHealth/(float)maxHealth;

    public void GetDamage(int dmgAmoutn){
        currentHealth -= dmgAmoutn;
        if(currentHealth > 0){
            OnHit.Invoke();
        }else{
            OnDestroy.Invoke();
        }
    }

}