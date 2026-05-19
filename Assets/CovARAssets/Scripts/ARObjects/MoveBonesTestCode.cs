using UnityEngine;
using DG.Tweening;

public class MoveBonesTestCode : MonoBehaviour
{
    public GameObject BoneA;
    public GameObject BoneB;
    public GameObject BoneC;
    public GameObject BoneD; 
    public float sumator;
    public float newXposition;
    public float newZposition;
    void Start()
    {
        newXposition = BoneC.transform.localPosition.x;
        newZposition = BoneC.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("BoneC pos: " + BoneC.transform.localPosition);
    }

    public void MoveBoneCD()
    {
        if(sumator != 0)
        {
            if (BoneC != null && BoneD != null)
            {
                newXposition += sumator;
                BoneC.transform.localPosition = new Vector3(newXposition, BoneC.transform.localPosition.y, BoneC.transform.localPosition.z);
                BoneD.transform.localPosition = new Vector3(newXposition, BoneD.transform.localPosition.y, BoneD.transform.localPosition.z);
            }
        }
    }
    public void MoveBoneBC()
    {
        if (sumator != 0)
        {
            if (BoneC != null && BoneB != null)
            {
                newZposition -= sumator;
                BoneC.transform.localPosition = new Vector3(BoneC.transform.localPosition.x, BoneC.transform.localPosition.y, newZposition);
                BoneB.transform.localPosition = new Vector3(BoneB.transform.localPosition.x, BoneB.transform.localPosition.y, newZposition);
            }
        }
    }

}
