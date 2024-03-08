using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Linq;
using System;
//using System;

public class AuthHandler : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    public GameObject Panel, LeadPanel;

    //private List<GameObject> LeaderboardItems;

    public string Token { get; private set; }
    public string Username { get; private set; }

    public TMP_Text[] textUsers = new TMP_Text[0];

    //public TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
        }
        else
        {
            Username = PlayerPrefs.GetString("username");
            StartCoroutine("GetProfile");
        }
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

    public void cerrarSesion()
    {
        //GameObject.Find("Panel").SetActive(true);

        Panel.SetActive(true);


        Token = null;
        PlayerPrefs.SetString("token", Token);
    }

    public void abrirLB()
    {
        //GameObject.Find("LeadBoard").SetActive(true);
        LeadPanel.SetActive(true);
    }

    public void cerrarLB()
    {
        //GameObject.Find("LeadBoard").SetActive(false);
        LeadPanel.SetActive(false);
    }

    public void updateScore()
    {
        if (Username != null)
        {
            //AuthenticationData data = new AuthenticationData();

            UsuarioJson data = new UsuarioJson();

            data.data = new DataUser();

            //if (data.usuario == null)
            //{
            //    data.usuario = new UsuarioJson();
            //}

            //if (data.usuario.data == null)
            //{
            //    data.usuario.data = new DataUser();
            //}

            data.username = Username;

            // Asigna el nuevo valor al score
            data.data.score = Convert.ToInt32(GameObject.Find("InputFieldScore").GetComponent<TMP_InputField>().text);

            StartCoroutine("AsignScore", JsonUtility.ToJson(data));
        }
        else
        {
            Debug.LogError("No se puede actualizar el puntaje, el nombre de usuario no está inicializado.");
        }
    }

    public void ShowBoard(UsuarioJson[] usuarios)
    {
        for (int i = 0; i < usuarios.Length; i++)
        {
            textUsers[i].text = usuarios[i].username + $" score: {usuarios[i].data.score}";
        }
    }

    //void SetLeaderboardItem(GameObject item, UsuarioJson usuario)
    //{
    //    LeaderboardItems.Add(item);

    //    LeaderboardItem leaderboardItem = item.GetComponent<LeaderboardItem>();

    //    leaderboardItem.SetItem(usuario, LeaderboardItems.Count);
    //}

    public void ConsultarLeaderboard()
    {
        abrirLB();

        StartCoroutine("ObtenerUsuarios");
    }


    IEnumerator ObtenerUsuarios()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios");
        request.SetRequestHeader("x-token", Token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {

            if (request.responseCode == 200)
            {
                ListaUsuarios data = JsonUtility.FromJson<ListaUsuarios>(request.downloadHandler.text);
            
                var userList = data.usuarios.OrderByDescending(u => u.data.score).Take(5).ToArray();
            
                ShowBoard(userList);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
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

    IEnumerator AsignScore(string json)
    {
        //UnityWebRequest request = UnityWebRequest.PostWwwForm(url+"/api/usuarios", json);
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios", json);
        request.method = "PATCH";
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-token", Token);


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

                Debug.Log("El nuevo score de " + Username + " es " + data.usuario.data.score);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);

                Debug.Log(json);
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

                Token = data.token;
                Username = data.usuario.username;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);

                Panel.SetActive(false);
                //GameObject.Find("PanelInGame").SetActive(true);
                Debug.Log(data.token);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }

    }


    IEnumerator GetProfile()
    {
        //UnityWebRequest request = UnityWebRequest.PostWwwForm(url+"/api/usuarios", json);
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios/"+Username);
        Debug.Log("Sending Request GetProfile");
        request.SetRequestHeader("x-token", Token);

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

                Debug.Log("El usuario "+data.usuario.username+" se encuentra autenticado y su puntaje es "+data.usuario.data.score);
                //GameObject.Find("Panel").SetActive(false);

                Panel.SetActive(false);

                //UsuarioJson[] usuarios = new UsuarioJson[10];

                //UsuarioJson[] usuariosOrganizados = usuarios.OrderByDescending(user => user.data.score).Take(5).ToArray();

            }
            else
            {
                //Debug.Log(request.responseCode + "|" + request.error);
                Debug.Log("El usuario no está autenticado");
            }

        }

    }
}

[System.Serializable]
public class ListaUsuarios
{
    public UsuarioJson[] usuarios;
}

[System.Serializable]
public class AuthenticationData
{
    public string username;
    public string password;
    public UsuarioJson usuario;
    public string token;
    public UserData[] usuarios;
}

[System.Serializable]
public class UsuarioJson
{
    //public string _id;
    public string username;
    public DataUser data;
}

[System.Serializable]
public class DataUser
{
    public int score;
}
