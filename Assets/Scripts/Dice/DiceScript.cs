﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour {
    
    private bool _isRolling = true;
    private Vector3 _randomRotation;
    private bool _isD2 = false;



    // Use this for initialization
    void Start() {
        _randomRotation = new Vector3(UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360));
    }

    public void SetIsRolling(bool isRolling)
    {
        _isRolling = isRolling;
    }

    public void SetIsD2(bool isD2)
    {
        _isD2 = isD2;
    }

    void OnCollisionEnter(Collision collision)
    {
        _isRolling = false;
    }

    // Update is called once per frame
    void Update () {
        if (_isRolling == true)
        {
            if (_isD2)
            {
                this.gameObject.transform.Rotate(_randomRotation.x * Time.deltaTime, _randomRotation.y * Time.deltaTime, 0);
            }
            else
            {
                this.gameObject.transform.Rotate(_randomRotation.x * Time.deltaTime, _randomRotation.y * Time.deltaTime, _randomRotation.z * Time.deltaTime);
            }
            
        }
        

    }
}
