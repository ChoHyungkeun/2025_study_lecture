using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Transform canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitBtn();
    }

    void InitBtn()
    {
        canvas.Find("Login/LoginBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        canvas.Find("Login/JoinBtn").GetComponent<Button>().onClick.AddListener(OnClickJoinPage);

        canvas.Find("Join/BackBtn").GetComponent<Button>().onClick.AddListener(OnClickLoginPage);
        canvas.Find("Join/JoinBtn").GetComponent<Button>().onClick.AddListener(OnClickJoin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickLogin()
    {
        string id = canvas.Find("Login/ID").GetComponent<InputField>().text;
        string password = canvas.Find("Login/Password").GetComponent<InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        // todo �α��� ���� - �ӽ÷� ����ȯ
        SceneManager.LoadScene("GameScene");
        //NetworkManager.Instance.SendLoginServer(CommonDefine.LOGIN_URL, id, password, LoginAction);
    }

    void OnClickJoinPage()
    {
        canvas.Find("Login").gameObject.SetActive(false);
        canvas.Find("Join").gameObject.SetActive(true);

        canvas.Find("Join/ID").GetComponent<InputField>().text = "";
        canvas.Find("Join/Password").GetComponent<InputField>().text = "";

    }

    void OnClickLoginPage()
    {
        canvas.Find("Login").gameObject.SetActive(true);
        canvas.Find("Join").gameObject.SetActive(false);

        canvas.Find("Login/ID").GetComponent<InputField>().text = "";
        canvas.Find("Login/Password").GetComponent<InputField>().text = "";

    }

    void OnClickJoin()
    {
        string id = canvas.Find("Join/ID").GetComponent<InputField>().text;
        string password = canvas.Find("Join/Password").GetComponent<InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        // todo ȸ������ ���� + ȸ������ ���� ��Ŷ ó��
        //NetworkManager.Instance.SendLoginServer(CommonDefine.REGISTER_URL, id, password, JoinAction);
    }

    void JoinAction(bool result)
    {
        if(result)
        {
            // todo ȸ������ �Ϸ�â
            OnClickLoginPage();
        }
        else
        {
            // todo ����â ����
        }
    }

    void LoginAction(bool result)
    {
        if (result)
        {
            // todo �α��� �Ϸ� �ȳ�â
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            // todo ����â ����
        }
    }



}
