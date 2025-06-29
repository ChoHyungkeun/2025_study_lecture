using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    GameObject lobbyObj = null;
    GameObject battleObj = null;

    BATTLE_STATE state = BATTLE_STATE.NONE;
    int myBattleTurn = -1;

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
        GameObject prefab = Resources.Load<GameObject>("prefabs/GameLobby");
        lobbyObj = Instantiate(prefab, canvas);

        lobbyObj.transform.Find("MakeRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickMakeRoom);
        lobbyObj.transform.Find("RoomListBtn").GetComponent<Button>().onClick.AddListener(EnterBattle);
        lobbyObj.transform.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterShop);
        lobbyObj.transform.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterInventory);
    }

    // Update is called once per frame
    void Update()
    {
        BattleState();
    }

    void EnterBattle()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Battle");
        battleObj = Instantiate(prefab, canvas);

        // ���� ����
        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        battleObj.transform.Find("Boss/Image").GetComponent<Image>().sprite = spriteFrontAll[158];

        Slider bossHpSlider = battleObj.transform.Find("Boss/HpBar").GetComponent<Slider>();
        bossHpSlider.maxValue = 150;
        bossHpSlider.value = 150;
        battleObj.transform.Find("Boss/HpBar/HpText").GetComponent<TMP_Text>().text = "150 / 150";

        // ���� ����
        List<BattlePoke> userList = new List<BattlePoke>();
        for (int i = 0; i < 4; ++i)
        {
            BattlePoke data = new BattlePoke
            {
                pokeIdx = i + 10,
                curHp = (i + 1) * 10,
                maxHp = (i + 1) * 10,
            };

            userList.Add(data);
        }

        Sprite[] spriteBackAll = Resources.LoadAll<Sprite>("images/pokemon-back");
        for (int i = 0; i < userList.Count; i++)
        {
            var user = userList[i];
            string player = "4Player/Player" + (i + 1).ToString();

            battleObj.transform.Find(player + "/Image").GetComponent<Image>().sprite = spriteBackAll[user.pokeIdx];

            Slider hpSlider = battleObj.transform.Find(player + "/HpBar").GetComponent<Slider>();
            hpSlider.maxValue = user.maxHp;
            hpSlider.value = user.curHp;

            battleObj.transform.Find(player + "/HpBar/HpText").GetComponent<TMP_Text>().text = user.curHp.ToString() + " / " + user.maxHp;
        }

    }

    void BattleState()
    {
        // todo ��Ʋ�� ���� ���� ó��
        switch (state)
        {
            case BATTLE_STATE.NONE:
                {
                    // ���� ���� �ƴ�.
                }
                break;
            case BATTLE_STATE.WAIT:
                {
                    // �ٸ� ����� ��.
                    battleObj.transform.Find("State/state").gameObject.SetActive(true);
                    battleObj.transform.Find("State/Skill").gameObject.SetActive(false);
                }
                break;
            case BATTLE_STATE.PLAYER1_TURN:
                {
                    SetBattleTurn(1);
                }
                break;
            case BATTLE_STATE.PLAYER2_TURN:
                {
                    SetBattleTurn(2);
                }
                break;
            case BATTLE_STATE.PLAYER3_TURN:
                {
                    SetBattleTurn(3);
                }
                break;
            case BATTLE_STATE.PLAYER4_TURN:
                {
                    SetBattleTurn(4);
                }
                break;
            case BATTLE_STATE.BOSS_TURN:
                {
                    battleObj.transform.Find("State/state").GetComponent<TMP_Text>().text = "������ �����Դϴ�.";
                    state = BATTLE_STATE.WAIT;
                }
                break;
            case BATTLE_STATE.VICTORY:
                {
                    myBattleTurn = -1;
                }
                break;
            case BATTLE_STATE.DEFEAT:
                {
                    myBattleTurn = -1;
                }
                break;
        }
    }

    void SetBattleTurn(int turn)
    {
        if (myBattleTurn == turn)
        {
            battleObj.transform.Find("State/Skill").gameObject.SetActive(true);

            var myPokemon = GameDataManager.Instance.myCurPokemon;

            battleObj.transform.Find("State/Skill/skill1Btn").GetComponent<Button>().onClick.AddListener(() => UseSkill(myPokemon.skill1_idx));
            battleObj.transform.Find("State/Skill/skill2Btn").GetComponent<Button>().onClick.AddListener(() => UseSkill(myPokemon.skill2_idx));
            battleObj.transform.Find("State/Skill/skill3Btn").GetComponent<Button>().onClick.AddListener(() => UseSkill(myPokemon.skill3_idx));

            battleObj.transform.Find("State/Skill/skill1Btn/Text").GetComponent<TMP_Text>().text = myPokemon.skill1_name;
            battleObj.transform.Find("State/Skill/skill2Btn/Text").GetComponent<TMP_Text>().text = myPokemon.skill2_name;
            battleObj.transform.Find("State/Skill/skill3Btn/Text").GetComponent<TMP_Text>().text = myPokemon.skill3_name;

        }
        else
        {
            battleObj.transform.Find("State/state").GetComponent<TMP_Text>().text = "Player " + turn.ToString() + "�� �����Դϴ�.";
            state = BATTLE_STATE.WAIT;
        }
    }

    async void UseSkill(int skillIdx)
    {
        await NetworkManager.Instance.SendMessageToRoom(skillIdx.ToString());
    }

    async void ConnectSocket()
    {
        await NetworkManager.Instance.ConnectSocket();
    }

    void OnClickRoomList()
    {
        // todo �������� �븮��Ʈ �޾ƿ���
        ConnectSocket();
        NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_LIST_URL, null, null, CallbackRoomList);
        

    }

    void CallbackRoomList(bool result)
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/RoomList");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        List<Room> roomList = new List<Room>();
        for(int i = 0; i < 5; ++i)
        {
            Room data = new Room
            {
                idx = i,
                title = "�̸��̸�" + i.ToString(),
                level = i * 2,
                masterPokeIdx = i * 20,
            };

            roomList.Add(data);
        }

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < roomList.Count; i++)
        {
            var room = roomList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/RoomListItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[room.masterPokeIdx];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = room.title;
            itemObj.transform.Find("Level").GetComponent<TMP_Text>().text = "level " + room.level.ToString();

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.idx));
        }

    }

    void JoinRoom(int idx)
    {
        // todo ���ϸ� ������ ������ ����
        Debug.Log("JoinRoom : " + idx);
    }

    void OnClickEnterShop()
    {
        // todo GameDataManager�� ���ϸ� ������ Ȯ���� ������ �������� ���ϸ� ������ �޾ƿ���
        if(GameDataManager.Instance.pokemonShopList == null)
        {
            //NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);
            GameDataManager.Instance.pokemonShopList = new List<PokemonShop>();
            for (int i = 0; i < 5; ++i)
            {
                PokemonShop data = new PokemonShop
                {
                    idx = i,
                    name = "�̻��ؾ�" + i.ToString(),
                    desc = "�̻��ؾ��� �̻���" + i.ToString(),
                    price = i * 100,
                };

                GameDataManager.Instance.pokemonShopList.Add(data);
            }
            
        }
        

        CreateShop();

    }

    void CreateShop()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Shop");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.pokemonShopList.Count; i++)
        {
            var pokemon = GameDataManager.Instance.pokemonShopList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/ShopItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[pokemon.idx];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = pokemon.desc;

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => BuyPokemon(pokemon.idx));
        }

    }

    void BuyPokemon(int idx)
    {
        // todo ���ϸ� ������ ������ ����
        Debug.Log("BuyPokemon : " + idx);
    }
    

    void OnClickMakeRoom()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/MakeRoom");
        GameObject obj = Instantiate(prefab, canvas);

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

        NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText, CallbackMakeRoom);

    }

    void CallbackMakeRoom(bool result)
    {
        if (result)
        {
            // todo �α��� �Ϸ� �ȳ�â
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            CreateMsgBoxOneBtn("����� ����");
        }
    }

    void OnClickEnterInventory()
    {
        // todo GameDataManager�� �� ���ϸ� ������ Ȯ���� ������ �������� ���ϸ� ������ �޾ƿ���
        if (GameDataManager.Instance.myPokemonList == null)
        {
            //NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);
            GameDataManager.Instance.myPokemonList = new List<Pokemon>();
            for (int i = 0; i < 5; ++i)
            {
                Pokemon data = new Pokemon
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
        GameObject prefab = Resources.Load<GameObject>("prefabs/Inventory");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myPokemonList.Count; i++)
        {
            var pokemon = GameDataManager.Instance.myPokemonList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[pokemon.idx];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = pokemon.desc;

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => UsePokemon(pokemon.idx));
        }

    }

    void UsePokemon(int idx)
    {
        // todo �� ���ϸ� ������ ������ ����
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

    void CreateMsgBoxOneBtn(string desc)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_1Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("CheckBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
    }

    void CreateMsgBoxTwoBtn(string desc, Action<bool> yesResult, Action<bool> noResult)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_2Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("YesBtn").GetComponent<Button>().onClick.AddListener(() => yesResult(obj));
        obj.transform.Find("NoBtn").GetComponent<Button>().onClick.AddListener(() => noResult(obj));
    }

}
