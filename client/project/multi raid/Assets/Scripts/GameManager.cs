using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    GameObject lobbyObj = null;

    protected override void Awake()
    {
        base.Awake();  // �̱��� �ʱ�ȭ

        Debug.Log("GameManager init");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    void Init()
    {
        GameObject lobbyPrefab = Resources.Load<GameObject>("prefabs/GameLobby");
        lobbyObj = Instantiate(lobbyPrefab, canvas);

        lobbyObj.transform.Find("MakeRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickMakeRoom);
        //lobbyObj.transform.Find("EnterRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        lobbyObj.transform.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterShop);
        lobbyObj.transform.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterInventory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickEnterShop()
    {
        // todo GameDataManager�� ���ϸ� ������ Ȯ���� ������ �������� ���ϸ� ������ �޾ƿ���
        if(GameDataManager.Instance.pokemonShopList == null)
        {
            //NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);
            GameDataManager.Instance.pokemonShopList = new List<GameDataManager.PokemonShop>();
            for (int i = 0; i < 5; ++i)
            {
                GameDataManager.PokemonShop data = new GameDataManager.PokemonShop
                {
                    idx = i,
                    name = "�̻��ؾ�" + i.ToString(),
                    desc = "�̻��ؾ��� �̻���" + i.ToString(),
                };

                GameDataManager.Instance.pokemonShopList.Add(data);
            }
            
        }
        

        CreateShop();

    }

    void CreateShop()
    {
        GameObject ShopPrefab = Resources.Load<GameObject>("prefabs/Shop");
        GameObject shopObj = Instantiate(ShopPrefab, canvas);

        shopObj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(shopObj));

        Sprite[] spriteAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.pokemonShopList.Count; i++)
        {
            var pokemon = GameDataManager.Instance.pokemonShopList[i];

            GameObject ShopItemPrefab = Resources.Load<GameObject>("prefabs/ShopItem");
            GameObject itemObj = Instantiate(ShopItemPrefab, shopObj.transform.Find("ShopItemScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteAll[pokemon.idx];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = pokemon.desc;

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => BuyPokemon(pokemon.idx));
        }

    }

    void BuyPokemon(int idx)
    {
        Debug.Log("BuyPokemon : " + idx);
    }
    

    void OnClickMakeRoom()
    {
        GameObject MakeRoomPrefab = Resources.Load<GameObject>("prefabs/MakeRoom");
        GameObject obj = Instantiate(MakeRoomPrefab, canvas);

        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        List<string> list = new List<string>();
        for(int i = 0; i < 20; ++i)
        {
            list.Add("level " + (i + 1));
        }
        dropdown.AddOptions(list);

        obj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => MakeRoom(obj));
        obj.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));


    }

    void MakeRoom(GameObject obj)
    {
        string title = obj.transform.Find("Title/InputField").GetComponent<TMP_InputField>().text;
        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        string dropdownText = dropdown.options[dropdown.value].text;
        Debug.Log("title : " + title + " / ropdown : " + dropdownText);

        NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);

    }
    
   void OnClickEnterInventory()
    {
        // todo GameDataManager�� �� ���ϸ� ������ Ȯ���� ������ �������� ���ϸ� ������ �޾ƿ���
        if (GameDataManager.Instance.myPokemonList == null)
        {
            //NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);
            GameDataManager.Instance.myPokemonList = new List<GameDataManager.Pokemon>();
            for (int i = 0; i < 5; ++i)
            {
                GameDataManager.Pokemon data = new GameDataManager.Pokemon
                {
                    idx = i,
                    name = "�̻��ؾ�" + i.ToString(),
                    desc = "�̻��ؾ��� �̻���" + i.ToString(),
                };

                GameDataManager.Instance.myPokemonList.Add(data);
            }

        }


        CreateInventory();

    }

    void CreateInventory()
    {
        GameObject InventoryPrefab = Resources.Load<GameObject>("prefabs/Inventory");
        GameObject invenObj = Instantiate(InventoryPrefab, canvas);

        invenObj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(invenObj));

        Sprite[] spriteAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myPokemonList.Count; i++)
        {
            var pokemon = GameDataManager.Instance.myPokemonList[i];

            GameObject InventoryItemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");
            GameObject itemObj = Instantiate(InventoryItemPrefab, invenObj.transform.Find("InventoryItemScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteAll[pokemon.idx];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = pokemon.desc;

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => UsePokemon(pokemon.idx));
        }

    }

    void UsePokemon(int idx)
    {
        Debug.Log("UsePokemon : " + idx);
    }

    void SetTransformTwoBtn(Transform trans, Action<bool> firstResult, Action<bool> secondResult)
    {
        trans.Find("firstBtn").GetComponent<Button>().onClick.AddListener(() => firstResult(true));
        trans.Find("secondBtn").GetComponent<Button>().onClick.AddListener(() => secondResult(true));
    }

    void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

}
