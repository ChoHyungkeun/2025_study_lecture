using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    protected override void Awake()
    {
        base.Awake();  // �̱��� �ʱ�ȭ

        Debug.Log("GameManager init");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
