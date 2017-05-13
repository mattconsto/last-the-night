using UnityEngine;
using System.Collections;

public class BowString : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float factor;

    Vector3 firstPosition;
    Vector3 lastPosition;

    public bool stretching;
    public bool releasing;

    public float stretchSpeed = 0.5f;
    public float releaseSpeed = 0.5f;

    // Use this for initialization
    void Start () {
        firstPosition = transform.localPosition;
        lastPosition = transform.localPosition + Vector3.up * 0.45f;
    }
	
	// Update is called once per frame
	void Update () {

        if (stretching)
        {
            factor += stretchSpeed * Time.deltaTime;

            if (factor > 1.0f)
            {
                factor = 1.0f;
            }
        }
        if (releasing)
        {
            factor -= releaseSpeed* Time.deltaTime;

            if (factor < 0.0f)
            {
                factor = 0.0f;
            }
        }

        transform.localPosition = Vector3.Lerp(firstPosition, lastPosition, factor);
    }

    //You probably want to call this somewhere
    void Stretch()
    {
        stretching = true;
        releasing = false;
    }

    void Release()
    {
        releasing = true;
        stretching = false;
    }
}
