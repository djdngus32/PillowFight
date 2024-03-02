using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NicknameTagUIHandler : MonoBehaviour
{
    public static NicknameTagUIHandler Instance { get; private set; }

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int objectPoolSize = 8;

    private Queue<NicknameTagItemUIHandler> pool = new Queue<NicknameTagItemUIHandler>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Start()
    {
        for(int i = 0; i < objectPoolSize;  i++)
        {
            CreateNewNicknameTag();
        }
    }

    public NicknameTagItemUIHandler GetNicknameTag()
    {
        if (pool.Count == 0)
        {
            if (CreateNewNicknameTag() == false)
                return null;
        }

        var pooledItem = pool.Dequeue();
        pooledItem.gameObject.SetActive(true);
        return pooledItem;
    }

    public void ReturnNicknameTag(NicknameTagItemUIHandler item)
    {
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    private bool CreateNewNicknameTag()
    {
        GameObject obj = Instantiate(itemPrefab);
        var item = obj.GetComponent<NicknameTagItemUIHandler>();

        if (item == null)
        {
            Destroy(obj);
            return false;
        }

        obj.SetActive(false);
        pool.Enqueue(item);
        obj.transform.SetParent(this.transform);
        return true;
    }
}
