using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace QR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboDevice.Items.Add(filterInfo.Name);
            cboDevice.SelectedIndex = 0;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);

            captureDevice.NewFrame += captureDevice_NewFrame;
            captureDevice.VideoResolution = captureDevice.VideoCapabilities[6];

            /*for (int i = 0; i < captureDevice.VideoCapabilities.Length; i++)
            {

                string resolution = "Resolution Number " + Convert.ToString (i);
                string resolution_size = captureDevice.VideoCapabilities[i].FrameSize.ToString();

            }*/
            captureDevice.Start();
            timer1.Start();
        }

        private void captureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
                captureDevice.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox.Image!=null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox.Image);
                if (result!=null)
                {
                    txtQRCode.Text = result.ToString();
                    timer1.Stop();
                    if (captureDevice.IsRunning)
                        captureDevice.Stop();
                }
            }
        }

       
    }
}
