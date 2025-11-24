namespace Delegates
{
    internal class SmartHome
    {

        public class SmartHomeEventArgs(string location, DateTime time, int intensity) : EventArgs
        {
            public string Location { get; } = location;
            public DateTime Time { get; } = time;
            public int Intensity { get; } = intensity;
        }

        //Publisher
        public class MotionDector
        {

            public enum MotionIntesityType
            {
                Low,
                Medium,
                High
            }

            public event EventHandler<SmartHomeEventArgs>? MotionChanged;
            private bool _motionDetected = false;
            public MotionIntesityType MotionType { get; set; }


            protected void OnMotion(SmartHomeEventArgs e)
            {
                MotionChanged?.Invoke(this, e);
            }

            public void DetectMotion(string location, int intesity)
            {
                if (intesity > 50)
                {
                    _motionDetected = true;
                    MotionType = MotionIntesityType.High;
                    Console.WriteLine($"WARNING SOMEON IS TRYING TO GET IN FROM THE LOCATION: {location}");
                    OnMotion(new SmartHomeEventArgs(location, DateTime.Now, intesity));

                }
                else if (intesity >= 25)
                {
                    _motionDetected = true;
                    MotionType = MotionIntesityType.Medium;
                    Console.WriteLine($"{location} there was an intesity of {intesity}");
                    OnMotion(new SmartHomeEventArgs(location, DateTime.Now, intesity));
                }
                else
                {
                    Console.WriteLine($"No Intesity Found !");
                }
            }
        }

        //Subscriber
        public class LightsController
        {
            private bool _lights_spamming = false;
            private bool _basic_lights = false;
            public bool IsSpammingLightsOn { get => _lights_spamming; set => _lights_spamming = value; }
            public bool IsBasicLightsOn { get => _basic_lights; set => _basic_lights = value; }

            public void Lights(object? sender, SmartHomeEventArgs e)
            {
                if (e.Intensity >= 50)
                {
                    _lights_spamming = true;
                    Console.WriteLine($"[DANGER] Spamming Lights someon is inside the {e.Location}");
                }
                else if (e.Intensity >= 25)
                {
                    _lights_spamming = false;
                    _basic_lights = true;
                    Console.WriteLine($"[WARNING] Basic Lights Are on Heard noise in the {e.Location}");
                }
                else
                {
                    _lights_spamming = false;
                    _basic_lights = false;
                    Console.WriteLine("All Lights Are Off");
                }

            }
        }

        public class SecuritySystemController
        {
            private bool _is_alrm_on = false;

            private bool _armed = false;
            public bool IsAlarmOn { get => _is_alrm_on; set => _is_alrm_on = value; }
            public bool IsArmed { get => _armed; set => _armed = value; }

            public void Security(object? sender, SmartHomeEventArgs e)
            {

                if (e.Intensity >= 50)
                {
                    _is_alrm_on = true;
                    _armed = true;

                    Console.WriteLine($"[DANGER Arming House & Locking Doors] Someon is trying to get from the {e.Location}");
                }
                else if (e.Intensity >= 25)
                {
                    _is_alrm_on = true;
                    _armed = false;

                    Console.WriteLine($"[WARNIGN] Please Check the Cameras some intesity Came from {e.Location} ");
                }
                else
                {
                    _is_alrm_on = false;
                    _armed = false;
                    Console.WriteLine("No Security Measure On");
                }
            }
        }

        static void Main(string[] args)
        {
            MotionDector motion = new();
            LightsController lights = new();
            SecuritySystemController securitySystem = new SecuritySystemController();

            motion.MotionChanged += lights.Lights;
            motion.MotionChanged += securitySystem.Security;

            motion.DetectMotion("Living Room", 25);
        }
    }
}
