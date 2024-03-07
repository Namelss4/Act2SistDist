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


        StartCoroutine("Registro", JsonUtility.ToJson(data));
        
    }

    public void enviarLogin()
    {
        AuthenticationData data = new AuthenticationData();
        data.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;


        StartCoroutine("Login", JsonUtility.ToJson(data));
    }

    IEnumerator Registro(string json)
    {
        //UnityWebRequest request = UnityWebRequest.PostWwwForm(url+"/api/usuarios", json);
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios", json);
        request.method = "POST";
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
                Debug.Log("Registro Exitoso!");
                StartCoroutine("Login", json);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }

    }

    IEnumerator Login(string json)
    {
        //UnityWebRequest request = UnityWebRequest.PostWwwForm(url+"/api/usuarios", json);
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/auth/login", json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                AuthenticationData data = JsonUtility.FromJson<AuthenticationData>(request.downloadHandler.text);

                Debug.Log(data.token);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }

    }
}

[System.Serializable]
public class AuthenticationData
{
    public string username;
    public string password;
    public UsuarioJson usuario;
    public string token;
}

[System.Serializable]
public class UsuarioJson
{
    public string _id;
    public string username;
}
