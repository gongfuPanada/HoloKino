using UnityEngine;
using System.Collections;

public class EffectObjectPlayerObject : MonoBehaviour
{
    public GameObject _object = null;
    public float begineTime = 0.0f;
    public Vector3 position;

    public void DataLoad(GameObject _object,float begine, Vector3 position)
    {
        this._object = _object;
        this.begineTime = begine;
        this.position = position;
    }

    public void PlayerObjectLoad(GameObject parent)
    {
        _object = Instantiate(_object);
        _object.SetActive(false);

        _object.transform.parent = parent.transform;

        _object.transform.position = position;

    }

    public void EffectStart()
    {
        Invoke("EffectPlay", begineTime);
    }

    void EffectPlay()
    {
        _object.SetActive(true);

        Destroy(_object.gameObject, 4);

        Transform[] allChildren = _object.GetComponentsInChildren<Transform>();
        for (int j = 0; j < allChildren.Length; j++)
        {
            Rigidbody rigid = allChildren[j].GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.isKinematic = false;
            }
            Destroy(allChildren[j].gameObject, 4);
        }
    }
}
