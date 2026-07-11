using UnityEngine;

public class Poolable : MonoBehaviour {
    private ObjectPool pool;

    public void Init(ObjectPool pool) {
        this.pool = pool;
    }

    public void ReturnToPool() {
        gameObject.SetActive(false);
    }
}