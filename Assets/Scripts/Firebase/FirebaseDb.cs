using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class FirebaseDb : MonoBehaviour
{
    DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveData()
    {

    }

}
