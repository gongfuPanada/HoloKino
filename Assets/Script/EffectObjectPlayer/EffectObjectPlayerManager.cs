using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class EffectObjectPlayerManager : MonoBehaviour
{
    public GameObject objectParents;

    Dictionary<string, GameObject> effects;

    [SerializeField]
    private string[] effectNames;
    [SerializeField]
    private GameObject[] effectGameObjects;

    [SerializeField]
    private string MovieName = "null";
    string path;
    string master;

    void Awake()
    {
        path = MovieName + ".xml";
        master = Application.streamingAssetsPath + "/Movie/" + path;

        effects = new Dictionary<string, GameObject>();

        // 이펙트를 불러옴
        if (effectNames.Length != effectGameObjects.Length)
        {
            Debug.LogError("Not Equal Array Size");
        }
        else
        {
            for (int i = 0; i < effectGameObjects.Length; i++)
            {
                effects.Add(effectNames[i], effectGameObjects[i]);
            }
        }

        StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        WWW www = new WWW(master);

        yield return www;

        Interpret(www.text);
    }

    private void Interpret(string _strSource)
    {
        // 인코딩 문제 예외처리.
        // 읽은 데이터의 앞 2바이트 제거(BOM제거)
        // 혹시 오류나시면 BOM제거 부분 코드 없애고 해보시길 바랍니다~!

        System.Xml.XmlDocument Document = new System.Xml.XmlDocument();

#if NETFX_CORE
        StringReader stringReader = new StringReader(_strSource);

        Document.Load(stringReader);
#else
        Document.Load(master);
#endif
        System.Xml.XmlElement MovieDataList = Document["MovieDataList"];

        Queue<EffectObjectPlayerObject> effectPlayers = new Queue<EffectObjectPlayerObject>();

        EffectObjectPlayerObject playerObject = null;

        foreach (System.Xml.XmlElement data in MovieDataList.ChildNodes)
        {
            string objectName = data.GetAttribute("ObjectName");
            float begine = Convert.ToSingle(data.GetAttribute("Begine"));

            Vector3 pos;
            pos.x = Convert.ToSingle(data.GetAttribute("x"));
            pos.y = Convert.ToSingle(data.GetAttribute("y"));
            pos.z = Convert.ToSingle(data.GetAttribute("z"));

            // Mono를 상속받아 생성자 호출 X
            playerObject = gameObject.AddComponent(typeof(EffectObjectPlayerObject)) as EffectObjectPlayerObject;
            playerObject.DataLoad(effects[objectName], begine, pos);
            playerObject.PlayerObjectLoad(objectParents);
            playerObject.EffectStart();

            effectPlayers.Enqueue(playerObject);

            playerObject = null;
        }
    }
}