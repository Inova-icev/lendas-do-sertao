using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Vector3 _startPos;
    public int _columnNum;
    public List<PlayerObj> _playerList = new List<PlayerObj>();
    public PlayerObj _nowObj;
    public Transform _playerObjCircle;
    public Transform _goalObjCircle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                if(hit.collider.CompareTag("Player"))
                {
                    _nowObj = hit.collider.GetComponent<PlayerObj>();
                }
                else
                {
                    //Set move Player object to this point
                    if(_nowObj!=null)
                    {
                        Vector2 goalPos = hit.point;
                        _goalObjCircle.transform.position = hit.point;
                        _nowObj.SetMovePos(goalPos);
                    }
                }
            }
        }

        if(_nowObj!=null)
        {
            _playerObjCircle.transform.position = _nowObj.transform.position;
        }
    }
}
