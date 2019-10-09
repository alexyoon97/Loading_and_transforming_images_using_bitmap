using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDIDrawer;
using System.IO;
using System.Threading;



namespace lab4
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            
        }

        //initiallize variables
        Bitmap _image; CDrawer _canvas; Thread thOne;  Thread thBW; Thread thFlip;
        int BW = 1;
        delegate void delVoidInt(int i);
        delegate void delVoidVoid();


        private void UI_Load_Click(object sender, EventArgs e)
        {
 
            if (openFileDialog1.ShowDialog() == DialogResult.OK)//open opendialog when load button clicked
            {
                _image = new Bitmap(openFileDialog1.FileName);//initiallize new instance of the specific file
                _canvas = new CDrawer(_image.Width, _image.Height);//set window size as image of height and width
                thOne = new Thread(new ThreadStart(RunOriginal));//initiallize new thread for original copy
                thOne.IsBackground = true;//background thread on
                thOne.Start();//start origianl thread
            }
        }
        

        private void UI_Transform_Click(object sender, EventArgs e)
        {
            //make one more bitmap variable and copy original
            Bitmap changedimage = _image;
            //initiallize new threads for chaning options and make thread runs on background
            thBW = new Thread(new ParameterizedThreadStart(RunBlackWhite));
            thFlip = new Thread(new ParameterizedThreadStart(RunFlip)); 
            thOne = new Thread(new ThreadStart(RunOriginal));
            thBW.IsBackground = true;
            thFlip.IsBackground = true;

            //if flip radio button is checked start flip thread
            if (UI_rbFlip.Checked)
            {
                thFlip.Start(changedimage);
            }
            //if black and white radio button is checked start flip thread
            else if (UI_rbBlacknWhite.Checked)
            {
                //this BW variable is for black and white and divide RGB color by BW
                BW = 3;
                thBW.Start(changedimage);

                //if original radio button is checked start flip thread
            }
            else if (UI_rbOrg.Checked)
            {
                //this BW variable is for black and white and divide RGB color by BW
                BW = 1;
                thOne.Start();

            }
            
        }
        public void RunOriginal()
        {
            try
            {
                //copy opend image information onto image variable
                _image = new Bitmap(openFileDialog1.FileName);

                for (int x = 0; x < _image.Width; x++)
                {
                    //while x value goes up make progress bar follows
                    Invoke(new delVoidInt(cbUpdateProgress), x);
                    for (int y = 0; y < _image.Height; y++)
                    {
                        //draw image point by point
                        _canvas.SetBBPixel(x, y, _image.GetPixel(x, y));
                        
                    }
                }
                //finish prograss bar
                Invoke(new delVoidVoid(cbUpdateComplete));
            }
            catch (Exception ex)
            {
                //if cath an exception goes here
                MessageBox.Show(ex.Message, ("GDIImage Example"));
            }
        }

        public void RunBlackWhite(object changedimage)
        {
            //make bitmap variable and copy thread's parameter
            Bitmap changed = (Bitmap)changedimage;
            for (int x = 0; x < changed.Width; x++)
            {
                //while x value goes up make progress bar follows
                Invoke(new delVoidInt(cbUpdateProgress), x);
                for (int y = 0; y < changed.Height; y++)
                {
                    //invoke image pixels and divide by 3 RGB will decrease to make black and white color and put on the gdi drawer
                    Color pixelColor = changed.GetPixel(x, y);
                    int newColor = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / BW);
                    _canvas.SetBBPixel(x, y, Color.FromArgb(newColor, newColor, newColor));
                }
            }
            //finish the prograss bar
            Invoke(new delVoidVoid(cbUpdateComplete));
        }

        public void RunFlip(object changedimage)
        {
            //make bitmap variable and copy thread's parameter
            Bitmap changed = (Bitmap)changedimage;
            try
            {
                //flip the image respect on x
                changed.RotateFlip(RotateFlipType.RotateNoneFlipY);
                for (int x = 0; x < changed.Width; x++)
                {
                    //while x value goes up make progress bar follows
                    Invoke(new delVoidInt(cbUpdateProgress), x);
                    for (int y = 0; y < changed.Height; y++)
                    {
                        //if black and white is not operate just print the image
                        if (BW == 1)
                            _canvas.SetBBPixel(x, y, _image.GetPixel(x, y));
                        else if (BW == 3)
                        {
                            //if black and white is operated set color divide by 3 to decrease RGB color and shows black and white
                            Color pixelColor = changed.GetPixel(x, y);
                            int newColor = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / BW);
                            _canvas.SetBBPixel(x, y, Color.FromArgb(newColor, newColor, newColor));
                        }

                        
                    }
                }
                Invoke(new delVoidVoid(cbUpdateComplete));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ("GDIImage Example"));
            }
        }
        private void cbUpdateProgress(int i)
        {
            //when the program is on progress disable buttons and set progress bar maximum value repect on image width and increase the progress bar by i
            UI_prgbar.Maximum = _image.Width;
            UI_Load.Enabled = false;
            UI_Transform.Enabled = false;
            UI_rbBlacknWhite.Enabled = false;
            UI_rbFlip.Enabled = false;
            UI_rbOrg.Enabled = false;
            UI_prgbar.Value = i;
        }
        private void cbUpdateComplete()
        {
            //when the program is finished enable the buttons
            UI_Load.Enabled = true;
            UI_Transform.Enabled = true;
            UI_rbBlacknWhite.Enabled = true;
            UI_rbFlip.Enabled = true;
            UI_rbOrg.Enabled = true;
        }

    }
    
}
