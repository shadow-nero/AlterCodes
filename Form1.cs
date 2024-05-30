using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlterAFS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using(OpenFileDialog AFS = new OpenFileDialog())
            {
                AFS.Title = "Select AFS (PS2)";
                AFS.Filter = "AFS Files (*.afs)|*.afs|All Files (*.*)|*.*";
                AFS.FileName = "Data.afs";

                using(OpenFileDialog EBOOT = new OpenFileDialog())
                {
                    EBOOT.Title = "Select SLPM_551.08 (PS2)";
                    EBOOT.Filter = "SLPM_551.08 (*.08)|*.08|All Files (*.*)|*.*";
                    EBOOT.FileName = "SLPM_551.08";

                    if (AFS.ShowDialog() == DialogResult.OK && EBOOT.ShowDialog() == DialogResult.OK)
                    {


                        string Dest = "Fate UC (PS2)\\EXTRACTED\\" + Path.GetFileNameWithoutExtension(AFS.FileName).ToLower();

                        if (Directory.Exists(Dest) == false)
                        {
                            Directory.CreateDirectory(Dest);
                        }

                        string[] Paths = EbootPath.DecompileEboot(EBOOT.FileName, AFS.FileName.ToLower());

                        AFSUnpacker.AFSExtract(AFS.FileName, Dest, Paths);
                        MessageBox.Show("Done!", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    //AFSPacker.AFSRepack("BGM", "BGM01.AFS", Paths);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog EBOOT = new OpenFileDialog())
            {
                EBOOT.Title = "Select SLPM_551.08 (PS2)";
                EBOOT.Filter = "SLPM_551.08 (*.08)|*.08|All Files (*.*)|*.*";
                EBOOT.FileName = "SLPM_551.08";

                if (EBOOT.ShowDialog() == DialogResult.OK)
                {
                    string Dest = "Fate UC (PS2)\\REPACK\\";
                    if (Directory.Exists(Dest) == false)
                    {
                        Directory.CreateDirectory(Dest);
                    }

                    foreach (string afsRepack in Directory.GetDirectories("Fate UC (PS2)\\EXTRACTED")) {


                        string[] Paths = EbootPath.DecompileEboot(EBOOT.FileName, Path.GetFileNameWithoutExtension(afsRepack) + ".afs");
                        //MessageBox.Show(Path.GetFileNameWithoutExtension(afsRepack), "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        AFSPacker.AFSRepack(afsRepack, Dest + Path.GetFileNameWithoutExtension(afsRepack) + ".afs", Paths);
                    }
                    MessageBox.Show("Done!", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //AFSPacker.AFSRepack("BGM", "BGM01.AFS", Paths);
            }
        }
    }
}
