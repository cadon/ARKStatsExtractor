using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class Kibbles
    {
        [DataMember]
        private string ver = "0.0";

        [DataMember]
        public Dictionary<string, Kibble> kibble = new Dictionary<string, Kibble>();

        public Version version = new Version(0, 0);
        public static Kibbles _K;
        public static Kibbles K
        {
            get
            {
                if (_K == null)
                    _K = new Kibbles();
                return _K;
            }
        }

        public bool loadValues()
        {
            bool loadedSuccessful = true;

            string filename = "json/kibbles.json";

            // check if file exists
            if (!File.Exists(filename))
            {
                if (MessageBox.Show("Kibble-File '" + filename + "' not found. This tool will not show kibble recipes without this file.\n\nDo you want to visit the homepage of the tool to redownload it?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/releases/latest");
                return false;
            }

            _K.version = new Version(0, 0);

            DataContractJsonSerializerSettings s = new DataContractJsonSerializerSettings();
            s.UseSimpleDictionaryFormat = true;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Kibbles), s);
            FileStream file = File.OpenRead(filename);

            try
            {
                _K = (Kibbles)ser.ReadObject(file);
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened or read.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadedSuccessful = false;
            }
            file.Close();

            if (loadedSuccessful)
            {
                try
                {
                    _K.version = new Version(_K.ver);
                }
                catch
                {
                    _K.version = new Version(0, 0);
                }
            }

            return loadedSuccessful;
        }
    }

}
