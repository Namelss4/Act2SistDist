using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class AuthHandler : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void enviarRegistro()
    {
        AuthenticationData data = new AuthenticationData();
        data.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;


        JsonUtility.ToJson(data);
    }

    public void enviarLogin()
    {

    }

    IEnumerator Registro(string json)
    {
        UnityWebRequest request = UnityWebRequest.Post(url+"/api/usuarios", json);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            if(request.responseCode == 200)
            {

            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }

    }
}

public class AuthenticationData
{
    public string username;
    public string password;
}
