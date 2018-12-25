using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;

namespace QubitControl
{
    public partial class frmMain : Form, IMessageFilter
    {
        private string _arg;
        private bool ScriptsInjected = false;

        public frmMain()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            controlsToMove.Add(this.pnlTitleBar); //Add whatever controls here you want to move the form when it is clicked and dragged
        }
        public frmMain(string arg)
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            controlsToMove.Add(this.pnlTitleBar); //Add whatever controls here you want to move the form when it is clicked and dragged
            _arg = arg;
        }
        private void form_Load(object sender, EventArgs e)
        {
            webBro.Navigate(Settings.btCompleteURL); 
        }
        private bool ArgsProvided()
        {
            if (_arg is null)
                return false;
            if (_arg.Equals(string.Empty))
                return false;

            return true;
        }

        #region "HTTP/HTML works"
        private void webBro_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!ArgsProvided()) return;

            switch(e.Url.AbsolutePath)
            {
                case "/":
                    InjectSripts(webBro.Document);
                    InvokeScript();
                    break;
                case "/download.html":
                    ProcessMagnet();
                    break;
                case "/upload.html":
                    ProcessTorrent();
                    break;
                default:
                    return;
            }
        }

        private void ProcessTorrent()
        {
            HtmlDocument doc = this.webBro.Document.Window.Frames["uploadPage_iframe"].Document;
            HtmlElement fs = doc.GetElementById("fileselect");
            SelectFile();
            fs.InvokeMember("Click");
        }

        private async void SelectFile()
        {
            await Task.Delay(1000);
            SendKeys.Send(_arg);
            SendKeys.SendWait("{ENTER}");
        }
        private void ProcessMagnet()
        {
            HtmlDocument doc = this.webBro.Document.Window.Frames["downloadPage_iframe"].Document;
            doc.GetElementById("urls").SetAttribute("Value", _arg);
        }

        private void InvokeScript()
        {
            if(_arg.Substring(0,6)=="magnet")
                webBro.Document.InvokeScript("OpenMagnetLink");
            else
                webBro.Document.InvokeScript("OpenTorrentFile");
        }

        private void InjectSripts(HtmlDocument document)
        {
            if (ScriptsInjected) return;
            HtmlElement head = webBro.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = webBro.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = Scripts();
            head.AppendChild(scriptEl);
            ScriptsInjected = true;
        }

        private string Scripts()
        {

            string s1 = @"function OpenTorrentFile()
            {
                new MochaUI.Window(
                    {
                        id: 'uploadPage',
                        title: ""Upload local torrent"",
                        loadMethod: 'iframe',
                        contentURL: 'upload.html',
                        addClass: 'windowFrame', // fixes iframe scrolling on iOS Safari
                        scrollbars: true,
                        maximizable: false,
                        paddingVertical: 0,
                        paddingHorizontal: 0,
                        width: 500,
                        height: 400

                    });
                updateMainData();
            };";

            string s2 = @"function OpenMagnetLink()
            {
                new MochaUI.Window(
                    {
                        id: 'downloadPage',
                        title: 'Download from URLs',
                        loadMethod: 'iframe',
                        contentURL: 'download.html',
                        addClass: 'windowFrame', // fixes iframe scrolling on iOS Safari
                        scrollbars: true,
                        maximizable: false,
                        paddingVertical: 0,
                        paddingHorizontal: 0,
                        width: 500,
                        height: 550

                    });
                updateMainData();
            };";

            return s1 + "\r\n" + s2;


        }
        #endregion


        #region "IMessageFilter, moving form and Pseudo Title Bar buttons events"
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;

        [DllImportAttribute("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, new IntPtr(0));
                return true;
            }
            return false;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            frmSettings f = new frmSettings();
            f.ShowDialog(this);
        }
        #endregion



 
    }
}

