using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;


namespace PAAtoPNG
{
    public partial class Form1 : Form
    {
        string[] dirs;
        int dirslength;
        string pal2pacepath = "";
        string sourcefolder = "";
        string destinationfolder = "";
        bool sourceSet = false;
        bool destinationSet = false;
        bool pal2paceSet = false;
        bool savesource = false;
        bool savedest = false;
        string stringsavesource = "false";
        string stringsavedest = "false";
        public class ReadWriteINIfile
        {
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string name, string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            public string path;

            public ReadWriteINIfile(string inipath)
            {
                path = inipath;
            }
            public void WriteINI(string name, string key, string value)
            {
                WritePrivateProfileString(name, key, value, this.path);
            }
            public string ReadINI(string name, string key)
            {
                StringBuilder sb = new StringBuilder(255);
                int ini = GetPrivateProfileString(name, key, "", sb, 255, this.path);
                return sb.ToString();
            }
        }

        public Form1()
        {
            InitializeComponent();
            var IniSettings = new IniFile("Settings.ini");
            pal2pacepath = IniSettings.Read("Pac", "Settings");
            textBox1.Text = pal2pacepath;
            sourcefolder = IniSettings.Read("Source", "Settings");
            textBox4.Text = sourcefolder;
            destinationfolder = IniSettings.Read("Dest", "Settings");
            textBox6.Text = destinationfolder;
            destinationfolder = IniSettings.Read("SaveSource", "Settings");
            checkBox1.Checked = savesource;
            destinationfolder = IniSettings.Read("SaveDest", "Settings");
            checkBox1.Checked = savedest;
            if (savesource == true)
            {
                checkBox1.Checked = true;
            }
            if (savedest == true)
            {
                checkBox2.Checked = true;
            }
            //public string stringsavesource = System.Convert.ToString(savesource);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("Settings.ini"))
            {
                MessageBox.Show("Settings Found!");
            }
            else
            {
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("Pac", "", "Settings");
                IniSettings.Write("Source", "", "Settings");
                IniSettings.Write("Dest", "", "Settings");
                IniSettings.Write("SaveSource", "No", "Settings");
                IniSettings.Write("SaveDest", "No", "Settings");
            }
        }

        //Set the path to Pal2PacE.exe **REQUIRED**
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pal2pacepath = openFileDialog1.FileName;
                string pal2pacefile = "Pal2PacE.exe";
                bool pal2paceselected = false;

                
                if (pal2pacepath.Contains(pal2pacefile))
                {
                    pal2paceselected = true;
                }

                if (pal2paceselected == true)
                {
                    textBox1.Text = pal2pacepath;
                    pal2paceSet = true;
                    var IniSettings = new IniFile("Settings.ini");
                    IniSettings.Write("Pac", pal2pacepath, "Settings");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        //Set the source folder that contains the paa files to convert
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();


            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sourcefolder = folderBrowserDialog1.SelectedPath;
                textBox4.Text = sourcefolder;
                sourceSet = true;
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("Source", sourcefolder, "Settings");
            }

        }

        //get all paa files in directory
        public void getpaalist()
        {
            if (sourceSet == true)
            {
                dirs = Directory.GetFiles(@sourcefolder, "*.paa*");
                Console.WriteLine("The number of paa files is {0}.", dirs.Length);
                //string message = "The number of paa files is {0}.", dirs.Length();
                dirslength = dirs.Length;
            }
            else
            {
                Console.WriteLine("Source path not set!");
            }
        }

        //Define the folder to deposit converted images from source folder
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog2 = new FolderBrowserDialog();


            if (folderBrowserDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                destinationfolder = folderBrowserDialog2.SelectedPath;
                textBox6.Text = destinationfolder;
                destinationSet = true;
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("Dest", destinationfolder, "Settings");
            }
        }

        //Run the Pal2PacE.exe using the files from source, and place them in destination folder
        private void button4_Click(object sender, EventArgs e)
        {
            getpaalist();
            if (pal2paceSet == true | destinationSet == true | sourceSet == true)
            {
                for (int i = 0; i < dirslength; i++)
                {
                    string current = dirs[i];
                    string output = Path.GetFileName(current).Replace("paa", "png");
                    string strCmdText;
                    strCmdText = pal2pacepath;
                    var converter = System.Diagnostics.Process.Start(strCmdText, "\"" + current + "\"" + " " + "\"" + destinationfolder + "\\" + output + "\"");
                    converter.Close();
                    Array.ForEach(dirs, Console.WriteLine);                  
                }             
            }
            else
            {
                Console.WriteLine("Paths not set!");
            }
            MessageBox.Show(dirslength + "" + "files converted", dirs.Length.ToString()); //Show user how many files converted
        }

        //Checkbox to save current path
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                savesource = true;
                stringsavesource = "Yes";
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("SaveSource", stringsavesource, "Settings");
            }
            else
            {
                savesource = false;
                stringsavesource = "No";
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("SaveSource", stringsavesource, "Settings");
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                savedest = true;
                stringsavedest = "Yes";
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("SaveDest", stringsavedest, "Settings");
            }
            else
            {
                savedest = false;
                stringsavedest = "No";
                var IniSettings = new IniFile("Settings.ini");
                IniSettings.Write("SaveDest", stringsavedest, "Settings");
            }
        }

        //Checkbox to save current path
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
