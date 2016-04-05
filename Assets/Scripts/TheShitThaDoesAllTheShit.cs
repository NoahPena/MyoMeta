using UnityEngine;
using System.Collections;
using AR.Drone.Client;
using AR.Drone.Video;
using AR.Drone.Data;
using AR.Drone.Data.Navigation;
using FFmpeg.AutoGen;
using NativeWifi;
using UnityEngine.UI;
using ISuckAtCoding;

namespace Program
{
    public class TheShitThaDoesAllTheShit : MonoBehaviour
    {

        // Stick which is moved analogical to the movement of the gamepad stick
        public Transform Stick;
        // Modifies the stick rotation
        public float StickRotationModifier = 0.15f;
        // Plane on which the main camera is mapped
        public Renderer MainRenderer;
        // Plane on which the secondary camera is mapped
        public Renderer SecondaryRenderer;
        // Rotation limit for the switch between the main camera and the secondary camera
        public float SwitchRotation = 0.4f;
        // The camera used for the switch test
        public Transform CameraForSwitchCheck;
        // Ambient Light
        public Light[] AmbientLights;
        // Status text
        public TextMesh StatusText;
        // Wifi status
        public TextMesh WifiText;
        public TextMesh WifiChart;
        public int maxChartBars = 20;

        // Gamepad variables
        private bool playerIndexSet = false;
        //private XInputDotNetPure.PlayerIndex playerIndex;

        // Indicates that the drone is landed
        private bool isLanded = true;
        // Indicates that the startButton is Pressed
        private bool startButtonPressed = false;
        // Texture used for the camera content
        private Texture2D cameraTexture;
        // A black texture used for the inactive plane
        private Texture2D blackTexture;
        // byte array which will be filled by the camera data
        private byte[] data;
        // Drone variables
        private VideoPacketDecoderWorker videoPacketDecoderWorker;
        private DroneClient droneClient;
        private NavigationData navigationData;

        // Width and height if the camera
        private int width = 640;
        private int height = 360;

        // wlanclient for signal strength
        private WlanClient client;

        private bool dumbShit = false;

        //Text[] texts;
        //Text navigationText;

        // Use this for initialization
        void Start()
        {
            Debug.Log("Start This Shit");
           
          //  navigationText = gameObject.GetComponent<Text>();
           // Debug.Log(navigationText.text);
            // texts = gameObject.GetComponents<Text>();
            // navigationText = texts[1];

            data = new byte[width * height * 3];

            //set Textures
            MainRenderer.material.mainTexture = cameraTexture;
            //MainRenderer.material.mainTexture = cameraTexture;
            cameraTexture = new Texture2D(width, height);

            // Initialize drone
            videoPacketDecoderWorker = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnVideoPacketDecoded);
            videoPacketDecoderWorker.Start();
            droneClient = new DroneClient("192.168.1.1");
            droneClient.UnhandledException += HandleUnhandledException;
            droneClient.VideoPacketAcquired += OnVideoPacketAcquired;
            droneClient.NavigationDataAcquired += navData => navigationData = navData;
            
            videoPacketDecoderWorker.UnhandledException += HandleUnhandledException;
            droneClient.Start();

            switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType.HorizontalPlusSmallVertical);

            if (!GlobalVariables.useKeyboard)
            {
                Server server = new Server();
            }

            client = new WlanClient();

           // Debug.Log("Oh shit Nigga");
        }

        void OnDestroy()
        {
            droneClient.Land();
            droneClient.Stop();
        }

        IEnumerator WaitingTime()
        {
            yield return new WaitForSeconds(5);
        }
        

        // Update is called once per frame
        void Update()
        {
            if (dumbShit)
            {
                Debug.Log("Ahhh");
                if (Input.GetKey(KeyCode.Return))
                {
                    droneClient.Land();
                    getDroneNavigationData();
                    Debug.Log("Landed");
                    dumbShit = false;
                }
            }
            else
            {
                convertCameraData();

                if (GlobalVariables.isVertical)
                {
                    switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType.Vertical);
                }
                else
                {
                    switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType.Horizontal);
                }

                if (Input.GetKey("tab"))
                {
                    GlobalVariables.isVertical = !GlobalVariables.isVertical;
                    //switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType.Next);
                }

                if (Input.GetKey("escape"))
                {
                    Application.Quit();
                }

                //Drone Moving

                //Start or Land
                if (Input.GetKey("return"))
                {
                    Debug.Log("Nigga we made it");
                    if (!GlobalVariables.inAir)
                    {
                        droneClient.Takeoff();
                        Debug.Log("Take off");
                        //float x = 0.0f;
                        if (GlobalVariables.debug)
                        {
                            getDroneNavigationData();
                        }

                    }
                    else
                    {
                        if (GlobalVariables.debug)
                        {
                            while (GlobalVariables.altitude > .15)
                            {
                                Debug.Log("Altitude: " + GlobalVariables.altitude);
                                getDroneNavigationData();
                                GlobalVariables.gaz -= .5f;
                                droneClient.Progress(AR.Drone.Client.Command.FlightMode.Progressive, pitch: GlobalVariables.pitch, roll: GlobalVariables.roll, gaz: GlobalVariables.gaz, yaw: GlobalVariables.yaw);
                                GlobalVariables.gaz = 0;

                            }
                            getDroneNavigationData();

                            Debug.Log("Should Start Hovering");

                            GlobalVariables.pitch = 0;
                            GlobalVariables.roll = 0;
                            GlobalVariables.yaw = 0;
                            GlobalVariables.gaz = 0;

                            droneClient.Progress(AR.Drone.Client.Command.FlightMode.Progressive, pitch: GlobalVariables.pitch, roll: GlobalVariables.roll, gaz: GlobalVariables.gaz, yaw: GlobalVariables.yaw);


                           // droneClient.Hover();

                            dumbShit = true;

                            }
                        else
                        {
                                droneClient.Land();
                            }
                        }

                        GlobalVariables.inAir = !GlobalVariables.inAir;
                        //isLanded = !isLanded;
                    }
                else
                {
                        // Debug.Log("Currently Flying: " + GlobalVariables.currentlyFlying);
                        // Debug.Log("In Air: " + GlobalVariables.inAir);
                        if (GlobalVariables.inAir)
                        {
                            if (!GlobalVariables.currentlyFlying)
                            {
                                droneClient.Takeoff();
                                GlobalVariables.currentlyFlying = true;
                                Debug.Log("Drone Should Takeoff");
                            }
                        }
                        else
                        {
                            if (GlobalVariables.currentlyFlying)
                            {
                                droneClient.Land();
                                GlobalVariables.currentlyFlying = false;
                                Debug.Log("Drone Should Land");
                            }
                        }
                    }


                    if (GlobalVariables.useKeyboard)
                    {

                        GlobalVariables.pitch = 0;
                        GlobalVariables.roll = 0;
                        GlobalVariables.yaw = 0;
                        GlobalVariables.gaz = 0;

                        if (Input.GetKey(KeyCode.I))
                        {
                            droneClient.FlatTrim();
                        }
                        if (Input.GetKey(KeyCode.W))
                        {
                            GlobalVariables.pitch -= 1;
                        }

                        if (Input.GetKey(KeyCode.A))
                        {
                            GlobalVariables.roll -= 1;
                        }

                        if (Input.GetKey(KeyCode.S))
                        {
                            GlobalVariables.pitch += 1;
                        }

                        if (Input.GetKey(KeyCode.D))
                        {
                            GlobalVariables.roll += 1;
                        }

                        if (Input.GetKey(KeyCode.UpArrow))
                        {
                            if (GlobalVariables.debug)
                            {
                                getDroneNavigationData();
                                if (GlobalVariables.altitude >= 2.3)
                                {
                                    GlobalVariables.gaz = 0;
                                }
                                else
                                {
                                    GlobalVariables.gaz += 1;
                                }
                            }
                            else
                            {
                                GlobalVariables.gaz += 1;
                            }
                        }

                        if (Input.GetKey(KeyCode.DownArrow))
                        {
                            getDroneNavigationData();
                            GlobalVariables.gaz -= 1;
                        }

                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            GlobalVariables.yaw -= 1;
                        }

                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            GlobalVariables.yaw += 1;
                        }
                    }
                    else
                    {
                    GlobalVariables.gaz = 0.0f;

                    if (GlobalVariables.useWASD)
                    {
                        GlobalVariables.yaw = 0;
                        if (Input.GetKey(KeyCode.W))
                        {
                            if (GlobalVariables.debug)
                            {
                                getDroneNavigationData();
                                if (GlobalVariables.altitude >= 2.3)
                                {
                                    GlobalVariables.gaz = 0;
                                }
                                else
                                {
                                    GlobalVariables.gaz += 1;
                                }
                            }
                            else
                            {
                                GlobalVariables.gaz += 1;
                            }
                        }

                        if (Input.GetKey(KeyCode.S))
                        {
                            getDroneNavigationData();
                            GlobalVariables.gaz -= 1;
                        }

                        if(Input.GetKey(KeyCode.A))
                        {
                            GlobalVariables.yaw -= 1;
                        }
                        if(Input.GetKey(KeyCode.D))
                        {
                            GlobalVariables.yaw += 1;
                        }
                    }
                    else
                    {
                        GlobalVariables.yaw = -GlobalVariables.yaw;
                    }
                    //GlobalVariables.yaw = 0.0f;
                    
                    }

                    if (GlobalVariables.debug)
                    {
                        if (GlobalVariables.prevAltitude - GlobalVariables.altitude > .25)
                        {
                            GlobalVariables.gaz -= 1;
                        }
                    }

                    droneClient.Progress(AR.Drone.Client.Command.FlightMode.Progressive, pitch: GlobalVariables.pitch, roll: GlobalVariables.roll, gaz: GlobalVariables.gaz, yaw: GlobalVariables.yaw);

                    //droneClient.Progress(AR.Drone.Client.Command.FlightMode.AbsoluteControl, pitch: GlobalVariables.pitch, roll: GlobalVariables.roll, gaz: GlobalVariables.gaz, yaw: GlobalVariables.yaw);
                    //droneClient.Progress(AR.Drone.Client.Command.FlightMode.Hover, pitch: GlobalVariables.pitch, roll: GlobalVariables.roll, gaz: GlobalVariables.gaz, yaw: GlobalVariables.yaw);
                    //droneClient.Progress(AR.Drone.Client.Command.FlightMode.Progressive, pitch: vPitch, roll: vRoll, gaz: vGaz, yaw: vYaw);


                if(GlobalVariables.debug)
                {
                    getDroneNavigationData();
                }
                    getDroneNavigationData();
                }
            }

        public void getDroneNavigationData()
        {
            if (navigationData != null)
            {
                GlobalVariables.navigationData = string.Format("Battery: {0} % \nYaw: {1:f} \nPitch: {2:f} \nRoll: {3:f} \nAltitude: {4} m",
                                                navigationData.Battery.Percentage, navigationData.Yaw, navigationData.Pitch,
                                                navigationData.Roll, navigationData.Altitude);

                GlobalVariables.prevAltitude = GlobalVariables.altitude;
                GlobalVariables.altitude = navigationData.Altitude;

            }
        }

        private void switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType Type)
        {
            var configuration = new AR.Drone.Client.Configuration.Settings();
            configuration.Video.Channel = Type;
            droneClient.Send(configuration);
        }

        private void convertCameraData()
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int total = 0;
            var colorArray = new Color32[data.Length / 3];
            for (var i = 0; i < data.Length; i += 3)
            {
                colorArray[i / 3] = new Color32(data[i + 2], data[i + 1], data[i + 0], 1);
                r += data[i + 2];
                g += data[i + 1];
                b += data[i + 0];
                total++;
            }
            r /= total;
            g /= total;
            b /= total;
            cameraTexture.SetPixels32(colorArray);
            cameraTexture.Apply();
            GameObject.Find("CameraFeed").GetComponent<Renderer>().material.mainTexture = cameraTexture;
            //MainRenderer.material.mainTexture = cameraTexture;
            //renderer.material.mainTexture = cameraTexture;
           // Debug.Log("Nigga we Made it");
            foreach (Light light in AmbientLights)
                light.color = new Color32(System.Convert.ToByte(r), System.Convert.ToByte(g), System.Convert.ToByte(b), 1);
        }

        /// <summary>
        /// Determines what happens if a video packet is acquired.
        /// </summary>
        /// <param name="packet">Packet.</param>
        private void OnVideoPacketAcquired(VideoPacket packet)
        {
            if (videoPacketDecoderWorker.IsAlive)
                videoPacketDecoderWorker.EnqueuePacket(packet);
        }

        /// <summary>
        /// Determines what happens if a video packet is decoded.
        /// </summary>
        /// <param name="frame">Frame.</param>
        private void OnVideoPacketDecoded(VideoFrame frame)
        {
            data = frame.Data;
        }

        void HandleUnhandledException(object arg1, System.Exception arg2)
        {
            Debug.Log(arg2);
        }
    }
}