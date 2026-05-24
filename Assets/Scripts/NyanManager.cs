using UnityEngine;

public class NyanManager : MonoBehaviour
{
    public static NyanManager instance;
    public GameObject PlayerRef;
    public Vector3 _lastPlayerPos;
    public float playerMagnitude;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        _lastPlayerPos = PlayerRef.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerMagnitude = getPlayerMagnitude();
    }

    public float getPlayerMagnitude()
    {
        float magnitude = Vector3.Distance(_lastPlayerPos, PlayerRef.transform.position);
        _lastPlayerPos = PlayerRef.transform.position;

        
        return magnitude;
    }


}
