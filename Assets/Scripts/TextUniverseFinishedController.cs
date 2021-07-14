using UnityEngine;
using TMPro;

public class TextUniverseFinishedController : MonoBehaviour
{
  TMP_Text text;
  string letters = "abcdefghijklmnopqrstuvwxyz";

  void Awake()
  {
    text = GetComponent<TMP_Text>();
  }

  void Start()
  {
    UpdateName(RandomUniverseName());
  }

  string RandomUniverseName()
  {
    int universeNumber = Random.Range(1000, 9999);
    string universeLetters = "";

    for (int i = 0; i < 3; i++)
    {
        universeLetters += letters[Random.Range(0, letters.Length)];
    }

    string universeName = universeNumber + universeLetters;

    return universeName;
  }

  void UpdateName(string universeName)
  {
    text.text = text.text.Replace("UNIVERSENAME", universeName);
  }

}
