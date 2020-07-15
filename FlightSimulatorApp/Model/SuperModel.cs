using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FlightSimulatorApp.Model
{
    /**
     * the model class
     */
    public class SuperModel : INotifyPropertyChanged
    {
        private Client client;
        private double airSpeed = 0.0;
        private double verticalSpeed = 0.0;
        private double headingDeg = 0.0;
        private double groundSpeed = 0.0;
        private double rollDeg = 0.0;
        private double speedKt = 0.0;
        private double gpsAltFt = 0.0;
        private double pitchDeg = 0.0;
        private double indicAlt = 0.0;
        private double lon = 0.0;//longitude
        private double lat = 0.0;//latitude  
        private double fakeLat = 0.0;
        private double fakeLon = 0.0;
        private Location location;
        private Visibility visible = Visibility.Hidden;
        private Visibility cntButtonVisible = Visibility.Visible;
        private DispatcherTimer timer;
        private string errormsg;
        private volatile Boolean stop;
        private volatile Boolean first = true;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Mutex mutex = new Mutex();

        /// <summary>Initializes a new instance of the <see cref="SuperModel" /> class.</summary>
        /// <param name="client">The client.</param>
        public SuperModel(Client client)
        {
            this.client = client;
            stop = false;
        }

        /// <summary>Connects the specified ip.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        public async Task<bool> Connect(String ip, int port)
        {
            this.timer = new DispatcherTimer();
            await client.Connect(ip, port);
            stop = false;
            if (!client.Isconnected())
            {
                //Errormsg = "could not connect to server check if the ip or port are correct\n";
                return false;
            }

            CntButtonVisible = Visibility.Hidden;
            return true;
        }

        /// <summary>Determines whether this instance is connected.</summary>
        /// <returns>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.</returns>
        public bool IsConnected()
        {
            if (client != null)
            {
                return this.client.Isconnected();
            }

            return false;

        }

        /// <summary>Resets all.</summary>
        public void ResetAll()
        {
            this.client = new Client();
            CntButtonVisible = Visibility.Visible;
        }

        /// <summary>Disconnects this instance.</summary>
        public void Disconnect()
        {
            stop = true;
            client.Disconnect();
            //CntButtonVisible = Visibility.Visible;
            ResetAll();
        }

        //The dashboard properties
        public double HeadingDeg
        {
            get
            {
                return this.headingDeg;
            }
            set
            {
                this.headingDeg = value;
                NotifyPropertyChanged("HeadingDeg");
            }
        }

        public double AirSpeed
        {
            get
            {
                return this.airSpeed;
            }
            set
            {
                this.airSpeed = value;
                NotifyPropertyChanged("AirSpeed");
            }
        }

        public double VerticalSpeed
        {
            get
            {
                return this.verticalSpeed;
            }
            set
            {
                this.verticalSpeed = value;
                NotifyPropertyChanged("VerticalSpeed");
            }
        }

        public double GroundSpeed
        {
            get
            {
                return this.groundSpeed;
            }
            set
            {
                this.groundSpeed = value;
                NotifyPropertyChanged("GroundSpeed");
            }
        }

        public double RollDeg
        {
            get
            {
                return this.rollDeg;
            }
            set
            {
                this.rollDeg = value;
                NotifyPropertyChanged("RollDeg");
            }
        }

        public double SpeedKt
        {
            get
            {
                return this.speedKt;
            }
            set
            {
                this.speedKt = value;
                NotifyPropertyChanged("SpeedKt");
            }
        }
        public double GpsAltFt
        {
            get
            {
                return this.gpsAltFt;
            }
            set
            {
                this.gpsAltFt = value;
                NotifyPropertyChanged("GpsAltFt");
            }
        }

        public double PitchDeg
        {
            get
            {
                return this.pitchDeg;
            }
            set
            {
                this.pitchDeg = value;
                NotifyPropertyChanged("PitchDeg");
            }
        }
        public double IndicAlt
        {
            get
            {
                return this.indicAlt;
            }
            set
            {
                this.indicAlt = value;
                NotifyPropertyChanged("IndicAlt");
            }
        }

        //Objects that on map properties
        public double Lon
        {
            get
            {
                return this.lon;
            }
            set
            {
                if (this.lon != value)
                {
                    Location = new Location(lat, value);
                    this.lon = value;
                    this.fakeLon = value;

                    this.NotifyPropertyChanged("Lon");
                }
            }
        }
        public double Lat
        {
            get
            {
                return this.lat;
            }
            set
            {
                this.lat = value;
                Location = new Location(value, lon);
                this.NotifyPropertyChanged("Lat");
            }
        }

        //The location of the plane it composed of lat and lon
        public Location Location
        {
            get
            {
                return this.location;
            }
            set
            {
                location = value;
                Location.Altitude = lat;
                Location.Longitude = lon;
                this.NotifyPropertyChanged("Location");
            }
        }

        public Visibility Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                visible = value;
                this.NotifyPropertyChanged("Visible");
            }
        }
        public Visibility CntButtonVisible
        {
            get
            {
                return this.cntButtonVisible;
            }
            set
            {
                // Console.WriteLine("inisde get"+value);
                cntButtonVisible = value;
                this.NotifyPropertyChanged("CntButtonVisible");
            }
        }

        //string that holds the error messages
        public String Errormsg
        {
            get
            {
                return this.errormsg;
            }
            set
            {
                errormsg = value;
                this.NotifyPropertyChanged("Errormsg");
            }
        }

        /// <summary>Notifies the property changed.</summary>
        /// <param name="propName">Name of the property.</param>
        public void NotifyPropertyChanged(String propName)
        {

            if (this.PropertyChanged != null)
            {
                if (propName.Equals("Lon") || propName.Equals("Lat"))
                {
                    //check if it goes to set method-it should
                    location.Longitude = Lon;
                    location.Altitude = Lat;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Location"));
                }//well se the name of the changed property
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        //will send alot of get requests to the server and will update the propeties
        /// <summary>Starts this instance.</summary>
        public void Start()
        {
            new Thread(delegate ()
            {
                try
                {
                    bool legalLat = false;
                    bool legalLon = false;
                    bool diss = false;
                    string mes = "get /instrumentation/heading-indicator/indicated-heading-deg\nget /instrumentation/gps/indicated-vertical-speed\nget /instrumentation/gps/indicated-ground-speed-kt\n";
                    mes += "get /instrumentation/airspeed-indicator/indicated-speed-kt\nget /instrumentation/gps/indicated-altitude-ft\nget /instrumentation/attitude-indicator/internal-roll-deg\n";
                    mes += "get /instrumentation/attitude-indicator/internal-pitch-deg\nget /instrumentation/altimeter/indicated-altitude-ft\nget /position/latitude-deg\nget /position/longitude-deg\n";
                    while (!stop)
                    {
                        string buff = "";
                        string s = "";
                        string index;
                        mutex.WaitOne();
                        client.Write(mes);
                        var i = 0;
                        index = i.ToString();
                        while (i < 10)
                        {
                            if (s == "dissc")
                            {
                                Disconnect();
                                diss = true;
                                break;
                            }
                            s = client.ReadLinee();
                            if (s != "slowloris" && s!="dissc")
                            {

                                switch (index)
                                {
                                    case "0":
                                        if (s == "ERR" || s == "slowloris" || IsLegalNumber(s) == false)
                                        {
                                            if (s == "ERR" || IsLegalNumber(s) == false)
                                            {
                                                Errormsg += "An ERR value has returned from the server in Heading(deg)\n";
                                            }
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //Heading(deg)
                                                HeadingDeg = Double.Parse(s);
                                            }

                                        }
                                        break;
                                    case "1":
                                        if (s == "ERR" || s == "slowloris" || IsLegalNumber(s) == false)
                                        {
                                            if (s == "ERR" || IsLegalNumber(s) == false)
                                            {
                                                Errormsg += "An ERR value has returned from the server in vertical speed\n";
                                            }
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //Heading(deg)
                                                VerticalSpeed = Double.Parse(s);
                                            }

                                        }
                                        break;
                                    case "2":
                                        if (s == "ERR" || IsLegalNumber(s) == false)
                                        {
                                            Errormsg += "An ERR value has returned from the server in ground speed\n";
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //ground speed
                                                GroundSpeed = Double.Parse(s);
                                            }
                                        }
                                        break;
                                    case "3":
                                        if (s == "ERR" || IsLegalNumber(s) == false)
                                        {
                                            Errormsg += "An ERR value has returned from the server in air speed\n";
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //air speed
                                                AirSpeed = Double.Parse(s);
                                            }

                                        }
                                        break;
                                    case "4":
                                        if (s == "ERR" || IsLegalNumber(s) == false)
                                        {
                                            Errormsg += "An ERR value has returned from the server in altitude\n";
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //altitude
                                                GpsAltFt = Double.Parse(s);
                                            }
                                        }
                                        break;
                                    case "5":
                                        if (s == "ERR" || IsLegalNumber(s) == false)
                                        {
                                            Errormsg += "An ERR value has returned from the server in roll\n";
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //roll
                                                RollDeg = Double.Parse(s);
                                            }

                                        }
                                        break;
                                    case "6":
                                        if (s == "ERR" || IsLegalNumber(s) == false)
                                        {
                                            Errormsg += "An ERR value has returned from the server in pitch\n";
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //pitch
                                                PitchDeg = Double.Parse(s);
                                            }
                                        }
                                        break;
                                    case "7":
                                        if (s == "ERR" || IsLegalNumber(s) == false)
                                        {
                                            Errormsg += "An ERR value has returned from the server in altimeter\n";
                                        }
                                        else
                                        {
                                            if (s != "slowloris")
                                            {
                                                //altimeter
                                                IndicAlt = Double.Parse(s);
                                            }
                                        }
                                        break;
                                }

                                if (i == 8 || i == 9)
                                {
                                    buff += s;
                                    buff += '\n';
                                }

                                i++;
                                index = i.ToString();
                            }
                        }
                        mutex.ReleaseMutex();
                        if (diss == true)
                        {
                            break;
                        }
                        string[] stringSeparators = new string[] { "\n" };
                        string[] lines = buff.Split(stringSeparators, StringSplitOptions.None);
                        if (lines[0] == "ERR" || IsLegalNumber(lines[0]) == false)
                        {
                            Errormsg += "An ERR value has returned from the server in latitiude\n";
                        }
                        else
                        {
                            if (lines[0] != "slowloris" && lines[0]!="dissc")
                            {
                                this.fakeLat = double.Parse(lines[0]);

                                if (Double.Parse(lines[0]) <= -90 || Double.Parse(lines[0]) >= 90)
                                {
                                    legalLat = false;
                                    //Console.WriteLine("here");
                                    Errormsg += "The latitude value is not in range!\n";
                                }
                                else
                                {
                                    legalLat = true;
                                    Lat = Double.Parse(lines[0]);
                                }
                            }
                        }

                        if (lines[1] == "ERR" || IsLegalNumber(lines[1]) == false)
                        {
                            Errormsg += "An ERR value has returned from the server in longtitude\n";
                        }

                        else
                        {
                            if (lines[1] != "slowloris" && lines[1] != "dissc")
                            {
                                this.fakeLon = double.Parse(lines[1]);

                                if (Double.Parse(lines[1]) <= -180 || Double.Parse(lines[1]) >= 180)
                                {
                                    legalLon = false;
                                    Errormsg += "The longitude value is not in range!\n";
                                }
                                else
                                {
                                    legalLon = true;
                                    Lon = Double.Parse(lines[1]);
                                }
                            }
                        }

                        if (lines[0] != "ERR" && lines[1] != "slowloris" && lines[0] != "dissc" && lines[1] != "dissc" && lines[1] != "ERR" &&
                            lines[0] != "slowloris" && IsLegalNumber(lines[0]) == true && IsLegalNumber(lines[1]) == true)
                        {
                            if ((Double.Parse(lines[0]) <= -90 || Double.Parse(lines[0]) >= 90) &&
                               (Double.Parse(lines[1]) <= -180 || Double.Parse(lines[1]) >= 180))
                            {
                                //Visible = Visibility.Hidden;
                                Errormsg += "The plane is out of range!\n";
                            }
                            else if (((Double.Parse(lines[0]) <= -90 || Double.Parse(lines[0]) >= 90) ||
                              (Double.Parse(lines[1]) <= -180 || Double.Parse(lines[1]) >= 180)))
                            {
                                //Visible = Visibility.Hidden;
                                Errormsg += "Illegal latitude or longtitude!\n";
                            }
                        }

                        //Make the plane visible
                        if (lines[1] != "slowloris" && lines[0] != "dissc" && lines[1] != "dissc" && lines[0] != "slowloris" && lines[0] != "ERR" && lines[1] != "ERR" &&
                            first == true && (legalLat == true && legalLon == true) && IsLegalNumber(lines[0]) == true && IsLegalNumber(lines[1]) == true)
                        {
                            Visible = Visibility.Visible;
                            first = false;
                        }

                        if (lines[1] != "slowloris" && lines[0] != "slowloris" && lines[0] != "ERR" && lines[1] != "ERR" && lines[0] != "dissc" && lines[1] != "dissc"&&
                          first == true && (legalLat == false || legalLon == false) && IsLegalNumber(lines[0]) == true && IsLegalNumber(lines[1]) == true)
                        {
                            Visible = Visibility.Hidden;
                            first = false;
                        }

                        if(lines[0]=="dissc"|| lines[1] == "dissc")
                        {
                            Disconnect();
                        }
                      
                        Thread.Sleep(250);
                    }
                }

                catch (Exception)
                {
                    Errormsg += "Something went worng! Please check connection\n";
                }

            }).Start();

            first = true;
        }
         
        /// <summary>Determines whether [is legal number] [the specified check string].</summary>
        /// <param name="checkStr">The check string.</param>
        /// <returns>
        ///   <c>true</c> if [is legal number] [the specified check string]; otherwise, <c>false</c>.</returns>
        public bool IsLegalNumber(string checkStr)
        {
            try
            {
                // If its slowloris ill return true-always -we dont get slowloris from the server im the one who sending it
                if (checkStr == "slowloris"|| checkStr=="dissc")
                {
                    return true;
                }

                double.Parse(checkStr);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Sets the rudder.</summary>
        /// <param name="val">The value.</param>
        public void SetRudder(double val)
        {
            string str = "set /controls/flight/rudder " + val + "\n";

            SetData(str);
        }

        /// <summary>Sets the elevator.</summary>
        /// <param name="val">The value.</param>
        public void SetElevator(double val)
        {
            string str = "set /controls/flight/elevator " + val + "\n";

            SetData(str);
        }

        /// <summary>Sets the throttle.</summary>
        /// <param name="val">The value.</param>
        public void SetThrottle(double val)
        {
            string str = "set /controls/engines/current-engine/throttle " + val + "\n";

            SetData(str);
        }

        /// <summary>Sets the aileron.</summary>
        /// <param name="val">The value.</param>
        public void SetAileron(double val)
        {
            string str = "set /controls/flight/aileron " + val + "\n";

            SetData(str);
        }

        /// <summary>Sets the data.</summary>
        /// <param name="path">The path.</param>
        public void SetData(string path)
        {
            try
            {
                if (this.mutex.WaitOne(1))
                {
                    this.client.Write(path);
                    string ans = this.client.ReadLinee();
                    this.mutex.ReleaseMutex();

                    if (ans == "ERR" || ans == "ERR\n" || !IsLegalNumber(ans))
                    {
                        Errormsg += "Server says: Error setting value (Joystick or Slider) to server" + "\n";
                    }
                }

                else
                {
                    this.timer.Interval = TimeSpan.FromSeconds(1);
                    Errormsg += "Server says: Error writting (set) value (Joystick or Slider) to server" + "\n";
                }
            }

            catch (Exception)
            {
                this.timer.Interval = TimeSpan.FromSeconds(10);
                string msg = "Server says: " + "Information Error" + "\n";
                Errormsg += msg;
            }
        }

        /// <summary>Returns the current visible</summary>
        public Visibility GetVisible()
        {
            return this.visible;
        }

        /// <summary>Gets the fake latitude.</summary>
        /// <returns></returns>
        public double GetFakeLatitude()
        {
            return this.fakeLat;
        }

        /// <summary>Gets the fake longitude.</summary>
        /// <returns></returns>
        public double GetFakeLongitude()
        {
            return this.fakeLon;
        }
    }
}