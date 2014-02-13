using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using SCTFanControl.FanProfiles;
using System.Collections;
using System.Collections.Generic;

namespace SCTFanControl
{
    class Config
    {
        private List<IProfile> _profiles = new List<IProfile>();
        private IProfile _defaultProfile = null;

        public Config(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            //profiles
            foreach (XmlNode profile in doc.SelectNodes("//fanspeed/profile"))
            {
                profiles.Add(new Override(profile));
                //default profile
                if (profile.Attributes["default"] != null && profile.Attributes["default"].Value.ToLower() == "true") _defaultProfile = profiles[profiles.Count - 1];
            }
        }

        public List<IProfile> profiles
        {
            get { return _profiles; }
        }

        public IProfile defaultProfile
        {
            get { return _defaultProfile; }
        }
    }
}
