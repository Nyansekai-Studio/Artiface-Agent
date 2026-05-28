using System.Collections.Generic;
using UnityEngine;

public class NyanManager : MonoBehaviour
{
    public static NyanManager instance;
    public List<GameObject> PlayerRef = new List<GameObject>();
    public List<Vector3> _lastPlayerPos = new List<Vector3>();
    public List<float> playerMagnitude = new List<float>();
    public float magnitudeAmplifier;

    public void Awake()
    {
        instance = this;
        for (int i = 0; i < PlayerRef.Count; i++)
        {
            _lastPlayerPos.Add(Vector3.zero);
            playerMagnitude.Add(0);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < PlayerRef.Count; i++)
        {
            _lastPlayerPos[i] = PlayerRef[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < PlayerRef.Count; i++)
        {
            playerMagnitude[i] = getPlayerMagnitudes(i) * magnitudeAmplifier;
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
