using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections; //arraylist
using System.Security.Cryptography;

using System.Web;

using System.Xml;
using Newtonsoft.Json;

namespace Tweeter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void set_encoded_consumer_strings()
        {
            encoded_consumer_key.Text = Uri.EscapeDataString(consumer_key_2.Text);
            encoded_consumer_secret.Text = Uri.EscapeDataString(consumer_secret_2.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            set_encoded_consumer_strings();
        }

        public void set_bearer_token_credentials()
        {
            bearer_token_credentials.Text = encoded_consumer_key.Text + ":" + encoded_consumer_secret.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            set_bearer_token_credentials();
        }

        public void set_base64_encoded_bearer_token_credentials()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(bearer_token_credentials.Text);
            encoded_bearer_token_credentials.Text = System.Convert.ToBase64String(byteArray);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            set_base64_encoded_bearer_token_credentials();
        }
        public void set_app_auth_request_parameters()
        {
            request_uri.Text = "https://api.twitter.com/oauth2/token";
            request_query.Text = "";
            request_method.Text = "POST";
            request_host.Text = "api.twitter.com";
            request_user_agent.Text = "Alan's Test Application";
            request_body.Text = "grant_type=client_credentials";
            authorization_header_textbox.Text = "Basic " + encoded_bearer_token_credentials.Text;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            set_app_auth_request_parameters();
        }

        private static string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();
        public void set_oauth_nonce()
        {
            int length = 42;
            var nonceString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                nonceString.Append(validChars[random.Next(0, validChars.Length - 1)]);
            }
            oauth_nonce_textbox.Text = nonceString.ToString();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            set_oauth_nonce();
        }

        public void set_oauth_timestamp()
        {
            oauth_timestamp_textbox.Text = "" + (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            set_oauth_timestamp();
        }
        public static string PercentEncode(string s, bool is_query_string = false)
        {
            //https://en.wikipedia.org/wiki/Percent-encoding#Percent-encoding_reserved_characters

            string result = "";
            foreach (char c in s)
            {
                switch (c)
                {
                    case ' ':
                        if (is_query_string) { result += "+"; } else { result += "%20"; }
                        break;
                    case '!':
                        result += "%21";
                        break;
                    case '#':
                        result += "%23";
                        break;
                    case '$':
                        result += "%24";
                        break;
                    case '%':
                        result += "%25";
                        break;
                    case '&':
                        result += "%26";
                        break;
                    case '\'':
                        result += "%27";
                        break;
                    case '(':
                        result += "%28";
                        break;
                    case ')':
                        result += "%29";
                        break;
                    case '*':
                        result += "%2A";
                        break;
                    case '+':
                        result += "%2B";
                        break;
                    case ',':
                        result += "%2C";
                        break;
                    case '/':
                        result += "%2F";
                        break;
                    case ':':
                        result += "%3A";
                        break;
                    case ';':
                        result += "%3B";
                        break;
                    case '=':
                        result += "%3D";
                        break;
                    case '?':
                        result += "%3F";
                        break;
                    case '@':
                        result += "%40";
                        break;
                    case '[':
                        result += "%5B";
                        break;
                    case ']':
                        result += "%5D";
                        break;
                    // these must be done for arb data
                    case '"':
                        if (is_query_string) { result += "%22"; } else { result += c; }
                        break;
                    case '-':
                        if (is_query_string) { result += "%2D"; } else { result += c; }
                        break;
                    case '.':
                        if (is_query_string) { result += "%2E"; } else { result += c; }
                        break;
                    case '<':
                        if (is_query_string) { result += "%3C"; } else { result += c; }
                        break;
                    case '>':
                        if (is_query_string) { result += "%3E"; } else { result += c; }
                        break;
                    case '\\':
                        if (is_query_string) { result += "%5C"; } else { result += c; }
                        break;
                    case '^':
                        if (is_query_string) { result += "%5E"; } else { result += c; }
                        break;
                    case '_':
                        if (is_query_string) { result += "%5F"; } else { result += c; }
                        break;
                    case '`':
                        if (is_query_string) { result += "%60"; } else { result += c; }
                        break;
                    case '{':
                        if (is_query_string) { result += "%7B"; } else { result += c; }
                        break;
                    case '|':
                        if (is_query_string) { result += "%7C"; } else { result += c; }
                        break;
                    case '}':
                        if (is_query_string) { result += "%7D"; } else { result += c; }
                        break;
                    case '~':
                        if (is_query_string) { result += "%7E"; } else { result += c; }
                        break;
                    default:
                        result += c;
                        break;
                }
            }
            return result;
        }

        public void set_oauth_signature()
        {
            string method = request_method.Text;
            string uri = request_uri.Text;
            string query = request_query.Text;
            string body = request_body.Text;
            string oauth_consumer_key = consumer_key_2.Text; //specific to app
            string oauth_consumer_secret = consumer_secret_2.Text; //specific to app
            string oauth_nonce = oauth_nonce_textbox.Text; //some random string
            string oauth_signature = ""; //need to calculate
            string oauth_signature_method = oauth_signature_method_textbox.Text; // always
            string oauth_timestamp = oauth_timestamp_textbox.Text; //need to calculate
            string oauth_token = access_token.Text; //specific to app/user
            string oauth_token_secret = access_token_secret.Text;
            string oauth_version = oauth_version_textbox.Text; //always

            //
            // first construct parameter string
            // https://dev.twitter.com/docs/auth/creating-signature
            // https://dev.twitter.com/docs/auth/authorizing-request
            //
            ArrayList parameters = new ArrayList();
            string parameter_string = "";
            foreach (string s in query.Split(new Char[] { '&' }))
            {
                if (s.Equals(""))
                {
                    continue;
                }
                string query_key = s.Split(new Char[] { '=' })[0];
                string query_value = s.Split(new Char[] { '=' })[1];
                if (query_key.Equals("status"))
                {
                    parameters.Add(query_key + "=" + (query_value));
                }
                else
                {
                    parameters.Add(query_key + "=" + PercentEncode(query_value));
                }
            }

            parameters.Add("oauth_consumer_key=" + PercentEncode(oauth_consumer_key));
            parameters.Add("oauth_nonce=" + PercentEncode(oauth_nonce));
            parameters.Add("oauth_signature_method=" + PercentEncode(oauth_signature_method));
            parameters.Add("oauth_timestamp=" + PercentEncode(oauth_timestamp));
            parameters.Add("oauth_token=" + PercentEncode(oauth_token));
            parameters.Add("oauth_version=" + PercentEncode(oauth_version));
            parameters.Sort();

            foreach (String s in parameters)
            {
                parameter_string += s + "&";
            }
            parameter_string = parameter_string.Substring(0, parameter_string.Length - 1);

            string signature_base_string = "";
            signature_base_string = method.ToUpper();
            signature_base_string += "&";
            signature_base_string += PercentEncode(uri);
            signature_base_string += "&";
            signature_base_string += PercentEncode(parameter_string);
            signature_base_string_textbox.Text = signature_base_string;

            byte[] key = Encoding.UTF8.GetBytes(PercentEncode(oauth_consumer_secret) + "&" + PercentEncode(oauth_token_secret));
            string input = signature_base_string;
            oauth_signature = Encode(input, key);
            oauth_signature_textbox.Text = oauth_signature;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            set_oauth_signature();
        }

        public static string Encode(string input, byte[] key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);

            byte[] calc = myhmacsha1.ComputeHash(stream);
            return System.Convert.ToBase64String(calc);
        }
        public void set_get_user_timeline_parameters()
        {
            request_method.Text = "GET";
            request_uri.Text = "https://api.twitter.com/1.1/statuses/user_timeline.json";
            request_query.Text = "";
            request_host.Text = "api.twitter.com";
            request_user_agent.Text = "Alan's Test Application";
            request_body.Text = "";

            if (user_id_checkbox.Checked)
            {
                request_query.Text += "&user_id=" + user_id_textbox.Text;
            }
            if (screen_name_checkbox.Checked)
            {
                request_query.Text += "&screen_name=" + screen_name_textbox.Text;
            }
            if (since_id_checkbox.Checked)
            {
                request_query.Text += "&since_id=" + since_id_textbox.Text;
            }
            if (count_checkbox.Checked)
            {
                request_query.Text += "&count=" + count_textbox.Text;
            }
            if (max_id_checkbox.Checked)
            {
                request_query.Text += "&max_id=" + max_id_textbox.Text;
            }
            if (trim_user_checkbox.Checked)
            {
                request_query.Text += "&trim_user=" + trim_user_textbox.Text;
            }
            if (exclude_replies_checkbox.Checked)
            {
                request_query.Text += "&exclude_replies=" + exclude_replies_textbox.Text;
            }
            if (contributor_details_checkbox.Checked)
            {
                request_query.Text += "&contributor_details=" + contributor_details_textbox.Text;
            }
            if (include_rts_checkbox.Checked)
            {
                request_query.Text += "&include_rts=" + include_rts_textbox.Text;
            }

            request_query.Text = request_query.Text.Substring(1, request_query.Text.Length - 1); // trim off leader & to make it look nice

        }
        private void button12_Click(object sender, EventArgs e)
        {
            set_get_user_timeline_parameters();
        }

        public void set_app_auth_token()
        {
            authorization_header_textbox.Text = "Bearer " + bearer_token.Text;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            set_app_auth_token();
        }

        public void set_oauth_token()
        {
            string DST = "OAuth ";
            DST += "oauth_consumer_key=\"" + PercentEncode(consumer_key_2.Text) + "\", ";
            DST += "oauth_nonce=\"" + PercentEncode(oauth_nonce_textbox.Text) + "\", ";
            DST += "oauth_signature=\"" + PercentEncode(oauth_signature_textbox.Text) + "\", ";
            DST += "oauth_signature_method=\"" + PercentEncode(oauth_signature_method_textbox.Text) + "\", ";
            DST += "oauth_timestamp=\"" + PercentEncode(oauth_timestamp_textbox.Text) + "\", ";
            DST += "oauth_token=\"" + PercentEncode(access_token.Text) + "\", ";
            DST += "oauth_version=\"" + PercentEncode(oauth_version_textbox.Text) + "\", ";

            authorization_header_textbox.Text = DST;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            set_oauth_token();
        }

        public void set_post_status_parameters()
        {
            request_method.Text = "POST";
            request_uri.Text = "https://api.twitter.com/1.1/statuses/update.json";
            request_query.Text = "status=" + PercentEncode(status_textbox.Text);
            request_host.Text = "api.twitter.com";
            request_user_agent.Text = "Alan's Test Application";
            request_body.Text = "status=" + PercentEncode(status_textbox.Text, true);
            if (in_reply_to_status_id_checkbox.Checked)
            {
                request_query.Text += "&in_reply_to_status_id=" + in_reply_to_status_id_textbox.Text;
                request_body.Text += "&in_reply_to_status_id=" + in_reply_to_status_id_textbox.Text;
            }
            if (lat_checkbox.Checked)
            {
                request_query.Text += "&lat=" + lat_textbox.Text;
                request_body.Text += "&lat=" + lat_textbox.Text;
            }
            if (long_checkbox.Checked)
            {
                request_query.Text += "&long=" + long_textbox.Text;
                request_body.Text += "&long=" + long_textbox.Text;
            }
            if (place_id_checkbox.Checked)
            {
                request_query.Text += "&place_id=" + place_id_textbox.Text;
                request_body.Text += "&place_id=" + place_id_textbox.Text;
            }
            if (display_coordinates_checkbox.Checked)
            {
                request_query.Text += "&display_coordinates=" + display_coordinates_textbox.Text;
                request_body.Text += "&display_coordinates=" + display_coordinates_textbox.Text;
            }
            if (trim_user_checkbox_2.Checked)
            {
                request_query.Text += "&trim_user_checkbox=" + trim_user_textbox_2.Text;
                request_body.Text += "&trim_user_checkbox=" + trim_user_textbox_2.Text;
            }
            //body handled differently from header
        }
        private void button15_Click(object sender, EventArgs e)
        {
            set_post_status_parameters();
        }
        public void do_request()
        {
            response.Text = "";
            HttpWebRequest http_request = null;
            Stream dataStream = null;
            HttpWebResponse http_response = null;
            StreamReader reader = null;

            if (request_method.Text.Equals("GET"))
            {
                http_request = (HttpWebRequest)WebRequest.Create(request_uri.Text + "?" + request_query.Text);
            }
            else if (request_method.Text.Equals("POST"))
            {
                http_request = (HttpWebRequest)WebRequest.Create(request_uri.Text);
            }
            else
            {
                response.Text = "Error!";
                return;
            }

            http_request.Method = request_method.Text;
            http_request.Host = request_host.Text;
            http_request.UserAgent = request_user_agent.Text;
            http_request.Headers.Add("Authorization", authorization_header_textbox.Text);

            if (request_method.Text.Equals("POST"))
            {
                string body = request_body.Text;
                byte[] bodydata = Encoding.UTF8.GetBytes(body);
                http_request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                http_request.ContentLength = bodydata.Length;
                dataStream = http_request.GetRequestStream();
                dataStream.Write(bodydata, 0, bodydata.Length);
                dataStream.Close();
            }

            string responseFromServer = "";

            try
            {
                http_response = (HttpWebResponse)http_request.GetResponse();
                dataStream = http_response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();

                response.Text = responseFromServer;
            }
            catch (Exception e2)
            {
                response.Text = e2.ToString();
            }
            finally
            {
                // Cleanup the streams and the response.
                if (reader != null) { reader.Close(); }
                if (http_response != null) { http_response.Close(); }
                if (dataStream != null) { dataStream.Close(); }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            do_request();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dynamic stuff = JObject.Parse(response.Text);
            bearer_token.Text += stuff.access_token;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxBase box = (TextBoxBase)ActiveControl;
            box.SelectAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxBase box = (TextBoxBase)ActiveControl;
            box.Clear();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxBase box = (TextBoxBase)ActiveControl;
            box.Paste();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxBase box = (TextBoxBase)ActiveControl;
            box.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxBase box = (TextBoxBase)ActiveControl;
            box.Cut();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void aPIDocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Twitter API Docs: https://dev.twitter.com/docs/api/1.1");
            if (MessageBox.Show("Click Yes to view docs at https://dev.twitter.com/docs/api/1.1", "Twitter API Docs", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://dev.twitter.com/docs/api/1.1");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Alan Ing was here 2013! http://twitter.alaning.me");
        }

        private void resources_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (resources_rate_checkbox.Checked)
            {
                account_rate_checkbox.Enabled = true;
                application_rate_checkbox.Enabled = true;
                blocks_rate_checkbox.Enabled = true;
                direct_messages_rate_checkbox.Enabled = true;
                favorites_rate_checkbox.Enabled = true;
                followers_rate_checkbox.Enabled = true;
                friends_rate_checkbox.Enabled = true;
                friendships_rate_checkbox.Enabled = true;
                geo_rate_checkbox.Enabled = true;
                help_rate_checkbox.Enabled = true;
                lists_rate_checkbox.Enabled = true;
                saved_searches_rate_checkbox.Enabled = true;
                search_rate_checkbox.Enabled = true;
                statuses_rate_checkbox.Enabled = true;
                trends_rate_checkbox.Enabled = true;
                users_rate_checkbox.Enabled = true;
            }
            else
            {
                account_rate_checkbox.Enabled = false;
                application_rate_checkbox.Enabled = false;
                blocks_rate_checkbox.Enabled = false;
                direct_messages_rate_checkbox.Enabled = false;
                favorites_rate_checkbox.Enabled = false;
                followers_rate_checkbox.Enabled = false;
                friends_rate_checkbox.Enabled = false;
                friendships_rate_checkbox.Enabled = false;
                geo_rate_checkbox.Enabled = false;
                help_rate_checkbox.Enabled = false;
                lists_rate_checkbox.Enabled = false;
                saved_searches_rate_checkbox.Enabled = false;
                search_rate_checkbox.Enabled = false;
                statuses_rate_checkbox.Enabled = false;
                trends_rate_checkbox.Enabled = false;
                users_rate_checkbox.Enabled = false;
            }
        }

        public void set_get_rate_limit_parameters()
        {
            request_method.Text = "GET";
            request_uri.Text = "https://api.twitter.com/1.1/application/rate_limit_status.json";
            request_query.Text = "";
            request_host.Text = "api.twitter.com";
            request_user_agent.Text = "Alan's Test Application";

            if (resources_rate_checkbox.Checked)
            {
                request_query.Text = "resources=";
            }
            if (account_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "account,";
            }
            if (application_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "application,";
            }
            if (blocks_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "blocks,";
            }
            if (direct_messages_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "direct_messages,";
            }
            if (favorites_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "favorites,";
            }
            if (followers_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "followers,";
            }
            if (friends_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "friends,";
            }
            if (friendships_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "friendships,";
            }
            if (geo_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "geo,";
            }
            if (help_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "help,";
            }
            if (lists_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "lists,";
            }
            if (saved_searches_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "saved_searches,";
            }
            if (search_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "search,";
            }
            if (statuses_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "statuses,";
            }
            if (trends_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "trends,";
            }
            if (users_rate_checkbox.Checked && resources_rate_checkbox.Checked)
            {
                request_query.Text += "users";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            set_get_rate_limit_parameters();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            

            screen_name_textbox.Text = target_user_textbox.Text;
            count_textbox.Text = "200";
            user_id_checkbox.Checked = false;
            screen_name_checkbox.Checked = true;
            since_id_checkbox.Checked = false;
            count_checkbox.Checked = true;
            max_id_checkbox.Checked = false;
            trim_user_checkbox.Checked = true;
            exclude_replies_checkbox.Checked = false;
            contributor_details_checkbox.Checked = false;
            include_rts_checkbox.Checked = false;



            if (loop_textbox.Text == "" || int.Parse(loop_textbox.Text) > 150)
            {
                loop_textbox.Text = "150";
            }

            int LIMIT = int.Parse(loop_textbox.Text);
            try
            {
                for (int i = 0; i < LIMIT; i++)
                {

                    auto_request();
                    max_id_checkbox.Checked = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                MessageBox.Show("Done!");

            }
                
        }
        public void auto_request()
        {
            //error checking???
            set_get_user_timeline_parameters();
            if (app_radio.Checked)
            {
                set_app_auth_token();
            }
            else if (oauth_radio.Checked)
            {
                set_oauth_nonce();
                set_oauth_timestamp();
                set_oauth_signature();
                set_oauth_token();
            }
            else
            {
                return;
            }

            do_request();

            string rep = response.Text;

            rep = @"{ info: " + rep + "}";
            dynamic stuff = JObject.Parse(rep);
            ArrayList sht = new ArrayList();
            textBox1.Text = target_user_textbox.Text + "_" + stuff.info[0].id;
            textBox1.Text += "_" + stuff.info.Count;
            textBox1.Text += "_" + stuff.info[stuff.info.Count - 1].id;

            // need to set max_id
            // max_id_textbox

            string save_dir = save_dir_textbox.Text + "\\";
            save_dir += textBox1.Text + ".txt";
            System.IO.File.WriteAllText(save_dir, response.Text);

            max_id_textbox.Text = "" + (stuff.info[stuff.info.Count - 1].id - 1);

        }

        private void button16_Click(object sender, EventArgs e)
        {
            resources_rate_checkbox.Checked = true;
            set_get_rate_limit_parameters();
            if (app_radio.Checked)
            {
                set_app_auth_token();
            }
            else if (oauth_radio.Checked)
            {
                set_oauth_nonce();
                set_oauth_timestamp();
                set_oauth_signature();
                set_oauth_token();
            }
            else
            {
                return;
            }
            do_request();
            dynamic stuff = JObject.Parse(response.Text);
            textBox2.Text = "/statuses/user_timeline: ";
            textBox2.Text += stuff.resources.statuses["/statuses/user_timeline"].remaining;
            textBox2.Text += " of ";
            textBox2.Text += "" + stuff.resources.statuses["/statuses/user_timeline"].limit;
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            FolderBrowserDialog saveFileDialog1 = new FolderBrowserDialog();
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            save_dir_textbox.Text = "" + saveFileDialog1.SelectedPath;
        }

        private void authorization_header_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    authorization_header_textbox.SelectAll();
                }
                if (e.KeyCode == Keys.Back)
                {
                    e.SuppressKeyPress = true;
                    int selStart = authorization_header_textbox.SelectionStart;
                    while (selStart > 0 && authorization_header_textbox.Text.Substring(selStart - 1, 1) == " ")
                    {
                        selStart--;
                    }
                    int prevSpacePos = -1;
                    if (selStart != 0)
                    {
                        prevSpacePos = authorization_header_textbox.Text.LastIndexOf(' ', selStart - 1);
                    }
                    authorization_header_textbox.Select(prevSpacePos + 1, authorization_header_textbox.SelectionStart - prevSpacePos - 1);
                    authorization_header_textbox.SelectedText = "";
                }
            }
        }
        private void response_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    response.SelectAll();
                }
                if (e.KeyCode == Keys.Back)
                {
                    e.SuppressKeyPress = true;
                    int selStart = textBox1.SelectionStart;
                    while (selStart > 0 && response.Text.Substring(selStart - 1, 1) == " ")
                    {
                        selStart--;
                    }
                    int prevSpacePos = -1;
                    if (selStart != 0)
                    {
                        prevSpacePos = response.Text.LastIndexOf(' ', selStart - 1);
                    }
                    response.Select(prevSpacePos + 1, response.SelectionStart - prevSpacePos - 1);
                    response.SelectedText = "";
                }
            }
        }
    }
}