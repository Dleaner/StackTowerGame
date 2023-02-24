using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private int _score;
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
        GameManager.OnCubeSpawned += GameManager_OnCubeSpawned;
    }

    private void OnDestroy() 
    {
        GameManager.OnCubeSpawned -= GameManager_OnCubeSpawned;    
    }

    private void GameManager_OnCubeSpawned()
    {
        _score++;
        _text.text = "Score: " + _score;
    }

}
