using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Address
{
    public string house_number;
    public string road;
    public string town;
    public string city;
    public string village;
    public string hamlet;
    public string municipality;
    public string county;
    public string state;
    public string postcode;
    public string country;
}

[System.Serializable]
public class JsonData
{
    public string place_id;
    public string display_name;
    public Address address;
}
public class GeolocalizationClass : MonoBehaviour
{
    
}
