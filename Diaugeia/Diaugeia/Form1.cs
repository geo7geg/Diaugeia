using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using System.Net;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Windows;
using System.Windows.Input;


namespace Diaugeia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            List<ComboBoxPairs> cbp = new List<ComboBoxPairs>();

            string f = Path.GetDirectoryName(Application.ExecutablePath) + @"\deua.txt"; ;

            List<string> lines = new List<string>();

            using (StreamReader r = new StreamReader(f, Encoding.Default))
            {           
                string line;
                while ((line = r.ReadLine()) != null)
                {                
                    lines.Add(line);
                }
            }
          
            foreach (string s in lines)
            {
                
                string[] words = s.Split(',');

                cbp.Add(new ComboBoxPairs(words[0], words[1]));
                words[0] = "";
                words[1] = "";
            }

            comboBox1.DataSource = cbp;
            comboBox1.DisplayMember = "org";
            comboBox1.ValueMember = "org_latin";

            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";       
        }

        public void findPDF()
        {
            if (checkBox4.Checked)
            {
                string ada = textBox1.Text;
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyy-MM-dd");
                ComboBoxPairs cbp = (ComboBoxPairs)comboBox1.SelectedItem;
                string org = cbp.org_latin;

                string url = "https" + "://diavgeia.gov.gr/opendata/search.json?ada="+ada;
                //string url = "https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04";
                //string url = "https://diavgeia.gov.gr/opendata/search.json?ada="+ ada.ToString();
                //"https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04"
                //"https://diavgeia.gov.gr/opendata/search.json?org=dimos_karpathou;deyaalex;deyaarg;deyaedessas;deyakranidi;deya_kavalas;deyakastorias;deyahortiati;dimotikiepihirisiydrefsisapohetefsislarisas;deyaxanthis;deyax;deyakyparissias;deyadidymoteichoy;deyaakastypalaias;DEYAD;deya_eretrias;deyathiras;deyakarditsas;deyakallikrateias;deyaparou;deyafarsalon;deyan_naoussas;deya_amfiloxias;deyamv;deyaselinou;deya_pylou;deyahg;deyaargostoli;deyalivadia;deyan;deyar;deyakom;deyamantoudioulimnisagannas;deyamaleviziou;deya_florinas;deya_mylopotamou;deyath;deyak_karpenissi;deyaskydras;deyaxalkidas;deyaxyl;deyav;deyakor;deyasitias;deyasymis;deya_tyrnavou;deya_kerkini;deyapyl;deyakaterinis;deyaxiou;deyapalama;deyasofadon;deya_aigialias;deya_artas;deyaep;deyaoorestiadas;DEYATRIP;deya_nessonos;deyavdera;deyaaol;deyak_thermis;DEYAKileler;deyas;deyamin;deyanisyrou;deyaa_alexandrias;deya_kilkis;deyamouzakiou;deyaalmopias;deyaba;deya_ptolemaidas;deyak_kalimnos;deyak_kos;deyafestou;deyalagada;deyaxersonisou;deyanestou;DEYA_SPARTIS;deyaskopelou;deya_almirou;deyavolvis;deyaelassonas;deyaz_zakynthou;deya_thermaikou;deyaker;deya_lamias;deyalesvou;deyathl;deyaa;deyaz;deyaprevezas;deyamessologhiou;deyapellas;deyanm;deya_hr;deya_thassou;deyakal;deyakozanis;deyakial;deya_sik;deya_agias;deyaioannina;deya_skiathos;deyal_p;deyapyrgou;deyatrikalon;deyaxanion;deyamessinis;deyaboriaskinourias;deyagrevena;deyah;deya_nafpaktias;deyaes;deyalerou;deyapaggaiou;deyapatras;deyaw1&from_issue_date=" + s;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader readStream1 = new StreamReader(resStream);

                string json = readStream1.ReadToEnd();
                RootObject Decision = JsonConvert.DeserializeObject<RootObject>(json);

                string path = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια";
                Directory.CreateDirectory(path);

                foreach (var item in Decision.decisions)
                {
                    if (item.documentUrl.Length != 0)
                    {
                        WebClient webClient = new WebClient();
                        var file1 = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια\" + item.issueDate + ".pdf";
                        //webClient.DownloadFile(item.documentUrl, file1);

                        var text = new StringBuilder();

                        // The PdfReader object implements IDisposable.Dispose, so you can
                        // wrap it in the using keyword to automatically dispose of it
                        using (var pdfReader = new PdfReader(item.documentUrl))
                        {
                            // Loop through each page of the document
                            for (var page = 1; page <= pdfReader.NumberOfPages; page++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                                var currentText = PdfTextExtractor.GetTextFromPage(
                                    pdfReader,
                                    page,
                                    strategy);

                                currentText =
                                    Encoding.UTF8.GetString(Encoding.Convert(
                                        Encoding.Default,
                                        Encoding.UTF8,
                                        Encoding.Default.GetBytes(currentText)));

                                if (currentText.Contains(textBox2.Text))
                                {
                                    webClient.DownloadFile(item.documentUrl, file1);
                                }

                                text.Append(currentText);

                            }
                            //MessageBox.Show(text.ToString());    
                        }
                    }
                }
            }else if(checkBox3.Checked)
            {
                string ada = textBox1.Text;
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyy-MM-dd");
                ComboBoxPairs cbp = (ComboBoxPairs)comboBox1.SelectedItem;
                string org = cbp.org_latin;

                string url = "https" + "://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "&subject=" + textBox3.Text;
                //string url = "https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04";
                //string url = "https://diavgeia.gov.gr/opendata/search.json?ada="+ ada.ToString();
                //"https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04"
                //"https://diavgeia.gov.gr/opendata/search.json?org=dimos_karpathou;deyaalex;deyaarg;deyaedessas;deyakranidi;deya_kavalas;deyakastorias;deyahortiati;dimotikiepihirisiydrefsisapohetefsislarisas;deyaxanthis;deyax;deyakyparissias;deyadidymoteichoy;deyaakastypalaias;DEYAD;deya_eretrias;deyathiras;deyakarditsas;deyakallikrateias;deyaparou;deyafarsalon;deyan_naoussas;deya_amfiloxias;deyamv;deyaselinou;deya_pylou;deyahg;deyaargostoli;deyalivadia;deyan;deyar;deyakom;deyamantoudioulimnisagannas;deyamaleviziou;deya_florinas;deya_mylopotamou;deyath;deyak_karpenissi;deyaskydras;deyaxalkidas;deyaxyl;deyav;deyakor;deyasitias;deyasymis;deya_tyrnavou;deya_kerkini;deyapyl;deyakaterinis;deyaxiou;deyapalama;deyasofadon;deya_aigialias;deya_artas;deyaep;deyaoorestiadas;DEYATRIP;deya_nessonos;deyavdera;deyaaol;deyak_thermis;DEYAKileler;deyas;deyamin;deyanisyrou;deyaa_alexandrias;deya_kilkis;deyamouzakiou;deyaalmopias;deyaba;deya_ptolemaidas;deyak_kalimnos;deyak_kos;deyafestou;deyalagada;deyaxersonisou;deyanestou;DEYA_SPARTIS;deyaskopelou;deya_almirou;deyavolvis;deyaelassonas;deyaz_zakynthou;deya_thermaikou;deyaker;deya_lamias;deyalesvou;deyathl;deyaa;deyaz;deyaprevezas;deyamessologhiou;deyapellas;deyanm;deya_hr;deya_thassou;deyakal;deyakozanis;deyakial;deya_sik;deya_agias;deyaioannina;deya_skiathos;deyal_p;deyapyrgou;deyatrikalon;deyaxanion;deyamessinis;deyaboriaskinourias;deyagrevena;deyah;deya_nafpaktias;deyaes;deyalerou;deyapaggaiou;deyapatras;deyaw1&from_issue_date=" + s;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader readStream1 = new StreamReader(resStream);

                string json = readStream1.ReadToEnd();
                RootObject Decision = JsonConvert.DeserializeObject<RootObject>(json);

                string path = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια";
                Directory.CreateDirectory(path);

                foreach (var item in Decision.decisions)
                {
                    if (item.documentUrl.Length != 0)
                    {
                        WebClient webClient = new WebClient();
                        var file1 = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια\" + item.issueDate + ".pdf";
                        //webClient.DownloadFile(item.documentUrl, file1);

                        var text = new StringBuilder();

                        // The PdfReader object implements IDisposable.Dispose, so you can
                        // wrap it in the using keyword to automatically dispose of it
                        using (var pdfReader = new PdfReader(item.documentUrl))
                        {
                            // Loop through each page of the document
                            for (var page = 1; page <= pdfReader.NumberOfPages; page++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                                var currentText = PdfTextExtractor.GetTextFromPage(
                                    pdfReader,
                                    page,
                                    strategy);

                                currentText =
                                    Encoding.UTF8.GetString(Encoding.Convert(
                                        Encoding.Default,
                                        Encoding.UTF8,
                                        Encoding.Default.GetBytes(currentText)));

                                if (currentText.Contains(textBox2.Text))
                                {
                                    webClient.DownloadFile(item.documentUrl, file1);
                                }

                                text.Append(currentText);

                            }
                            //MessageBox.Show(text.ToString());    
                        }
                    }
                }
            }
            else if (checkBox2.Checked)
            {
                string ada = textBox1.Text;
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyy-MM-dd");
                ComboBoxPairs cbp = (ComboBoxPairs)comboBox1.SelectedItem;
                string org = cbp.org_latin;

                string url = "https" + "://diavgeia.gov.gr/opendata/search.json?org=" + org;
                //string url = "https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04";
                //string url = "https://diavgeia.gov.gr/opendata/search.json?ada="+ ada.ToString();
                //"https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04"
                //"https://diavgeia.gov.gr/opendata/search.json?org=dimos_karpathou;deyaalex;deyaarg;deyaedessas;deyakranidi;deya_kavalas;deyakastorias;deyahortiati;dimotikiepihirisiydrefsisapohetefsislarisas;deyaxanthis;deyax;deyakyparissias;deyadidymoteichoy;deyaakastypalaias;DEYAD;deya_eretrias;deyathiras;deyakarditsas;deyakallikrateias;deyaparou;deyafarsalon;deyan_naoussas;deya_amfiloxias;deyamv;deyaselinou;deya_pylou;deyahg;deyaargostoli;deyalivadia;deyan;deyar;deyakom;deyamantoudioulimnisagannas;deyamaleviziou;deya_florinas;deya_mylopotamou;deyath;deyak_karpenissi;deyaskydras;deyaxalkidas;deyaxyl;deyav;deyakor;deyasitias;deyasymis;deya_tyrnavou;deya_kerkini;deyapyl;deyakaterinis;deyaxiou;deyapalama;deyasofadon;deya_aigialias;deya_artas;deyaep;deyaoorestiadas;DEYATRIP;deya_nessonos;deyavdera;deyaaol;deyak_thermis;DEYAKileler;deyas;deyamin;deyanisyrou;deyaa_alexandrias;deya_kilkis;deyamouzakiou;deyaalmopias;deyaba;deya_ptolemaidas;deyak_kalimnos;deyak_kos;deyafestou;deyalagada;deyaxersonisou;deyanestou;DEYA_SPARTIS;deyaskopelou;deya_almirou;deyavolvis;deyaelassonas;deyaz_zakynthou;deya_thermaikou;deyaker;deya_lamias;deyalesvou;deyathl;deyaa;deyaz;deyaprevezas;deyamessologhiou;deyapellas;deyanm;deya_hr;deya_thassou;deyakal;deyakozanis;deyakial;deya_sik;deya_agias;deyaioannina;deya_skiathos;deyal_p;deyapyrgou;deyatrikalon;deyaxanion;deyamessinis;deyaboriaskinourias;deyagrevena;deyah;deya_nafpaktias;deyaes;deyalerou;deyapaggaiou;deyapatras;deyaw1&from_issue_date=" + s;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader readStream1 = new StreamReader(resStream);

                string json = readStream1.ReadToEnd();
                RootObject Decision = JsonConvert.DeserializeObject<RootObject>(json);

                string path = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια";
                Directory.CreateDirectory(path);

                foreach (var item in Decision.decisions)
                {
                    if (item.documentUrl.Length != 0)
                    {
                        WebClient webClient = new WebClient();
                        var file1 = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια\" + item.issueDate + ".pdf";
                        //webClient.DownloadFile(item.documentUrl, file1);

                        var text = new StringBuilder();

                        // The PdfReader object implements IDisposable.Dispose, so you can
                        // wrap it in the using keyword to automatically dispose of it
                        using (var pdfReader = new PdfReader(item.documentUrl))
                        {
                            // Loop through each page of the document
                            for (var page = 1; page <= pdfReader.NumberOfPages; page++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                                var currentText = PdfTextExtractor.GetTextFromPage(
                                    pdfReader,
                                    page,
                                    strategy);

                                currentText =
                                    Encoding.UTF8.GetString(Encoding.Convert(
                                        Encoding.Default,
                                        Encoding.UTF8,
                                        Encoding.Default.GetBytes(currentText)));

                                if (currentText.Contains(textBox2.Text))
                                {
                                    webClient.DownloadFile(item.documentUrl, file1);
                                }

                                text.Append(currentText);

                            }
                            //MessageBox.Show(text.ToString());    
                        }
                    }
                }
            }
            else if (checkBox1.Checked)
            {
                string ada = textBox1.Text;
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyy-MM-dd");
                ComboBoxPairs cbp = (ComboBoxPairs)comboBox1.SelectedItem;
                string org = cbp.org_latin;

                string url = "https" + "://diavgeia.gov.gr/opendata/search.json?from_issue_date=" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "&to_issue_date=" + dateTimePicker2.Value.ToString("yyyy-MM-dd");
                //string url = "https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04";
                //string url = "https://diavgeia.gov.gr/opendata/search.json?ada="+ ada.ToString();
                //"https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04"
                //"https://diavgeia.gov.gr/opendata/search.json?org=dimos_karpathou;deyaalex;deyaarg;deyaedessas;deyakranidi;deya_kavalas;deyakastorias;deyahortiati;dimotikiepihirisiydrefsisapohetefsislarisas;deyaxanthis;deyax;deyakyparissias;deyadidymoteichoy;deyaakastypalaias;DEYAD;deya_eretrias;deyathiras;deyakarditsas;deyakallikrateias;deyaparou;deyafarsalon;deyan_naoussas;deya_amfiloxias;deyamv;deyaselinou;deya_pylou;deyahg;deyaargostoli;deyalivadia;deyan;deyar;deyakom;deyamantoudioulimnisagannas;deyamaleviziou;deya_florinas;deya_mylopotamou;deyath;deyak_karpenissi;deyaskydras;deyaxalkidas;deyaxyl;deyav;deyakor;deyasitias;deyasymis;deya_tyrnavou;deya_kerkini;deyapyl;deyakaterinis;deyaxiou;deyapalama;deyasofadon;deya_aigialias;deya_artas;deyaep;deyaoorestiadas;DEYATRIP;deya_nessonos;deyavdera;deyaaol;deyak_thermis;DEYAKileler;deyas;deyamin;deyanisyrou;deyaa_alexandrias;deya_kilkis;deyamouzakiou;deyaalmopias;deyaba;deya_ptolemaidas;deyak_kalimnos;deyak_kos;deyafestou;deyalagada;deyaxersonisou;deyanestou;DEYA_SPARTIS;deyaskopelou;deya_almirou;deyavolvis;deyaelassonas;deyaz_zakynthou;deya_thermaikou;deyaker;deya_lamias;deyalesvou;deyathl;deyaa;deyaz;deyaprevezas;deyamessologhiou;deyapellas;deyanm;deya_hr;deya_thassou;deyakal;deyakozanis;deyakial;deya_sik;deya_agias;deyaioannina;deya_skiathos;deyal_p;deyapyrgou;deyatrikalon;deyaxanion;deyamessinis;deyaboriaskinourias;deyagrevena;deyah;deya_nafpaktias;deyaes;deyalerou;deyapaggaiou;deyapatras;deyaw1&from_issue_date=" + s;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader readStream1 = new StreamReader(resStream);

                string json = readStream1.ReadToEnd();
                RootObject Decision = JsonConvert.DeserializeObject<RootObject>(json);

                string path = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια";
                Directory.CreateDirectory(path);

                foreach (var item in Decision.decisions)
                {
                    if (item.documentUrl.Length != 0)
                    {
                        WebClient webClient = new WebClient();
                        var file1 = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια\" + item.issueDate + ".pdf";
                        //webClient.DownloadFile(item.documentUrl, file1);

                        var text = new StringBuilder();

                        // The PdfReader object implements IDisposable.Dispose, so you can
                        // wrap it in the using keyword to automatically dispose of it
                        using (var pdfReader = new PdfReader(item.documentUrl))
                        {
                            // Loop through each page of the document
                            for (var page = 1; page <= pdfReader.NumberOfPages; page++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                                var currentText = PdfTextExtractor.GetTextFromPage(
                                    pdfReader,
                                    page,
                                    strategy);

                                currentText =
                                    Encoding.UTF8.GetString(Encoding.Convert(
                                        Encoding.Default,
                                        Encoding.UTF8,
                                        Encoding.Default.GetBytes(currentText)));

                                if (currentText.Contains(textBox2.Text))
                                {
                                    webClient.DownloadFile(item.documentUrl, file1);
                                }

                                text.Append(currentText);

                            }
                            //MessageBox.Show(text.ToString());    
                        }
                    }
                }
            }
            else
            {
                string ada = textBox1.Text;
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyy-MM-dd");
                ComboBoxPairs cbp = (ComboBoxPairs)comboBox1.SelectedItem;
                string org = cbp.org_latin;

                string url = "https" + "://diavgeia.gov.gr/opendata/search.json?org=" + org + "&from_issue_date=" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "&to_issue_date=" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "&subject=" + textBox3.Text;
                //string url = "https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04";
                //string url = "https://diavgeia.gov.gr/opendata/search.json?ada="+ ada.ToString();
                //"https://diavgeia.gov.gr/opendata/search.json?org=ypeka&from_issue_date=2014-12-04"
                //"https://diavgeia.gov.gr/opendata/search.json?org=dimos_karpathou;deyaalex;deyaarg;deyaedessas;deyakranidi;deya_kavalas;deyakastorias;deyahortiati;dimotikiepihirisiydrefsisapohetefsislarisas;deyaxanthis;deyax;deyakyparissias;deyadidymoteichoy;deyaakastypalaias;DEYAD;deya_eretrias;deyathiras;deyakarditsas;deyakallikrateias;deyaparou;deyafarsalon;deyan_naoussas;deya_amfiloxias;deyamv;deyaselinou;deya_pylou;deyahg;deyaargostoli;deyalivadia;deyan;deyar;deyakom;deyamantoudioulimnisagannas;deyamaleviziou;deya_florinas;deya_mylopotamou;deyath;deyak_karpenissi;deyaskydras;deyaxalkidas;deyaxyl;deyav;deyakor;deyasitias;deyasymis;deya_tyrnavou;deya_kerkini;deyapyl;deyakaterinis;deyaxiou;deyapalama;deyasofadon;deya_aigialias;deya_artas;deyaep;deyaoorestiadas;DEYATRIP;deya_nessonos;deyavdera;deyaaol;deyak_thermis;DEYAKileler;deyas;deyamin;deyanisyrou;deyaa_alexandrias;deya_kilkis;deyamouzakiou;deyaalmopias;deyaba;deya_ptolemaidas;deyak_kalimnos;deyak_kos;deyafestou;deyalagada;deyaxersonisou;deyanestou;DEYA_SPARTIS;deyaskopelou;deya_almirou;deyavolvis;deyaelassonas;deyaz_zakynthou;deya_thermaikou;deyaker;deya_lamias;deyalesvou;deyathl;deyaa;deyaz;deyaprevezas;deyamessologhiou;deyapellas;deyanm;deya_hr;deya_thassou;deyakal;deyakozanis;deyakial;deya_sik;deya_agias;deyaioannina;deya_skiathos;deyal_p;deyapyrgou;deyatrikalon;deyaxanion;deyamessinis;deyaboriaskinourias;deyagrevena;deyah;deya_nafpaktias;deyaes;deyalerou;deyapaggaiou;deyapatras;deyaw1&from_issue_date=" + s;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader readStream1 = new StreamReader(resStream);

                string json = readStream1.ReadToEnd();
                RootObject Decision = JsonConvert.DeserializeObject<RootObject>(json);

                string path = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια";
                Directory.CreateDirectory(path);

                foreach (var item in Decision.decisions)
                {
                    if (item.documentUrl.Length != 0)
                    {
                        WebClient webClient = new WebClient();
                        var file1 = @"c:\Users\" + Environment.UserName + @"\Desktop\Διαύγεια\" + item.issueDate + ".pdf";
                        //webClient.DownloadFile(item.documentUrl, file1);

                        var text = new StringBuilder();

                        // The PdfReader object implements IDisposable.Dispose, so you can
                        // wrap it in the using keyword to automatically dispose of it
                        using (var pdfReader = new PdfReader(item.documentUrl))
                        {
                            // Loop through each page of the document
                            for (var page = 1; page <= pdfReader.NumberOfPages; page++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                                var currentText = PdfTextExtractor.GetTextFromPage(
                                    pdfReader,
                                    page,
                                    strategy);

                                currentText =
                                    Encoding.UTF8.GetString(Encoding.Convert(
                                        Encoding.Default,
                                        Encoding.UTF8,
                                        Encoding.Default.GetBytes(currentText)));

                                if (currentText.Contains(textBox2.Text))
                                {
                                    webClient.DownloadFile(item.documentUrl, file1);
                                }

                                text.Append(currentText);

                            }
                            //MessageBox.Show(text.ToString());    
                        }
                    }
                }
            }
            MessageBox.Show("Download complete.");
            
        }

            public class Decision
        {
            public string protocolNumber { get; set; }
            public string subject { get; set; }
            public string issueDate { get; set; }
            public string organizationId { get; set; }
            public List<String> signerIds { get; set; }
            public List<String> unitIds { get; set; }
            public string decisionTypeId { get; set; }
            public List<String> thematicCategoryIds { get; set; } 
            public string privateData { get; set; }
            public string ada { get; set; }
            public string publishTimestamp { get; set; }
            public string submissionTimestamp { get; set; }
            public string versionId { get; set; }
            public string status { get; set; }
            public string url { get; set; }
            public string documentUrl { get; set; }
            public string documentChecksum { get; set; }
            public string warnings { get; set; }
            public string correctedVersionId { get; set; }
        
        }

        public class RootObject
        {
            public List<Decision> decisions { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            findPDF();
        }

        public class ComboBoxPairs
        {
            public string org { get; set; }
            public string org_latin { get; set; }

            public ComboBoxPairs(string Org,
                                 string Org_latin)
            {
                org = Org;
                org_latin = Org_latin;
            }
        }
    }
}
