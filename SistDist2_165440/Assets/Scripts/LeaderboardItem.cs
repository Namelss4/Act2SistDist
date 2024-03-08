using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    public string Username {  get; set; }

    public string Score { get; set; }

    public string Position { get; set; }

    public TMP_Text TextUsername;
    public TMP_Text TextScore;

    public void SetItem(UsuarioJson usuario, int pos)
    {
        TextUsername.text = usuario.username;
        TextScore.text = "" + usuario.data.score;

        transform.position = new Vector3(0, 100 - (pos * 100), 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
