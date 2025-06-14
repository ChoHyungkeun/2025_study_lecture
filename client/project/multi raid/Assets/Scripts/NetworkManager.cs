using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class NetworkManager : Singleton<NetworkManager>
{
    [System.Serializable]
    public class LoginPostData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginData
    {
        public string sessionId;
        public string username;
    }

    [System.Serializable]
    public class PostData
    {
        public API_TYPE api;
        public string packetName;
        public string packetData;
    }

    protected override void Awake()
    {
        base.Awake();  // �̱��� �ʱ�ȭ

        Debug.Log("NetworkManager init");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region WEB_POST

    

    

    public void SendLoginServer(string api, string username, string password, Action<bool> onResult)
    {
        Debug.Log(api);
        StartCoroutine(ServerLoginCall(api, username, password, onResult));
    }

    IEnumerator ServerLoginCall(string api, string username, string password, Action<bool> onResult)
    {
        LoginPostData data = new LoginPostData
        {
            username = username,
            password = password
        };
        string json = JsonUtility.ToJson(data);

        Debug.LogError("before return www");
        UnityWebRequest request = new UnityWebRequest(CommonDefine.WEB_POST_BASE_URL + api, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("����: " + request.downloadHandler.text);

            if (api == CommonDefine.LOGIN_URL)
            {
                LoginData res = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);

                GameDataManager.Instance.sessionId = res.sessionId;
            }

            onResult?.Invoke(true);

        }
        else
        {
            Debug.LogError("POST ����: " + request.error);
            onResult?.Invoke(false);
        }

    }



    public void SendServer(API_TYPE api, string packetName, string packetData)
    {
        Debug.Log(api);
        StartCoroutine(ServerCall(api, packetName, packetData));
    }

    IEnumerator ServerCall(API_TYPE api, string packetName, string packetData)
    {
        PostData data = new PostData
        {
            api = api,
            packetName = packetName,
            packetData = packetData
        };
        string json = JsonUtility.ToJson(data);

        Debug.LogError("before return www");
        UnityWebRequest request = new UnityWebRequest(CommonDefine.WEB_POST_BASE_URL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            //LoginData res = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);
            Debug.Log("����: " + request.downloadHandler.text);

            //GameDataManager.Instance.token = res.token;

        }
        else
        {
            Debug.LogError("POST ����: " + request.error);
        }

    }

    #endregion

    #region WEB_SOCKET

    private WebSocket ws;

    public void ConnectSocket()
    {
        if (ws == null) {
            ws = new WebSocket(CommonDefine.WEB_SOCKET_URL);

            ws.OnOpen += OnWebSocketOpen;
            ws.OnMessage += OnWebSocketMessage;
            ws.OnError += OnWebSocketError;
            ws.OnClose += OnWebSocketClose;

            ws.Connect();
        }
        
    }

    private void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("������ ����Ǿ����ϴ�.");
        ws.Send("Hello from Unity!");
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("���� �޽��� ����: " + e.Data);
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("���� �߻�: " + e.Message);
    }

    private void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("���� ���� �����. ����: " + e.Reason);
    }



    #endregion

    void OnApplicationQuit()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
