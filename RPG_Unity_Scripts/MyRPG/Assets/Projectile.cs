using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 3f;
    public Color textCol = Color.white;
    public bool isPoison = false;

    private Transform target;

    public void Launch(Transform _target, int _dmg, Color _col, bool _isPoison = false)
    {
        target = _target;
        damage = _dmg;
        textCol = _col;
        isPoison = _isPoison;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null) {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        
        // Quay mũi tên theo hướng bay
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector2.Distance(transform.position, target.position) < 0.3f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        Monster m = target.GetComponent<Monster>();
        if (m != null) {
            m.TakeDamage(damage);
            if (isPoison) m.gameObject.SendMessage("ApplyPoison", SendMessageOptions.DontRequireReceiver);
        }
        
        // Hiện hiệu ứng chữ bay
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, (isPoison ? "🧪 -" : "🏹 -") + damage, textCol);
        
        Destroy(gameObject);
    }
}
