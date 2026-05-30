using System.Collections.Generic;
using UnityEngine;

public class NyanManager : MonoBehaviour
{
    public static NyanManager instance;
    public float timerRate;
    public List<GameObject> PlayerRef = new List<GameObject>();
    public List<Vector3> _lastPlayerPos = new List<Vector3>();
    public List<float> playerMagnitude = new List<float>();
    public bool magnitudeEnabled;
    public float magnitudeTimer;
    public float magnitudeTimerGoal;
    public float magnitudeAmplifier;

    public void Awake()
    {
        instance = this;
        GameObject[] playerSearch = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < playerSearch.Length; i++)
        {
            Debug.Log(playerSearch[i]);
            PlayerRef.Add(playerSearch[i]);
        }


        for (int i = 0; i < PlayerRef.Count; i++)
        {
            _lastPlayerPos.Add(Vector3.zero);
            playerMagnitude.Add(0);
        }
        for (int i = 0; i < PlayerRef.Count; i++)
        {
            _lastPlayerPos[i] = PlayerRef[i].transform.position;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (magnitudeEnabled)
        {
            if (magnitudeTimer < magnitudeTimerGoal)
            {
                magnitudeTimer += Time.deltaTime * timerRate;
                return;
            }
            magnitudeTimer = 0;
            for (int i = 0; i < PlayerRef.Count; i++)
            {
                playerMagnitude[i] = getPlayerMagnitudes(i) * magnitudeAmplifier;
            }
        }
    }

    public float getPlayerMagnitudes(int player)
    {
        float magnitude

         = Vector3.Distance(_lastPlayerPos[player], PlayerRef[player].transform.position);
        _lastPlayerPos[player] = PlayerRef[player].transform.position;


        return magnitude;
    }


}
