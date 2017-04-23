using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {
	// Update is called once per frame
	void LateUpdate () {
        Vector3 up = Camera.main.transform.up;

        if(SystemInfo.deviceModel == "LGE Nexus 5X")
        {
            up = up * -1;
        }

        transform.LookAt(Camera.main.transform, up);
	}
}
