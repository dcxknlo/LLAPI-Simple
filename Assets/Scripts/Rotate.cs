using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    Transform myTransform;
    public Quaternion updateQuat;
    public Quaternion recUpdateQuat;
    public Vector3 updatePos;
    public Vector3 recUpdatePos;
    public LLAPI llapi;
    float xCounter = 100;
	// Use this for initialization
	void Start () {
        recUpdateQuat = Quaternion.Euler(0f, 0f, 0f);
           
        llapi = GameObject.FindObjectOfType<LLAPI>();
        myTransform = this.transform;

    }
	
	// Update is called once per frame
	void Update () {
        if (!llapi.isConnectionServer && llapi.isStart)
        {
        
            updateQuat = Quaternion.Euler(xCounter % 359, 0, 0);

            updatePos = new Vector3(xCounter % 700, 0, 0);

            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, updateQuat, 5 * Time.deltaTime);

			myTransform.position = Vector3.Lerp(myTransform.position, updatePos, 5 * Time.deltaTime);
            xCounter += 2;

        }
        else if (llapi.isConnectionServer && llapi.isStart)
        {
			myTransform.rotation = Quaternion.Slerp(myTransform.rotation, recUpdateQuat, 5 * Time.deltaTime);
			myTransform.position = Vector3.Lerp(myTransform.position, recUpdatePos, 5 * Time.deltaTime);
        }

    }

    public void ShowHostColour()
    {
      
            this.GetComponent<Renderer>().materials[0].color = Color.red;
        return;
    }

      

}
