using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace MultiClipboard
{


    public partial class Form1 : Form
    {
        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;


        public string clipboardText = "";

        private int pictureIndex = 0;
        private int textIndex = 0;

        private Blue.Windows.StickyWindow stickyWindow;
        PictureBox[] pictureBoxes;

        public Form1()
        {
            stickyWindow = new Blue.Windows.StickyWindow(this);
            stickyWindow.StickToScreen = true;
            InitializeComponent();
            AddClipboardFormatListener(this.Handle);
            pictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12 };

            foreach(PictureBox pb in pictureBoxes)
            {
                //add the same event handler to all the picture boxes
                pb.Click += pictureBoxes_CommonClickHandler;
            }
            this.Size = new Size(360, Int32.Parse(Screen.PrimaryScreen.Bounds.Height.ToString()));
            this.StartPosition = FormStartPosition.Manual;
            this.Left = Int32.Parse(Screen.PrimaryScreen.Bounds.Width.ToString()) - 360;
            this.Top = 0;
        }

        private void clipboardSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var copiedElement = clipboardSelector.Items[clipboardSelector.SelectedIndex].ToString();
            Clipboard.SetDataObject(copiedElement);
        }

        private void pictureBoxes_CommonClickHandler(object sender, EventArgs e)
        {
            PictureBox pib = sender as PictureBox; //to avoid declaring PictureBox as a parameter, because there is no overload for event handling accepting 3 args
            List<Image> pbs = new List<Image>();
            var selectedImage = pib.BackgroundImage;
            for (var i = 0; i <= 11; i++)
            {
                if(pictureBoxes[i].BackgroundImage != null)
                {
                    pbs.Add(pictureBoxes[i].BackgroundImage);
                }
            }

            if ((selectedImage != null) && (!pbs.Contains(selectedImage)))
            {
                Clipboard.SetDataObject(selectedImage);
            }

        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                IDataObject iData = Clipboard.GetDataObject();      // Clipboard data.

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)iData.GetData(DataFormats.Text);
                    // do something with it
                    OnTextCopiedToClipboard(text, textIndex);
                }
                else if (iData.GetDataPresent(DataFormats.Bitmap))
                {
                    
                    Bitmap image = (Bitmap)iData.GetData(DataFormats.Bitmap);
                    // do something with it
                    OnImageCopiedToClipboard(image, pictureIndex);
                }

             //   else if(iData.GetDataPresent(DataFormats.))
            }
        }

        public void OnTextCopiedToClipboard(String text, int i)
        {  
            //limit the number of entries we can have
            if (i >= 12)
            {
                i = 0;
                textIndex = 0;
            }

            //if the same text has been copied to clipboard it wont go onto the list again
            if ((clipboardSelector.Items.IndexOf(text) == -1) && text != "")
            {
                clipboardSelector.Items.Insert(i, text);
                textIndex++;
            }
        }

        public void OnImageCopiedToClipboard(Bitmap image, int i)
        {
            if(i >= 12)
            {
                i = 0;
                textIndex = 0;
            }
            pictureBoxes[i].BackgroundImage = image;
            pictureIndex++;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveClipboardFormatListener(this.Handle);     // remove the window from the clipboard's format listener list.
        }
    }
}
