using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SCTFanControl.FanProfiles
{
    class Override: IProfile
    {
        Dictionary<int, int> points = new Dictionary<int,int>();

        private int _intervalMs;
        private int _safe_temperature;
        private int _trip_temperature;
        private int _fanspeed;
        private string _name;

        public Override(XmlNode config)
        {
            _name = config.SelectSingleNode("name").InnerText;
            _intervalMs = int.Parse(config.SelectSingleNode("interval").InnerText);
            _safe_temperature = int.Parse(config.SelectSingleNode("safetemp").InnerText);
            _trip_temperature = int.Parse(config.SelectSingleNode("triptemp").InnerText);
            _fanspeed = int.Parse(config.SelectSingleNode("steadyspeed").InnerText);

            if (intervalMs > 2000 || intervalMs < 500) 
                throw new Exception(String.Format("{0}: invalid interval (must be 500~2000ms)", name, intervalMs));
            if (_safe_temperature < 0 || _safe_temperature > 60)
                throw new Exception(String.Format("{0}: invalid safe temperature (must be 0~60)", name, _safe_temperature));
            if (_trip_temperature < 0 || _trip_temperature > 80)
                throw new Exception(String.Format("{0}: invalid safe temperature (must be 0~80)", name, _trip_temperature));
            if (_fanspeed < 0) 
                throw new Exception(String.Format("{0}: invalid fanspeed", name, _fanspeed));
        }

        #region IProfile Members

        public int intervalMs
        {
            get { return _intervalMs; }
        }

        public int safe
        {
            get { return _safe_temperature; }
        }

        public int trip
        {
            get { return _trip_temperature; }
        }

        public int speed
        {
            get { return _fanspeed; }
        }

        public string name
        {
            get { return _name; }
        }

        #endregion
    }
}
