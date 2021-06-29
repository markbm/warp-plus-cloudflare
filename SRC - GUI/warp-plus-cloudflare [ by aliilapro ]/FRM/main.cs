using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace warp_plus_cloudflare___by_aliilapro__.FRM
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }
        private List<string> ProxyList = new List<string>();
        private static Random rnd = new Random();
        private bool _start;
        private int _error;
        private int _test;
        private int _send;
        private string GenerateUniqCode(int len)
        {
            string _allstring = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string _re = "";
            while (_re.Length < len)
            {
                _re += _allstring[rnd.Next(0, _allstring.Length - 1)].ToString();
            }
            return _re;
        }

        private string GenRndNum(int len)
        {
            string num = "0123456789";
            string num2 = "";
            while (num2.Length < len)
            {
                num2 += num[rnd.Next(0, num.Length - 1)].ToString();
            }
            return num2;
        }

        private void ch_hideid_CheckedChanged(object sender, EventArgs e)
        {
            bool @true = ch_hideid.Checked;
            if (@true)
            {
                txtid.PasswordChar = '*';
            }
            else
            {
                txtid.PasswordChar = '\0';
            }
        }

        private void ch_proxy_CheckedChanged(object sender, EventArgs e)
        {
            bool @true = ch_proxy.Checked;
            if (@true)
            {
                btnloadproxy.Enabled = true;
                btngetproxy.Enabled = false;
            }
            else
            {
                btnloadproxy.Enabled = false;
                btngetproxy.Enabled = true;
            }
        }

        private void btnloadproxy_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File (*.txt)|*.txt|All File (*.*)|*.*";
            openFileDialog.Title = "Open Proxylist ( ONLY HTTP-S )";
            ProxyList.Clear();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ProxyList.AddRange(File.ReadAllLines(openFileDialog.FileName));
                lblproxy.Text = ProxyList.Count.ToString();
            }
        }

        private void btngetproxy_Click(object sender, EventArgs e)
        {
            ProxyList.Clear();
            lblproxy.Text = "0";
            try
            {
                HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all")).GetResponse();
                string end = new StreamReader(response.GetResponseStream()).ReadToEnd();
                MatchCollection matchCollections = new Regex("[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}:[0-9]{1,5}").Matches(end);
                try
                {
                    try
                    {
                        foreach (object obj in matchCollections)
                        {
                            Match objectValue = (Match)RuntimeHelpers.GetObjectValue(obj);
                            ProxyList.Add(objectValue.Value);
                        }
                    }
                    finally
                    {
                    }
                }
                finally
                {
                }
                int List = ProxyList.Count;
                lblproxy.Text = string.Concat(new string[]
                {
                    List.ToString()
                });
                List = ProxyList.Count;
                lblproxy.Text = List.ToString();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            if (ProxyList.Count != 0)
            {
                _start = true;
                int num = Convert.ToInt32(speed.Text);
                ThreadPool.SetMinThreads(num, num);
                ThreadPool.SetMaxThreads(num, num);
                try
                {
                    List<string>.Enumerator enumerator = ProxyList.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(mtd), current);
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                }
            }
            else
            {
                MessageBox.Show("First Load Proxylist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void mtd(object Proxiess)
        {
            bool start = _start;

            if (start)
            {
                try
                {
                    string host = Proxiess.ToString().Split(new char[]
                              {
                        ':'
                              })[0];
                    string value = Proxiess.ToString().Split(new char[]
                    {
                        ':'
                    })[1];
                    WebProxy proxy = new WebProxy(host, Convert.ToInt32(value));
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://api.cloudflareclient.com/v0a{GenRndNum(3)}/reg");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Headers.Add("Accept-Encoding", "gzip");
                    httpWebRequest.ContentType = "application/json; charset=UTF-8";
                    httpWebRequest.Host = "api.cloudflareclient.com";
                    httpWebRequest.KeepAlive = true;
                    httpWebRequest.UserAgent = "okhttp/3.12.1";
                    httpWebRequest.Proxy = proxy;
                    string install_id = GenerateUniqCode(22);
                    string key = GenerateUniqCode(43) + "=";
                    string tos = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fff") + "+07:00";
                    string fcm_token = install_id + ":APA91b" + GenerateUniqCode(134);
                    string referer = txtid.Text;
                    string type = "Android";
                    string locale = "en-GB";
                    var body = new
                    {
                        install_id = install_id,
                        key = key,
                        tos = tos,
                        fcm_token = fcm_token,
                        referrer = referer,
                        warp_enabled = false,
                        type = type,
                        locale = locale
                    };
                    string jsonBody = JsonConvert.SerializeObject(body);
                    using (StreamWriter sw = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        sw.Write(jsonBody);
                    }
                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (StreamReader sw = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string result = sw.ReadToEnd();
                    }
                    httpResponse = null;
                    _test++;
                    _send++;
                    lblgood.Text = _send.ToString();
                    lbltest.Text = _test.ToString();
                    lblgb.Text = lblgood.Text + " GB Successfully added to your account.";
                }
                catch
                {
                    _test++;
                    _error++;
                    lblbad.Text = _error.ToString();
                    lbltest.Text = _test.ToString();
                }

            }
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            _start = false;
        }

        private void telegramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/aliilapro");
        }
    }
}
