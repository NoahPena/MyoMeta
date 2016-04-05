﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.Xml.Linq;
//using System.Web;
using System.Net;
using System;

public class Weather : MonoBehaviour
{
    Text weatherText;
    Text[] texts;
    string data;
    

    void Start()
    {
        weatherText = gameObject.GetComponent<Text>();
        //texts = gameObject.GetComponents<Text>();
        //weatherText = texts[0];       
        weatherText.text = getWeather();
    }

    void Update()
    {

    }

    private static XmlDocument WeatherAPI(string sLocation, int nDays)
    {
        HttpWebRequest WP_Request;
        HttpWebResponse WP_Response = null;
        XmlDocument WP_XMLdoc = null;

        string sKey = "4bdd8b83f6d3093803c9cbc5d661a"; //The API key generated by World Weather Online
        string sRequestUrl = "http://api.worldweatheronline.com/free/v2/weather.ashx?"; //The request URL for XML format

        try
        {
            //Here we are concatenating the parameters
            WP_Request = (HttpWebRequest)WebRequest.Create(sRequestUrl + "key=" + sKey + "&q=" + sLocation + "&num_of_days=" + nDays + "&tp=3&format=xml");
            //WP_Request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";
            //Making the request 
            WP_Response = (HttpWebResponse)WP_Request.GetResponse();
            WP_XMLdoc = new XmlDocument();
            //Assigning the response to our XML objectc
            WP_XMLdoc.Load(WP_Response.GetResponseStream());

            //Console.WriteLine("Test: " + WP_XMLdoc.GetElementsByTagName("windspeedMiles").Item(0).ChildNodes[1].ChildNodes[1].InnerText);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        WP_Response.Close();
        return WP_XMLdoc;
    }

    protected static string getWeather()
    {
        //XPathNavigator navigator = WeatherAPI("Corpus+Christi", 1).CreateNavigator();
        XmlNodeList xnlWeather;

        // string name = RegionInfo.CurrentRegion.DisplayName;

        //Console.WriteLine(name + "\n\n");

        xnlWeather = WeatherAPI("Corpus+Christi", 1).SelectNodes("data");

        string tempC = xnlWeather.Item(0).ChildNodes[1].ChildNodes[1].InnerText;
        string tempF = xnlWeather.Item(0).ChildNodes[1].ChildNodes[2].InnerText;
        string windSpeed = xnlWeather.Item(0).ChildNodes[1].ChildNodes[6].InnerText;
        string visibility = xnlWeather.Item(0).ChildNodes[1].ChildNodes[12].InnerText;


        return "Corpus Christi:\n\n" + "\tTemperature (Celsius): " + tempC + "\n\tTemperature (Fahrenheit): " + tempF + "\n\tWind Speed (mph): " + windSpeed + "\n\tVisibility (miles): " + visibility;
    }
}