using UnityEngine;
using System.Collections;

public class Modify : MonoBehaviour
{
    LineRenderer blah;
    Vector2 rot;
    ObjectChunk oc;
    bool isObject;

    void Start()
    {
        blah = GetComponent<LineRenderer>();
        isObject = false;
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            isObject = !isObject;
        }

        print(isObject);

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                blah.SetPosition(0, transform.position);
                blah.SetPosition(1, hit.point);
                blah.SetWidth(0.1f, 0.1f);
                Terrain.SetBlock(hit, BlockType.Air);
                Terrain.GetBlockPos(hit, true);
            }
        }

        if(Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                Terrain.SetBlock(hit, BlockType.Rock, true);
            }

            if(isObject)
            {
                if(oc == null)
                {
                    oc = new ObjectChunk(hit);
                }
                else
                {
                    oc.addNewBlock(hit);
                }
            }
        }

        Debug.Log("oc " + oc == null);
        if(Input.GetKeyDown(KeyCode.F) && !isObject && oc != null)
        {
            print("fuck");
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                oc.copyNewObject(hit);
            }
        }

        rot = new Vector2(
            rot.x + Input.GetAxis("Mouse X") * 3,
            rot.y + Input.GetAxis("Mouse Y") * 3);

        transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);

        transform.position += transform.forward * 3 * Input.GetAxis("Vertical");
        transform.position += transform.right * 3 * Input.GetAxis("Horizontal");
    }
}