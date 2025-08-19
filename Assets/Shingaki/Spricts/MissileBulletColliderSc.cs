using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBulletColliderSc : MonoBehaviour
{
    enum Type
    {
        bullet,
        explosion
    }
    [SerializeField] Type colliderType;
    MissileBulletSc missile;

    private void Awake()
    {
        //�e�̃X�N���v�g���擾
        transform.parent.TryGetComponent(out missile);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(colliderType == Type.bullet)
        {
            missile.OnBulletTriggerEnter(other);
        }
        if(colliderType == Type.explosion)
        {
            missile.OnExplosionTriggerEnter(other);
        }
    }
}
